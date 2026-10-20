import { useApi } from '~/composables/useApi'
import type { ApiResponse, BackendPagedList, PagedList, PagedQuery } from '~/models/ApiResponse'
import { toBackendQuery, toPagedResponse } from '~/models/ApiResponse'
import type { CarMakeDto, CreateCarMakeDto, UpdateCarMakeDto } from '~/models/CarMake'

export type CarMakeQueryParameters = PagedQuery

export const useCarMakeService = () => {
  const api = useApi('car-makes')

  async function getPaged(params: CarMakeQueryParameters): Promise<ApiResponse<PagedList<CarMakeDto>>> {
    const res = await api.get<ApiResponse<BackendPagedList<CarMakeDto>>>('', toBackendQuery(params))
    return toPagedResponse(res)
  }

  async function getById(id: number | string): Promise<ApiResponse<CarMakeDto>> {
    return api.get<ApiResponse<CarMakeDto>>(String(id))
  }

  async function create(dto: CreateCarMakeDto): Promise<ApiResponse<CarMakeDto>> {
    return api.post<ApiResponse<CarMakeDto>>('', dto)
  }

  async function update(id: number | string, dto: UpdateCarMakeDto | CreateCarMakeDto): Promise<ApiResponse<CarMakeDto>> {
    const { name } = dto
    return api.put<ApiResponse<CarMakeDto>>(String(id), { name })
  }

  async function remove(id: number | string): Promise<ApiResponse<unknown>> {
    return api.delete<ApiResponse<unknown>>(String(id))
  }

  return { getPaged, getById, create, update, remove }
}
