/** Matches backend CustomerDto. */
export interface CustomerDto {
  id: number
  name: string
  nationalId: string
  mobile: string
  address: string
  createdDate: string
}

export interface CreateCustomerDto {
  name: string
  nationalId: string
  mobile: string
  address: string
}

export interface UpdateCustomerDto extends CreateCustomerDto {
  id: number
}
