import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

const oAuthConfig = {
  issuer: 'https://localhost:44311/',
  redirectUri: baseUrl,
  clientId: 'MyStore_App',
  responseType: 'code',
  scope: 'offline_access MyStore',
  requireHttps: true,
};

export const environment = {
  production: true,
  application: {
    baseUrl,
    name: 'MyStore',
  },
  oAuthConfig,
  apis: {
    default: {
      url: 'https://localhost:44311',
      rootNamespace: 'MyStore',
    },
    AbpAccountPublic: {
      url: oAuthConfig.issuer,
      rootNamespace: 'AbpAccountPublic',
    },
  },
  remoteEnv: {
    url: '/getEnvConfig',
    mergeStrategy: 'deepmerge'
  }
} as Environment;
