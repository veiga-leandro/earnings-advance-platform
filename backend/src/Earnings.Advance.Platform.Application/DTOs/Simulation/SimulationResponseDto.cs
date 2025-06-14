namespace Earnings.Advance.Platform.Application.DTOs.Simulation
{
    /// <summary>
    /// DTO of advance simulation response
    /// </summary>
    public class SimulationResponseDto
    {
        public decimal RequestedAmount { get; set; }
        public decimal Fee { get; set; }
        public decimal NetAmount { get; set; }
        public decimal FeeRate { get; set; }
    }
}
