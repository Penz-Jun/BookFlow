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
        _client= factory.CreateClient();
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

        var reservation = Assert.Single(reservations);
        Assert.Equal(1, reservation.Id);
        Assert.Equal("Kim", reservation.CustomerName);
        Assert.Equal("Confirmed", reservation.Status);
    }
}
