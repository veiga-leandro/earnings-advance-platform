using Earnings.Advance.Platform.Domain.Constants;
using Earnings.Advance.Platform.Domain.Enums;

namespace Earnings.Advance.Platform.Domain.Entities
{
    /// <summary>
    /// Entity represents a anticipation request
    /// </summary>
    public class AdvanceRequest
    {
        public Guid Id { get; private set; }
        public Guid CreatorId { get; private set; }
        public decimal RequestedAmount { get; private set; }
        public decimal Fee { get; private set; }
        public decimal NetAmount { get; private set; }
        public AdvanceStatus Status { get; private set; }
        public DateTime RequestDate { get; private set; }
        public DateTime? ProcessedDate { get; private set; }

        // Private constructor to EF Core
        private AdvanceRequest() { }

        /// <summary>
        /// Constructor to create a new advance request
        /// </summary>
        public AdvanceRequest(Guid creatorId, decimal requestedAmount, DateTime requestDate)
        {
            if (creatorId == Guid.Empty)
                throw new ArgumentException("Creator ID is required", nameof(creatorId));

            if (requestedAmount < AdvanceConstants.MINIMUM_AMOUNT)
                throw new ArgumentException(
                    $"Requested amount must be bigger than {AdvanceConstants.FormatAmount(AdvanceConstants.MINIMUM_AMOUNT)}",
                    nameof(requestedAmount));

            Id = Guid.NewGuid();
            CreatorId = creatorId;
            RequestedAmount = requestedAmount;
            RequestDate = requestDate;
            Status = AdvanceStatus.Pending;

            CalculateFeeAndNetAmount();
        }

        /// <summary>
        /// Calculates the fee and net amount
        /// </summary>
        private void CalculateFeeAndNetAmount()
        {
            Fee = RequestedAmount * AdvanceConstants.FEE_RATE;
            NetAmount = RequestedAmount - Fee;
        }

        /// <summary>
        /// Approves the anticipation request
        /// </summary>
        public void Approve()
        {
            if (Status != AdvanceStatus.Pending)
                throw new InvalidOperationException("Only pending requests should be approved");

            Status = AdvanceStatus.Approved;
            ProcessedDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Reject the anticipation request
        /// </summary>
        public void Reject()
        {
            if (Status != AdvanceStatus.Pending)
                throw new InvalidOperationException("Only pending requests should be rejected");

            Status = AdvanceStatus.Rejected;
            ProcessedDate = DateTime.UtcNow;
        }
    }
}