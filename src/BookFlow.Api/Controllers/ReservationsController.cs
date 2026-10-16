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
    [HttpPost]
    // (수정)HttpPost란 이 메서드를 POST요청에 연결함 이라는 의미이다. (참고로 주소는 위의 적혀있는 [Route("api/[controller]")]에 의해 /api/reservations가 된다)
    public ActionResult<Reservation> CreateReservation( //(수정)aspnetcore의 MVC인 ActionResult는 이 메서드가 Reservation이라는 미리 지정된 객체(예약객체)나 HTTP응답을 반환할수 있게 한다
        [FromBody] CreateReservationRequest request) //(수정) [Frombody]를 통해 요청 본문의 JSON을 읽어 CreateReservationRequest형태의 객체로 변환함
    {
        if (string.IsNullOrWhiteSpace(request.CustomerName)) // 만약 string형태의 request.CustomerName값이 Null이거나 Space값이면 (즉 공백이면)
        {
            return BadRequest("Customer name is required."); // BadRequest를 통해 400을 반환하고 Customer name is required를 표시한다
        }

        if (request.EndTime <= request.StartTime) // 만약 request.EndTime이 request.StartTime과 같거나 작으면(시간이므로 더 이른 시점이라면)
        {
            return BadRequest("End time must be after start time."); // BadRequest를 통해 400을 반환하고 Customer name must be after start time을 표시한다.
        }
        var reservation = new Reservation // var형태의 변수 reservation을 정의하고 그 값을 새로운 Reservation(클래스)로서 정의한다 , 단 여기서 var는 어떤 형태의 변수이든 알아서 정의하는 변수이므로 클래스형태의 변수가 되게된다. 그리고 그 클래스에 각각 들어갈 항목은 밑에서 정의함
        {
            Id = Reservations.Max(item => item.Id) + 1, // (수정)Reservation의 Id값은 Reservation에 들어있는 항목 중 최대값을 가져오는데 이때 item이라는 임시변수를 설정하고 그 안에 Id값만 비교하라는 것이 item =>item.Id이다 이 값에 +1한 값을 Id값으로 정하라는 의미.
            CustomerName = request.CustomerName, // Reservation의 CustomerName은 request(CreateReservationRequest에서 가져온 값)의 CustomerName으로 한다
            StartTime = request.StartTime, // 위와 동일하지만 시작시간과 관련
            EndTime = request.EndTime, // 위와 동일하지만 종료시간과 관련
            Status = "Confirmed" //Reservation의 Status값을 Confirmed로 한다
        };

        Reservations.Add(reservation); // (수정)위에서 정의한 변수 reservation값을 List형태의 비공개 정적(Static) 읽기전용(Readonly)인 Reservations에 Add(추가)한다 다만 readonly여도 Add로 추가할 수 있다는 점을 알면 좋다

        return CreatedAtAction( // CreatedAtAction은 201을 반환하는 것
            nameof(GetReservationById), //(수정)상세조회 메서드인 GetReservationById메서드의 이름을 가져오고 CreatedAtAction의 첫번째 인수로 설정(이때 첫번째 인수는 이 액션의 이름을 뜻함)
            new { id = reservation.Id }, //(수정)새 예약의 ID를 상세 조회 주소의 {id}에 넣기 위해 전달한다
            reservation); // (수정)이미 만든 예약 객체를 응답 본문에 담는다
    }
}