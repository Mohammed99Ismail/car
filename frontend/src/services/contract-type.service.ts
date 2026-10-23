import { useApi } from '~/composables/useApi'
import type { ApiResponse } from '~/models/ApiResponse'
import type { ContractTypeDto } from '~/models/ContractType'

let cachedContractTypes: ApiResponse<ContractTypeDto[]> | null = null

/** Contract types are a read-only lookup (no CRUD). */
export const useContractTypeService = () => {
  const api = useApi('contract-types')

  async function getAll(): Promise<ApiResponse<ContractTypeDto[]>> {
    if (cachedContractTypes && (cachedContractTypes.isSuccess || cachedContractTypes.data)) {
      return cachedContractTypes
    }
    const res = await api.get<ApiResponse<ContractTypeDto[]>>('')
    if (res && (res.isSuccess || res.data)) {
      cachedContractTypes = res
    }
    return res
  }

  return { getAll }
}
