using acordemus.DTOs;
using acordemus.Models;
using acordemus.Services;

namespace acordemus.Endpoints
{
    public static class LoginEndpoints
    {
        public static void MapLoginEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/login");
            group.MapPost("/", (ILoginService svc, Otp otp) => svc.SendOtpAsync(otp.email));
            group.MapPost("/verify", (ILoginService svc, LoginVerify loginVerify) => svc.VerifyOtpAsync(loginVerify.email, loginVerify.otp));

        }
    }
}
