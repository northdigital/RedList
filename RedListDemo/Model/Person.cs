﻿using gr.northdigital;

namespace RedListDemo.Model
{
  public class Person : RedisEntity<Person>
  {
    [Pk]
    public int Id { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Gender { get; set; }
  }
}