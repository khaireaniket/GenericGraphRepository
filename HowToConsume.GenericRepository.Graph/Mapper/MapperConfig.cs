using AutoMapper;
using HowToConsume.GenericRepository.Graph.Models.Node;
using HowToConsume.GenericRepository.Graph.Models.Request;

namespace HowToConsume.GenericRepository.Graph.Mapper
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<EntityCreateRequest, Entity>();
            CreateMap<EntityUpdateRequest, Entity>();
            CreateMap<Entity, Entity>();
        }
    }
}
