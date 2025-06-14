using Earnings.Advance.Platform.Application.DTOs.Advance;
using Earnings.Advance.Platform.Application.DTOs.Common;
using Earnings.Advance.Platform.Application.DTOs.Simulation;
using Earnings.Advance.Platform.Application.Interfaces;
using Earnings.Advance.Platform.Domain.Entities;
using Earnings.Advance.Platform.Domain.Exceptions;
using Earnings.Advance.Platform.Domain.Interfaces;

namespace Earnings.Advance.Platform.Application.Services
{
    /// <summary>
    /// Service responsible by the business logic of anticipation
    /// </summary>
    public class AdvanceService : IAdvanceService
    {
        private readonly IAdvanceRepository _anticipationRepository;
        private const decimal ANTICIPATION_FEE_RATE = 0.05m;

        public AdvanceService(IAdvanceRepository anticipationRepository)
        {
            _anticipationRepository = anticipationRepository;
        }

        /// <summary>
        /// Create a new anticipation request
        /// </summary>
        public async Task<AdvanceRequestResponseDto> CreateAdvanceAsync(CreateAdvanceRequestDto dto)
        {
            // Verify if creator already has a peding request
            var hasPendingRequest = await _anticipationRepository.HasPendingRequestAsync(dto.CreatorId);
            if (hasPendingRequest)
                throw new BusinessException("Creator already has a pending request");

            // Create the domain entity
            var anticipationRequest = new AdvanceRequest(
                dto.CreatorId,
                dto.RequestedAmount,
                DateTime.UtcNow
            );

            // Persists
            var created = await _anticipationRepository.CreateAsync(anticipationRequest);

            // Return the response DTO
            return MapToResponseDto(created);
        }

        /// <summary>
        /// Get requests by creator ID
        /// </summary>
        public async Task<PagedResultDto<AdvanceRequestResponseDto>> GetByCreatorIdAsync(Guid creatorId, int pageNumber, int pageSize)
        {
            var skip = (pageNumber - 1) * pageSize;
            var (items, totalCount) = await _anticipationRepository.GetByCreatorIdAsync(creatorId, skip, pageSize);

            var dtos = items.Select(MapToResponseDto);

            return new PagedResultDto<AdvanceRequestResponseDto>(dtos, totalCount, pageNumber, pageSize);
        }

        /// <summary>
        /// Approve an anticipation request
        /// </summary>
        public async Task<AdvanceRequestResponseDto> ApproveAsync(Guid id)
        {
            var anticipationRequest = await _anticipationRepository.GetByIdAsync(id);
            if (anticipationRequest == null)
            {
                throw new BusinessException("Request not found");
            }

            anticipationRequest.Approve();
            var updated = await _anticipationRepository.UpdateAsync(anticipationRequest);

            return MapToResponseDto(updated);
        }

        /// <summary>
        /// Reject an anticipation request
        /// </summary>
        public async Task<AdvanceRequestResponseDto> RejectAsync(Guid id)
        {
            var anticipationRequest = await _anticipationRepository.GetByIdAsync(id);
            if (anticipationRequest == null)
            {
                throw new BusinessException("Request not found");
            }

            anticipationRequest.Reject();
            var updated = await _anticipationRepository.UpdateAsync(anticipationRequest);

            return MapToResponseDto(updated);
        }

        /// <summary>
        /// Simulate an anticipation without creating the request
        /// </summary>
        public async Task<SimulationResponseDto> SimulateAsync(decimal amount)
        {
            var fee = amount * ANTICIPATION_FEE_RATE;
            var netAmount = amount - fee;

            return new SimulationResponseDto
            {
                RequestedAmount = amount,
                Fee = fee,
                NetAmount = netAmount,
                FeeRate = ANTICIPATION_FEE_RATE
            };
        }

        /// <summary>
        /// Map entity to response DTO
        /// </summary>
        private static AdvanceRequestResponseDto MapToResponseDto(AdvanceRequest entity)
        {
            return new AdvanceRequestResponseDto
            {
                Id = entity.Id,
                CreatorId = entity.CreatorId,
                RequestedAmount = entity.RequestedAmount,
                Fee = entity.Fee,
                NetAmount = entity.NetAmount,
                Status = entity.Status.ToString().ToLowerInvariant(),
                RequestDate = entity.RequestDate,
                ProcessedDate = entity.ProcessedDate
            };
        }
    }
}
