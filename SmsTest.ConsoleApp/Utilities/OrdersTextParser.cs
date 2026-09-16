using System.Globalization;
using System.Text.RegularExpressions;

namespace SmsTest.ConsoleApp.Utilities;

internal static partial class OrdersTextParser
{
    private static readonly Regex _orderItemsRegex = OrderItemsRegex();

    public static List<UserInputOrderItem> Parse(string ordersText)
    {
        var matches = _orderItemsRegex.Matches(ordersText);

        if (matches.Count < 1)
        {
            throw new FormatException("Строка заказов имеет неверный формат");
        }

        var result = new List<UserInputOrderItem>();

        foreach (Match match in matches)
        {
            var article = match.Groups["article"].Value;
            var quantityStr = match.Groups["quantity"].Value;

            if (double.TryParse(
                quantityStr.Replace(',', '.'),
                CultureInfo.InvariantCulture,
                out var quantity))
            {
                result.Add(new UserInputOrderItem(article, quantity));
            }
            else
            {
                throw new FormatException($"Строка заказов имеет неверный формат: {quantityStr} не число");
            }
        }

        return result;
    }

    [GeneratedRegex("(?<article>[^:;]+):(?<quantity>\\d+(?:[.,]\\d+)?);")]
    private static partial Regex OrderItemsRegex();
}