﻿using Interfaces.IRepositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services;

public class CheckoutReminderWorkerService(ILogger<CheckoutReminderWorkerService> logger, IServiceScopeFactory scopeFactory) : BackgroundService
{

    private readonly TimeSpan _interval = TimeSpan.FromHours(1);

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        logger.LogInformation("Checkout-reminder worker started");

        while (!token.IsCancellationRequested)
        {
            await SendRemindersAsync(token);
            await Task.Delay(_interval, token);
        }
    }

    private async Task SendRemindersAsync(CancellationToken token)
    {
        using var scope = scopeFactory.CreateScope();

        var bookingRepo      = scope.ServiceProvider.GetRequiredService<IBookingRepository>();
        var emailSender      = scope.ServiceProvider.GetRequiredService<EmailSenderService>();

        var today = DateTime.Today;
        var bookingsDue   = bookingRepo.GetBookingsDue(today);

        foreach (var (booking, email,name) in bookingsDue)
        {
            try
            {
                await emailSender.SendEmailForCheckout(email,booking,name);

                bookingRepo.MarkCheckoutReminderSent(booking.Id);
                logger.LogInformation("Reminder sent for booking {Id}", booking.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send reminder for booking {Id}", booking.Id);
            }
        }
    }
}
