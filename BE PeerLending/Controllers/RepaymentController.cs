using DAL.DTO.Req.Loan;
using DAL.DTO.Res;
using DAL.Repositories.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BE_PeerLending.Controllers
{

    [Route("rest/v1/repayment/")]
    [ApiController]
    public class RepaymentController : ControllerBase
    {
        private readonly IRepaymentServices _repaymentServices;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RepaymentController(IRepaymentServices repaymentServices, IHttpContextAccessor httpContextAccessor)
        {
            _repaymentServices = repaymentServices;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("repay-loan")]
        [Authorize(Roles = "borrower")]
        public async Task<IActionResult> RepayLoan(ReqRepaymentDto req)
        {
            var borrowerId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.UserData)?.Value;

            try
            {
                if (string.IsNullOrEmpty(borrowerId))
                {
                    return Unauthorized(new ResBaseDto<object>
                    {
                        Success = false,
                        Message = "Unauthorized",
                        Data = null
                    });
                }

                var res = await _repaymentServices.RepayLoan(req);

                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "Success repay loan",
                    Data = res
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpGet("schedule/{loanId}")]
        [Authorize(Roles = "borrower")]
        public async Task<IActionResult> GetSchedule(string loanId)
        {
            var borrowerId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.UserData)?.Value;

            try
            {
                if (string.IsNullOrEmpty(borrowerId))
                {
                    return Unauthorized(new ResBaseDto<object>
                    {
                        Success = false,
                        Message = "Unauthorized",
                        Data = null
                    });
                }

                var res = await _repaymentServices.GetRepaymentSchedule(loanId);

                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "Success get schedule",
                    Data = res
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpGet("payment-detail/{loanId}")]
        [Authorize(Roles = "borrower")]
        public async Task<IActionResult> GetPaymentDetail(string loanId)
        {
            var borrowerId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.UserData)?.Value;

            try
            {
                if (string.IsNullOrEmpty(borrowerId))
                {
                    return Unauthorized(new ResBaseDto<object>
                    {
                        Success = false,
                        Message = "Unauthorized",
                        Data = null
                    });
                }

                var res = await _repaymentServices.GetPaymentDetail(loanId);

                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "Success get payment detail",
                    Data = res
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
    }
}
