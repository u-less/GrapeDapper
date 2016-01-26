using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrapeDapper.Core
{
    public class SingleInstance<T> where T:new()
    {
        public static T Instance = new T();
    }
}
