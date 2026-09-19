using SmsTest.Application;
using SmsTest.Application.DTO;
using SmsTest.ConsoleApp.DAL;
using SmsTest.ConsoleApp.DAL.Entities;
using SmsTest.ConsoleApp.Utilities;
using System.Globalization;
using System.Text;

namespace SmsTest.ConsoleApp;

internal class ConsoleApplication(
    ISmsTestServiceClient client,
    DishRepository repository,
    IConsole console)
{
    public async Task RunAsync(CancellationToken token)
    {
        try
        {
            var getMenuResponse = await client.GetMenuAsync(
                new GetMenuRequestDto(true),
                token);

            if (!getMenuResponse.Success)
            {
                throw new InvalidOperationException($"Сервер вернул ошибку на запрос получения меню: {getMenuResponse.ErrorMessage}");
            }

            await repository.ReplaceDishesAsync(getMenuResponse.MenuItems.Select(x => new Dish
            {
                Id = x.Id,
                Name = x.Name,
                Article = x.Article,
                Price = x.Price
            }),
            token);

            // Форматирование для удобства пользователя
            var maxNameLength = 0;
            var maxArticleLength = 0;

            foreach (var item in getMenuResponse.MenuItems)
            {
                if (item.Name.Length > maxNameLength)
                {
                    maxNameLength = item.Name.Length;
                }

                if (item.Article.Length > maxArticleLength)
                {
                    maxArticleLength = item.Article.Length;
                }
            }

            var sb = new StringBuilder();

            foreach (var item in getMenuResponse.MenuItems)
            {
                sb.Append(item.Name.PadRight(maxNameLength));
                sb.Append(" - ");
                sb.Append(item.Article.PadRight(maxArticleLength));
                sb.Append(" - ");
                sb.AppendLine(item.Price.ToString(CultureInfo.InvariantCulture));
            }

            console.WriteLine(sb.ToString());

            do
            {
                console.WriteLine("Введите заказ в формате: Код1:Количество1;Код2:Количество2;Код3:Количество3;...");

                var line = console.ReadLine();

                List<UserInputOrderItem> userInputOrderItems;

                try
                {
                    // Заказ по вводу пользователя
                    userInputOrderItems = OrdersTextParser.Parse(line);
                }
                catch (FormatException formatException)
                {
                    console.WriteLine(formatException.Message);
                    continue;
                }

                // Валидация заказа по количеству
                var wrongQuantityOrder = userInputOrderItems.FirstOrDefault(x => x.Quantity <= 0.0);

                if (wrongQuantityOrder is not null)
                {
                    console.WriteLine($"Для блюда {wrongQuantityOrder.Article} задано неверное количество {wrongQuantityOrder.Quantity}. Ожидается больше нуля");
                    continue;
                }

                var articles = userInputOrderItems
                    .Select(x => x.Article)
                    .ToArray();

                // Поиск товаров в БД по артикулам
                var dishes = await repository.FindDishesByArticlesAsync(articles, token);

                // Сопоставление блюд по артикулу, формирование заказа
                var orderItems = userInputOrderItems
                    .Join(
                        dishes,
                        input => input.Article,
                        dish => dish.Article,
                        (input, dish) => new OrderItemDto(
                            dish.Id,
                            input.Quantity))
                    .ToArray();

                // Валидация заказа по наличию блюд
                if (orderItems.Length != userInputOrderItems.Count)
                {
                    console.WriteLine("Одно или несколько блюд не найдены");
                    continue;
                }

                // Отправка заказа
                var sendOrderResponse = await client.SendOrderAsync(
                    new SendOrderRequestDto(
                        Guid.NewGuid(),
                        [.. orderItems]),
                    token);

                if (!sendOrderResponse.Success)
                {
                    console.WriteLine($"Сервер вернул ошибку на запрос создания заказа: {sendOrderResponse.ErrorMessage}");
                    continue;
                }

                console.WriteLine("УСПЕХ");
            }
            while (!token.IsCancellationRequested); // Пользователь может отправлять заказы сколько угодно
        }
        catch (OperationCanceledException)
        {
            // Ignore
        }
        catch (Exception ex)
        {
            console.WriteLine(ex.Message);
        }
    }
}