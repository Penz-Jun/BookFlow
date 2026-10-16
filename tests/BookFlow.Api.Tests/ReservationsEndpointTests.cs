using System.Net;
using System.Net.Http.Json;
using BookFlow.Api.Models;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BookFlow.Api.Tests;

public class ReservationsEndpointTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ReservationsEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetReservations_ReturnsOkAndReservationList()
    {
        // Act
        var response = await _client.GetAsync("/api/reservations");

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var reservations = await response.Content.ReadFromJsonAsync<List<Reservation>>();

        Assert.NotNull(reservations);

        var reservation = reservations.FirstOrDefault(item => item.Id == 1); // (수정)var 형태의 reservation을 정의하고 FirstOrDefault(item => item.Id == 1)란 목록의 각 예약을 차례로 살펴보고, ID가 1인 첫 예약을 반환한다 ( 여기서 id가 1인 예약이 없다면 null을 반호나하게 된다)
        Assert.NotNull(reservation); // reservation 값이 null값이 아닌경우를 Assert한다 (응답검사를 한다)
        Assert.Equal(1, reservation.Id);
        Assert.Equal("Kim", reservation.CustomerName);
        Assert.Equal("Confirmed", reservation.Status);
        Assert.Equal(new DateTime(2026, 9, 1, 10, 0, 0), reservation.StartTime);
        Assert.Equal(new DateTime(2026, 9, 1, 10, 30, 0), reservation.EndTime);
    }
    [Fact]
    public async Task GetReservationById_WhenRservationExists_ReturnsOkAndReservation()
    {
        // Act
        var response = await _client.GetAsync("/api/reservations/1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var reservation =
            await response.Content.ReadFromJsonAsync<Reservation>();

        Assert.NotNull(reservation);
        Assert.Equal(1, reservation.Id);
        Assert.Equal("Kim", reservation.CustomerName);
        Assert.Equal(
            new DateTime(2026, 9, 1, 10, 0, 0),
            reservation.StartTime);
        Assert.Equal(
            new DateTime(2026, 9, 1, 10, 30, 0),
            reservation.EndTime);
        Assert.Equal("Confirmed", reservation.Status);
    }
    [Fact]
    public async Task GetReservationById_WhenReservationDoesNotExist_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/reservations/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact] //(수정) C#의 어트리뷰트를 나타내는 것이 []로 나타냄 [Fact]는 아래 메서드는 테스트다 라고 표시하는 기능
    public async Task CreateReservation_ValidRequest_ReturnsCreated() // (수정)메서드를 정의할 때 async는 메서드 안에서 await을 쓸수있게 하고 이는 비동기 방식으로 동작하는 메서드 라는 의미임 Task는 비동기 작업의 완료를 나타냄
    {
        // Arrange: 보낼 예약 정보 준비
        var request = new CreateReservationRequest // var형태의 request변수를 정의하고 CreateReservationRequest 클래스의 객체로 지정한다
        {
            CustomerName = "Lee",
            StartTime = new DateTime(2026, 10, 20, 10, 0, 0),
            EndTime = new DateTime(2026, 10, 20, 10, 30, 0)
        };
        // Act: POST 요청
        var response = await _client.PostAsJsonAsync("/api/reservations", request); // (수정)PostAsJsonAsync는 JSON으로 POST요청을 보내는것. await는 응답이 올때까지 비동기작업의 완료를 기다린 후 그 결과값을 가져오는 것

        // Assert 응답 검사
        Assert.Equal(HttpStatusCode.Created, response.StatusCode); // Assert는 응답한 결과값, 그리고 .Equal은 그것과 같냐 라고 물어보는 것이고 첫번째 인수는 예상값, 즉 Created(201)라는 값이 나와야하는 것이고 response에서 가져온 StatusCode실제값 이 둘이 Equal이냐(동등하냐)라고 물어보는것

        var created = await response.Content.ReadFromJsonAsync<Reservation>(); // (수정) 위 메소드에서 실행한 JSON을 Reservation객체로 변환시키는 것
        Assert.NotNull(created); // 만들어진 created라는 변수가 Null값이 아닌지 확인
        Assert.True(created.Id > 0);//만들어진 created의 변수의 id값이 1보다 큰것이 True로 반환되는지 확인
        Assert.Equal(request.CustomerName, created.CustomerName); //아까 포스트요청 넣은 값에서의(request) CustomerName과 지금 만들어져서 조회하고 있는 created의 CustomerName이 동일한지 확인
        Assert.Equal(request.StartTime, created.StartTime); //위와 같은맥락
        Assert.Equal(request.EndTime, created.EndTime); //위와 같은 맥락
        Assert.Equal("Confirmed", created.Status); // confirmed라는 값이 만들어진 created의 Status값인지 아닌지 확인

        Assert.NotNull(response.Headers.Location); // response에 들어간 Headers Location값이 Null이 아닌것을 확인

        var detailResponse = await _client.GetAsync(response.Headers.Location); // (수정)POST 응답의 Location 주소로 GET 요청을 보내고, 돌아온 HTTP 응답 전체를 detailResponse에 저장한다.
        Assert.Equal(HttpStatusCode.OK, detailResponse.StatusCode); // (수정)위의 HTTP응답값이 스테이터스코드 200(OK)인지 확인한다

        var detail = await detailResponse.Content.ReadFromJsonAsync<Reservation>(); //detail이라는 변수를 만들고 거기에 detailResponse에서 실행한 JSON을 Reservation 객체로 변환시키는것
        Assert.NotNull(detail); // detail의 값이 null이 아닌것을 확인
        Assert.Equal(created.Id, detail.Id); // 아까 만든 created의 id와 지금만든 detail의 id값이 동일한지 확인

    }

    [Fact]
    public async Task CreateReservation_BlankName_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateReservationRequest
        {
            CustomerName = "   ",
            StartTime = new DateTime(2026, 10, 20, 10, 0, 0),
            EndTime = new DateTime(2026, 10, 20, 10, 30, 0)
        };
        // Act
        var response = await _client.PostAsJsonAsync("/api/reservations", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    [Fact]
    public async Task CreateReservation_EndTimeEqualsStartTime_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateReservationRequest
        {
            CustomerName = "LEE",
            StartTime = new DateTime(2026, 10, 20, 10, 30, 0),
            EndTime = new DateTime(2026, 10, 20, 10, 30, 0)
        };
        // Act
        var response = await _client.PostAsJsonAsync("/api/reservations", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    [Fact]
    public async Task CreateReservation_EndTimeBeforeThanStartTime_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateReservationRequest
        {
            CustomerName = "LEE",
            StartTime = new DateTime(2026, 10, 20, 10, 30, 0),
            EndTime = new DateTime(2026, 10, 20, 10, 0, 0)
        };
        // Act
        var response = await _client.PostAsJsonAsync("/api/reservations", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
