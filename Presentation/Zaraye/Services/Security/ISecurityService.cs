using Zaraye.Models.Api.V4.Buyer;
using Zaraye.Models.Api.V4.Security;

namespace Zaraye.Services.Security
{
    public interface ISecurityService
    {
        Task<string> PhoneNumberAuthenticationOtp(string phoneNumber);
        Task<object> Login(Models.Api.V4.Security.AccountApiModel.LoginApiModel model);
        Task<object> Tijara_Register(Models.Api.V4.Security.AccountApiModel.RegisterApiModel model);
        Task<string> Consumer_Register(Models.Api.V4.Security.AccountApiModel.RegisterApiModel model);
        Task<string> ChangePassword(Models.Api.V4.Security.AccountApiModel.ChangePasswordApiModel model);
        Task<string> PasswordRecoverySend(Models.Api.V4.Security.AccountApiModel.RequestPasswordApiModel model);
        Task<string> AccountDeactivate(string comment);
        Task<object> BookerLogin(Models.Api.V4.Security.AccountApiModel.LoginApiModel model);
        Task<object> TijaraLogin(Models.Api.V4.Security.AccountApiModel.LoginApiModel model);
        Task<string> BuyerContractSendOTP(string email);
        Task<string> BuyerContractVerifyOTP(string email, string otp);
        Task<string> ChangeBuyerPassword(ChangeBuyerPasswordApiModel model);
        Task<string> SupplierRegisterationAsync(RegisterSupplierRequest registerSupplierRequest);
        Task<bool> Logout();
    }
}
