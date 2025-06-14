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
        Task<(IEnumerable<AdvanceRequest> Items, int TotalCount)> GetByCreatorIdAsync(Guid creatorId, int skip, int take);
        Task<bool> HasPendingRequestAsync(Guid creatorId);
        Task<AdvanceRequest> UpdateAsync(AdvanceRequest anticipationRequest);
        Task<IEnumerable<AdvanceRequest>> GetAllAsync();
    }
}
