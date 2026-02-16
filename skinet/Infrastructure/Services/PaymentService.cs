using System;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Stripe;
using Stripe.Checkout;

namespace Infrastructure.Services;

public class PaymentService(
    ICartService cartService,
    IProductRepository productRepository,
    IConfiguration configuration,
    IHostEnvironment environment) : IPaymentService
{
    public async Task<string?> CreateCheckoutSessionAsync(string cartId, string origin)
    {
        var cart = await cartService.GetCartAsync(cartId);
        if (cart == null || cart.Items.Count == 0) return null;

        var lineItems = new List<SessionLineItemOptions>();
        foreach (var item in cart.Items)
        {
            var product = await productRepository.GetProductByIdAsync(item.ProductId);
            if (product == null) return null;

            var unitAmount = (long)Math.Round(product.Price * 100, MidpointRounding.AwayFromZero);
            lineItems.Add(new SessionLineItemOptions
            {
                Quantity = item.Quantity,
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "usd",
                    UnitAmount = unitAmount,
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = product.Name,
                    },
                },
            });
        }

        StripeConfiguration.ApiKey = GetSecretKey();

        var options = new SessionCreateOptions
        {
            Mode = "payment",
            PaymentMethodTypes = ["card"],
            LineItems = lineItems,
            SuccessUrl = $"{origin}/checkout?success=true&session_id={{CHECKOUT_SESSION_ID}}",
            CancelUrl = $"{origin}/checkout?canceled=true",
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options);
        return session.Url;
    }

    private string GetSecretKey()
    {
        var key = environment.IsDevelopment()
            ? configuration["Stripe:Test:SecretKey"]
            : configuration["Stripe:Live:SecretKey"];

        if (string.IsNullOrWhiteSpace(key))
        {
            key = configuration["Stripe:SecretKey"];
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            throw new InvalidOperationException("Stripe secret key is not configured.");
        }

        return key;
    }
}
