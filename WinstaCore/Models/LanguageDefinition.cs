namespace WinstaCore.Models
{
    public class LanguageDefinition
    {
        public LanguageDefinition(string name, string code)
        {
            DisplayName = name;
            LangCode = code;
        }

        public string DisplayName { get; }
        public string LangCode { get; }
    }
}
