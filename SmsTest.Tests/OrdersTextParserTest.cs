using SmsTest.ConsoleApp.Utilities;

namespace SmsTest.Tests;

public class OrdersTextParserTest
{
    [Fact]
    public void Parse_ValidInput_ReturnsItems()
    {
        var result1 =
            OrdersTextParser.Parse("A1004292:2;A1004293:1.5;A1004294:2,3");

        Assert.Equal(3, result1.Count);

        Assert.Equal("A1004292", result1[0].Article);
        Assert.Equal(2, result1[0].Quantity);

        Assert.Equal("A1004293", result1[1].Article);
        Assert.Equal(1.5, result1[1].Quantity);

        Assert.Equal("A1004294", result1[2].Article);
        Assert.Equal(2.3, result1[2].Quantity);

        var result2 =
            OrdersTextParser.Parse("A1004292:2;A1004293:1.5;A1004294:2,3;");

        Assert.Equal(3, result2.Count);

        Assert.Equal("A1004292", result2[0].Article);
        Assert.Equal(2, result2[0].Quantity);

        Assert.Equal("A1004293", result2[1].Article);
        Assert.Equal(1.5, result2[1].Quantity);

        Assert.Equal("A1004294", result2[2].Article);
        Assert.Equal(2.3, result2[2].Quantity);
    }

    [Fact]
    public void Parse_ValidInputWithSpaces_ReturnsItems()
    {
        var result1 =
            OrdersTextParser.Parse("    A1004292 : 2  ;A1004293 :1.5 ;  A1004294:2,3 ");

        Assert.Equal(3, result1.Count);

        Assert.Equal("A1004292", result1[0].Article);
        Assert.Equal(2, result1[0].Quantity);

        Assert.Equal("A1004293", result1[1].Article);
        Assert.Equal(1.5, result1[1].Quantity);

        Assert.Equal("A1004294", result1[2].Article);
        Assert.Equal(2.3, result1[2].Quantity);

        var result2 =
            OrdersTextParser.Parse("A1004292: 2;A1004293 :1.5 ;    A1004294:2,3 ; ");

        Assert.Equal(3, result2.Count);

        Assert.Equal("A1004292", result2[0].Article);
        Assert.Equal(2, result2[0].Quantity);

        Assert.Equal("A1004293", result2[1].Article);
        Assert.Equal(1.5, result2[1].Quantity);

        Assert.Equal("A1004294", result2[2].Article);
        Assert.Equal(2.3, result2[2].Quantity);
    }

    [Fact]
    public void Parse_InvalidInput_ThrowsException()
    {
        Assert.Throws<FormatException>(() =>
        {
            OrdersTextParser.Parse("A1004292");
        });

        Assert.Throws<FormatException>(() =>
        {
            OrdersTextParser.Parse("A1004292:");
        });

        Assert.Throws<FormatException>(() =>
        {
            OrdersTextParser.Parse("A1004292:123:A1004293");
        });

        Assert.Throws<FormatException>(() =>
        {
            OrdersTextParser.Parse("  A1004292 :  123  : A1004293  ");
        });

        Assert.Throws<FormatException>(() =>
        {
            OrdersTextParser.Parse("A1004292:;A1004293:123");
        });

        Assert.Throws<FormatException>(() =>
        {
            OrdersTextParser.Parse("A1004292:123qwe");
        });

        Assert.Throws<FormatException>(() =>
        {
            OrdersTextParser.Parse(":123");
        });

        Assert.Throws<FormatException>(() =>
        {
            OrdersTextParser.Parse(":123;");
        });

        Assert.Throws<FormatException>(() =>
        {
            OrdersTextParser.Parse("::123");
        });

        Assert.Throws<FormatException>(() =>
        {
            OrdersTextParser.Parse(";123");
        });

        Assert.Throws<FormatException>(() =>
        {
            OrdersTextParser.Parse(";;123");
        });

        Assert.Throws<FormatException>(() =>
        {
            OrdersTextParser.Parse("A1004292;");
        });

        Assert.Throws<FormatException>(() =>
        {
            OrdersTextParser.Parse("A1004292:123;;");
        });

        Assert.Throws<FormatException>(() =>
        {
            OrdersTextParser.Parse("A1004292:123;:;");
        });

        Assert.Throws<FormatException>(() =>
        {
            OrdersTextParser.Parse(":");
        });

        Assert.Throws<FormatException>(() =>
        {
            OrdersTextParser.Parse(";");
        });

        Assert.Throws<FormatException>(() =>
        {
            OrdersTextParser.Parse("");
        });

        Assert.Throws<FormatException>(() =>
        {
            OrdersTextParser.Parse(" ");
        });

        Assert.Throws<FormatException>(() =>
        {
            OrdersTextParser.Parse("A1:1;;A2:2;");
        });

        Assert.Throws<FormatException>(() =>
        {
            OrdersTextParser.Parse("  A1004292:123; :123");
        });
    }
}