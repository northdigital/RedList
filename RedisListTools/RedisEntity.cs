/*
MIT License

Copyright (c) 2022 Panagiotis Piperopoulos

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using Newtonsoft.Json;
using StackExchange.Redis;

namespace gr.northdigital
{
  public abstract class RedisEntity<T> where T : RedisEntity<T>
  {
    public static implicit operator RedisEntity<T>(string json)
    {
      var obj = JsonConvert.DeserializeObject<T>(json);
      return obj;
    }

    public static implicit operator RedisEntity<T>(RedisValue redisValue)
    {
      var obj = JsonConvert.DeserializeObject<T>(redisValue.ToString());
      return obj;
    }

    public static implicit operator string(RedisEntity<T> entity)
    {
      return entity.ToString();
    }

    public static implicit operator RedisValue(RedisEntity<T> entity)
    {
      return entity.ToString();
    }

    public override string ToString()
    {
      return JsonConvert.SerializeObject(this);
    }
  }
}