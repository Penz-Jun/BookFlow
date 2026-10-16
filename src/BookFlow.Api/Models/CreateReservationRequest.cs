namespace BookFlow.Api.Models
{
    public class CreateReservationRequest
    {
        public string CustomerName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
