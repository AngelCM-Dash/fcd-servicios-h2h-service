using AutoMapper;
using Interbank.Productos.Comercial.Fcd.Application.Features.Clientes.Queries.GetSuppliersList;
using Interbank.Productos.Comercial.Fcd.Application.Features.Common.Queries.GetParametersByDomain;
using Interbank.Productos.Comercial.Fcd.Application.Features.Common.Queries.GetTokenAuthorizationByChannel;
using Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.ActualizarPlanilla;
using Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.Encolamiento;
using Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.GenerarTramas;
using Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Query.ConsultaNroOperacionDesembolso;
using Interbank.Productos.Comercial.Fcd.Application.Features.Documento.Commands.ValidarDocumento;
using Interbank.Productos.Comercial.Fcd.Application.Features.Excepciones.Queries;
using Interbank.Productos.Comercial.Fcd.Application.Features.Planilla.Commands.CargaMasiva;
using Interbank.Productos.Comercial.Fcd.Application.Features.Planilla.Commands.ValidarPlanilla;
using Interbank.Productos.Comercial.Fcd.Application.Features.Seguimiento.Queries.GetListTrackingDetailHeader;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Dapper;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.EntityFramework;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Request;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Response;



namespace Interbank.Productos.Comercial.Fcd.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<EFClienteAfiliacion, ClienteAfiliacionVM>();
            CreateMap<DapperParametro, ParametrosVM>();
            CreateMap<DapperDocumentosDuplicados, ValidateDocumentResponse>();
            CreateMap<DapperDocumentosDuplicados, ValidatePlanillaResponse>();
            CreateMap<CreatePlanillasCommand, PlanillaResponse>();
            CreateMap<DapperComisionProveedor, ComisionProveedorVM>();
            CreateMap<ActualizarPlanillaCommand, DapperActualizarPlanilla>();
            CreateMap<GenerarTramasCommand, DapperGenerarTramasInput>();
            CreateMap<DapperSeguimientoDetalleCabecera, DetalleCabeceraVM>();
            CreateMap<EncolarDesembolsoCommand, DesembolsoRequest>();
            CreateMap<DapperNumeroOperacionDesembolso, GetNroOperacionMovimientoQueryVM>();

            CreateMap<TokenAuthorizationResponse, TokenAuthorizationVM>()
                    .ForMember(dest => dest.token_type, opt => opt.MapFrom(src => src.TipoToken))
                    .ForMember(dest => dest.access_token, opt => opt.MapFrom(src => src.AccesoToken))
                    .ForMember(dest => dest.scope, opt => opt.MapFrom(src => src.Alcance))
                    .ForMember(dest => dest.expires_in, opt => opt.MapFrom(src => src.Expiracion))
                    .ForMember(dest => dest.consented_on, opt => opt.MapFrom(src => src.Consentido));


        }
    }
}
