using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ROWM
{
    static internal class AtcSharePointConfig
    {
        static internal AppSecret SharePointAppSecret()
        {
            var home = new System.Uri("https://atc-rowm-key.vault.azure.net/");
            var tent = new VisualStudioCredentialOptions { TenantId = "a4390e1c-661f-4cab-a1cd-a2a9d8508b98" };
            var cred = new ChainedTokenCredential(
                new VisualStudioCredential(tent),
                new ManagedIdentityCredential(),
                new DefaultAzureCredential());

            var vault = new SecretClient(home, cred);

            var appid = vault.GetSecret("atc-client");
            var apps = vault.GetSecret("atc-secret");

            return new AppSecret(appid.Value.Value, apps.Value.Value);
        }
    }

    internal class AppSecret
    {
        internal string AppId { get; }
        internal string AppSec { get; }

        internal AppSecret(string id, string s) => (AppId, AppSec) = (id, s);
    }
}
