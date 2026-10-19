import { useApi } from '~/composables/useApi'
import type { ApiResponse, PagedList } from '~/models/ApiResponse'
import type {
  CreateCustomerDto,
  CustomerDto,
  UpdateCustomerDto,
} from '~/models/Customer'

export interface CustomerQueryParameters {
  pageIndex?: number
  pageSize?: number
  search?: string
  sortBy?: string
  isDescending?: boolean
}

export const useCustomerService = () => {
  const api = useApi('customers')

  async function getPaged(
    params: CustomerQueryParameters,
  ): Promise<ApiResponse<PagedList<CustomerDto>>> {
    return api.get<ApiResponse<PagedList<CustomerDto>>>('', params)
  }

  async function getById(id: string): Promise<ApiResponse<CustomerDto>> {
    return api.get<ApiResponse<CustomerDto>>(id)
  }

  async function create(
    dto: CreateCustomerDto,
  ): Promise<ApiResponse<CustomerDto>> {
    return api.post<ApiResponse<CustomerDto>>('', dto)
  }

  async function update(
    id: string,
    dto: UpdateCustomerDto,
  ): Promise<ApiResponse<CustomerDto>> {
    return api.put<ApiResponse<CustomerDto>>(id, dto)
  }

  async function remove(id: string): Promise<ApiResponse<boolean>> {
    return api.delete<ApiResponse<boolean>>(id)
  }

  return {
    getPaged,
    getById,
    create,
    update,
    remove,
  }
}
