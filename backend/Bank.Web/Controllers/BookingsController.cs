using System.Data;
using Bank.Core.Models;
using Bank.DbAccess.Repositories;
using Bank.Web.Dto;
using Bank.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Bank.Web.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class BookingsController(
    IBookingService bookingService,
    IBookingRepository bookingRepository
) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Administrators,Users")]
    public IEnumerable<Booking> Get()
    {
        var allBookings = bookingRepository.GetAllBookings();
        return allBookings;
    }

    [HttpGet("for/{id:int}")]
    [Authorize(Roles = "Administrators,Users")]
    public IEnumerable<Booking> GetBookingsForLedger(int id)
    {
        var bookings = bookingRepository.GetBookingsForLedger(id);
        return bookings;
    }

    [HttpPost]
    [Authorize(Roles = "Administrators")]
    public async Task<IActionResult> Post(
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] BookingDto booking
    )
    {
        return await Task.Run(() =>
        {
            IActionResult response = Ok();

            try
            {
                bookingService.Book(booking.SourceId, booking.DestinationId, booking.Amount);
            }
            catch (ConstraintException ce)
            {
                return BadRequest(ce.Message);
            }
            catch (Exception)
            {
                response = Conflict();
            }

            // Rufe "Book" im "BookingRepository" auf.
            // Noch besser wäre es, wenn du einen Service verwenden würdest, der die Geschäftslogik enthält.
            // Gib je nach Erfolg OK() oder Conflict() zurück
            return response;
        });
    }
}
