namespace ClinicAPI.Shared.Model
{
    /// <summary>
    /// Controls where absence deductions are recorded when importing fingerprint attendance.
    /// HolidayOnly  — stored only on Holiday.DeductionAmount; salary picks it up via unpaid-leave sum.
    /// DeductionOnly — stored only as a Deduction row; salary picks it up via the deductions sum.
    /// Both          — stored in both places (double-deducts from salary — use only for testing).
    /// </summary>
    public enum AbsenceDeductionTarget { HolidayOnly, DeductionOnly, Both }

    public class AttendanceSettings
    {
        /// <summary>Number of minutes that constitute one late-deduction unit (e.g. 60 = per full hour).</summary>
        public int LateDeductionMinutesUnit { get; set; } = 60;

        /// <summary>Fixed monetary amount deducted per completed late unit.</summary>
        public decimal LateDeductionAmountPerUnit { get; set; } = 10m;

        /// <summary>
        /// Where to record the monetary deduction for an unauthorized absence during import.
        /// Default: HolidayOnly — the deduction sits on Holiday.DeductionAmount and is summed
        /// as UnpaidLeaveDeductions in salary calculation. Set to DeductionOnly to use the
        /// Deductions table instead (supports IsTakenFromSalary tracking).
        /// </summary>
        public AbsenceDeductionTarget AbsenceDeductionTarget { get; set; } = AbsenceDeductionTarget.HolidayOnly;
    }
}
