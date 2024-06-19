﻿using AutoMapper;
using backend.api.Models.RequestModels;
using backend.api.Models.ResponseModels;
using backend.data.Models;
using backend.servicios.DTOs;

namespace backend.api.Mappers
{
    public class DonationProfile : Profile
    {
        public DonationProfile()
        {
            CreateMap<DonationDto, Donacion>();

            CreateMap<Donacion, DonationDto>();

            CreateMap<DonationRequestModel, DonationDto>();

            CreateMap<DonationDto, DonationRequestModel>();

            CreateMap<DonationDto, DonationResponseModel>();
        }
    }
}
