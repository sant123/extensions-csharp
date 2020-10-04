using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace System
{
    /// <summary>
    /// Añade extensiones a los tipos Object.
    /// </summary>
    public static class EObject
    {
        /// <summary>
        /// Permite obtener un valor de una propiedad.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="property">Propiedad a buscar.</param>
        /// <returns></returns>
        public static object GetValue(this object obj, string property)
        {
            object objValue = null;

            if (obj is IDynamicMetaObjectProvider)
                IsPropertyDefinedInDynamicObject(obj, property, out objValue);

            if (objValue != null)
                return objValue;

            var propertyValue = obj.GetType().GetProperty(property);

            return propertyValue?.GetValue(obj);
        }

        /// <summary>
        /// Permite copiar las propiedades de un objeto en otro.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Objeto a convertir.</param>
        /// <returns></returns>
        public static T ConvertObjTo<T>(this object obj) where T : new()
        {
            var newObject = new T();

            var typeObject = obj.GetType();
            var typeNewObject = newObject.GetType();

            if (obj is IDynamicMetaObjectProvider)
            {
                var dynamicObject = ((IDictionary<string, object>)obj);

                foreach (var property in dynamicObject.Keys)
                {
                    var prop = typeNewObject.GetProperty(property);

                    if (prop != null)
                    {
                        // Si la propiedad es de tipo nullable, obtenga el tipo del Nullable.
                        var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                        //Si es una enumeración, obtengala.
                        if (type.IsEnum && dynamicObject[property] != null)
                            prop.SetValue(newObject, Enum.ToObject(type, dynamicObject[property]));
                        else
                            prop.SetValue(newObject, dynamicObject[property]);
                    }
                }

                return newObject;
            }

            foreach (var property in typeObject.GetProperties())
            {
                var prop = typeNewObject.GetProperty(property.Name);

                if (prop != null)
                {
                    // Si la propiedad es de tipo nullable, obtenga el tipo del Nullable.
                    var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                    //Si es una enumeración, obtengala.
                    if (type.IsEnum && property.GetValue(obj) != null)
                        prop.SetValue(newObject, Enum.ToObject(type, property.GetValue(obj)));
                    else
                        prop.SetValue(newObject, property.GetValue(obj));

                }
            }

            return newObject;
        }

        /// <summary>
        /// Permite saber si un objeto es primitivo de acuerdo a VB.NET. Si es de tipo Nullable, va a evaluar el tipo que esta como Nullable.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsPrimitive(this object obj)
        {
            var type = obj as Type ?? obj.GetType();
            type = Nullable.GetUnderlyingType(type) ?? type;

            return type.IsPrimitive || type.Name == "String" || type.Name == "DateTime" || type.Name == "Decimal";
        }

        /// <summary>
        /// Permite obtener el nombre de todas las propiedades en un objeto.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetPropertiesNames(this object obj)
        {
            if (obj is IDynamicMetaObjectProvider)
                return ((IDictionary<string, object>)obj).Keys;

            return obj.GetType().GetProperties().Select(c => c.Name);
        }

        /// <summary>
        /// Permite saber si una propiedad esta presente en un ExpandoObject.
        /// </summary>
        /// <param name="obj">Objeto a evaluar.</param>
        /// <param name="propertyName">Nombre de la propiedad.</param>
        /// <param name="value">Valor retornado por el método.</param>
        /// <returns></returns>
        private static bool IsPropertyDefinedInDynamicObject(object obj, string propertyName, out object value)
        {
            if (!(obj is IDynamicMetaObjectProvider))
            {
                value = null;
                return false;
            }

            try
            {
                value = ((IDictionary<string, object>)obj)[propertyName];
                return true;
            }
            catch (Exception)
            {
                value = null;
                return false;
            }
        }
    }
}