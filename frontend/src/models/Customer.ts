export interface CustomerDto {
  id: string
  name: string
  nationalId: string
  mobile: string
  address: string
  createdAt: string
  updatedAt?: string | null
}

export interface CreateCustomerDto {
  name: string
  nationalId: string
  mobile: string
  address: string
}

export interface UpdateCustomerDto extends CreateCustomerDto {
  id: string
}
