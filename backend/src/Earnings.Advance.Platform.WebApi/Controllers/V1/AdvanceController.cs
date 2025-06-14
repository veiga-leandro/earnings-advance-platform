using Asp.Versioning;
using Earnings.Advance.Platform.Application.DTOs.Advance;
using Earnings.Advance.Platform.Application.DTOs.Simulation;
using Earnings.Advance.Platform.Application.Interfaces;
using Earnings.Advance.Platform.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Earnings.Advance.Platform.WebApi.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [SwaggerTag("Operations for advance management")]
    public class AdvanceController : ControllerBase
    {
        private readonly IAdvanceService _advanceService;
        private readonly ILogger<AdvanceController> _logger;

        public AdvanceController(IAdvanceService anticipationService, ILogger<AdvanceController> logger)
        {
            _advanceService = anticipationService;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new advance request
        /// </summary>
        /// <param name="dto">Request data</param>
        /// <returns>Created request data</returns>
        /// <response code="201">Request created successfully</response>
        /// <response code="400">Invalid data or business rule violated</response>
        [HttpPost]
        [ProducesResponseType(typeof(AdvanceRequestResponseDto), 201)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        public async Task<IActionResult> CreateAnticipation([FromBody] CreateAdvanceRequestDto dto)
        {
            try
            {
                _logger.LogInformation("Creating advance request for creator {CreatorId}", dto.CreatorId);

                var result = await _advanceService.CreateAdvanceAsync(dto);

                _logger.LogInformation("Advance request created successfully. ID: {Id}", result.Id);

                return CreatedAtAction(
                    nameof(GetByCreatorId),
                    new { creatorId = result.CreatorId },
                    result
                );
            }
            catch (BusinessException ex)
            {
                _logger.LogWarning("Business error while creating advance: {Message}", ex.Message);
                return BadRequest(new ProblemDetails
                {
                    Title = "Business Error",
                    Detail = ex.Message,
                    Status = 400
                });
            }
        }

        /// <summary>
        /// Lists advance requests for a creator
        /// </summary>
        /// <param name="creatorId">Creator ID</param>
        /// <returns>List of creator's requests</returns>
        /// <response code="200">List returned successfully</response>
        /// <response code="400">Invalid Creator ID</response>
        [HttpGet("creator/{creatorId}")]
        [ProducesResponseType(typeof(IEnumerable<AdvanceRequestResponseDto>), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        public async Task<IActionResult> GetByCreatorId([FromRoute] Guid creatorId)
        {
            if (creatorId == Guid.Empty)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid Parameter",
                    Detail = "Creator ID is required",
                    Status = 400
                });
            }

            _logger.LogInformation("Fetching requests for creator {CreatorId}", creatorId);

            var result = await _advanceService.GetByCreatorIdAsync(creatorId);

            return Ok(result);
        }

        /// <summary>
        /// Approves an advance request
        /// </summary>
        /// <param name="id">Request ID</param>
        /// <returns>Approved request data</returns>
        /// <response code="200">Request approved successfully</response>
        /// <response code="400">Request not found or already processed</response>
        [HttpPatch("{id}/approve")]
        [ProducesResponseType(typeof(AdvanceRequestResponseDto), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        public async Task<IActionResult> Approve([FromRoute] Guid id)
        {
            try
            {
                _logger.LogInformation("Approving request {Id}", id);

                var result = await _advanceService.ApproveAsync(id);

                _logger.LogInformation("Request {Id} approved successfully", id);

                return Ok(result);
            }
            catch (BusinessException ex)
            {
                _logger.LogWarning("Error approving request {Id}: {Message}", id, ex.Message);
                return BadRequest(new ProblemDetails
                {
                    Title = "Business Error",
                    Detail = ex.Message,
                    Status = 400
                });
            }
        }

        /// <summary>
        /// Rejects an advance request
        /// </summary>
        /// <param name="id">Request ID</param>
        /// <returns>Rejected request data</returns>
        /// <response code="200">Request rejected successfully</response>
        /// <response code="400">Request not found or already processed</response>
        [HttpPatch("{id}/reject")]
        [ProducesResponseType(typeof(AdvanceRequestResponseDto), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        public async Task<IActionResult> Reject([FromRoute] Guid id)
        {
            try
            {
                _logger.LogInformation("Rejecting request {Id}", id);

                var result = await _advanceService.RejectAsync(id);

                _logger.LogInformation("Request {Id} rejected successfully", id);

                return Ok(result);
            }
            catch (BusinessException ex)
            {
                _logger.LogWarning("Error rejecting request {Id}: {Message}", id, ex.Message);
                return BadRequest(new ProblemDetails
                {
                    Title = "Business Error",
                    Detail = ex.Message,
                    Status = 400
                });
            }
        }

        /// <summary>
        /// Simulates an advance without creating the request
        /// </summary>
        /// <param name="amount">Amount for simulation</param>
        /// <returns>Simulation data</returns>
        /// <response code="200">Simulation performed successfully</response>
        /// <response code="400">Invalid amount</response>
        [HttpGet("simulate")]
        [ProducesResponseType(typeof(SimulationResponseDto), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        public async Task<IActionResult> Simulate([FromQuery] decimal amount)
        {
            if (amount <= 100)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid Amount",
                    Detail = "Amount must be greater than $100.00",
                    Status = 400
                });
            }

            _logger.LogInformation("Simulating advance for amount {Amount}", amount);

            var result = await _advanceService.SimulateAsync(amount);

            return Ok(result);
        }
    }
}