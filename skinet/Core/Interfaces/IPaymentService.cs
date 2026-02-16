using System;
using System.Threading.Tasks;

namespace Core.Interfaces;

public interface IPaymentService
{
    Task<string?> CreateCheckoutSessionAsync(string cartId, string origin);
}
