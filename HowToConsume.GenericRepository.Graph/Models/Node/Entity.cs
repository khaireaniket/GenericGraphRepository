using GenericRepository.Graph;
using System;

namespace HowToConsume.GenericRepository.Graph.Models.Node
{
    public class Entity : BaseNode
    {
        public Entity() : base(nameof(Entity)) { }

        public string EntityId { get; set; }
        public string LegalName { get; set; }
        public string FormationState { get; set; }
        public int ProcessingOrderId { get; set; }
        public string SosId { get; set; }
        public DateTime FormationDate { get; set; }
        public string Ein { get; set; }
        public string OrganizationType { get; set; }
    }

    public class InvestorOf : BaseRelationship
    {
        public InvestorOf() : base("Investor_Of") { }
    }
}
