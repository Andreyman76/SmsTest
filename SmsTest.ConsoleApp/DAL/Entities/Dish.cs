namespace SmsTest.ConsoleApp.DAL.Entities;

internal sealed class Dish
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Article { get; set; }
    public required double Price { get; set; }
}