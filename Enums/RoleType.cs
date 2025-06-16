using System.ComponentModel.DataAnnotations;

namespace acordemus.Enums
{
    public static class RoleType
    {
        public static readonly List<string> AllowedRoles =
        [
            Admin,
            Manager,
            Moderator
        ];

        public const string Admin = "Admin";
        public const string Manager = "Manager";
        public const string Moderator = "Moderator";


    }
}