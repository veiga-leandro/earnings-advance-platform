namespace Earnings.Advance.Platform.Application.DTOs.Advance
{
    /// <summary>
    /// DTO of advance request response
    /// </summary>
    public class AdvanceRequestResponseDto
    {
        public Guid Id { get; set; }
        public Guid CreatorId { get; set; }
        public decimal RequestedAmount { get; set; }
        public decimal Fee { get; set; }
        public decimal NetAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public DateTime? ProcessedDate { get; set; }
    }
}
