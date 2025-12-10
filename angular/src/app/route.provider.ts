import { RoutesService, eLayoutType } from '@abp/ng.core';
import { inject, provideAppInitializer } from '@angular/core';

export const APP_ROUTE_PROVIDER = [
  provideAppInitializer(() => {
    configureRoutes();
  }),
];

function configureRoutes() {
  const routes = inject(RoutesService);
  routes.add([
      {
        path: '/sales',
        name: '::Menu:Sales',
        iconClass: 'fas fa-dollar-sign',
        order: 1,
        layout: eLayoutType.application,
      },
      {
        path: '/purchases',
        name: '::Menu:Purchases',
        iconClass: 'fas fa-truck',
        order: 3,
        layout: eLayoutType.application,
      },
      {
        path: '/stock-report',
        name: '::Menu:Stock Report',
        iconClass: 'fas fa-clipboard-list',
        order: 2,
        layout: eLayoutType.application,
      },
  ]);
}
