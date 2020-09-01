using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GenericRepository.Graph
{
    internal interface IGraphRepository<TModel> where TModel : INode
    {
        Task<IEnumerable<TModel>> GetAll();

        Task<IEnumerable<TModel>> Create(TModel model);

        Task<IEnumerable<TModel>> Update(Expression<Func<TModel, bool>> filter, TModel model);

        Task Delete(Expression<Func<TModel, bool>> query);

        Task<IEnumerable<TModel>> Where(Expression<Func<TModel, bool>> query);

        Task<TModel> FirstOrDefault(Expression<Func<TModel, bool>> query);

        Task CreateRelationship<TChildModel, TRelationship>(Expression<Func<TModel, bool>> parentQuery, Expression<Func<TChildModel, bool>> childQuery, TRelationship relationship)
        where TChildModel : BaseNode, INode, new()
        where TRelationship : BaseRelationship, IRelationship, new();

        Task DeleteRelationship<TChildModel, TRelationship>(Expression<Func<TModel, bool>> parentQuery, Expression<Func<TChildModel, bool>> childQuery, TRelationship relationship)
        where TChildModel : BaseNode, INode, new()
        where TRelationship : BaseRelationship, IRelationship, new();
    }
}
