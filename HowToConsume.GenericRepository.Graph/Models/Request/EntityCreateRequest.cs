using System;

namespace HowToConsume.GenericRepository.Graph.Models.Request
{
    public class EntityCreateRequest
    {
        public string EntityId { get; set; }
        public string LegalName { get; set; }
        public string FormationState { get; set; }
        public int ProcessingOrderId { get; set; }
        public string SosId { get; set; }
        public DateTime FormationDate { get; set; }
        public string Ein { get; set; }
        public string OrganizationType { get; set; }
    }
}
