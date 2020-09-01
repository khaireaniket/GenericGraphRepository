using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GenericRepository.Graph
{
    public abstract class Neo4jRepository<TModel> : IGraphRepository<TModel> where TModel : BaseNode, INode, new()
    {
        private readonly IGraphClient _graphClient;
        private const string lambdaParentNodeParameter = "e";
        private const string lambdaChildNodeParameter = "c";
        private const string lambdaParentRelationshipParameter = "r";

        public Neo4jRepository(IGraphClient graphClient)
        {
            _graphClient = graphClient;
        }

        /// <summary>
        /// Get all the nodes of type <typeparamref name="TModel"/>
        /// </summary>
        /// <typeparam name="TModel">Type of the model/node</typeparam>
        /// <returns>Returns all the node</returns>
        public virtual async Task<IEnumerable<TModel>> GetAll()
        {
            TModel model = new TModel();

            return await _graphClient.Cypher.Match($"({lambdaParentNodeParameter}:{model.Label})")
                                     .Return(e => e.As<TModel>())
                                     .ResultsAsync;
        }

        /// <summary>
        /// Creates a new node of type <typeparamref name="TModel"/>
        /// </summary>
        /// <param name="model">Model to add</param>
        /// <typeparam name="TModel">Type of the model/node</typeparam>
        /// <returns>Returns the created node</returns>
        public virtual async Task<IEnumerable<TModel>> Create(TModel model)
        {
            return await _graphClient.Cypher.Create($"({lambdaParentNodeParameter}:{model.Label} {{model}})")
                                     .WithParam("model", model)
                                     .Return(e => e.As<TModel>())
                                     .ResultsAsync;
        }

        /// <summary>
        /// Updates node/nodes of type <typeparamref name="TModel"/>
        /// </summary>
        /// <param name="query">Query to filter node to update (a => a.EntityId == id)</param>
        /// <param name="model">Model to update</param>
        /// <typeparam name="TModel">Type of the model/node</typeparam>
        /// <returns>Returns the updated node</returns>
        public virtual async Task<IEnumerable<TModel>> Update(Expression<Func<TModel, bool>> query, TModel model)
        {
            TModel modelToUpdate = await this.FirstOrDefault(query);
            Clone(modelToUpdate, model);

            (var rewrittenQuery, var parentParameterName, var parentModel) = QueryInterpreter(query, lambdaParentNodeParameter);

            return await _graphClient.Cypher.Match($"({parentParameterName}:{parentModel.Label})")
                               .Where(rewrittenQuery)
                               .Set($"{parentParameterName} = {{model}}")
                               .WithParam("model", modelToUpdate)
                               .Return(e => e.As<TModel>())
                               .ResultsAsync;
        }

        /// <summary>
        /// Delete node/nodes of type <typeparamref name="TModel"/> along with all its relationships 
        /// </summary>
        /// <param name="query">Query to filter node to delete (a => a.EntityId == "12345")</param>
        /// <typeparam name="TModel">Type of the model/node</typeparam>
        /// <returns>No return value</returns>
        public virtual async Task Delete(Expression<Func<TModel, bool>> query)
        {
            (var rewrittenQuery, var parentParameterName, var parentModel) = QueryInterpreter(query, lambdaParentNodeParameter);

            await _graphClient.Cypher.Match($"({parentParameterName}:{parentModel.Label})")
                        .Where(rewrittenQuery)
                        .DetachDelete(parentParameterName)
                        .ExecuteWithoutResultsAsync();
        }

        /// <summary>
        /// Get node/nodes of type <typeparamref name="TModel"/> (Works like a WHERE clause)
        /// </summary>
        /// <param name="query">Query to filter out node/nodes (a => a.Name == "Andy")</param>
        /// <typeparam name="TModel">Type of the model/node</typeparam>
        /// <returns>Returns all the nodes which matches query condition</returns>
        public virtual async Task<IEnumerable<TModel>> Where(Expression<Func<TModel, bool>> query)
        {
            (var rewrittenQuery, var parentParameterName, var parentModel) = QueryInterpreter(query, lambdaParentNodeParameter);

            return await _graphClient.Cypher.Match($"({parentParameterName}:{parentModel.Label})")
                                .Where(rewrittenQuery)
                                .Return(e => e.As<TModel>())
                                .ResultsAsync;
        }

        /// <summary>
        /// Get only first node of type <typeparamref name="TModel"/> (Works like a FIRST OR DEFAULT clause)
        /// </summary>
        /// <param name="query">Query to filter out first node (a => a.Name == "Andy")</param>
        /// <typeparam name="TModel">Type of the model/node</typeparam>
        /// <returns>Returns only first instance of the node which matches query condition</returns>
        public virtual async Task<TModel> FirstOrDefault(Expression<Func<TModel, bool>> query)
        {
            IEnumerable<TModel> results = await Where(query);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// Create relationship between two nodes
        /// </summary>
        /// <typeparam name="TChildModel">Type of model to associate relationship with</typeparam>
        /// <typeparam name="TRelationship">Type of relationaship to create</typeparam>
        /// <param name="parentQuery">Query to filter node to create relationship from (a => a.PersonId == "12345")</param>
        /// <param name="childQuery">Query to filter node to create relationship to (a => a.EntityId == "12345")</param>
        /// <param name="relationship">Relationship to create</param>
        /// <returns></returns>
        public virtual async Task CreateRelationship<TChildModel, TRelationship>(Expression<Func<TModel, bool>> parentQuery, Expression<Func<TChildModel, bool>> childQuery, TRelationship relationship)
            where TChildModel : BaseNode, INode, new()
            where TRelationship : BaseRelationship, IRelationship, new()
        {
            (var rewrittenParentQuery, var parentParameterName, var parentModel) = QueryInterpreter(parentQuery, lambdaParentNodeParameter);
            (var rewrittenChildQuery, var childParameterName, var childModel) = QueryInterpreter(childQuery, lambdaChildNodeParameter);

            await _graphClient.Cypher.Match($"({parentParameterName}:{parentModel.Label})", $"({childParameterName}:{childModel.Label})")
                        .Where(rewrittenParentQuery)
                        .AndWhere(rewrittenChildQuery)
                        .Merge($"({parentParameterName})-[{lambdaParentRelationshipParameter}:{relationship.Type}]->({childParameterName})")
                        .ExecuteWithoutResultsAsync();
        }

        /// <summary>
        /// Delete relationship between two nodes
        /// </summary>
        /// <typeparam name="TChildModel">Type of model to delete relationship from</typeparam>
        /// <typeparam name="TRelationship">Type of relationaship to delete</typeparam>
        /// <param name="parentQuery">Query to filter node to create relationship from (a => a.PersonId == "12345")</param>
        /// <param name="childQuery">Query to filter node to create relationship to (a => a.EntityId == "12345")</param>
        /// <param name="relationship">Relationship to delete</param>
        /// <returns></returns>
        public virtual async Task DeleteRelationship<TChildModel, TRelationship>(Expression<Func<TModel, bool>> parentQuery, Expression<Func<TChildModel, bool>> childQuery, TRelationship relationship)
           where TChildModel : BaseNode, INode, new()
           where TRelationship : BaseRelationship, IRelationship, new()
        {
            (var rewrittenParentQuery, var parentParameterName, var parentModel) = QueryInterpreter(parentQuery, lambdaParentNodeParameter);
            (var rewrittenChildQuery, var childParameterName, var childModel) = QueryInterpreter(childQuery, lambdaChildNodeParameter);

            await _graphClient.Cypher.Match($"({parentParameterName}:{parentModel.Label})-[{lambdaParentRelationshipParameter}:{relationship.Type}]->({childParameterName}:{childModel.Label})")
                        .Where(rewrittenParentQuery)
                        .AndWhere(rewrittenChildQuery)
                        .Delete(lambdaParentRelationshipParameter)
                        .ExecuteWithoutResultsAsync();
        }

        private (Expression<Func<T, bool>>, string, T) QueryInterpreter<T>(Expression<Func<T, bool>> query, string lambdaParameter) where T : BaseNode, INode, new()
        {
            Expression<Func<T, bool>> rewrittenQuery = PredicateRewriter.Rewrite(query, lambdaParameter);
            string parameterName = rewrittenQuery.Parameters[0].Name;
            T model = (T)Activator.CreateInstance(rewrittenQuery.Parameters[0].Type);

            return (rewrittenQuery, parameterName, model);
        }

        private void Clone(TModel target, TModel source)
        {
            Type t = typeof(TModel);

            var properties = t.GetProperties().Where(prop => prop.CanRead && prop.CanWrite);

            foreach (var prop in properties)
            {
                var value = prop.GetValue(source, null);
                if (value != null)
                    prop.SetValue(target, value, null);
            }
        }
    }
}
