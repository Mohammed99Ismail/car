import axios from 'axios'
import tokenService from '~/common/api/token.service'
import { useApi } from '~/composables/useApi'
import type { ApiResponse, BackendPagedList, PagedList } from '~/models/ApiResponse'
import { toBackendQuery, toPagedResponse } from '~/models/ApiResponse'
import type { CarDto, CarQueryParameters, SaveCarDto } from '~/models/Car'

export const useCarService = () => {
  const api = useApi('cars')

  async function getPaged(params: CarQueryParameters): Promise<ApiResponse<PagedList<CarDto>>> {
    const res = await api.get<ApiResponse<BackendPagedList<CarDto>>>('', toBackendQuery(params as any))
    return toPagedResponse(res)
  }

  async function getById(id: number): Promise<ApiResponse<CarDto>> {
    return api.get<ApiResponse<CarDto>>(String(id))
  }

  async function create(dto: SaveCarDto): Promise<ApiResponse<CarDto>> {
    return api.post<ApiResponse<CarDto>>('', dto)
  }

  async function update(id: number, dto: SaveCarDto): Promise<ApiResponse<CarDto>> {
    return api.put<ApiResponse<CarDto>>(String(id), dto)
  }

  async function remove(id: number): Promise<ApiResponse<unknown>> {
    return api.delete<ApiResponse<unknown>>(String(id))
  }

  /** POST /api/cars/{id}/image or POST /api/cars/upload-image */
  async function uploadImage(file: File, carId?: number | null): Promise<ApiResponse<string>> {
    const formData = new FormData()
    formData.append('file', file)
    const token = tokenService.getLocalAccessToken()
    const url = carId ? `/api/cars/${carId}/image` : '/api/cars/upload-image'
    const response = await axios.post<ApiResponse<string>>(url, formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
      },
    })
    return response.data
  }

  return { getPaged, getById, create, update, remove, uploadImage }
}
