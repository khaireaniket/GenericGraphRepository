using GenericRepository.Graph;
using HowToConsume.GenericRepository.Graph.Models.Node;
using Neo4jClient;

namespace HowToConsume.GenericRepository.Graph.Data
{
    public class EntityRepository : Neo4jRepository<Entity>
    {
        public EntityRepository(IGraphClient graphClient) : base(graphClient)
        {

        }
    }
}
