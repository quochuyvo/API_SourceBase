using AutoMapper;
using API_SourceBase.Data.Entities;
using API_SourceBase.Models.Request;
using API_SourceBase.Models.Response;

namespace API_SourceBase.Application.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Department
            CreateMap<Department, MRes_Department>();
            CreateMap<MReq_Department, Department>();

            // Position
            CreateMap<Position, MRes_Position>();
            CreateMap<MReq_Position, Position>();
        }
    }
}
