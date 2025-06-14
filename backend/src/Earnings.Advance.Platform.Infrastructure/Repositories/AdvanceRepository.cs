using Earnings.Advance.Platform.Domain.Entities;
using Earnings.Advance.Platform.Domain.Enums;
using Earnings.Advance.Platform.Domain.Interfaces;
using Earnings.Advance.Platform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Earnings.Advance.Platform.Infrastructure.Repositories
{
    /// <summary>
    /// Anticipation repository implementation
    /// </summary>
    public class AdvanceRepository : IAdvanceRepository
    {
        private readonly ApplicationDbContext _context;

        public AdvanceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AdvanceRequest> CreateAsync(AdvanceRequest anticipationRequest)
        {
            _context.AnticipationRequests.Add(anticipationRequest);
            await _context.SaveChangesAsync();
            return anticipationRequest;
        }

        public async Task<AdvanceRequest?> GetByIdAsync(Guid id)
        {
            return await _context.AnticipationRequests
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<(IEnumerable<AdvanceRequest> Items, int TotalCount)> GetByCreatorIdAsync(Guid creatorId, int skip, int take)
        {
            var query = _context.AnticipationRequests
                .Where(x => x.CreatorId == creatorId);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.RequestDate)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<bool> HasPendingRequestAsync(Guid creatorId)
        {
            return await _context.AnticipationRequests
                .AnyAsync(x => x.CreatorId == creatorId && x.Status == AdvanceStatus.Pending);
        }

        public async Task<AdvanceRequest> UpdateAsync(AdvanceRequest anticipationRequest)
        {
            _context.AnticipationRequests.Update(anticipationRequest);
            await _context.SaveChangesAsync();
            return anticipationRequest;
        }

        public async Task<IEnumerable<AdvanceRequest>> GetAllAsync()
        {
            return await _context.AnticipationRequests
                .OrderByDescending(x => x.RequestDate)
                .ToListAsync();
        }
    }
}
