using System;
using API.DTOs;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class PaymentsController(IPaymentService paymentService) : BaseApiController
{
    [HttpPost("{cartId}")]
    public async Task<ActionResult<CreateCheckoutSessionResponse>> CreateCheckoutSession(string cartId)
    {
        var origin = Request.Headers.Origin.ToString();
        if (string.IsNullOrWhiteSpace(origin)) return BadRequest("Missing Origin header.");

        var sessionUrl = await paymentService.CreateCheckoutSessionAsync(cartId, origin);
        if (string.IsNullOrWhiteSpace(sessionUrl))
        {
            return BadRequest("Unable to create checkout session.");
        }

        return Ok(new CreateCheckoutSessionResponse(sessionUrl));
    }
}
