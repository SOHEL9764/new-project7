using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyVault
{
    class Program
    {
        static void Main(string[] args)
        {
            string keyVaultUrl = "https://eastuskeyvault01.vault.azure.net/";
            var client = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
            KeyVaultSecret secret = client.GetSecret("azuresql");
            Console.WriteLine(secret.Value);
            Console.ReadKey();
        }
    }
}
