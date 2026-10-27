import { useApi } from '~/composables/useApi'
import type { ApiResponse, BackendPagedList, PagedList, PagedQuery } from '~/models/ApiResponse'
import { toBackendQuery, toPagedResponse } from '~/models/ApiResponse'
import type { CarColorDto, CreateCarColorDto, UpdateCarColorDto } from '~/models/CarColor'

export type CarColorQueryParameters = PagedQuery

export const useCarColorService = () => {
  const api = useApi('car-colors')

  async function getPaged(params: CarColorQueryParameters): Promise<ApiResponse<PagedList<CarColorDto>>> {
    const res = await api.get<ApiResponse<BackendPagedList<CarColorDto>>>('', toBackendQuery(params))
    return toPagedResponse(res)
  }

  async function getById(id: number | string): Promise<ApiResponse<CarColorDto>> {
    return api.get<ApiResponse<CarColorDto>>(String(id))
  }

  async function create(dto: CreateCarColorDto): Promise<ApiResponse<CarColorDto>> {
    return api.post<ApiResponse<CarColorDto>>('', dto)
  }

  async function update(id: number | string, dto: UpdateCarColorDto | CreateCarColorDto): Promise<ApiResponse<CarColorDto>> {
    const { name, hexCode } = dto
    return api.put<ApiResponse<CarColorDto>>(String(id), { name, hexCode })
  }

  async function remove(id: number | string): Promise<ApiResponse<unknown>> {
    return api.delete<ApiResponse<unknown>>(String(id))
  }

  return { getPaged, getById, create, update, remove }
}
