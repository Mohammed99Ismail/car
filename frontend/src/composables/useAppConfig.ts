import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { appConfig } from '~/config/app.config'

export function useAppConfig() {
  let localeRef: any = null
  try {
    const { locale } = useI18n()
    localeRef = locale
  } catch {}

  const appName = computed(() => appConfig.getAppName(localeRef?.value))
  const companyName = computed(() => appConfig.getCompanyName(localeRef?.value))
  const subTitle = computed(() => appConfig.getSubTitle(localeRef?.value))
  const logoAlt = computed(() => appName.value)

  return {
    appConfig,
    appName,
    companyName,
    subTitle,
    logoPath: appConfig.logoPath,
    logoAlt,
    constraints: appConfig.constraints,
  }
}
