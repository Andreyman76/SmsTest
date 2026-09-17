namespace SmsTest.Contracts.Http;

public sealed record HttpMenuItemDto(
    string Id,
    string Article,
    string Name,
    double Price,
    bool IsWeighted,
    string FullPath,
    string[] Barcodes
);