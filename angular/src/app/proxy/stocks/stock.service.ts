import type { StockDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class StockService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  getList = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, StockDto[]>({
      method: 'GET',
      url: '/api/app/stock',
    },
    { apiName: this.apiName,...config });
}