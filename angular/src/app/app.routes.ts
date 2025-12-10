import { authGuard, permissionGuard } from '@abp/ng.core';
import { Routes } from '@angular/router';

export const APP_ROUTES: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'sales',
  },

  {
    path: 'sales',
    pathMatch: 'full',
    loadComponent: () => import('./sales/sales').then(c => c.Sales),
  },
  {
    path: 'purchases',
    pathMatch: 'full',
    loadComponent: () => import('./purchases/purchases').then(c => c.Purchases),
  },
  {
    path: 'stock-report',
    pathMatch: 'full',
    loadComponent: () => import('./stock-report/stock-report').then(c => c.StockReport),
  },
  {
    path: 'account',
    loadChildren: () => import('@abp/ng.account').then(c => c.createRoutes()),
  },
  {
    path: 'identity',
    loadChildren: () => import('@abp/ng.identity').then(c => c.createRoutes()),
  },
  {
    path: 'tenant-management',
    loadChildren: () => import('@abp/ng.tenant-management').then(c => c.createRoutes()),
  },
  {
    path: 'setting-management',
    loadChildren: () => import('@abp/ng.setting-management').then(c => c.createRoutes()),
  },
];
