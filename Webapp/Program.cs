using Cerbos.Sdk;
using Cerbos.Sdk.Builder;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.IdentityModel.Tokens;
using WebApp.Configuration;



var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
//builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
//    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));
var idp = IdP.KeyCloak;
switch (idp)
{
    case IdP.Entra:
        builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));
        break;
    case IdP.KeyCloak:
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        })
        .AddCookie(options =>
        {
            options.LoginPath = "/Account/Login";
        })
        .AddOpenIdConnect(options =>
        {
            options.Authority = "http://keycloak.mydomain.com:8080/realms/dev/";
            //options.Authority = "http://localhost:8080/realms/dev/";
            //options.MetadataAddress = "http://keycloak:8080/realms/dev/.well-known/openid-configuration";
            options.RequireHttpsMetadata = false;
            options.ClientId = "aspnetcoretest";
            options.ClientSecret = "yRjYFfUnt57AaT8LmBUR3ccXrNNQN5x0";
            options.ResponseType = "code";
            options.SaveTokens = true; //needs to be true, for keycloak to be able to signout
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            
            options.TokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = "preferred_username",
            };
        });

        break;
    default:
        break;
}

builder.Services.Configure<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme, options => options.AccessDeniedPath = "/Account/AccessDenied");

builder.Services.AddSingleton<CerbosClient>(CerbosClientBuilder.ForTarget("http://localhost:3593").WithPlaintext()
                .Build());

builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});
builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{

}
//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
