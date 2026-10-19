import { useApi } from '~/composables/useApi'
import type { ApiResponse, PagedList } from '~/models/ApiResponse'
import type {
  BranchDto,
  CreateBranchDto,
  UpdateBranchDto,
} from '~/models/Branch'

export interface BranchQueryParameters {
  pageIndex?: number
  pageSize?: number
  search?: string
  sortBy?: string
  isDescending?: boolean
}

export const useBranchService = () => {
  const api = useApi('branches')

  async function getPaged(
    params: BranchQueryParameters,
  ): Promise<ApiResponse<PagedList<BranchDto>>> {
    return api.get<ApiResponse<PagedList<BranchDto>>>('', params)
  }

  async function getById(id: string): Promise<ApiResponse<BranchDto>> {
    return api.get<ApiResponse<BranchDto>>(id)
  }

  async function create(dto: CreateBranchDto): Promise<ApiResponse<BranchDto>> {
    return api.post<ApiResponse<BranchDto>>('', dto)
  }

  async function update(
    id: string,
    dto: UpdateBranchDto,
  ): Promise<ApiResponse<BranchDto>> {
    return api.put<ApiResponse<BranchDto>>(id, dto)
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
