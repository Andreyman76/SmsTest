using SmsTest.ConsoleApp.DAL;
using SmsTest.ConsoleApp.DAL.Entities;
using SmsTest.ConsoleApp.Utilities;
using SmsTest.Domain;
using SmsTest.Domain.DTO;

namespace SmsTest.ConsoleApp;

internal class ConsoleApplication(
    ISmsTestServiceClient client,
    DishRepository repository)
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

            await repository.ReplaceMenuAsync(getMenuResponse.MenuItems.Select(x => new Dish
            {
                Id = x.Id,
                Name = x.Name,
                Article = x.Article,
                Price = x.Price
            }),
            token);

            foreach (var item in getMenuResponse.MenuItems)
            {
                ConsoleLogger.WriteLine($"{item.Name} - {item.Article} - {item.Price}");
            }

            do
            {
                ConsoleLogger.WriteLine("Введите заказ в формате: Код1:Количество1;Код2:Количество2;Код3:Количество3;...");

                var line = ConsoleLogger.ReadLine();

                List<UserInputOrderItem> userInputOrderItems;

                try
                {
                    userInputOrderItems = OrdersTextParser.Parse(line);
                }
                catch (FormatException formatException)
                {
                    ConsoleLogger.WriteLine(formatException.Message);
                    continue;
                }

                var wrongQuantityOrder = userInputOrderItems.FirstOrDefault(x => x.Quantity <= 0.0);

                if (wrongQuantityOrder is not null)
                {
                    ConsoleLogger.WriteLine($"Для блюда {wrongQuantityOrder.Article} задано неверное количество {wrongQuantityOrder.Quantity}. Ожидается больше нуля");
                    continue;
                }

                var articles = userInputOrderItems
                    .Select(x => x.Article)
                    .ToArray();

                var dishes = await repository.FindDishesByArticlesAsync(articles, token);

                var orderItems = userInputOrderItems
                    .Join(
                        dishes,
                        input => input.Article,
                        dish => dish.Article,
                        (input, dish) => new OrderItemDto(
                            dish.Id,
                            input.Quantity))
                    .ToArray();

                if (orderItems.Length != userInputOrderItems.Count)
                {
                    ConsoleLogger.WriteLine("Одно или несколько блюд не найдены");
                    continue;
                }

                var sendOrderResponse = await client.SendOrderAsync(
                    new SendOrderRequestDto(
                        Guid.NewGuid(),
                        [.. orderItems]),
                    token);

                if (!sendOrderResponse.Success)
                {
                    throw new InvalidOperationException($"Сервер вернул ошибку на запрос создания заказа: {sendOrderResponse.ErrorMessage}");
                }

                ConsoleLogger.WriteLine("УСПЕХ");
            }
            while (!token.IsCancellationRequested);
        }
        catch (OperationCanceledException)
        {
            // Ignore
        }
        catch (Exception ex)
        {
            ConsoleLogger.WriteLine(ex.Message);
        }
    }
}