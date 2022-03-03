namespace ToDoApp.Models
{
    public readonly struct Language
    {
        public string Name { get; }

        public string Code { get; }

        public Language(string name, string code)
        {
            Name = name;
            Code = code;
        }
    }
}