namespace SmsTest.Contracts.Http;

public sealed class HttpSendOrderRequestDto
{
    public Guid OrderId { get; set; }
    public HttpOrderItem[] MenuItems { get; set; } = [];
}