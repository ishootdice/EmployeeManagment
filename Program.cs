using EmployeeManagment.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using NLog.Web;
using NLog;
using NLog.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using EmployeeManagment.Security;


//var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
//logger.Debug("init main");

var logger = LoggerFactory.Create(config =>
{
    config.AddDebug();
    config.AddEventSourceLogger();
    config.AddConsole();
    config.AddNLog();
}).CreateLogger("Program");


try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    string connection = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connection));

    builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredUniqueChars = 3;
        options.Password.RequiredLength = 6;

        options.SignIn.RequireConfirmedEmail = true;

        options.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";

        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders()
    .AddTokenProvider<CustomEmailConfirmationTokenProvider<ApplicationUser>>("CustomEmailConfirmation");

    builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
                    options.TokenLifespan = TimeSpan.FromHours(5));

    builder.Services.Configure<CustomEmailConfirmationTokenProviderOptions>(options =>
                    options.TokenLifespan = TimeSpan.FromDays(3));

    builder.Services.AddMvc(options =>
    {
        var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();

        options.Filters.Add(new AuthorizeFilter(policy));
    });
    builder.Services.AddAuthorization(options =>
    {

        //Authorisation with required Claim
        //
        options.AddPolicy("DeleteRolePolicy",
            policy => policy.RequireClaim("Delete Role", "true"));
        //options.AddPolicy("EditRolePolicy",
        //    policy => policy.RequireClaim("Edit Role", "true")
        //                    .RequireRole("Admin")
        //                    .RequireRole("Super Admin"));

        //Authorization with required assertions, can define couple logical expressions
        //
        //options.AddPolicy("EditRolePolicy",
        //    policy => policy.RequireAssertion(context =>
        //    context.User.IsInRole("Admin") && context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "true") ||
        //    context.User.IsInRole("Super Admin")
        //    ));

        //Authorization with custom authorization handler
        options.AddPolicy("EditRolePolicy",
            policy => policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement()));

        //Authorization with required role
        //
        options.AddPolicy("AdminRolePolicy",
            policy => policy.RequireRole("Admin"));
    });
    builder.Services.AddControllersWithViews();
    builder.Services.AddScoped<IEmployeeRepository, SqliteEmployeeRepository>();
    builder.Services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaimsHandler>();
    builder.Services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();
    builder.Services.AddHttpContextAccessor();

    builder.Services.AddAuthentication().AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["GoogleAuth:ClientID"];
        options.ClientSecret = builder.Configuration["GoogleAuth:ClientSecret"];
    })
    .AddFacebook(options =>
    {
        options.AppId = builder.Configuration["FacebookAuth:AppId"];
        options.AppSecret = builder.Configuration["FacebookAuth:AppSecret"];
    });

    var app = builder.Build();
      
    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        app.UseStatusCodePagesWithReExecute("/Error/{0}");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }
    else
    {
        app.UseDeveloperExceptionPage();
    }

    
    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
  

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Administration}/{action=ListUsers}/{id?}");

    app.Run();
}
catch(Exception exception)
{
    // NLog: catch setup errors
    logger.LogError(exception, "Stopped program because of exception");
    //logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
}
