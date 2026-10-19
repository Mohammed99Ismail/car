import axios from 'axios'
import tokenService from '~/common/api/token.service'
import { useApi } from '~/composables/useApi'
import type { ApiResponse, PagedList } from '~/models/ApiResponse'
import type {
  CarDto,
  CarQueryParameters,
  CreateCarDto,
  UpdateCarDto,
} from '~/models/Car'

export const useCarService = () => {
  const api = useApi('cars')

  async function getPaged(
    params: CarQueryParameters,
  ): Promise<ApiResponse<PagedList<CarDto>>> {
    return api.get<ApiResponse<PagedList<CarDto>>>('', params)
  }

  async function getById(id: string): Promise<ApiResponse<CarDto>> {
    return api.get<ApiResponse<CarDto>>(id)
  }

  async function create(dto: CreateCarDto): Promise<ApiResponse<CarDto>> {
    return api.post<ApiResponse<CarDto>>('', dto)
  }

  async function update(
    id: string,
    dto: UpdateCarDto,
  ): Promise<ApiResponse<CarDto>> {
    return api.put<ApiResponse<CarDto>>(id, dto)
  }

  async function remove(id: string): Promise<ApiResponse<boolean>> {
    return api.delete<ApiResponse<boolean>>(id)
  }

  async function uploadImage(
    file: File,
  ): Promise<ApiResponse<{ webPath: string; fileName: string }>> {
    const formData = new FormData()
    formData.append('file', file)

    const token = tokenService.getLocalAccessToken()
    const response = await axios.post<
      ApiResponse<{ webPath: string; fileName: string }>
    >('/api/cars/upload-image', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
      },
    })
    return response.data
  }

  return {
    getPaged,
    getById,
    create,
    update,
    remove,
    uploadImage,
  }
}
