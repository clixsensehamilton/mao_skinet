# Stripe setup and checkout flow

## Overview
The API creates a Stripe Checkout Session and returns the hosted checkout URL to the client. The Angular checkout page redirects the browser to Stripe and handles `success`/`canceled` query flags on return.

## Configure Stripe keys
The API selects keys based on the hosting environment.

- Development (`ASPNETCORE_ENVIRONMENT=Development`) uses `Stripe:Test:SecretKey`.cd
- Non-development environments use `Stripe:Live:SecretKey`.
- Fallback: `Stripe:SecretKey` is used if the environment-specific value is missing.

Recommended ways to provide secrets:

- Local dev: use user secrets or environment variables instead of committing keys.
- Hosted env: configure app settings or environment variables in the deployment platform.

Example environment variables (PowerShell):

```powershell
$Env:Stripe__Test__SecretKey = "sk_test_..."
$Env:Stripe__Live__SecretKey = "sk_live_..."
```

## Test vs live mode
- Test mode: use a test secret key and Stripe test card numbers.
- Live mode: use a live secret key and real payment methods.

Switching mode is done by the environment and corresponding secret key values (see above).

## Checkout flow usage

### Backend flow
- Endpoint: `POST /payments/{cartId}`
- The API reads the `Origin` header to build the `success` and `cancel` URLs.
- A Stripe Checkout Session is created with:
  - `Mode = payment`
  - `PaymentMethodTypes = ["card"]`
  - `SuccessUrl = {origin}/checkout?success=true&session_id={CHECKOUT_SESSION_ID}`
  - `CancelUrl = {origin}/checkout?canceled=true`
- The response includes the hosted session URL to redirect the browser.

### Frontend flow
- The checkout page calls `CheckoutService.createCheckoutSession(cartId)`.
- On success, the client redirects to `response.url` (Stripe Checkout).
- On return, the page reads the `success` or `canceled` query params to show status.

## Common issues
- Missing Origin header: requests must include `Origin` (browser calls do this by default).
- Missing keys: the API throws if no Stripe secret key is configured.
- Empty cart or missing product: the API returns a bad request if it cannot create a session.
