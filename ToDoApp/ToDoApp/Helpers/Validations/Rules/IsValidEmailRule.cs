namespace ToDoApp.Helpers.Validations.Rules
{
    class IsValidEmailRule<T> : IValidationRule<T>
    {
        public string ValidationMessage { get; set; }

        public bool Check(T value)
        {
            try
            {
                var address = new System.Net.Mail.MailAddress($"{value}");
                return address.Address == $"{value}";
            }
            catch
            {
                return false;
            }
        }
    }
}