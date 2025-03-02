using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

namespace RiftCouncilAppUI
{
    public static class RegisterServices
    {
        public static void ConfigureServices(this WebApplicationBuilder builder)
        {
            var azureB2COptions = new
            {
                Instance = Environment.GetEnvironmentVariable("AzureAdB2C_Instance"),
                ClientId = Environment.GetEnvironmentVariable("AzureAdB2C_ClientId"),
                Domain = Environment.GetEnvironmentVariable("AzureAdB2C_Domain"),
                SignUpSignInPolicyId = Environment.GetEnvironmentVariable("AzureAdB2C_SignUpSignInPolicyId"),
                ResetPasswordPolicyId = Environment.GetEnvironmentVariable("AzureAdB2C_ResetPasswordPolicyId"),
                EditProfilePolicyId = Environment.GetEnvironmentVariable("AzureAdB2C_EditProfilePolicyId"),
                CallbackPath = Environment.GetEnvironmentVariable("AzureAdB2C_CallbackPath")
            };

            // Sprawdzenie, czy wszystkie zmienne są ustawione
            if (string.IsNullOrEmpty(azureB2COptions.Instance) ||
                string.IsNullOrEmpty(azureB2COptions.ClientId) ||
                string.IsNullOrEmpty(azureB2COptions.Domain) ||
                string.IsNullOrEmpty(azureB2COptions.SignUpSignInPolicyId) ||
                string.IsNullOrEmpty(azureB2COptions.ResetPasswordPolicyId) ||
                string.IsNullOrEmpty(azureB2COptions.EditProfilePolicyId) ||
                string.IsNullOrEmpty(azureB2COptions.CallbackPath))
            {
                throw new InvalidOperationException("U didnt configure your Azure B2C. Check env values");
            }


            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor().AddMicrosoftIdentityConsentHandler();
            builder.Services.AddMemoryCache();
            builder.Services.AddControllersWithViews().AddMicrosoftIdentityUI();

            builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(options =>
                {
                    options.Instance = azureB2COptions.Instance;
                    options.ClientId = azureB2COptions.ClientId;
                    options.Domain = azureB2COptions.Domain;
                    options.SignUpSignInPolicyId = azureB2COptions.SignUpSignInPolicyId;
                    options.ResetPasswordPolicyId = azureB2COptions.ResetPasswordPolicyId;
                    options.EditProfilePolicyId = azureB2COptions.EditProfilePolicyId;
                    options.CallbackPath = azureB2COptions.CallbackPath;
                });


            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy =>
                {
                    policy.RequireClaim("jobTitle", "Admin");
                });
            });

            builder.Services.AddSingleton<IDbConnection, DbConnection>();
            builder.Services.AddSingleton<ICategoryData, MongoCategoryData>();
            builder.Services.AddSingleton<IStatusData, MongoStatusData>();
            builder.Services.AddSingleton<ISuggestionData, MongoSuggestionData>();
            builder.Services.AddSingleton<IUserData, MongoUserData>();
        }
    }
}
