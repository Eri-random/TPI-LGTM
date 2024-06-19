﻿using AutoMapper;
using backend.api.Models;
using backend.api.Models.RequestModels;
using backend.api.Models.ResponseModels;
using backend.data.Models;
using backend.servicios.DTOs;

namespace backend.api.Mappers
{
    public class OrganizationProfile : Profile
    {
        public OrganizationProfile()
        {
            CreateMap<Organizacion, OrganizationDto>();

            CreateMap<InfoOrganizacion, InfoOrganizationDto>()
                .ForMember(dest => dest.OrganizacionId, opt => opt.MapFrom(src => src.Id));

            CreateMap<InfoOrganizationDto, InfoOrganizacion>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.OrganizacionId));

            CreateMap<OrganizationDto, Organizacion>();

            CreateMap<OrganizationDto, OrganizationResponseModel>();

            CreateMap<OrganizationRequestModel, OrganizationDto>();

            CreateMap<InfoOrganizationRequestModel, OrganizationDto>();

            CreateMap<InfoOrganizationRequestModel, InfoOrganizationDto>()
                .ForMember(dest => dest.Img, opt => opt.MapFrom(src => src.ImageUrl));

            CreateMap<HeadquartersDto, Sede>();

            CreateMap<Sede, HeadquartersDto>();

            CreateMap<HeadquartersResponseModel, HeadquartersDto>();

            CreateMap<HeadquartersDto, HeadquartersRequestModel>();

            CreateMap<HeadquartersRequestModel, HeadquartersDto>();

            CreateMap<HeadquartersDto, HeadquartersResponseModel>();

            CreateMap<Organizacion, HeadquartersNearbyDto>();

            CreateMap<DataRequestModel, HeadquartersNearbyDto>();

            CreateMap<HeadquartersDto, HeadquartersNearbyResponseModel>();
        }
    }
}
