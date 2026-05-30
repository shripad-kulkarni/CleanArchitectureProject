namespace Project.Domain.Constants
{
    public static class RoleConstants
    {
        public const string Admin   = "Admin";
        public const string Manager = "Manager";
        public const string User    = "User";
        public const string Guest   = "Guest";

        public static readonly string[] AllRoles = [Admin, Manager, User, Guest];
    }
}
