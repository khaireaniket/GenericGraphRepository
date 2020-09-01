using AutoMapper;
using HowToConsume.GenericRepository.Graph.Data;
using HowToConsume.GenericRepository.Graph.Models.Node;
using HowToConsume.GenericRepository.Graph.Models.Request;
using Microsoft.AspNetCore.Mvc;
using Neo4jClient;
using System.Linq;
using System.Threading.Tasks;

namespace HowToConsume.GenericRepository.Graph.Controllers
{
    /// <summary>
    /// CONTROLLER USING GENERIC GRAPH REPOSITORY
    /// </summary>
    [Route("entity")]
    [ApiController]
    public class EntityController : ControllerBase
    {
        private readonly EntityRepository _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="repository"></param>
        /// <param name="graphClient"></param>
        public EntityController(IMapper mapper, EntityRepository repository, IGraphClient graphClient)
        {
            _mapper = mapper;
            _repository = repository;
        }

        /// <summary>
        /// Get all the nodes of Entity type
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAllNodes()
        {
            var nodes = await _repository.GetAll();
            return new JsonResult(nodes.ToList());
        }

        /// <summary>
        /// Get node by EntityId
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("id/{entityId}")]
        public async Task<IActionResult> GetNodeById([FromRoute] string entityId)
        {
            var node = await _repository.FirstOrDefault(a => a.EntityId == entityId);
            return new JsonResult(node);
        }

        /// <summary>
        /// Get node by legal name
        /// </summary>
        /// <param name="legalname"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("legalname/{legalname}")]
        public async Task<IActionResult> GetNodeByLegalName([FromRoute] string legalname)
        {
            var nodes = await _repository.Where(a => a.LegalName.ToLower() == legalname.ToLower());
            return new JsonResult(nodes);
        }

        /// <summary>
        /// Create a node of Entity type
        /// </summary>
        /// <param name="createNodeRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<IActionResult> CreateNode([FromBody] EntityCreateRequest createNodeRequest)
        {
            var nodes = await _repository.Create(_mapper.Map<Entity>(createNodeRequest));
            return new JsonResult(nodes.FirstOrDefault());
        }

        /// <summary>
        /// Update node of Entity type
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="updateNodeRequest"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{entityId}")]
        public async Task<IActionResult> UpdateNode([FromRoute] string entityId, [FromBody] EntityUpdateRequest updateNodeRequest)
        {
            var nodes = await _repository.Update(a => a.EntityId == entityId, _mapper.Map<Entity>(updateNodeRequest));
            return new JsonResult(nodes.FirstOrDefault());
        }

        /// <summary>
        /// Delete node by EntityId
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{entityId}")]
        public async Task<IActionResult> DeleteNode([FromRoute] string entityId)
        {
            await _repository.Delete(a => a.EntityId == entityId);
            return NoContent();
        }

        /// <summary>
        /// Create relationship between two nodes
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="childEntityId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{entityId}/to/entity/{childEntityId}")]
        public async Task<IActionResult> CreateRelationship([FromRoute] string entityId, [FromRoute] string childEntityId)
        {
            await _repository.CreateRelationship<Entity, InvestorOf>(a => a.EntityId == entityId, b => b.EntityId == childEntityId, new InvestorOf());
            return NoContent();
        }

        /// <summary>
        /// Delete relationship between two nodes
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="childEntityId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{entityId}/to/entity/{childEntityId}")]
        public async Task<IActionResult> DeleteRelationship([FromRoute] string entityId, [FromRoute] string childEntityId)
        {
            await _repository.DeleteRelationship<Entity, InvestorOf>(a => a.EntityId == entityId, b => b.EntityId == childEntityId, new InvestorOf());
            return NoContent();
        }
    }
}
