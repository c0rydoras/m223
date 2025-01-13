namespace Bank.DbAccess.Repositories;

public interface IBookingRepository
{
    void AddBooking(int sourceId, int destinationId, Decimal amount);
}