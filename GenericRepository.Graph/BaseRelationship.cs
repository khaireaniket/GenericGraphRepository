﻿using Newtonsoft.Json;

namespace GenericRepository.Graph
{
    public class BaseRelationship : IRelationship
    {
        [JsonIgnore]
        public string Type { get; private set; }

        /// <summary>
        /// Type/Name of the relationship
        /// </summary>
        /// <param name="type"></param>
        public BaseRelationship(string type)
        {
            this.Type = type;
        }
    }
}
