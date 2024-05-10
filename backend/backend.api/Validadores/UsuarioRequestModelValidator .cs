using backend.api.Models;
using FluentValidation;

namespace backend.api.Validadores
{
    public class UsuarioRequestModelValidator : AbstractValidator<UsuarioRequestModel>
    {
        public UsuarioRequestModelValidator()
        {
            RuleFor(x => x.Nombre).NotEmpty().WithMessage("El nombre es obligatorio");
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Debe proporcionar un email válido");
            RuleFor(x => x.Telefono).MinimumLength(8).WithMessage("El teléfono debe tener min. 8 dígitos");
            RuleFor(x => x.Telefono).MaximumLength(10).WithMessage("El teléfono debe tener max. 10 dígitos");
            RuleFor(x => x.Direccion).NotEmpty().WithMessage("La dirección no puede estar vacía");
            RuleFor(x => x.Localidad).NotEmpty().WithMessage("La localidad no puede estar vacía");
            RuleFor(x => x.Provincia).NotEmpty().WithMessage("La provincia no puede estar vacía");
            RuleFor(x => x.RolId).GreaterThan(0).LessThan(3).WithMessage("El rol es inválido");
            RuleFor(x => x.Password).MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres");
        }
    }
}
