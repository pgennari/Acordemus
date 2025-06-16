namespace acordemus.Enums
{
    public class ExcerptStatus
    {
        public static readonly List<string> AllowedStatus =
        [
            Active,
            Proposed,
            Obsolete,
            Deleted,
            Draft,
            Rejected
        ];

        public const string Active = "Active";
        public const string Proposed = "Proposed";
        public const string Obsolete = "";
        public const string Deleted= "Deleted";
        public const string Draft= "Draft";
        public const string Rejected= "Rejected";
    }
}