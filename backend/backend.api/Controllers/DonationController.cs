using AutoMapper;
using backend.api.Models.RequestModels;
using backend.api.Models.ResponseModels;
using backend.servicios.DTOs;
using backend.servicios.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace backend.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonationController(ILogger<DonationController> logger, IUserService userService, IDonationService donationService, IMapper mapper) : ControllerBase
    {
        private readonly ILogger<DonationController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IDonationService _donationService = donationService ?? throw new ArgumentNullException(nameof(donationService));
        private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        /// <summary>
        /// Save a new donation.
        /// </summary>
        /// <param name="donationRequest">The donation request model.</param>
        /// <response code="201">Returns the created donation.</response>
        /// <response code="400">If the request payload is invalid.</response>
        /// <response code="500">If there is an internal server error.</response>
        [HttpPost]
        [ProducesResponseType(typeof(DonationRequestModel), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SaveDonacion([FromBody] DonationRequestModel donationRequest)
        {
            if (donationRequest == null)
                return BadRequest("Invalid donation data");

            try
            {
                var newDonation = _mapper.Map<DonationDto>(donationRequest);
                await _donationService.SaveDonationAsync(newDonation);
                await GenerateNotification(newDonation, donationRequest.UsuarioId);

                return CreatedAtAction(nameof(SaveDonacion), new { organizacionId = donationRequest.OrganizacionId }, donationRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving donation");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get donations by user ID.
        /// </summary>
        /// <param name="usuarioId">The ID of the user.</param>
        /// <response code="200">Returns the list of donations.</response>
        /// <response code="500">If there is an internal server error.</response>
        [HttpGet("user/{usuarioId}")]
        [ProducesResponseType(typeof(IEnumerable<DonationResponseModel>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetDonationsByUserId(int usuarioId)
        {
            try
            {
                var donations = await _donationService.GetDonationsByUserIdAsync(usuarioId);
                var donationResponse = _mapper.Map<IEnumerable<DonationResponseModel>>(donations);

                return Ok(donationResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user donations");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get donations by organization ID.
        /// </summary>
        /// <param name="organizacionId">The ID of the organization.</param>
        /// <response code="200">Returns the list of donations.</response>
        /// <response code="500">If there is an internal server error.</response>
        [HttpGet("organization/{organizacionId}")]
        [ProducesResponseType(typeof(IEnumerable<DonationResponseModel>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetDonationsByOrganizationId(int organizacionId)
        {
            try
            {
                var donations = await _donationService.GetDonationsByOrganizationIdAsync(organizacionId);
                var donationResponse = _mapper.Map<IEnumerable<DonationResponseModel>>(donations);

                return Ok(donationResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving organization donations");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update the state of donations.
        /// </summary>
        /// <param name="request">The request model containing donation IDs and the new state.</param>
        /// <response code="200">If the donations' state was successfully updated.</response>
        /// <response code="500">If there is an internal server error.</response>
        [HttpPut("updateState")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateDonationsState([FromBody] UpdateDonationsStateRequest request)
        {
            try
            {
                await _donationService.UpdateDonationsStateAsync(request.DonationIds, request.State);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating donation state");
                return StatusCode(500, "Internal server error");
            }
        }

        private async Task GenerateNotification(DonationDto donation, int userId)
        {
            donation.Id = await _donationService.GetDonationIdAsync(donation);
            var user = await _userService.GetUserByIdAsync(userId);
            var response = new
            {
                donation,
                user
            };

            await WebSocketHandler.NotifyNewDonationAsync(response);
        }
    }
}
