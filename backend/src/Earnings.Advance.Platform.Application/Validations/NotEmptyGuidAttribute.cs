using System.ComponentModel.DataAnnotations;

namespace Earnings.Advance.Platform.Application.Validations
{
    /// <summary>
    /// Validation attribute to ensure a GUID is not empty
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class NotEmptyGuidAttribute : ValidationAttribute
    {
        public NotEmptyGuidAttribute()
            : base("The {0} field cannot be an empty GUID.")
        {
        }

        public override bool IsValid(object? value)
        {
            if (value is null) return false;

            return value is Guid guid && guid != Guid.Empty;
        }
    }
}
