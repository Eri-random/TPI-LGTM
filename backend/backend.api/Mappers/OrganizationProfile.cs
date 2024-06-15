using AutoMapper;
using backend.api.Models;
using backend.data.Models;
using backend.servicios.DTOs;

namespace backend.api.Mappers
{
    public class OrganizationProfile : Profile
    {
        public OrganizationProfile()
        {
            CreateMap<Organizacion, OrganizationDto>();

            CreateMap<InfoOrganizacion, InfoOrganizationDto>();

            CreateMap<OrganizationDto, Organizacion>();

            CreateMap<OrganizationDto, OrganizationResponseModel>();

            CreateMap<OrganizationRequestModel, OrganizationDto>();
        }
    }
}
