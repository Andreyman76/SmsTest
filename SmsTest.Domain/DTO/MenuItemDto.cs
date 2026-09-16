namespace SmsTest.Domain.DTO;

public sealed record MenuItemDto(
    string Id,
    string Article,
    string Name,
    double Price,
    bool IsWeighted,
    string FullPath,
    string[] Barcodes
);