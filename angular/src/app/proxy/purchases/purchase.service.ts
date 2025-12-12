import type { CreateUpdatePurchaseDto, PurchaseDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class PurchaseService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateUpdatePurchaseDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PurchaseDto>({
      method: 'POST',
      url: '/api/app/purchase',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/purchase/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PurchaseDto>({
      method: 'GET',
      url: `/api/app/purchase/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getByCode = (purchaseCode: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PurchaseDto>({
      method: 'GET',
      url: '/api/app/purchase/by-code',
      params: { purchaseCode },
    },
    { apiName: this.apiName,...config });
  

  getList = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, PurchaseDto[]>({
      method: 'GET',
      url: '/api/app/purchase',
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdatePurchaseDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'PUT',
      url: `/api/app/purchase/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}