<script setup lang="ts">
import {
  Add24Regular as AddIcon,
  Building24Regular as BranchesIcon,
  VehicleCar24Regular as CarsIcon,
  DocumentBulletList24Regular as ContractTypesIcon,
  People24Regular as CustomersIcon,
  ShieldKeyhole24Regular as RolesIcon,
  ArrowRight24Regular as ArrowRightIcon,
  CheckmarkCircle24Regular as CheckCircleIcon,
} from '@vicons/fluent'
import { ResultStatusEnum, isApiSuccess } from '~/models/ApiResponse'
import { useBranchService } from '~/services/branch.service'
import { useCarService } from '~/services/car.service'
import { useContractTypeService } from '~/services/contract-type.service'
import { useCustomerService } from '~/services/customer.service'

const { t } = useI18n()
const accountStore = useAccountStore()
const carService = useCarService()
const customerService = useCustomerService()
const branchService = useBranchService()
const contractTypeService = useContractTypeService()

const stats = reactive({
  carsCount: 0,
  customersCount: 0,
  branchesCount: 0,
  contractTypesCount: 0,
  loading: true,
})

async function loadStats() {
  stats.loading = true
  try {
    const [carsRes, custRes, branchRes, ctRes] = await Promise.all([
      carService.getPaged({ pageIndex: 1, pageSize: 1 }),
      customerService.getPaged({ pageIndex: 1, pageSize: 1 }),
      branchService.getPaged({ pageIndex: 1, pageSize: 1 }),
      contractTypeService.getAll(),
    ])

    if (isApiSuccess(carsRes) && carsRes.data) {
      stats.carsCount = carsRes.data.totalCount
    }
    if (isApiSuccess(custRes) && custRes.data) {
      stats.customersCount = custRes.data.totalCount
    }
    if (isApiSuccess(branchRes) && branchRes.data) {
      stats.branchesCount = branchRes.data.totalCount
    }
    if (isApiSuccess(ctRes) && ctRes.data) {
      stats.contractTypesCount = ctRes.data.length
    }
  } catch (err) {
    console.error('Error loading dashboard stats', err)
  } finally {
    stats.loading = false
  }
}

onMounted(() => {
  loadStats()
})
</script>

<route lang="yaml">
meta:
  title: dashboard
  layout: default
</route>

<template>
  <div class="p-6 space-y-6">
    <!-- Welcome Banner -->
    <div class="relative overflow-hidden rounded-2xl bg-gradient-to-r from-blue-700 via-indigo-600 to-primary p-8 text-white shadow-lg">
      <div class="relative z-10 max-w-2xl">
        <div class="inline-flex items-center gap-2 px-3 py-1 rounded-full bg-white/20 backdrop-blur-md text-xs font-semibold mb-3">
          <CheckCircleIcon class="w-4 h-4 text-emerald-300" />
          <span>النظام متصل وقيد العمل بكفاءة عالية</span>
        </div>
        <h1 class="text-3xl font-extrabold tracking-tight">
          مرحباً بك، {{ accountStore.user?.fullName || 'المسؤول' }} 👋
        </h1>
        <p class="mt-2 text-blue-100 text-sm md:text-base leading-relaxed">
          نظام إدارة معرض وتأجير السيارات المتكامل — لوحة تحكم ذكية وشاملة لمتابعة الأسطول، الفروع، العملاء، والصلاحيات بدقة وسرعة فائقة.
        </p>

        <div class="mt-6 flex flex-wrap gap-3">
          <RouterLink
            v-if="accountStore.hasPermission('Cars.Create')"
            to="/cars"
            class="inline-flex items-center gap-2 px-4 py-2.5 rounded-xl bg-white text-blue-700 font-semibold hover:bg-blue-50 transition shadow-sm text-sm"
          >
            <AddIcon class="w-4 h-4" />
            <span>إضافة سيارة جديدة</span>
          </RouterLink>

          <RouterLink
            v-if="accountStore.hasPermission('Customers.Create')"
            to="/customers"
            class="inline-flex items-center gap-2 px-4 py-2.5 rounded-xl bg-white/10 hover:bg-white/20 text-white font-semibold backdrop-blur-md transition text-sm"
          >
            <AddIcon class="w-4 h-4" />
            <span>إضافة عميل جديد</span>
          </RouterLink>
        </div>
      </div>

      <!-- Background decorative circle -->
      <div class="absolute -right-10 -bottom-10 w-64 h-64 rounded-full bg-white/10 blur-2xl pointer-events-none" />
    </div>

    <!-- Stat KPI Cards -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-5">
      <!-- 1. Cars Card -->
      <RouterLink
        to="/cars"
        class="group p-6 rounded-2xl bg-white dark:bg-slate-800 border border-slate-100 dark:border-slate-700/60 shadow-sm hover:shadow-md hover:border-blue-500/50 transition relative overflow-hidden"
      >
        <div class="flex items-center justify-between">
          <div class="w-12 h-12 rounded-xl bg-blue-500/10 text-blue-600 dark:text-blue-400 flex items-center justify-center group-hover:scale-110 transition">
            <CarsIcon class="w-6 h-6" />
          </div>
          <span class="text-xs font-semibold px-2 py-1 rounded bg-blue-50 dark:bg-blue-900/40 text-blue-600 dark:text-blue-300">
            الأسطول
          </span>
        </div>

        <div class="mt-4">
          <div class="text-3xl font-extrabold text-slate-800 dark:text-white">
            {{ stats.carsCount }}
          </div>
          <div class="text-sm font-medium text-slate-500 dark:text-slate-400 mt-1">
            إجمالي السيارات المسجلة
          </div>
        </div>

        <div class="mt-4 flex items-center gap-1 text-xs font-semibold text-blue-600 dark:text-blue-400 group-hover:translate-x-1 transition">
          <span>عرض الأسطول</span>
          <ArrowRightIcon class="w-3.5 h-3.5" />
        </div>
      </RouterLink>

      <!-- 2. Customers Card -->
      <RouterLink
        to="/customers"
        class="group p-6 rounded-2xl bg-white dark:bg-slate-800 border border-slate-100 dark:border-slate-700/60 shadow-sm hover:shadow-md hover:border-emerald-500/50 transition relative overflow-hidden"
      >
        <div class="flex items-center justify-between">
          <div class="w-12 h-12 rounded-xl bg-emerald-500/10 text-emerald-600 dark:text-emerald-400 flex items-center justify-center group-hover:scale-110 transition">
            <CustomersIcon class="w-6 h-6" />
          </div>
          <span class="text-xs font-semibold px-2 py-1 rounded bg-emerald-50 dark:bg-emerald-900/40 text-emerald-600 dark:text-emerald-300">
            العملاء
          </span>
        </div>

        <div class="mt-4">
          <div class="text-3xl font-extrabold text-slate-800 dark:text-white">
            {{ stats.customersCount }}
          </div>
          <div class="text-sm font-medium text-slate-500 dark:text-slate-400 mt-1">
            إجمالي العملاء
          </div>
        </div>

        <div class="mt-4 flex items-center gap-1 text-xs font-semibold text-emerald-600 dark:text-emerald-400 group-hover:translate-x-1 transition">
          <span>إدارة العملاء</span>
          <ArrowRightIcon class="w-3.5 h-3.5" />
        </div>
      </RouterLink>

      <!-- 3. Branches Card -->
      <RouterLink
        to="/branches"
        class="group p-6 rounded-2xl bg-white dark:bg-slate-800 border border-slate-100 dark:border-slate-700/60 shadow-sm hover:shadow-md hover:border-purple-500/50 transition relative overflow-hidden"
      >
        <div class="flex items-center justify-between">
          <div class="w-12 h-12 rounded-xl bg-purple-500/10 text-purple-600 dark:text-purple-400 flex items-center justify-center group-hover:scale-110 transition">
            <BranchesIcon class="w-6 h-6" />
          </div>
          <span class="text-xs font-semibold px-2 py-1 rounded bg-purple-50 dark:bg-purple-900/40 text-purple-600 dark:text-purple-300">
            الفروع
          </span>
        </div>

        <div class="mt-4">
          <div class="text-3xl font-extrabold text-slate-800 dark:text-white">
            {{ stats.branchesCount }}
          </div>
          <div class="text-sm font-medium text-slate-500 dark:text-slate-400 mt-1">
            فروع المعرض
          </div>
        </div>

        <div class="mt-4 flex items-center gap-1 text-xs font-semibold text-purple-600 dark:text-purple-400 group-hover:translate-x-1 transition">
          <span>عرض الفروع</span>
          <ArrowRightIcon class="w-3.5 h-3.5" />
        </div>
      </RouterLink>

      <!-- 4. Contract Types Card -->
      <RouterLink
        to="/contract-types"
        class="group p-6 rounded-2xl bg-white dark:bg-slate-800 border border-slate-100 dark:border-slate-700/60 shadow-sm hover:shadow-md hover:border-amber-500/50 transition relative overflow-hidden"
      >
        <div class="flex items-center justify-between">
          <div class="w-12 h-12 rounded-xl bg-amber-500/10 text-amber-600 dark:text-amber-400 flex items-center justify-center group-hover:scale-110 transition">
            <ContractTypesIcon class="w-6 h-6" />
          </div>
          <span class="text-xs font-semibold px-2 py-1 rounded bg-amber-50 dark:bg-amber-900/40 text-amber-600 dark:text-amber-300">
            مرجعي
          </span>
        </div>

        <div class="mt-4">
          <div class="text-3xl font-extrabold text-slate-800 dark:text-white">
            {{ stats.contractTypesCount }}
          </div>
          <div class="text-sm font-medium text-slate-500 dark:text-slate-400 mt-1">
            أنواع العقود المعتمدة
          </div>
        </div>

        <div class="mt-4 flex items-center gap-1 text-xs font-semibold text-amber-600 dark:text-amber-400 group-hover:translate-x-1 transition">
          <span>عرض الأنواع</span>
          <ArrowRightIcon class="w-3.5 h-3.5" />
        </div>
      </RouterLink>
    </div>

    <!-- Quick Navigation Modules Grid -->
    <div class="grid grid-cols-1 md:grid-cols-2 gap-5">
      <!-- Fleet & Showroom Overview -->
      <div class="bg-white dark:bg-slate-800 rounded-2xl p-6 border border-slate-100 dark:border-slate-700/60 shadow-sm">
        <h2 class="font-bold text-lg text-slate-800 dark:text-white mb-3 flex items-center gap-2">
          <span>🚙</span> إدارة الأسطول والسيارات
        </h2>
        <p class="text-sm text-slate-500 dark:text-slate-400 mb-4">
          تسجيل كامل لـ 12 حقلاً معتمداً لكل سيارة تشمل: الماركة، الموديل، سنة الصنع، اللون، اللوحة، الفرع، تاريخ وسعر الشراء، الهيكل، المحرك، الصورة، والملاحظات.
        </p>
        <RouterLink
          to="/cars"
          class="inline-flex items-center gap-2 text-sm font-semibold text-primary hover:underline"
        >
          <span>الانتقال إلى صفحة السيارات</span>
          <ArrowRightIcon class="w-4 h-4" />
        </RouterLink>
      </div>

      <!-- Security & Permissions Overview -->
      <div class="bg-white dark:bg-slate-800 rounded-2xl p-6 border border-slate-100 dark:border-slate-700/60 shadow-sm">
        <h2 class="font-bold text-lg text-slate-800 dark:text-white mb-3 flex items-center gap-2">
          <span>🛡️</span> الصلاحيات وإدارة المستخدمين
        </h2>
        <p class="text-sm text-slate-500 dark:text-slate-400 mb-4">
          تحكم ديناميكي كامل في إنشاء الأدوار وتعديل صلاحيات كل دور بحرية، مع تفعيل وتعطيل حسابات المستخدمين وتعيين الأدوار بمرونة.
        </p>
        <div class="flex items-center gap-4">
          <RouterLink
            to="/roles"
            class="inline-flex items-center gap-1 text-sm font-semibold text-primary hover:underline"
          >
            <span>الأدوار والصلاحيات</span>
            <ArrowRightIcon class="w-4 h-4" />
          </RouterLink>
          <span class="text-slate-300 dark:text-slate-600">|</span>
          <RouterLink
            to="/users"
            class="inline-flex items-center gap-1 text-sm font-semibold text-primary hover:underline"
          >
            <span>إدارة المستخدمين</span>
            <ArrowRightIcon class="w-4 h-4" />
          </RouterLink>
        </div>
      </div>
    </div>
  </div>
</template>
