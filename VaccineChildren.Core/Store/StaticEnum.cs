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
        [CustomName("userAccount")] UserAccount,
    }

    public enum RoleEnum
    {
        [CustomName("admin")] Admin,
        [CustomName("manager")] Manager,
        [CustomName("staff")] Staff,
        [CustomName("user")] User,
    }

    public enum StatusEnum
    {
        [CustomName("active")] Active,
        [CustomName("inactive")] Inactive 
    }

    public enum PaymentStatusEnum
    {
        [CustomName("pending")] Pending,
        [CustomName("completed")] Completed,
        [CustomName("cancelled")] Cancelled,
        [CustomName("failed")] Failed
    }
    
}