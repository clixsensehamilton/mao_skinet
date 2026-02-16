import { Component, inject } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MatButton } from '@angular/material/button';
import { CheckoutService } from '../../core/services/checkout.service';
import { CartService } from '../../core/services/cart.service';
import { OrderSummaryComponent } from '../../shared/components/order-summary/order-summary.component';

@Component({
  selector: 'app-checkout',
  imports: [MatButton, OrderSummaryComponent, RouterLink],
  templateUrl: './checkout.component.html',
  styleUrl: './checkout.component.scss',
})
export class CheckoutComponent {
  private route = inject(ActivatedRoute);
  private checkoutService = inject(CheckoutService);
  cartService = inject(CartService);

  isSuccess = false;
  isCanceled = false;
  isLoading = false;

  constructor() {
    this.route.queryParamMap.subscribe((params) => {
      this.isSuccess = params.get('success') === 'true';
      this.isCanceled = params.get('canceled') === 'true';
    });
  }

  startCheckout() {
    const cart = this.cartService.cart();
    if (!cart) return;

    this.isLoading = true;
    this.checkoutService.createCheckoutSession(cart.id).subscribe({
      next: (response) => {
        window.location.assign(response.url);
      },
      error: () => {
        this.isLoading = false;
      },
    });
  }
}
