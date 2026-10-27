import { appConfig } from '~/config/app.config'

export function useAppConfig() {
  return {
    appConfig,
    appName: appConfig.appName,
    companyName: appConfig.companyName,
    subTitle: appConfig.subTitle,
    logoPath: appConfig.logoPath,
    logoAlt: appConfig.logoAlt,
    constraints: appConfig.constraints,
  }
}
