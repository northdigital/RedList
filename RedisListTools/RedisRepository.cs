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

using System;
using System.Collections.Generic;

namespace gr.northdigital
{
  public class RedisRepository<T> where T : RedisEntity<T>
  {
    private IRedisManager _redisManager;

    public RedisRepository(IRedisManager redis = null)
    {
      _redisManager = redis;
    }

    /// <summary>
    /// Find and return the index of the element with PK == Id in the list under the key redisKey.
    /// </summary>
    /// <param name="redisKey"></param>
    /// <param name="Id"></param>
    /// <returns></returns>
    public int GetIndex(string redisKey, IComparable Id)
    {
      var redisValues = _redisManager.Db.ListRange(redisKey);

      int index = 0;

      foreach (var redisValue in redisValues)
      {
        T t = (T)redisValue;
        if (t.GetPkValue().Equals(Id))
          return index;

        index++;
      }

      return -1;
    }

    /// <summary>
    /// Find and return the element with PK == Id in the list under the key redisKey.
    /// </summary>
    /// <param name="redisKey"></param>
    /// <param name="Id"></param>
    /// <returns></returns>
    public T Select(string redisKey, IComparable Id)
    {
      var redisValues = _redisManager.Db.ListRange(redisKey);

      foreach (var redisValue in redisValues)
      {
        T t = (T)redisValue;
        if (t.GetPkValue().Equals(Id))
          return t;
      }

      return default;
    }

    public List<T> Select(string redisKey, Func<T, bool> filter)
    {
      List<T> retVal = new List<T>();

      var redisValues = _redisManager.Db.ListRange(redisKey);

      foreach (var redisValue in redisValues)
      {
        T t = (T)redisValue;
        if (filter(t))
          retVal.Add(t);
      }

      return retVal;
    }

    /// <summary>
    /// Insert the element t in the list under the key redisKey.
    /// </summary>
    /// <param name="redisKey"></param>
    /// <param name="t"></param>
    public void Insert(string redisKey, T t)
    {
      _redisManager.Db.ListRightPush(redisKey, t);      
    }

    /// <summary>
    /// Delete the element with Pk == Id from the list under the key redisKey.
    /// </summary>
    /// <param name="redisKey"></param>
    /// <param name="Id"></param>
    /// <returns></returns>
    public bool Delete(string redisKey, IComparable Id)
    {
      var index = GetIndex(redisKey, Id);
      
      if (index != -1)
      {
        var DELETED = "__DELETED__";
        _redisManager.Db.ListSetByIndex(redisKey, index, DELETED);
        _redisManager.Db.ListRemove(redisKey, DELETED);

        return true;
      }

      return false;
    }

    /// <summary>
    /// Updates the element of the key redisKey witch has the same Pk with the new values.
    /// The old element is deleted and the new T is inserted on the list.
    /// </summary>
    /// <param name="redisKey"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public bool Update(string redisKey, T t)
    {
      if(Delete(redisKey, t.GetPkValue()))
      {
        Insert(redisKey, t);
        return true;
      }

      return false;
    }

    /// <summary>
    /// Return all list elements for the key redisKey as a List<T>.
    /// </summary>
    /// <param name="redisKey"></param>
    /// <returns></returns>
    public List<T> GetAll(string redisKey)
    {
      List<T> retVal = new List<T>();

      var redisValues = _redisManager.Db.ListRange(redisKey);
      foreach (var redisValue in redisValues)
      {
        retVal.Add((T)redisValue);
      }

      return retVal;
    }

    /// <summary>
    /// Set key timeout.    
    /// </summary>
    /// <param name="redisKey"></param>
    /// <param name="timeSpan"></param>
    /// <returns></returns>
    public bool SetTimeout(string redisKey, TimeSpan timeSpan)
    {
       if (_redisManager.Db.KeyTimeToLive(redisKey) == null)
       {
         _redisManager.Db.KeyExpire(redisKey, timeSpan);
         return true;
       }
       
       return false;
    }
  }
}