using Newtonsoft.Json;
using System.Collections.Generic;

namespace System
{
    /// <summary>
    /// Añade más métodos para serialización.
    /// </summary>
    public static class ESerialization
    {
        /// <summary>
        /// Permite serializar una enumeración a un JSON.
        /// </summary>
        /// <typeparam name="T">Enumeración.</typeparam>
        /// <returns></returns>
        public static string SerializeEnum<T>()
        {
            var enumeration = typeof(T);

            if (!enumeration.IsEnum) { return "Cannot deserialize object different of Enum"; }

            var enumVals = new List<object>();

            foreach (var item in Enum.GetValues(enumeration))
            {

                enumVals.Add(new
                {
                    id = (int)item,
                    name = item.ToString()
                });
            }
            return JsonConvert.SerializeObject(enumVals);
        }
    }
}
