using Microsoft.AspNetCore.Identity;

namespace ManejoPresupuestos.Servicio
{
    public class MensajesErrorIdentity : IdentityErrorDescriber
    {
        public override IdentityError DefaultError()
        {
            return new IdentityError()
            {
                Code = nameof(DefaultError),
                Description = "Ha ocurrido un Error"
            };
        }

        public override IdentityError DuplicateEmail(string email)
        {
            return base.DuplicateEmail(email);
        }

        public override IdentityError UserAlreadyHasPassword()
        {
            return base.UserAlreadyHasPassword();
        }
    }
}
