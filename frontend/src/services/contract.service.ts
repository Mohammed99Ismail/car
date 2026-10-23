import { useApi } from '~/composables/useApi'
import type { ApiResponse, BackendPagedList, PagedList } from '~/models/ApiResponse'
import { toBackendQuery, toPagedResponse } from '~/models/ApiResponse'
import type {
  ContractCalculationRequest,
  ContractCalculationResultDto,
  ContractDto,
  ContractQueryParameters,
  ContractReportDto,
  ContractTermsConfigDto,
  CreateContractRequest,
  UpdateContractRequest,
  UpdateContractTermsConfigRequest,
} from '~/models/Contract'

export const useContractService = () => {
  const api = useApi('contracts')

  async function getPaged(params: ContractQueryParameters): Promise<ApiResponse<PagedList<ContractDto>>> {
    const res = await api.get<ApiResponse<BackendPagedList<ContractDto>>>('', toBackendQuery(params as any))
    return toPagedResponse(res)
  }

  async function getById(id: number): Promise<ApiResponse<ContractDto>> {
    return api.get<ApiResponse<ContractDto>>(String(id))
  }

  async function getReport(id: number): Promise<ApiResponse<ContractReportDto>> {
    return api.get<ApiResponse<ContractReportDto>>(`${id}/report`)
  }

  async function create(dto: CreateContractRequest): Promise<ApiResponse<ContractDto>> {
    return api.post<ApiResponse<ContractDto>>('', dto)
  }

  async function update(id: number, dto: UpdateContractRequest): Promise<ApiResponse<ContractDto>> {
    return api.put<ApiResponse<ContractDto>>(String(id), dto)
  }

  async function remove(id: number): Promise<ApiResponse<unknown>> {
    return api.delete<ApiResponse<unknown>>(String(id))
  }

  async function calculate(request: ContractCalculationRequest): Promise<ApiResponse<ContractCalculationResultDto>> {
    return api.post<ApiResponse<ContractCalculationResultDto>>('calculate', request)
  }

  async function getTermsConfig(): Promise<ApiResponse<ContractTermsConfigDto>> {
    return api.get<ApiResponse<ContractTermsConfigDto>>('terms-config')
  }

  async function updateTermsConfig(dto: UpdateContractTermsConfigRequest): Promise<ApiResponse<ContractTermsConfigDto>> {
    return api.put<ApiResponse<ContractTermsConfigDto>>('terms-config', dto)
  }

  return {
    getPaged,
    getById,
    getReport,
    create,
    update,
    remove,
    calculate,
    getTermsConfig,
    updateTermsConfig,
  }
}
