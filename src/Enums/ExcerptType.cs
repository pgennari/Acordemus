namespace acordemus.Enums
{
    public static class ExcerptType
    {
        public static readonly List<string> AllowedTypes =
        [
            Chapter,
            Section,
            Subsection,
            Article,
            Paragraph,
            Item
        ];

        public const string Chapter = "Chapter";
        public const string Section = "Section";
        public const string Subsection = "Subsection";
        public const string Article = "Article";
        public const string Paragraph = "Paragraph";
        public const string Item = "Item";
    }
}