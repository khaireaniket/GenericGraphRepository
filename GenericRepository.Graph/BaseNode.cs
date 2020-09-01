using Newtonsoft.Json;

namespace GenericRepository.Graph
{
    public abstract class BaseNode : INode
    {
        [JsonIgnore]
        public string Label { get; private set; }

        /// <summary>
        /// Label of the node
        /// </summary>
        /// <param name="label"></param>
        public BaseNode(string label)
        {
            this.Label = label;
        }
    }
}
