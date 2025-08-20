using acordemus.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Security.Claims;

namespace acordemus.Middleware
{
    public class ExternalTokenValidatorMiddleware(
        RequestDelegate next,
        IConfiguration configuration,
        [FromServices] IMemoryCache cache,
        [FromServices] DevKeys devKeys
    )
    {

        public async Task Invoke(HttpContext context, IServiceScopeFactory scopeFactory)
        {
            // Verifica se a rota exige autorização
            var endpoint = context.GetEndpoint();
            var authorizeAttribute = endpoint?.Metadata.GetMetadata<AuthorizeAttribute>();

            if (authorizeAttribute == null)
            {
                // Não exige autorização => segue direto
                await next(context);
                return;
            }

            if (Environment.GetEnvironmentVariable("REQUIRE_AUTHORIZATION").Equals("False"))
            {
                AddClaimsToContext(context, "6852f8de1e618658ce399d3b", scopeFactory);

                await next(context);
                return;
            }



            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Token ausente.");
                return;
            }

            if (cache.TryGetValue(token, out ValidationResponse cachedValidation))
            {
                if (!cachedValidation.Valid)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Token inválido (cache).");
                    return;
                }

                AddClaimsToContext(context, cachedValidation.UserId, scopeFactory);
                await next(context);
                return;
            }

            var tokenHandler = new JsonWebTokenHandler();
            var tokenValidationResult = await tokenHandler.ValidateTokenAsync(
                token,
                new TokenValidationParameters
                {
                    IssuerSigningKey = devKeys.RsaSecurityKey,
                    ValidAudience = configuration["Jwt:Audience"],
                    ValidIssuer = configuration["Jwt:Issuer"]
                }
            );

            if (tokenValidationResult.Exception != null)
            {
                cache.Set(token, new ValidationResponse { Valid = false }, TimeSpan.FromMinutes(1)); // cache negativo curto
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Token inválido.");
                return;
            }

            var validationResult = new ValidationResponse() { UserId = tokenValidationResult.Claims["sub"].ToString() , Valid = true};
            cache.Set(token, validationResult, TimeSpan.FromMinutes(15)); // cache positivo

            AddClaimsToContext(context, validationResult.UserId, scopeFactory);

            await next(context);
        }

        private void AddClaimsToContext(HttpContext context, string userId, IServiceScopeFactory scopeFactory)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
                var members = database.GetCollection<Member>("membership");

                // Busca o membro pelo PersonId
                var member = members.Find(m => m.PersonId == userId).FirstOrDefault();

                var claims = new List<Claim> { new Claim("sub", userId) };

                if (member != null && member.Roles != null)
                {
                    foreach (var role in member.Roles)
                    {
                        // Adiciona cada role como claim
                        if (!string.IsNullOrEmpty(role?.Name))
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role.Name));
                        }
                    }
                }

                var identity = new ClaimsIdentity(claims, "Custom");
                context.User = new ClaimsPrincipal(identity);

            }
        }

        public class ValidationResponse
        {
            public bool Valid { get; set; }
            public string UserId { get; set; }
        }

    }
}
