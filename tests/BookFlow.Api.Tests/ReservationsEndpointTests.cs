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

        var reservation = Assert.Single(reservations);
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
}
