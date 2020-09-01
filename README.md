# GenericRepository.Graph
This is a .Net Standard library which is built to handle Graph queries with Neo4j as a backend.

**Steps to consume the library:**

- In appsettings.json or web.config file, add Neo4j database configuration in following format,

```json
"Neo4jConfig": 
{
    "uri": "bolt://localhost:7687/",
    "username": "neo4j",
    "password": "localhost"
}
```
> Please note: Configurations are case sensitive.

- In Startup.cs add the following code,

```csharp
services.AddGraphDependencies(Configuration);
services.AddScoped<EntityRepository>(); // where EntityRepository is repository class, refer point 4
```
> This line will connect your application to Neo4j database provided in the first step.

- To start using the repository, firstly create node classes by inherting from 'BaseNode' and relationship classes by inherting from 'BaseRelationship' as follows,

```csharp
public class Person : BaseNode
{
    public Person() : base("Person") { }
}
```
> Here name of the class can be different from that of node's label in Neo4j, but you need to pass node's label in base class's constructor.

```csharp
public class EmployeeOf : BaseRelationship
{
    public EmployeeOf() : base("Employee_Of") { }
}
```
> Here, name of the class can be different from that of relationship's type in Neo4j, but you need to pass relationship's type in base class's constructor.

- Once node and relationship classes are created, create a repository class which will inherit from the abstract class 'Neo4jRepository' as follows,

```csharp
public class GenericRepositoryDBContext : DbContext
public class EntityRepository : Neo4jRepository<Person>
{
    public EntityRepository(IGraphClient graphClient) : base(graphClient)
    {
    
    }
}
```

> Please note: All the repository methods are implemented as virtual, meaning you have an option of overriding repository methods as per needs.

- Following is the list of repository methods supported in the current version,

```csharp
GetAll();
Create(TModel model);
Update(Expression<Func<TModel, bool>> query, TModel model);
Delete(Expression<Func<TModel, bool>> query);
Where(Expression<Func<TModel, bool>> query);
FirstOrDefault(Expression<Func<TModel, bool>> query);
CreateRelationship<TChildModel, TRelationship>(Expression<Func<TModel, bool>> parentQuery, Expression<Func<TChildModel, bool>> childQuery, TRelationship relationship);
DeleteRelationship<TChildModel, TRelationship>(Expression<Func<TModel, bool>> parentQuery, Expression<Func<TChildModel, bool>> childQuery, TRelationship relationship);
```

> You can always refer to 'HowToConsume.GenericRepository.Graph' project to understand how to consume the library
