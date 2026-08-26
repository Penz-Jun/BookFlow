namespace BookFlow.Api.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime ReservationTime { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
