using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace ToDoApp.Helpers.Validations.Rules
{
    public class IsValidPasswordRule<T> : IValidationRule<T>
    {
        public string ValidationMessage { get; set; }

        public bool Check(T value)
        {
            //return (RegexPassword.IsMatch($"{value}"));
            if (value is string checkedValue)
            {
                return checkedValue.Any(c => char.IsUpper(c))
                           && checkedValue.Length >= 6;
            }

            return false;
        }
    }
}
