import { useApi } from '~/composables/useApi'
import type { ApiResponse, BackendPagedList, PagedList, PagedQuery } from '~/models/ApiResponse'
import { toBackendQuery, toPagedResponse } from '~/models/ApiResponse'
import type { CreateCustomerDto, CustomerDto, UpdateCustomerDto } from '~/models/Customer'

export type CustomerQueryParameters = PagedQuery

export const useCustomerService = () => {
  const api = useApi('customers')

  async function getPaged(params: CustomerQueryParameters): Promise<ApiResponse<PagedList<CustomerDto>>> {
    const res = await api.get<ApiResponse<BackendPagedList<CustomerDto>>>('', toBackendQuery(params))
    return toPagedResponse(res)
  }

  async function getById(id: number | string): Promise<ApiResponse<CustomerDto>> {
    return api.get<ApiResponse<CustomerDto>>(String(id))
  }

  async function create(dto: CreateCustomerDto): Promise<ApiResponse<CustomerDto>> {
    return api.post<ApiResponse<CustomerDto>>('', dto)
  }

  async function update(id: number | string, dto: UpdateCustomerDto | CreateCustomerDto): Promise<ApiResponse<CustomerDto>> {
    const { name, nationalId, mobile, address } = dto
    return api.put<ApiResponse<CustomerDto>>(String(id), { name, nationalId, mobile, address })
  }

  async function remove(id: number | string): Promise<ApiResponse<unknown>> {
    return api.delete<ApiResponse<unknown>>(String(id))
  }

  return { getPaged, getById, create, update, remove }
}
