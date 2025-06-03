using BookingWebApp.ViewModels;
using MSSQL;
using Services;
using Interfaces.IServices;
using Interfaces.IRepositories;
using System.Security.Claims;
using BookingWebApp.Hub;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace BookingWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();


            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(5);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddHostedService<CheckoutReminderWorkerService>();


            builder.Services.AddScoped<IBookingRepository, BookingRepository>();
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            builder.Services.AddScoped<IAccountHolderRepository, AccountHolderRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IApartmentRepository, ApartmentRepository>();
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
            builder.Services.AddScoped<IAmenitiesRepository, AmenitiesRepository>();
            builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
            builder.Services.AddScoped<IChatMessageRepository, ChatMessageRepository>();


            builder.Services.AddScoped<BookingService>();
            builder.Services.AddScoped<PaymentService>();
            builder.Services.AddScoped<AccountHolderService>();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<ApartmentService>();
            builder.Services.AddScoped<CheckOutService>();
            builder.Services.AddScoped<IReviewService,ReviewService>();
            builder.Services.AddScoped<IPasswordSecurityService,PasswordSecurityService>();
            builder.Services.AddScoped<EmailSenderService>();
            builder.Services.AddScoped<DashboardService>();
            builder.Services.AddScoped<ChatService>();


            builder.Services.AddSignalR(); // we are able to set up signal r hub, which we can use to connect to .

            builder.Services.AddAuthentication("UserScheme")         
                .AddCookie("UserScheme", opts =>
                {
                    opts.Cookie.Name = "UserLoginCookie";
                    opts.LoginPath = "/Authentication/LogIn";
                    opts.AccessDeniedPath = "/Account/AccessDenied";
                    opts.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                })
                .AddCookie("HostScheme", opts =>
                {
                    opts.Cookie.Name = "HostLoginCookie";
                    opts.LoginPath = "/Authentication/LogIn";
                    opts.AccessDeniedPath = "/Authentication/AccessDenied";
                    opts.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                });


            builder.Services.AddAuthorization(options =>
            {

                options.AddPolicy("User", policy =>
                {
                    policy.AuthenticationSchemes.Add("UserScheme");
                    policy.RequireRole("User");
                });


                options.AddPolicy("Host", policy =>
                {
                    policy.AuthenticationSchemes.Add("HostScheme");
                    policy.RequireRole("Host");

                });

            });

         var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();


            app.MapHub<ChatHub>("/chatHub");



            app.MapControllerRoute(
                name: "default",    
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
