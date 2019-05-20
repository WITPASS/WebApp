using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin
{
    public class Enum<T> where T : struct, IConvertible
    {
        public static IList<T> TypedList
        {
            get
            {
                if (!typeof(T).IsEnum) throw new ArgumentException("T must be an enumerated type");

                var list = new List<T>();

                foreach (var en in Enum.GetValues(typeof(T))) list.Add((T)en);

                return list;
            }
        }

        public static int Count
        {
            get
            {
                if (!typeof(T).IsEnum) throw new ArgumentException("T must be an enumerated type");

                return Enum.GetNames(typeof(T)).Length;
            }
        }
    }
}
