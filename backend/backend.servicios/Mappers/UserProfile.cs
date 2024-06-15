using AutoMapper;
using backend.data.Models;
using backend.servicios.DTOs;

namespace backend.servicios.Mappers
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<Usuario, UserDto>()
                .ForMember(dest => dest.RolNombre, opt => opt.MapFrom(src => src.Rol.Nombre));
        }
    }
}
