namespace StockManagementSystem.Core.Domain.Identity
{
    public enum UserLoginResults
    {
        Successful = 1,

        UserNotExist = 2,

        WrongPassword = 3,

        NotActive = 4,

        Deleted = 5,

        NotRegistered = 6,

        LockedOut = 7
    }
}