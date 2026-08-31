using BookFlow.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    [HttpGet]
    public IEnumerable<Reservation> GetReservations()
    {
        return new[]
        {
            new Reservation
            {
                Id = 1,
                CustomerName = "Kim",
                StartTime = new DateTime(2026, 9, 1, 10, 0, 0),
                EndTime = new DateTime(2026, 9, 1, 10, 30, 0),
                Status = "Confirmed"
            }
        };
    }
}