import { useApi } from '~/composables/useApi'
import type { ApiResponse, BackendPagedList, PagedList, PagedQuery } from '~/models/ApiResponse'
import { toBackendQuery, toPagedResponse } from '~/models/ApiResponse'
import type { BranchDto, CreateBranchDto, UpdateBranchDto } from '~/models/Branch'

export type BranchQueryParameters = PagedQuery

export const useBranchService = () => {
  const api = useApi('branches')

  async function getPaged(params: BranchQueryParameters): Promise<ApiResponse<PagedList<BranchDto>>> {
    const res = await api.get<ApiResponse<BackendPagedList<BranchDto>>>('', toBackendQuery(params))
    return toPagedResponse(res)
  }

  async function getById(id: number | string): Promise<ApiResponse<BranchDto>> {
    return api.get<ApiResponse<BranchDto>>(String(id))
  }

  async function create(dto: CreateBranchDto): Promise<ApiResponse<BranchDto>> {
    return api.post<ApiResponse<BranchDto>>('', dto)
  }

  async function update(id: number | string, dto: UpdateBranchDto | CreateBranchDto): Promise<ApiResponse<BranchDto>> {
    const { name, address, mobile } = dto
    return api.put<ApiResponse<BranchDto>>(String(id), { name, address, mobile })
  }

  async function remove(id: number | string): Promise<ApiResponse<unknown>> {
    return api.delete<ApiResponse<unknown>>(String(id))
  }

  return { getPaged, getById, create, update, remove }
}
