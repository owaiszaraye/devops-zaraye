using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zaraye.Core.Domain;
using Zaraye.Core.Http;
using Zaraye.Models.Api.V4.Buyer;
using Zaraye.Models.Api.V4.Security;
using Zaraye.Services.Logging;
using Zaraye.Services.Security;

namespace Zaraye.Controllers.V4.Security
{
    [ApiController]
    [Produces("application/json")]
    [ApiVersion("4")]
    [Route("v{version:apiVersion}/security")]
    public class SecurityController : BaseApiController
    {
        #region Fields
        private readonly ISecurityService _securityService;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly IAppLoggerService _appLoggerService;
        #endregion

        #region Ctor

        public SecurityController(ISecurityService securityService, StoreInformationSettings storeInformationSettings, IAppLoggerService appLoggerService)
        {
            _securityService = securityService;
            _storeInformationSettings = storeInformationSettings;
            _appLoggerService = appLoggerService;
        }

        #endregion

        #region Phone Authentication / Login / Register

        [HttpPost("phone-number-authentication-otp")]
        public async Task<IActionResult> PhoneNumberAuthenticationOtp(string phoneNumber)
        {
            try
            {
                var data = await _securityService.PhoneNumberAuthenticationOtp(phoneNumber);
                return Ok(new { success = true, message = data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AccountApiModel.LoginApiModel model)
        {
            try
            {
                var data = await _securityService.Login(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Tijara_Register([FromBody] AccountApiModel.RegisterApiModel model)
        {
            try
            {
                var data = await _securityService.Tijara_Register(model);
                return Ok(new { success = true, message = data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("consumer-register")]
        public async Task<IActionResult> Consumer_Register([FromBody] AccountApiModel.RegisterApiModel model)
        {
            try
            {
                var data = await _securityService.Consumer_Register(model);
                return Ok(new { success = true, message = data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Other Methods

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {

            try
            {
                var data = await _securityService.Logout();
                //EU Cookie
                if (_storeInformationSettings.DisplayEuCookieLawWarning)
                {
                    //the cookie law message should not pop up immediately after logout.
                    //otherwise, the user will have to click it again...
                    //and thus next visitor will not click it... so violation for that cookie law..
                    //the only good solution in this case is to store a temporary variable
                    //indicating that the EU cookie popup window should not be displayed on the next page open (after logout redirection to homepage)
                    //but it'll be displayed for further page loads
                    TempData[$"{ZarayeCookieDefaults.Prefix}{ZarayeCookieDefaults.IgnoreEuCookieLawWarning}"] = true;
                }

                return Ok(new { success = true, message = "" });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [AuthorizeApi]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] AccountApiModel.ChangePasswordApiModel model)
        {
            try
            {
                var data = await _securityService.ChangePassword(model);
                return Ok(new { success = true, message = data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("password-recovery-send")]
        public async Task<IActionResult> PasswordRecoverySend([FromBody] AccountApiModel.RequestPasswordApiModel model)
        {
            try
            {
                var data = await _securityService.PasswordRecoverySend(model);
                return Ok(new { success = true, message = data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [AuthorizeApi]
        [HttpPost("account-deactivate")]
        public async Task<IActionResult> AccountDeactivate(string comment)
        {
            try
            {
                var data = await _securityService.AccountDeactivate(comment);
                return Ok(new { success = true, message = data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Booker Login

        [AllowAnonymous]
        [HttpPost("booker-login")]
        public async Task<IActionResult> BookerLogin([FromBody] AccountApiModel.LoginApiModel model)
        {
            try
            {
                var data = await _securityService.BookerLogin(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Tijara Login

        [AllowAnonymous]
        [HttpPost("tijara-login")]
        public async Task<IActionResult> TijaraLogin([FromBody] AccountApiModel.LoginApiModel model)
        {
            try
            {
                var data = await _securityService.TijaraLogin(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Buyer Contract Authorization

        [HttpPost("buyer-send-otp/{email}")]
        public async Task<IActionResult> BuyerContractSendOTP(string email)
        {
            try
            {
                var data = await _securityService.BuyerContractSendOTP(email);
                return Ok(new { success = true, message = data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("buyer-verify-otp/{email}/{otp}")]
        public async Task<IActionResult> BuyerContractVerifyOTP(string email, string otp)
        {
            try
            {
                var data = await _securityService.BuyerContractVerifyOTP(email, otp);
                return Ok(new { success = true, message = data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("buyer-change-password")]
        public async Task<IActionResult> ChangeBuyerPassword([FromBody] ChangeBuyerPasswordApiModel model)
        {
            try
            {
                var data = await _securityService.ChangeBuyerPassword(model);
                return Ok(new { success = true, message = data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Supplier
        [AllowAnonymous]
        [HttpPost("register-supplier")]
        public async Task<IActionResult> SupplierRegisterationAsync([FromBody] RegisterSupplierRequest registerSupplierRequest)
        {
            try
            {
                var data = await _securityService.SupplierRegisterationAsync(registerSupplierRequest);
                return Ok(new { success = true, message = data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }
        #endregion
    }
}
