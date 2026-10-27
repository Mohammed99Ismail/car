import { useApi } from '~/composables/useApi'
import type { ApiResponse } from '~/models/ApiResponse'

/** Aggregated dashboard counts returned by GET /api/dashboard/stats. */
export interface DashboardStats {
  carsCount: number
  customersCount: number
  branchesCount: number
  contractTypesCount: number
}

export const useDashboardService = () => {
  const api = useApi('dashboard')

  /** Single round-trip for all home-page KPI counts (avoids 4 list requests). */
  async function getStats(): Promise<ApiResponse<DashboardStats>> {
    return api.get<ApiResponse<DashboardStats>>('stats')
  }

  return { getStats }
}
