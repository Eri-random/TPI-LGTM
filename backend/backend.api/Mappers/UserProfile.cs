using AutoMapper;
using backend.api.Models;
using backend.data.Models;
using backend.servicios.DTOs;

namespace backend.api.Mappers
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<Usuario, UserDto>()
                .ForMember(dest => dest.RolNombre, opt => opt.MapFrom(src => src.Rol.Nombre));

            CreateMap<UserDto, Usuario>();

            CreateMap<UserDto, UserResponseModel>();

            CreateMap<UserRequestModel, UserDto>();
        }
    }
}
