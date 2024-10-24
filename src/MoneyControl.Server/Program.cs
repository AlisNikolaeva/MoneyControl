using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoneyControl.Application;
using MoneyControl.Application.CSV;
using MoneyControl.Application.Handlers.Account.CreateAccount;
using MoneyControl.Infrastructure;
using MoneyControl.Server.Validators.Account;
using MoneyControl.Server.Validators.Transaction;
using MoneyControl.Shared.Queries.Account.CreateAccount;
using MoneyControl.Shared.Queries.Account.UpdateAccount;
using MoneyControl.Shared.Queries.Transaction.CreateTransaction;
using MoneyControl.Shared.Queries.Transaction.UpdateTransaction;

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
        builder.Services.AddScoped<IValidator<CreateTransactionCommand>, CreateTransactionCommandValidator>();
        builder.Services.AddScoped<IValidator<UpdateTransactionCommand>, UpdateTransactionCommandValidator>();
        builder.Services.AddScoped<CsvReport>();

        var connection = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<ApplicationDbContext>(option => option.UseSqlServer(connection));
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreateAccountHandler>());
        
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        
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

        app.UseMiddleware<UserContextMiddleware>();
        app.UseMiddleware<ValidationExceptionMiddleware>();
        app.UseRouting();
        app.UseCors("AllowAll");

        app.MapRazorPages();
        app.MapControllers();
        app.MapFallbackToFile("index.html");

        app.Run();
    }
}