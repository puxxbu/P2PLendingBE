using DAL.DTO.Req.Funding;
using DAL.DTO.Res;
using DAL.Repositories.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BE_PeerLending.Controllers
{
    [Route("rest/v1/funding/")]
    [ApiController]
    public class FundingController : ControllerBase
    {
        private readonly IFundingServices _fundingServices;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public FundingController(IFundingServices fundingServices , IHttpContextAccessor httpContextAccessor)
        {
            _fundingServices = fundingServices;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("lend-money")]
        [Authorize(Roles = "lender")]

        public async Task<IActionResult> LendMoney(ReqFundingDto req)
        {
            var lenderId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.UserData)?.Value;

            try
            {
               

                if (string.IsNullOrEmpty(lenderId))
                {
                    return Unauthorized(new ResBaseDto<object>
                    {
                        Success = false,
                        Message = "Unauthorized",
                        Data = null
                    });
                }


                var res = await _fundingServices.LendMoney(req.LoanId, lenderId);

                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "Success lend money",
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


        [HttpGet("funding-history")]
        [Authorize(Roles = "lender")]
        public async Task<IActionResult> FundingHistory()
        {
            try
            {
                var id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.UserData)?.Value;

                if (string.IsNullOrEmpty(id))
                {
                    return Unauthorized(new ResBaseDto<object>
                    {
                        Success = false,
                        Message = "Unauthorized",
                        Data = null
                    });
                }
                var res = await _fundingServices.FundingHistory(id);

                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "Success get funding list",
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
