using gr.northdigital;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RedListDemo.Model;
using RedListDemo.Repository;
using System.Collections.Generic;

namespace RedListDemo.Pages
{
  public class IndexModel : PageModel
  {
    private readonly IRedisManager _redisManager;
    private readonly PersonRepository _personRepository;
    private const string REDIS_KEY = "people";

    public List<Person> People { get; set; } = new List<Person>();

    public IndexModel(IRedisManager redisManager, PersonRepository personRepository)
    {
      _redisManager = redisManager;
      _personRepository = personRepository;
    }

    public void OnPostFlushDb()
    {
      _redisManager.Server.FlushDatabase();

      People = _personRepository.GetAll(REDIS_KEY);
    }

    public void OnPostInsert()
    {
      _personRepository.Insert(REDIS_KEY, new Person {Id = 100, FirstName = "Lakis", LastName = "Lalakis", Gender="M"});
      _personRepository.Insert(REDIS_KEY, new Person {Id = 200, FirstName = "Pakis", LastName = "Papakis", Gender = "M" });
      _personRepository.Insert(REDIS_KEY, new Person {Id = 300, FirstName = "Foula", LastName = "Mpoula", Gender = "F" });

      People = _personRepository.GetAll(REDIS_KEY);
    }

    public void OnPostSelect()
    {
      var person = _personRepository.Select(REDIS_KEY, 300);

      People.Clear();

      if(person != null)
        People.Add(person);
    }

    public void OnPostSelectFilter()
    {
      People = _personRepository.Select(REDIS_KEY, (p) => p.FirstName.Contains("s"));
    }

    public void OnPostGetAll()
    {
      People = _personRepository.GetAll(REDIS_KEY);
    }

    public void OnPostDelete()
    {
      _personRepository.Delete(REDIS_KEY, 200);

      People = _personRepository.GetAll(REDIS_KEY);
    }

    public void OnPostUpdate()
    {
      var person = _personRepository.Select(REDIS_KEY, 100);

      if (person != null)
      {
        person.FirstName = "Tsaf";
        person.LastName = "Tsouf";
        _personRepository.Update(REDIS_KEY, person);
      }

      People = _personRepository.GetAll(REDIS_KEY);
    }
  }
}
