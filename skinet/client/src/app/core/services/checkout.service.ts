import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

type CreateCheckoutSessionResponse = {
  url: string;
};

@Injectable({
  providedIn: 'root',
})
export class CheckoutService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;

  createCheckoutSession(cartId: string) {
    return this.http.post<CreateCheckoutSessionResponse>(
      `${this.baseUrl}payments/${cartId}`,
      {},
    );
  }
}
