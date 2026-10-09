using FluentValidation;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.DesembolsoPlanilla
{
    public class DesembolsoPlanillaValidator : AbstractValidator<DesembolsoPlanillaCommand>
    {
        public DesembolsoPlanillaValidator()
        {
            RuleFor(p => p.NumeroPlanilla).Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("{NumeroPlanilla} no puede ser vacio o nulo")
                    .Length(10).WithMessage("{NumeroPlanilla} tiene que ser de 10 digitos");


            RuleFor(p => p.EstadoPlanilla).Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("{estadoPlanilla} no puede ser vacio o nulo")
                    .Equal(15).WithMessage("{PropertyValue} no puede ser de ese tipo de valor");

            RuleFor(p => p.EstadoDocumento).Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("{estadoDocumento} no puede ser vacio o nulo")
                    .Equal(27).WithMessage("{PropertyValue} no puede ser de ese tipo de valor");


            RuleFor(p => p.CodigoPerfilUsuario).Cascade(CascadeMode.Stop)
                     .NotEmpty().WithMessage("{codigoPerfilUsuario} no puede ser vacio o nulo")
                     .Must(x => x <= 99999).WithMessage("{codigoPerfilUsuario} solo puede tener maximo 5 caracteres");

            RuleFor(request => request.CodigoAgrupamiento).Cascade(CascadeMode.Stop)
                .Must((request, codigoAgrupamiento) => !(request.FlagDesembolsoTotal && !string.IsNullOrEmpty(codigoAgrupamiento)))
                .WithMessage("no se puede enviar codigo de Agrupamiento para un desembolso masivo de toda la Planilla")
                .Must((request, codigoAgrupamiento) => request.FlagDesembolsoTotal || !string.IsNullOrEmpty(codigoAgrupamiento))
                .WithMessage("se debe enviar codigoAgrupamiento");


            RuleFor(p => p.CodigoTienda).Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("{codigoTienda} no puede ser vacio o nulo")
                    .MaximumLength(10).WithMessage("{codigoTienda} solo puede tener maximo 10 caracteres");

            RuleFor(p => p.Comentario).Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("{comentario} no puede ser vacio o nulo")
                    .MaximumLength(250).WithMessage("{comentario} solo puede tener maximo 250 caracteres");

            RuleFor(p => p.NombreUsuarioRegistro).Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("{nombreUsuarioRegistro} no puede ser vacio o nulo")
                    .MaximumLength(20).WithMessage("{nombreUsuarioRegistro} solo puede tener maximo 20 caracteres");

            RuleFor(p => p.CanalAtencion).Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("{canalAtencion} no puede ser vacio o nulo")
                    .MaximumLength(20).WithMessage("{canalAtencion} solo puede tener maximo 20 caracteres");

            RuleFor(p => p.CodigoUsuario).Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("{codigoUsuario} no puede ser vacio o nulo")
                    .MaximumLength(10).WithMessage("{codigoUsuario} solo puede tener maximo 10 caracteres");

            RuleFor(p => p.CodigoUnico).Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("{codigoUnico} no puede ser vacio o nulo")
                    .Length(10).WithMessage("{codigoUnico} solo puede tener maximo 10 caracteres");

            RuleFor(p => p.CodigoReserva).Cascade(CascadeMode.Stop)
                    .Must(codigoReserva => codigoReserva == null || codigoReserva.All(codigo => codigo.ToString().Length == 10))
                    .When(p => p.CodigoReserva != null && p.CodigoReserva.Any(c => c != 0))
                    .WithMessage("{PropertyValue} cada codigo de reserva debe estar formado por 10 digitos");

        }
    }
}
