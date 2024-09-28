using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Repositories.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;

namespace BE_PeerLending.Controllers
{
    [Route("rest/v1/")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userservices;

        public UserController(IUserServices userServices)
        {
            _userservices = userServices;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(ReqRegisterUserDto register)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Any())
                        .Select(x => new
                        {
                            Field = x.Key,
                            Messages = x.Value.Errors.Select(e => e.ErrorMessage).ToList()
                        }).ToList();

                    var errorMessage = new StringBuilder("Validation error occured!");

                    return BadRequest(new ResBaseDto<object>
                    {
                        Success = false,
                        Message = errorMessage.ToString(),
                        Data = errors
                    });
                }

                var res = await _userservices.Register(register);

                return Ok(new ResBaseDto<string>
                {
                    Success = true,
                    Message = "User registered successfully",
                    Data = res
                });
            }
            catch (Exception ex)
            {
                if (ex.Message == "Email already used")
                {
                    return BadRequest(new ResBaseDto<object>
                    {
                        Success = false,
                        Message = ex.Message,
                        Data = null
                    });
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpGet("users")]
        [Authorize (Roles = "admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userservices.GetAllUsers();

                return Ok(new ResBaseDto<List<ResUserDto>>
                {
                    Success = true,
                    Message = "List of users",
                    Data = users
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

        [HttpPost("login")]
        public async Task<IActionResult> Login(ReqLoginDto loginDto)
        {
            try
            {
                var response = await _userservices.Login(loginDto);
                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "User login success",
                    Data = response
                });
            }
            catch (Exception ex)
            {
               if(ex.Message == "Invalid email or password")
               {
                    return BadRequest(new ResBaseDto<string>
                    {
                        Success = false,
                        Message = ex.Message,
                        Data = null
                    });
                }

                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });

            }

        }


        [HttpDelete("user/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var response = await _userservices.Delete(id);
                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "User berhasil di delete",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                if (ex.Message == "User not found")
                {
                    return BadRequest(new ResBaseDto<string>
                    {
                        Success = false,
                        Message = ex.Message,
                        Data = null
                    });
                }

                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });

            }

        }

        [HttpPut("user/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdatebyAdmin(string id, ReqUpdateAdminDto reqUpdate)
        {
            try
            {
                var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.UserData)?.Value;
                var role = User?.IsInRole("admin");

                if (userRole != "admin")
                {
                    var res = await _userservices.UpdateUser(reqUpdate.Name, id);
                    return Ok(new ResBaseDto<object>
                    {
                        Success = true,
                        Message = "User Updated! ",
                        Data = res,
                    });
                }
                else
                {
                    var res = await _userservices.UpdateUserbyAdmin(reqUpdate, id);
                    return Ok(new ResBaseDto<object>
                    {
                        Success = true,
                        Message = "User Updated! by admin",
                        Data = res,
                    });
                }

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null,
                });
            }
        }


        [HttpPut("user/balance")]
        [Authorize(Roles = "lender")]
        public async Task<IActionResult> AddBalance(ReqUpdateBalanceDto reqUpdate)
        {
            try
            {
                var id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.UserData)?.Value;


                var res = await _userservices.UpdateBalance(reqUpdate, id);
                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "Balance Added! ",
                    Data = res,
                });

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null,
                });
            }
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUserData()
        {
            try
            {
                var id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.UserData)?.Value;
                var response = await _userservices.GetById(id);
                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "User found",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                if (ex.Message == "User not found")
                {
                    return BadRequest(new ResBaseDto<string>
                    {
                        Success = false,
                        Message = ex.Message,
                        Data = null
                    });
                }

                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });

            }

        }


        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUserbyId( string id)
        {
            try
            {
                var response = await _userservices.GetById(id);
                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "User found",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                if (ex.Message == "User not found")
                {
                    return BadRequest(new ResBaseDto<string>
                    {
                        Success = false,
                        Message = ex.Message,
                        Data = null
                    });
                }

                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });

            }

        }
    }
}