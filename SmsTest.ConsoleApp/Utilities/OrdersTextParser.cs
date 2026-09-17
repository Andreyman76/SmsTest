using System.Globalization;
using System.Text;

namespace SmsTest.ConsoleApp.Utilities;

/// <summary>
/// Парсер пользовательского ввода заказа в следующем формате: Код1:Количество1;Код2:Количество2;Код3:Количество3; 
/// </summary>
internal static class OrdersTextParser
{
    private const char OrdersSeparator = ';';
    private const char OrderDataSeparator = ':';

    public static List<UserInputOrderItem> Parse(ReadOnlySpan<char> ordersText)
    {
        var result = new List<UserInputOrderItem>();

        var sb = new StringBuilder();
        var article = string.Empty;
        var searchArticle = true;

        foreach (var c in ordersText)
        {
            if (searchArticle)
            {
                if (c == OrderDataSeparator)
                {
                    article = sb.ToString().Trim();
                    sb.Clear();
                    searchArticle = false;
                    continue;
                }

                sb.Append(c);
            }
            else
            {
                if (c == OrdersSeparator)
                {
                    result.Add(CreateOrderItem(
                        article,
                        sb.ToString()));

                    sb.Clear();
                    searchArticle = true;
                    continue;
                }

                sb.Append(c);
            }
        }

        if (string.IsNullOrEmpty(article))
        {
            throw new FormatException("Строка заказов имеет неверный формат");
        }

        if (sb.Length > 0)
        {
            if (!searchArticle)
            {
                result.Add(CreateOrderItem(
                        article,
                        sb.ToString()));
            }
            else if (!string.IsNullOrWhiteSpace(sb.ToString()))
            {
                throw new FormatException("Строка заказов имеет неверный формат");
            }
        }

        return result;
    }

    private static UserInputOrderItem CreateOrderItem(string article, string quantity)
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