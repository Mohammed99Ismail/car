import * as XLSX from 'xlsx'

export type ExcelFileFormat = 'xlsx' | 'xlsm' | 'xls'

export interface ExcelColumn<T> {
  header: string
  key?: keyof T | string
  formatter?: (val: any, row: T) => string | number | null | undefined
  width?: number
}

export interface ExportToExcelOptions<T> {
  filename: string
  sheetName?: string
  columns: ExcelColumn<T>[]
  data: T[]
  format?: ExcelFileFormat
}

/**
 * Ensures the filename is 100% ASCII-clean so Chrome, Edge, Safari, and Android
 * never strip or discard the file name and extension on Blob downloads.
 */
function getSafeDownloadFilename(rawName: string, ext: string): string {
  const trimmed = (rawName || 'export').trim().replace(/\.(xlsx|xlsm|xls|csv)$/i, '')

  // If already pure ASCII (letters, numbers, underscores, hyphens)
  if (/^[a-zA-Z0-9_\-\.\s]+$/.test(trimmed)) {
    return `${trimmed.replace(/[\s]+/g, '_')}.${ext}`
  }

  // Transliterate Arabic names into standardized English names with dates
  let prefix = 'Export_Data'
  if (trimmed.includes('عقود') || trimmed.includes('عقد')) {
    prefix = 'Contracts_List'
  } else if (trimmed.includes('عملاء') || trimmed.includes('عميل')) {
    prefix = 'Customers_List'
  } else if (trimmed.includes('سيارات') || trimmed.includes('سيارة')) {
    prefix = 'Cars_List'
  }

  const dateMatch = trimmed.match(/\d{4}-\d{2}-\d{2}/)
  const dateSuffix = dateMatch ? `_${dateMatch[0]}` : `_${new Date().toISOString().slice(0, 10)}`

  return `${prefix}${dateSuffix}.${ext}`
}

/**
 * Universal browser utility to export data to real Microsoft Excel files (.xlsx / .xlsm)
 * with full RTL Arabic support, proper MIME types, and clean file downloads.
 */
export function exportToExcel<T>(options: ExportToExcelOptions<T>): boolean {
  try {
    const {
      filename,
      sheetName = 'البيانات',
      columns,
      data,
      format = 'xlsx',
    } = options

    // 1. Build Header Row
    const headerRow = columns.map(c => c.header)

    // 2. Build Data Rows
    const dataRows = data.map((row, index) => {
      return columns.map(col => {
        let val: any
        if (col.key === '#') {
          val = index + 1
        } else if (col.key) {
          val = (row as any)[col.key]
        }

        if (col.formatter) {
          return col.formatter(val, row) ?? ''
        }

        if (val === null || val === undefined) {
          return ''
        }

        return val
      })
    })

    const aoa = [headerRow, ...dataRows]

    // 3. Create Worksheet
    const worksheet = XLSX.utils.aoa_to_sheet(aoa)

    // 4. Set RTL direction for Arabic support
    if (!worksheet['!views']) {
      worksheet['!views'] = []
    }
    worksheet['!views'].push({ rightToLeft: true })

    // 5. Configure Column Widths (in characters)
    worksheet['!cols'] = columns.map(col => {
      if (col.width) {
        return { wch: col.width }
      }
      // Auto-compute reasonable width based on header length
      const headerLen = (col.header || '').length * 2
      return { wch: Math.max(headerLen, 14) }
    })

    // 6. Create Workbook and Append Sheet
    const workbook = XLSX.utils.book_new()
    const cleanSheetName = (sheetName || 'بيانات').substring(0, 31)
    XLSX.utils.book_append_sheet(workbook, worksheet, cleanSheetName)

    // 7. Resolve Format and MIME type
    // If user requested .xlsm, generate valid workbook (or .xlsx)
    const bookType = format === 'xlsm' ? 'xlsm' : format === 'xls' ? 'biff8' : 'xlsx'
    const extension = format === 'xlsm' ? 'xlsm' : format === 'xls' ? 'xls' : 'xlsx'

    let mimeType = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
    if (format === 'xlsm') {
      mimeType = 'application/vnd.ms-excel.sheet.macroEnabled.12'
    } else if (format === 'xls') {
      mimeType = 'application/vnd.ms-excel'
    }

    // 8. Generate real binary buffer
    const excelBuffer = XLSX.write(workbook, {
      bookType: bookType as any,
      type: 'array',
      compression: true,
    })

    // 9. Build typed Blob
    const blob = new Blob([excelBuffer], { type: `${mimeType};charset=utf-8` })

    // 10. Generate clean, ASCII-safe filename to ensure Chrome/Android NEVER strip the name or extension
    const fullFilename = getSafeDownloadFilename(filename, extension)

    // 11. Trigger standard browser download
    if (typeof (window.navigator as any)?.msSaveOrOpenBlob === 'function') {
      (window.navigator as any).msSaveOrOpenBlob(blob, fullFilename)
      return true
    }

    const blobUrl = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = blobUrl
    link.download = fullFilename
    link.setAttribute('download', fullFilename)
    link.style.display = 'none'

    document.body.appendChild(link)
    link.click()

    // Keep blob URL active long enough for Chrome to safely complete the download
    setTimeout(() => {
      if (link.parentNode) {
        link.parentNode.removeChild(link)
      }
      window.URL.revokeObjectURL(blobUrl)
    }, 30000)

    return true
  } catch (error) {
    console.error('Error generating Excel file:', error)
    throw error
  }
}
