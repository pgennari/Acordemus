using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Driver;
using System.Security.Claims;

namespace acordemus.Middleware
{
    public class ExternalTokenValidatorMiddleware(
        RequestDelegate next,
        [FromServices] IMemoryCache cache,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        IHostEnvironment host
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
                AddClaimsToContext(context, "7e8436d9-0af8-4b70-9868-f2628a1aaa6c", scopeFactory);

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

            var client = httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync($"{configuration["AppSettings:url_auth"]}/auth/validate", new { token });

            if (!response.IsSuccessStatusCode)
            {
                cache.Set(token, new ValidationResponse { Valid = false }, TimeSpan.FromMinutes(1)); // cache negativo curto
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Token inválido.");
                return;
            }

            var validationResult = await response.Content.ReadFromJsonAsync<ValidationResponse>();
            cache.Set(token, validationResult, TimeSpan.FromMinutes(15)); // cache positivo

            AddClaimsToContext(context, validationResult.UserId, scopeFactory);

            await next(context);
        }

        private void AddClaimsToContext(HttpContext context, string userId, IServiceScopeFactory scopeFactory)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();

                var claims = new List<Claim> { new Claim("sub", userId) };
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
