﻿using AutoMapper;
using backend.api.Models.RequestModels;
using backend.api.Models.ResponseModels;
using backend.data.Models;
using backend.servicios.DTOs;

namespace backend.api.Mappers
{
    public class IdeaProfile : Profile
    {
        public IdeaProfile()
        {
            CreateMap<Idea, IdeaDto>();

            CreateMap<IdeaDto, Idea>();

            CreateMap<Paso, StepDto>();

            CreateMap<StepDto, Paso>();

            CreateMap<IdeaDto, IdeaResponseModel>();

            CreateMap<IdeaRequestModel, IdeaDto>();
        }
    }
}
