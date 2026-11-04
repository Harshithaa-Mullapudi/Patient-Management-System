using System.ComponentModel.DataAnnotations;

namespace PatientManagement.Application.Validation;

public class NotFutureDateAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value == null)
        {
            return true;
        }

        if (value is not DateTime date)
        {
            return false;
        }

        return date.Date <= DateTime.Today;
    }
}