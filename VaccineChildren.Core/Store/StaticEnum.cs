namespace VaccineChildren.Core.Store;

public static class StaticEnum
{
    public enum AccountEnum
    {
        [CustomName("staffWorking")] StaffWorking,
        [CustomName("staffResigned")] StaffResigned,
        [CustomName("adminWorking")] AdminWorking,
        [CustomName("adminResigned")] AdminResigned,
        [CustomName("managerWorking")] ManagerWorking,
        [CustomName("managerResigned")] ManagerResigned,
        [CustomName("doctorWorking")] DoctorWorking,
        [CustomName("doctorResigned")] DoctorResigned,
        [CustomName("userAccount")] UserAccount,
    }

    public enum RoleEnum
    {
        [CustomName("admin")] Admin,
        [CustomName("manager")] Manager,
        [CustomName("staff")] Staff,
        [CustomName("user")] User,
        [CustomName("doctor")] Doctor,
    }

    public enum StatusEnum
    {
        [CustomName("active")] Active,
        [CustomName("inactive")] Inactive 
    }

    public enum PaymentStatusEnum
    {
        [CustomName("pending")] Pending,
        [CustomName("paid")] Paid,
        [CustomName("cancelled")] Cancelled,
        [CustomName("failed")] Failed,
        [CustomName("rejected")] Rejected,
    }
    
    public enum OrderStatusEnum
    {
        [CustomName("processing")] Processing,
        [CustomName("completed")] Completed,
        [CustomName("cancelled")] Cancelled,
    }

    public enum PaymentMethodEnum
    {
        [CustomName("VNPay")] VnPay,
        [CustomName("Momo")] Momo,
    }
    
    public enum ScheduleStatusEnum
    {
        [CustomName("upcoming")] Upcoming,
        [CustomName("completed")] Completed,
        [CustomName("check-in")] CheckIn,
        [CustomName("vaccinated")] Vaccinated,
    }
    public enum EmailTemplateEnum
    {
        [CustomName("Email Verification")]
        [CustomId(1)]
        EmailVerification,

        [CustomName("Password Reset")]
        [CustomId(3)]
        PasswordReset,

        [CustomName("Appointment Confirmation")]
        [CustomId(2)]
        AppointmentConfirmation
    }

    public enum VnpResponseCode
    {
        [CustomName("24")] Cancelled,
        [CustomName("00")] Completed = 00,
    }
    
    public enum MomoResponseCode
    {
        [CustomName("0")] Completed,
        [CustomName("44")] Failed,
        [CustomName("4005")] Rejected,
    }
}