using acordemus.DTOs;
using acordemus.Models;
using acordemus.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Security.Claims;

namespace acordemus.Endpoints
{
    public static class LoginEndpoints
    {
        public static void MapLoginEndpoints(this WebApplication app)
        {

        }

        public static async Task<IResult> VerifyOtp(
            [FromBody] LoginVerify loginVerify,
            [FromServices] AppDbContext dbContext,
            [FromServices] DevKeys devKeys,
            [FromServices] IConfiguration configuration)
        {
            
        }
    }
}
