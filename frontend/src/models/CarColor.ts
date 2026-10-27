export interface CarColorDto {
  id: number
  name: string
  hexCode?: string | null
  carsCount: number
}

export interface CreateCarColorDto {
  name: string
  hexCode?: string | null
}

export interface UpdateCarColorDto {
  name: string
  hexCode?: string | null
}
