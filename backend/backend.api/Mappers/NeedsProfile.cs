using AutoMapper;
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
        }
    }
}
