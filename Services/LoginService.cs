using acordemus.DTOs;
using acordemus.Endpoints;
using acordemus.Models;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Security.Claims;

namespace acordemus.Services
{
    public interface ILoginService
    {
        // Define methods for login service, e.g., SendOTP, VerifyOtp, etc.
        Task SendOTPAsync(string peopleId);
        Task<bool> VerifyOtpAsync(string peopleId, string otp);
    }
    public class LoginService(IMongoDatabase database, IOTPService otpService) : ILoginService
    {
        private readonly IMongoCollection<People> _peopleCollection = database.GetCollection<People>("people");
        public async Task<IResult> SendOTPAsync(string peopleId)
        {
            var existingPerson = await _peopleCollection.Find(p => p.Id == peopleId).FirstOrDefaultAsync();
            if (existingPerson == null)
                return Results.BadRequest();

            await otpService.SendOTP(peopleId);
            return Results.Created();
        }

    }
}
