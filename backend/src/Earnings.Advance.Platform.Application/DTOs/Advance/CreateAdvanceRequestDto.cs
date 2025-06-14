using Earnings.Advance.Platform.Application.Validations;
using System.ComponentModel.DataAnnotations;

namespace Earnings.Advance.Platform.Application.DTOs.Advance
{
    /// <summary>
    /// DTO for advance request creation
    /// </summary>
    public class CreateAdvanceRequestDto
    {
        [Required(ErrorMessage = "Creator ID is required")]
        [NotEmptyGuid(ErrorMessage = "Creator ID is required")]
        public Guid CreatorId { get; set; }

        [Required(ErrorMessage = "Requested amount is required")]
        [MinimumAmount]
        public decimal RequestedAmount { get; set; }
    }
}
