using Ada.FirstCatering.API.Models.Entities;
using Ada.FirstCatering.API.Models.Requests;
using AutoMapper;

namespace Ada.FirstCatering.API.MapperProfiles;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<CreateEmployeeModel, Employee>();
    }
}