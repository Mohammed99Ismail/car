<script lang="ts" setup>
import {
  ArrowUpload24Regular as UploadIcon,
  Document24Regular as DocIcon,
  DocumentPdf24Regular as PdfIcon,
  Image24Regular as ImageIcon,
  Delete24Regular as DeleteIcon,
  ArrowDownload24Regular as DownloadIcon,
  FolderOpen24Regular as FolderIcon,
} from '@vicons/fluent'
import { storeToRefs } from 'pinia'
import { useMessage } from 'naive-ui'
import type { UserAttachment } from '~/models/Profile'

const { t, locale } = useI18n()
const message = useMessage()
const store = useProfileStore()
const { userProfile, isUploadingAttachment } = storeToRefs(store)

const fileInputRef = ref<HTMLInputElement | null>(null)
const selectedFile = ref<File | null>(null)
const selectedDocType = ref<string>('nationalId')

const docTypeOptions = computed(() => [
  { label: t('profile.documentTypes.nationalId'), value: 'nationalId' },
  { label: t('profile.documentTypes.drivingLicense'), value: 'drivingLicense' },
  { label: t('profile.documentTypes.contract'), value: 'contract' },
  { label: t('profile.documentTypes.certificate'), value: 'certificate' },
  { label: t('profile.documentTypes.other'), value: 'other' },
])

function onFileChange(event: Event) {
  const target = event.target as HTMLInputElement
  const file = target.files?.[0]
  if (file) {
    if (file.size > 10 * 1024 * 1024) {
      message.error(t('profile.supportedFiles'))
      target.value = ''
      selectedFile.value = null
      return
    }
    selectedFile.value = file
  }
}

async function handleUpload() {
  if (!selectedFile.value) {
    message.warning(t('profile.dragOrClick'))
    return
  }

  const docTypeLabel = docTypeOptions.value.find(o => o.value === selectedDocType.value)?.label || selectedDocType.value

  const result = await store.uploadAttachment(selectedFile.value, docTypeLabel)
  if (result.success) {
    message.success(result.message || t('profile.uploadBtn'))
    selectedFile.value = null
    if (fileInputRef.value) {
      fileInputRef.value.value = ''
    }
  } else {
    message.error(result.message || 'Error uploading document')
  }
}

async function handleDelete(id: number) {
  const result = await store.deleteAttachment(id)
  if (result.success) {
    message.success(result.message || t('profile.delete'))
  } else {
    message.error(result.message || 'Error deleting document')
  }
}

function formatBytes(bytes: number) {
  if (!bytes || bytes === 0) return '0 B'
  const k = 1024
  const sizes = ['B', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return `${parseFloat((bytes / Math.pow(k, i)).toFixed(1))} ${sizes[i]}`
}

function formatDate(dateStr?: string) {
  if (!dateStr) return '-'
  try {
    const d = new Date(dateStr)
    return d.toLocaleDateString(locale.value === 'ar' ? 'ar-EG' : 'en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    })
  } catch {
    return dateStr
  }
}

function isImage(filePath: string) {
  return /\.(jpg|jpeg|png|webp|gif)$/i.test(filePath)
}

function isPdf(filePath: string) {
  return /\.pdf$/i.test(filePath)
}
</script>

<template>
  <div class="py-4 max-w-4xl mx-auto space-y-6">
    <!-- Upload Section Box -->
    <div class="bg-white dark:bg-slate-900 border border-slate-200/80 dark:border-slate-800 rounded-2xl p-6 md:p-8 shadow-sm">
      <div class="flex items-center gap-3 mb-6">
        <div class="w-10 h-10 rounded-xl bg-primary/10 text-primary flex items-center justify-center">
          <n-icon size="22"><UploadIcon /></n-icon>
        </div>
        <div>
          <h3 class="text-lg font-bold text-slate-800 dark:text-white">
            {{ t('profile.uploadDocument') }}
          </h3>
          <p class="text-xs text-slate-500 dark:text-slate-400">
            {{ t('profile.supportedFiles') }}
          </p>
        </div>
      </div>

      <div class="grid grid-cols-1 md:grid-cols-12 gap-4 items-end">
        <!-- Document Type Selector -->
        <div class="md:col-span-5">
          <label class="block text-xs font-semibold text-slate-700 dark:text-slate-300 mb-1.5">
            {{ t('profile.documentType') }}
          </label>
          <n-select
            v-model:value="selectedDocType"
            :options="docTypeOptions"
            :placeholder="t('profile.selectDocumentType')"
          />
        </div>

        <!-- File Input -->
        <div class="md:col-span-5">
          <label class="block text-xs font-semibold text-slate-700 dark:text-slate-300 mb-1.5">
            {{ t('profile.documentName') }}
          </label>
          <div class="relative">
            <input
              ref="fileInputRef"
              type="file"
              accept=".pdf,.doc,.docx,.xls,.xlsx,.png,.jpg,.jpeg,.webp"
              class="block w-full text-xs text-slate-600 dark:text-slate-300 file:mr-4 file:py-2.5 file:px-4 file:rounded-xl file:border-0 file:text-xs file:font-semibold file:bg-primary/10 file:text-primary hover:file:bg-primary/20 cursor-pointer border border-slate-200 dark:border-slate-700 rounded-xl"
              @change="onFileChange"
            />
          </div>
        </div>

        <!-- Upload Button -->
        <div class="md:col-span-2">
          <n-button
            type="primary"
            class="w-full"
            :disabled="!selectedFile"
            :loading="isUploadingAttachment"
            @click="handleUpload"
          >
            <template #icon>
              <n-icon><UploadIcon /></n-icon>
            </template>
            {{ t('profile.uploadBtn') }}
          </n-button>
        </div>
      </div>
    </div>

    <!-- Uploaded Documents List -->
    <div class="bg-white dark:bg-slate-900 border border-slate-200/80 dark:border-slate-800 rounded-2xl p-6 md:p-8 shadow-sm">
      <div class="flex items-center justify-between mb-4">
        <h3 class="text-lg font-bold text-slate-800 dark:text-white flex items-center gap-2">
          <n-icon size="20" class="text-primary"><FolderIcon /></n-icon>
          {{ t('profile.attachments') }}
          <span class="text-xs px-2 py-0.5 rounded-full bg-slate-100 dark:bg-slate-800 text-slate-500 font-normal">
            {{ userProfile?.attachments?.length || 0 }}
          </span>
        </h3>
      </div>

      <!-- Empty State -->
      <div v-if="!userProfile?.attachments || userProfile.attachments.length === 0" class="py-12 text-center">
        <n-empty :description="t('profile.noAttachments')">
          <template #icon>
            <n-icon size="40" class="text-slate-300 dark:text-slate-600"><DocIcon /></n-icon>
          </template>
        </n-empty>
      </div>

      <!-- Documents Cards Grid -->
      <div v-else class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div
          v-for="item in userProfile.attachments"
          :key="item.id"
          class="flex items-center justify-between p-4 rounded-xl border border-slate-100 dark:border-slate-800 hover:border-primary/40 dark:hover:border-primary/40 bg-slate-50/50 dark:bg-slate-800/40 transition group"
        >
          <div class="flex items-center gap-3 overflow-hidden">
            <!-- File Type Icon -->
            <div
              class="w-11 h-11 rounded-lg flex items-center justify-center shrink-0"
              :class="{
                'bg-red-50 dark:bg-red-950/40 text-red-500': isPdf(item.filePath),
                'bg-blue-50 dark:bg-blue-950/40 text-blue-500': isImage(item.filePath),
                'bg-emerald-50 dark:bg-emerald-950/40 text-emerald-500': !isPdf(item.filePath) && !isImage(item.filePath),
              }"
            >
              <n-icon size="22">
                <PdfIcon v-if="isPdf(item.filePath)" />
                <ImageIcon v-else-if="isImage(item.filePath)" />
                <DocIcon v-else />
              </n-icon>
            </div>

            <!-- File Details -->
            <div class="overflow-hidden">
              <p class="text-sm font-semibold text-slate-800 dark:text-slate-200 truncate" :title="item.fileName">
                {{ item.fileName }}
              </p>
              <div class="flex items-center gap-2 text-xs text-slate-500 dark:text-slate-400 mt-0.5">
                <n-tag v-if="item.documentType" size="tiny" type="default" round>
                  {{ item.documentType }}
                </n-tag>
                <span>{{ formatBytes(item.fileSizeBytes) }}</span>
                <span>•</span>
                <span>{{ formatDate(item.createdDate) }}</span>
              </div>
            </div>
          </div>

          <!-- Actions -->
          <div class="flex items-center gap-1 shrink-0 ms-2">
            <!-- Download / Preview -->
            <a
              :href="item.filePath"
              target="_blank"
              download
              class="p-2 rounded-lg text-slate-500 hover:text-primary hover:bg-slate-200/60 dark:hover:bg-slate-700/60 transition inline-flex items-center justify-center"
              :title="t('profile.download')"
            >
              <n-icon size="18"><DownloadIcon /></n-icon>
            </a>

            <!-- Delete with Popconfirm -->
            <n-popconfirm
              @positive-click="handleDelete(item.id)"
              :positive-text="t('profile.delete')"
              :negative-text="t('profile.reset')"
            >
              <template #trigger>
                <button
                  type="button"
                  class="p-2 rounded-lg text-slate-400 hover:text-red-500 hover:bg-red-50 dark:hover:bg-red-950/40 transition cursor-pointer inline-flex items-center justify-center"
                  :title="t('profile.delete')"
                >
                  <n-icon size="18"><DeleteIcon /></n-icon>
                </button>
              </template>
              {{ t('profile.confirmDelete') }}
            </n-popconfirm>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
