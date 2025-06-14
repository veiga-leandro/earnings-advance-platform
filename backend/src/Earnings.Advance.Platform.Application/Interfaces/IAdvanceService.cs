using Earnings.Advance.Platform.Application.DTOs.Advance;
using Earnings.Advance.Platform.Application.DTOs.Common;
using Earnings.Advance.Platform.Application.DTOs.Simulation;

namespace Earnings.Advance.Platform.Application.Interfaces
{
    /// <summary>
    /// Interface of advance service
    /// </summary>
    public interface IAdvanceService
    {
        Task<AdvanceRequestResponseDto> CreateAdvanceAsync(CreateAdvanceRequestDto dto);
        Task<PagedResultDto<AdvanceRequestResponseDto>> GetByCreatorIdAsync(Guid creatorId, int pageNumber, int pageSize);
        Task<AdvanceRequestResponseDto> ApproveAsync(Guid id);
        Task<AdvanceRequestResponseDto> RejectAsync(Guid id);
        Task<SimulationResponseDto> SimulateAsync(decimal amount);
    }
}
