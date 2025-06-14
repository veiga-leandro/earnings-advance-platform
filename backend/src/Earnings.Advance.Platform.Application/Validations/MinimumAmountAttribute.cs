using Earnings.Advance.Platform.Domain.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Earnings.Advance.Platform.Application.Validations
{
    public class MinimumAmountAttribute : ValidationAttribute
    {
        public MinimumAmountAttribute()
            : base($"Amount must be bigger than {AdvanceConstants.FormatAmount(AdvanceConstants.MINIMUM_AMOUNT)}")
        {
        }

        public override bool IsValid(object? value)
        {
            if (value is not decimal amount) return false;

            return amount > AdvanceConstants.MINIMUM_AMOUNT;
        }
    }
}
