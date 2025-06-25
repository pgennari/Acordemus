namespace acordemus.Enums
{
    public static class UserType
    {
        public static readonly List<string> AllowedTypes =
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