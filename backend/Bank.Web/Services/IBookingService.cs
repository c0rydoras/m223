namespace Bank.Web.Services;

public interface IBookingService
{
    void Book(int sourceId, int destinationId, decimal amount, int retryCounter = 1);
}