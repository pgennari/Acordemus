using acordemus.Enums;

namespace acordemus.Models
{
    public class Role
    {
        private string _name;
        public string? Name
        {
            get => _name;
            set
            {
                if (!RoleType.AllowedRoles.Contains(value))
                    throw new ArgumentException($"Role name invalid: {value}");
                _name = value;
            }
        }
    }
}