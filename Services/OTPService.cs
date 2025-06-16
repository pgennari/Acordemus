using acordemus.DTOs;
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
    public interface IOtpService
    {
        Task<IResult> SendOtpAsync(string personId);
        Task<IResult> VerifyOtpAsync(string personId, string otp);
    }
    public class OTPService(
        IWebHostEnvironment environment,
        IConfiguration configuration,
        IMongoDatabase database
    ) : IOtpService
    {
        private readonly IMongoCollection<People> _peopleCollection = database.GetCollection<People>("people");
        public async Task<IResult> SendOtpAsync(string personId)
        {
            byte[] secretKey = Base32Encoding.ToBytes(configuration.GetValue<string>("OtpSecret"));
            string otpCode = new Totp(secretKey, mode: OtpHashMode.Sha256).ComputeTotp();

            var existingPerson = await _peopleCollection.Find(p => p.Id == personId).FirstOrDefaultAsync();
            if (existingPerson == null)
                return Results.BadRequest();

            existingPerson.Otp = otpCode;
            existingPerson.ExpiresAt = DateTime.Now.AddMinutes(5);
            await _peopleCollection.ReplaceOneAsync(p => p.Id == personId, existingPerson);

            try
            {
                if (environment.IsProduction()) 
                {
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("pgennari@gmail.com", "Acordemus"),
                        Subject = "Acordemus - Login",
                        Body = $"<h3>Olá, {existingPerson.SocialName}!</h3><p>Informe o código abaixo para realizar o login:</p><p><h1>{otpCode}</p><br/><br/><h5>Esse código expira em 5 minutos.</h5>",
                        IsBodyHtml = true,
                    };

                    mailMessage.To.Add(new MailAddress(existingPerson.Email, existingPerson.SocialName));

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

        public async Task<IResult> VerifyOtpAsync(string personId, string otp)
        {
            var existingPerson = await _peopleCollection.Find(p => p.Id == personId).FirstOrDefaultAsync();
            if (existingPerson == null)
                return Results.BadRequest();

            if (existingPerson.Otp != otp)
                return Results.BadRequest();

            if (existingPerson.ExpiresAt < DateTimeOffset.Now)
                return Results.BadRequest();

            existingPerson.Otp = null;
            existingPerson.ExpiresAt = null;
            await _peopleCollection.ReplaceOneAsync(p => p.Id == personId, existingPerson);
            
            var tokenHandler = new JsonWebTokenHandler();
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor()
            {
                Issuer = configuration["Jwt:Issuer"],
                Subject = new ClaimsIdentity(new[] {
                    new Claim("sub", personId),
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