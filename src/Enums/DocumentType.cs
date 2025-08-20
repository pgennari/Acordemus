namespace acordemus.Enums
{
    public static class DocumentType
    {
        public static readonly List<string> AllowedTypes =
        [
            Regiment,
            Convention
        ];

        public const string Regiment = "Regiment";
        public const string Convention = "Convention";
    }
}