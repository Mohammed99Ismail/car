import { useApi } from '~/composables/useApi'
import type { ApiResponse, BackendPagedList, PagedList, PagedQuery } from '~/models/ApiResponse'
import { toBackendQuery, toPagedResponse } from '~/models/ApiResponse'
import type { CarModelDto, CreateCarModelDto, UpdateCarModelDto } from '~/models/CarModel'

export interface CarModelQueryParameters extends PagedQuery {
  makeId?: number
}

export const useCarModelService = () => {
  const api = useApi('car-models')

  async function getPaged(params: CarModelQueryParameters): Promise<ApiResponse<PagedList<CarModelDto>>> {
    const backendParams = toBackendQuery(params)
    if (params.makeId) {
      backendParams.makeId = params.makeId
    }
    const res = await api.get<ApiResponse<BackendPagedList<CarModelDto>>>('', backendParams)
    return toPagedResponse(res)
  }

  async function getById(id: number | string): Promise<ApiResponse<CarModelDto>> {
    return api.get<ApiResponse<CarModelDto>>(String(id))
  }

  async function create(dto: CreateCarModelDto): Promise<ApiResponse<CarModelDto>> {
    return api.post<ApiResponse<CarModelDto>>('', dto)
  }

  async function update(id: number | string, dto: UpdateCarModelDto | CreateCarModelDto): Promise<ApiResponse<CarModelDto>> {
    const { makeId, name } = dto
    return api.put<ApiResponse<CarModelDto>>(String(id), { makeId, name })
  }

  async function remove(id: number | string): Promise<ApiResponse<unknown>> {
    return api.delete<ApiResponse<unknown>>(String(id))
  }

  return { getPaged, getById, create, update, remove }
}
