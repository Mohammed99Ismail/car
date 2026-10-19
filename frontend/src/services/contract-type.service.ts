import { useApi } from '~/composables/useApi'
import type { ApiResponse } from '~/models/ApiResponse'
import type { ContractTypeDto } from '~/models/ContractType'

/** Contract types are a read-only lookup (no CRUD). */
export const useContractTypeService = () => {
  const api = useApi('contract-types')

  async function getAll(): Promise<ApiResponse<ContractTypeDto[]>> {
    return api.get<ApiResponse<ContractTypeDto[]>>('')
  }

  return { getAll }
}
