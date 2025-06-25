using acordemus.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using OtpNet;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;

namespace acordemus.Services
{
    public interface ILoginService
    {
        Task<IResult> SendOtpAsync(string personId);
        Task<IResult> VerifyOtpAsync(string personId, string otp);
    }
    public class LoginService(
        IWebHostEnvironment environment,
        IConfiguration configuration,
        IMongoDatabase database,
        [FromServices] DevKeys devKeys
    ) : ILoginService
    {
        private readonly IMongoCollection<Models.Otp> _otpsCollection = database.GetCollection<Models.Otp>("otps");
        private readonly IMongoCollection<Models.Person> _peopleCollection = database.GetCollection<Models.Person>("people");
        public async Task<IResult> SendOtpAsync(string email)
        {
            byte[] secretKey = Base32Encoding.ToBytes(configuration.GetValue<string>("OtpSecret"));
            string otpCode = new Totp(secretKey, mode: OtpHashMode.Sha256).ComputeTotp();

            var otpModel = new Models.Otp
            {
                email = email,
                otp = otpCode,
                expiresAt = DateTimeOffset.Now.AddMinutes(5)
            };
            var existingPerson = await _otpsCollection.Find(p => p.email == email).FirstOrDefaultAsync();
            if (existingPerson != null)
            {
                await _otpsCollection.ReplaceOneAsync(p => p.email == email, otpModel);
            }
            else
            {
                await _otpsCollection.InsertOneAsync(otpModel);
            }

            try
                {
                    if (environment.IsProduction())
                    {
                        var mailMessage = new MailMessage
                        {
                            From = new MailAddress("pgennari@gmail.com", "Acordemus"),
                            Subject = "Acordemus - Login",
                            Body = $"<p>Informe o código abaixo para verificar o email:</p><p><h1>{otpCode}</p><br/><br/><h5>Esse código expira em 5 minutos.</h5>",
                            IsBodyHtml = true,
                        };

                        mailMessage.To.Add(new MailAddress(otpModel.email));

                        var smtpAddress = configuration.GetSection("Brevo").GetValue<string>("Address");
                        var smtpPort = configuration.GetSection("Brevo").GetValue<int>("Port");
                        var smtpUsername = configuration.GetSection("Brevo").GetValue<string>("Username");
                        var smtpPassword = configuration.GetSection("Brevo").GetValue<string>("Password");
                        var smtpClient = new SmtpClient(smtpAddress)
                        {
                            Port = smtpPort,
                            Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                            EnableSsl = true,
                        };
                        smtpClient.Send(mailMessage);
                        Console.WriteLine("E-mail enviado com sucesso!");
                        return Results.Ok();
                    }
                    else
                    {
                        Console.WriteLine(otpCode);
                        return Results.Ok(otpCode);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro ao enviar e-mail: " + ex.Message);
                    return Results.InternalServerError(ex);
                }
        }

        public async Task<IResult> VerifyOtpAsync(string email, string otp)
        {
            var existingOtp = await _otpsCollection.Find(p => p.email == email).FirstOrDefaultAsync();
            if (existingOtp == null)
                return Results.BadRequest();

            if (existingOtp.otp != otp)
                return Results.BadRequest();

            if (existingOtp.expiresAt < DateTimeOffset.Now)
                return Results.BadRequest();

            existingOtp.otp = null;
            existingOtp.expiresAt = null;
            await _otpsCollection.ReplaceOneAsync(p => p.email == email, existingOtp);

            var existingPerson = await _peopleCollection.Find(p => p.email == email).FirstOrDefaultAsync();
            if (existingPerson == null || string.IsNullOrEmpty(existingPerson.id)) // Ensure Id is not null or empty
                return Results.Ok();

            var tokenHandler = new JsonWebTokenHandler();
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor()
            {
                Issuer = configuration["Jwt:Issuer"],
                Subject = new ClaimsIdentity(new[] {
                    new Claim("sub", existingPerson.id ?? string.Empty), // Use null-coalescing operator to provide a fallback value
                }),
                Audience = configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(devKeys.RsaSecurityKey, SecurityAlgorithms.RsaSha256)
            });

            return Results.Ok(new
            {
                data = new
                {
                    access_token = token,
                    token_type = "Bearer"
                }
            });
        }
    }
}