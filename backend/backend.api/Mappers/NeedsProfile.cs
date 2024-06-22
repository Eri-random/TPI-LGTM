using AutoMapper;
using backend.api.Models.RequestModels;
using backend.api.Models.ResponseModels;
using backend.data.Models;
using backend.servicios.DTOs;

namespace backend.api.Mappers
{
    public class NeedsProfile : Profile
    {
        public NeedsProfile()
        {
            CreateMap<Subcategorium, SubcategoriesDto>();

            CreateMap<Necesidad, NeedDto>();

            CreateMap<NeedDto, NeedsResponseModel>();

            CreateMap<CampaignDto, Campaign>();

            CreateMap<Campaign, CampaignDto>();

            CreateMap<CampaignDto, CampaignResponseModel>();

            CreateMap<CampaignRequestModel, CampaignDto>();
        }
    }
}
