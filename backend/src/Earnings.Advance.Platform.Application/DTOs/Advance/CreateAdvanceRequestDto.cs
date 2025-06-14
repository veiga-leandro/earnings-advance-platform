using System.ComponentModel.DataAnnotations;

namespace Earnings.Advance.Platform.Application.DTOs.Advance
{
    /// <summary>
    /// DTO for advance request creation
    /// </summary>
    public class CreateAdvanceRequestDto
    {
        [Required(ErrorMessage = "Creator ID is required")]
        public Guid CreatorId { get; set; }

        [Required(ErrorMessage = "Requested amount is required")]
        [Range(100.01, double.MaxValue, ErrorMessage = "Amount must be bigger than R$ 100.00")]
        public decimal RequestedAmount { get; set; }
    }
}
