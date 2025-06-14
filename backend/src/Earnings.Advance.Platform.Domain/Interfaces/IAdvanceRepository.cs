using Earnings.Advance.Platform.Domain.Entities;

namespace Earnings.Advance.Platform.Domain.Interfaces
{
    /// <summary>
    /// Advance repository interface
    /// </summary>
    public interface IAdvanceRepository
    {
        Task<AdvanceRequest> CreateAsync(AdvanceRequest anticipationRequest);
        Task<AdvanceRequest?> GetByIdAsync(Guid id);
        Task<IEnumerable<AdvanceRequest>> GetByCreatorIdAsync(Guid creatorId);
        Task<bool> HasPendingRequestAsync(Guid creatorId);
        Task<AdvanceRequest> UpdateAsync(AdvanceRequest anticipationRequest);
        Task<IEnumerable<AdvanceRequest>> GetAllAsync();
    }
}
