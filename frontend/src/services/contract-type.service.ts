import { useApi } from '~/composables/useApi'
import type { ApiResponse } from '~/models/ApiResponse'
import type { ContractTypeDto } from '~/models/ContractType'

export const useContractTypeService = () => {
  const api = useApi('contract-types')

  async function getAll(): Promise<ApiResponse<ContractTypeDto[]>> {
    return api.get<ApiResponse<ContractTypeDto[]>>('')
  }

  async function getById(id: string): Promise<ApiResponse<ContractTypeDto>> {
    return api.get<ApiResponse<ContractTypeDto>>(id)
  }

  return {
    getAll,
    getById,
  }
}
