using System.Data;
using Bank.Web.Dto;
using Bank.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Bank.Web.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class BookingsController(IBookingService bookingService) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Administrators")]
    public async Task<IActionResult> Post([FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] BookingDto booking)
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
            } catch (Exception e)
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