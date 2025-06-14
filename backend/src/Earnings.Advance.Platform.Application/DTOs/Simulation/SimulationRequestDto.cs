using System.ComponentModel.DataAnnotations;

namespace Earnings.Advance.Platform.Application.DTOs.Simulation
{
    /// <summary>
    /// DTO for advance simulation
    /// </summary>
    public class SimulationRequestDto
    {
        [Required]
        [Range(100.01, double.MaxValue, ErrorMessage = "Amount must be bigger than R$ 100.00")]
        public decimal Amount { get; set; }
    }
}
