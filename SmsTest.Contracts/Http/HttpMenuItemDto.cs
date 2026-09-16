namespace SmsTest.Contracts.Http;

public sealed class HttpMenuItemDto
{
    public string Id { get; set; } = string.Empty;
    public string Article { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public double Price { get; set; }
    public bool IsWeighted { get; set; }
    public string FullPath { get; set; } = string.Empty;
    public string[] Barcodes { get; set; } = [];
}