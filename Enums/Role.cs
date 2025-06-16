namespace acordemus.Enums
{
    public class Role
    {
        public static readonly List<string> AllowedRoles =
        [
            Admin,
            Manager,
            Moderator,
            Commom
        ];

        public const string Admin = "Admin";
        public const string Manager = "Manager";
        public const string Moderator = "Moderator";
        public const string Commom = "Commom";
    }
}
