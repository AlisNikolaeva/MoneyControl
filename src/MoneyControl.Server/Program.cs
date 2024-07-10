using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using MoneyControl.Application.Handlers.Account.CreateAccount;
using MoneyControl.Application.Handlers.Account.UpdateAccount;
using MoneyControl.Infrastructure;
using MoneyControl.Server.Validators;

namespace MoneyControl.Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddEnvironmentVariables();
        builder.Services.AddControllers();
        builder.Services.AddRazorPages();
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddScoped<IValidator<CreateAccountCommand>, CreateAccountCommandValidator>();
        builder.Services.AddScoped<IValidator<UpdateAccountCommand>, UpdateAccountCommandValidator>();
        builder.Services.AddFluentValidationAutoValidation();

        var connection = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<ApplicationDbContext>(option => option.UseSqlServer(connection));

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseCors("AllowAll");

        app.MapRazorPages();
        app.MapControllers();
        app.MapFallbackToFile("index.html");

        app.Run();
    }
}