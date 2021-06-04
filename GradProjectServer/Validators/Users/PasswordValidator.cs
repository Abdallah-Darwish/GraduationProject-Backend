using System;
using System.Linq;
using FluentValidation;
using FluentValidation.Validators;

namespace GradProjectServer.Validators.Users
{
    public class PasswordValidator<T> : PropertyValidator<T, string?>
    {
        private static readonly char[] SpecialChars = "!@#$%^&*|\\+_.,".ToCharArray();
        public override bool IsValid(ValidationContext<T> context, string? value)
        {
            if (value == null)
            {
                return false;
            }

            if (value.Length < 8)
            {
                return false;
            }
            if (value.Any(char.IsWhiteSpace))
            {
                return false;
            }
            if (value.IndexOfAny(SpecialChars) == -1)
            {
                return false;
            }
            return true;
        }
        /// <inheritdoc/>
        public override string Name => "PasswordValidator";

        protected override string GetDefaultMessageTemplate(string errorCode)=>$"{{PropertyName}} must be at least 8 characters long and must contain at least one of ({string.Join(" ", SpecialChars)})";
    }
}