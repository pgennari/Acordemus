using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace acordemus
{
    public class DevKeys
    {
        public DevKeys(
            IWebHostEnvironment env
        )
        {
            RsaKey = RSA.Create();
            var path = Path.Combine(env.ContentRootPath, "cripto_key");
            if (File.Exists(path))
            {
                RsaKey.ImportRSAPrivateKey(File.ReadAllBytes(path), out _);
            }
            else
            {
                var privateKey = RsaKey.ExportRSAPrivateKey();
                File.WriteAllBytes(path, privateKey);
            }
        }

        public RSA RsaKey { get; }
        public RsaSecurityKey RsaSecurityKey => new RsaSecurityKey(RsaKey);
    }
}
