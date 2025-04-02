using AutoMapper;
using Domain.DTO;
using Domain.Models;
using EmailService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using StudyProj.JwtFeatures;

namespace StudyProj.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly JwtHandler _jwtHandler;
        private readonly IEmailSender _emailSender;
        public AccountsController(UserManager<User> userManager, IMapper mapper, JwtHandler jwtHandler, IEmailSender emailSender)
        {
            _userManager = userManager;
            _mapper = mapper;
            _jwtHandler = jwtHandler;
            _emailSender = emailSender;
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDto userForResitration)
        {
            if (userForResitration is null)
                return BadRequest();

            var user = _mapper.Map<User>(userForResitration);
            var result = await _userManager.CreateAsync(user, userForResitration.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);

                return BadRequest(new RegistrationResponseDto { Errors = errors});
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var param = new Dictionary<string, string>
            {
                { "token", token },
                { "email", user.Email}
            };

            var callback = QueryHelpers.AddQueryString(userForResitration.ClientUri!, param);

            var message = new Message([user.Email], "Token to confirm email", callback, null);

            await _emailSender.SendEmailAsync(message);

            await _userManager.AddToRoleAsync(user, "Chief");

            return StatusCode(201);
        }
        [HttpGet("emailconfirmation")]
        public async Task<IActionResult> EmailConfirmation([FromQuery] string email, [FromQuery] string token)
        {
            var user =  await _userManager.FindByEmailAsync(email);
            if (user is null)
                return BadRequest("Error email confirm");
            var originalToken = Uri.UnescapeDataString(token!);
            var confirmResult = await _userManager.ConfirmEmailAsync(user, originalToken);
            if (!confirmResult.Succeeded)
                return BadRequest("Error email confirm");
            return Ok("Now you can autorize");
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] UserForAuthenticationDto userForAuthentication)
        {
            var user = await _userManager.FindByNameAsync(userForAuthentication.Email!);
            if (user is null)
                return BadRequest("Ошибка запроса");

            if (!await _userManager.IsEmailConfirmedAsync(user))
                return Unauthorized(new AuthResponseDto { ErrorMessage = "Confirm email" });
            if (!await _userManager.CheckPasswordAsync(user, userForAuthentication.Password!))
                return Unauthorized(new AuthResponseDto { ErrorMessage = "Autentication error" });

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtHandler.CreateToken(user,roles);

            return Ok(new AuthResponseDto { IsAuthSuccessful = true, Token = token });
        }
        [HttpPost("forgotpassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPassword)
        {
            if(!ModelState.IsValid)
                return BadRequest();

            var user = await _userManager.FindByEmailAsync(forgotPassword.Email!);
            if(user is null)
                return BadRequest("Ошибка запроса");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var param = new Dictionary<string, string?>
            {
                {"token", token },
                {"email", forgotPassword.Email! }
            };

            var callback = QueryHelpers.AddQueryString(forgotPassword.ClientUri!, param);

            var message = new Message([user.Email], "Reset password token", callback, null);

            await _emailSender.SendEmailAsync(message);

            return Ok();
        }
        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPassword)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = await _userManager.FindByEmailAsync(resetPassword.Email!);
            if (user is null)
                return BadRequest("Ошибка запроса");
            var originalToken = Uri.UnescapeDataString(resetPassword.Token!);
            var result = await _userManager.ResetPasswordAsync(user, originalToken, resetPassword.Password!);
            if(!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);

                return BadRequest(new { Errors = errors });
            }

            return Ok();
        }
    }
}
