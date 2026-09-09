using BookFlow.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    private static readonly List<Reservation> Reservations =
        [
            new Reservation
            {
                Id = 1,
                CustomerName = "Kim",
                StartTime = new DateTime(2026, 9, 1, 10, 0, 0),
                EndTime = new DateTime(2026, 9, 1, 10, 30, 0),
                Status = "Confirmed"
            }
        ];

    [HttpGet]
    public IEnumerable<Reservation> GetReservations() { return Reservations; }
    [HttpGet("{id:int}")]
    public ActionResult<Reservation> GetReservationById(int id)
    {
        var reservation = Reservations.FirstOrDefault(
            reservation => reservation.Id == id);

        if (reservation is null)
        {
            return NotFound();
        }

        return Ok(reservation);
    }

}