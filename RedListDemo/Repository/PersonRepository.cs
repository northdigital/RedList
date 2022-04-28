using gr.northdigital;
using RedListDemo.Model;

namespace RedListDemo.Repository
{
  public class PersonRepository : RedisRepository<Person>
  {
    public PersonRepository(IRedisManager redisManager) : base(redisManager)
    {
    }
  }
}
