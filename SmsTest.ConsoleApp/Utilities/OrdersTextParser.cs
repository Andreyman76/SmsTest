using System.Globalization;

namespace SmsTest.ConsoleApp.Utilities;

/// <summary>
/// Парсер пользовательского ввода заказа в следующем формате: Код1:Количество1;Код2:Количество2;Код3:Количество3; 
/// </summary>
internal static class OrdersTextParser
{
    private const char OrdersSeparator = ';';
    private const char OrderParametersSeparator = ':';

    public static List<UserInputOrderItem> Parse(
        ReadOnlySpan<char> ordersText)
    {
        var text = ordersText.Trim();
        var result = new List<UserInputOrderItem>();

        var i = 0;

        do
        {
            var article = GetOrderParameterOrDefault(
                text,
                i
            );

            var cleanArticle = article.Trim();

            if (cleanArticle.Length < 1)
            {
                throw new FormatException($"Строка заказов имеет неверный формат: не заполнен артикул");
            }

            i += article.Length;

            if (i >= text.Length
                || (i < text.Length
                && text[i] != OrderParametersSeparator))
            {
                throw new FormatException($"Строка заказов имеет неверный формат: ожидается `{OrderParametersSeparator}` после артикула");
            }

            i++;

            var quantity = GetOrderParameterOrDefault(
                text,
                i
            );

            var cleanQuantity = quantity.Trim();

            if (cleanQuantity.Length < 1)
            {
                throw new FormatException($"Строка заказов имеет неверный формат: не заполнено количество");
            }

            i += quantity.Length;

            result.Add(
                ParseOrderItem(
                    cleanArticle.ToString(),
                    cleanQuantity.ToString())
                );

            if (i < text.Length
                && text[i] != OrdersSeparator)
            {
                throw new FormatException($"Строка заказов имеет неверный формат: ожидается `{OrdersSeparator}` перед следующим заказом");
            }

            i++;
        }
        while (i < text.Length);

        return result;
    }

    private static ReadOnlySpan<char> GetOrderParameterOrDefault(
        ReadOnlySpan<char> ordersText,
        int fromPosition
        )
    {
        var length = 0;

        for (int i = fromPosition; i < ordersText.Length; i++)
        {
            if (ordersText[i] == OrderParametersSeparator
                || ordersText[i] == OrdersSeparator)
            {
                return ordersText.Slice(fromPosition, length);
            }

            length++;
        }

        return ordersText[fromPosition..];
    }

    private static UserInputOrderItem ParseOrderItem(
        string article,
        string quantity)
    {
        if (double.TryParse(
            quantity.Replace(',', '.'),
            CultureInfo.InvariantCulture,
            out var q))
        {
            return new UserInputOrderItem(article, q);
        }
        else
        {
            throw new FormatException($"Строка заказов имеет неверный формат: {quantity} не число");
        }
    }
}