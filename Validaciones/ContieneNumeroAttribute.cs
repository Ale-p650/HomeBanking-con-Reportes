using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuestos.Validaciones
{
    public class ContieneNumeroAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if(value is null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }

            if(! value.ToString().Any(c=> char.IsNumber(c)))
            {
                return new ValidationResult("El valor no posee numeros");
            }

            return ValidationResult.Success;
        }
    }
}
