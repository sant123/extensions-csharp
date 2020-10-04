using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace System.Data.SqlClient
{
    /// <summary>
    /// Añade extensiones a los tipos IDataReader.
    /// </summary>
    public static class EReader
    {
        /// <summary>
        /// Permite crear una lista de objetos dinámica de acuerdo a las columnas presentes en la consulta. Si no hay más resultados en el reader, este se cierra.
        /// </summary>
        /// <param name="reader">IDatareader o SqlReader.</param>
        /// <returns>Lista dinamyc(ExpandoObject) con los datos de la consulta. Si se quiere acceder por indice e.g : obj["data"], se debe agregar este casting: ((IDictionary&lt;String, Object&gt;)obj)["data"].</returns>
        public static List<dynamic> GetListFromReader(this IDataReader reader)
        {
            //Se obtienen las llaves del reader.
            var keys = reader.Keys();

            //Creamos la lista.
            var list = new List<dynamic>();

            try
            {
                while (reader.Read())
                {
                    //Creamos un objeto dinámico.
                    dynamic obj = new ExpandoObject();

                    foreach (var key in keys)
                    {
                        var prop = reader[key];
                        //Las propiedades que no se encuentren en el objeto se crean dinámicamente. Si esta es un DBNull(para SQL que es igual a null), establezca null en la propiedad.
                        ((IDictionary<string, object>)obj)[key] = prop is DBNull ? null : prop;
                    }

                    //Retornamos el objeto.
                    list.Add(obj);
                }

                return list;
            }
            finally
            {
                //Avanza el reader a otro resultado si es que existe. Sino, cierra el reader.
                if (!reader.NextResult())
                    reader.Dispose();
            }
        }

        /// <summary>
        /// Permite crear una lista T de un DataReader. Si es un tipo primitivo o una enumeración, mapea solo la primera columna. Datetime y Decimal se incluyen como primitivos. Si no hay más resultados en el reader, este se cierra.
        /// </summary>
        /// <typeparam name="T">Tipo a retornar en la lista.</typeparam>
        /// <param name="reader">IDatareader o SqlReader.</param>
        /// <returns>Lista T con los datos de la consulta.</returns>
        public static List<T> GetListFromReader<T>(this IDataReader reader) where T : new()
        {
            //Se obtiene el tipo de la T, si es de un tipo nullable obtenga el tipo que se esta aplicando el nullable.
            var thisType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

            //Si es primitivo o de tipo Enum...
            if (thisType.IsPrimitive() || thisType.IsEnum)
                return reader._GetListFromReaderPrimitive<T>();

            return reader._GetListFromReader<T>();
        }

        /// <summary>
        /// Permite crear una lista de strings bazada en la primera columna del reader. Si no hay más resultados en el reader, este se cierra.
        /// </summary>
        /// <param name="reader">IDatareader o SqlReader.</param>
        /// <returns>Lista de strings con los datos de la consulta de la primera columna.</returns>
        public static List<string> GetStringListFromReader(this IDataReader reader)
        {
            return reader._GetListFromReaderPrimitive<string>();
        }

        #region Metodos privados

        private static List<T> _GetListFromReader<T>(this IDataReader reader) where T : new()
        {
            //  Columnas del reader.
            var readerKeys = reader.Keys();

            //  Hace una intersección entre las llaves que contiene el reader con las que tiene el objeto incluyendo el DataAnnotation ColumnAs.
            var keys = typeof(T).GetProperties().Select(property =>
            {
                var columnName = property.Name;
                var columnAs = GetColumnAsValue(property);

                return new
                {
                    ColumnName = columnName,
                    ColumnAux = columnAs ?? columnName
                };

            }).Where(obj => readerKeys.Any(key => key == obj.ColumnAux));
            
            //  Control de errores
            var currentColumn = string.Empty;
            var currentType = string.Empty;
            var typeOfObj = string.Empty;

            //Creamos la lista.

            var list = new List<T>();

            try
            {
                while (reader.Read())
                {
                    var newObj = new T();

                    foreach (var key in keys)
                    {
                        //Obtenemos una propiedad del objeto.
                        var prop = newObj.GetType().GetProperty(key.ColumnName);

                        //Obtenemos el valor del reader.
                        var propValue = reader[key.ColumnAux];

                        currentColumn = key.ColumnName;
                        currentType = prop.PropertyType.ToString();
                        typeOfObj = propValue.GetType().Name;

                        if (propValue is DBNull) continue;

                        //Si no es DBNull entonces agregue el valor al objeto.
                        // Obtiene el tipo de la propiedad, ya sea de tipo nullable o normal.
                        var someType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                        //Revisamos si los tipos entre la propiedad del objeto al valor del reader son iguales; sino...
                        if (propValue.GetType() != someType)
                        {
                            // Si es una enumeración obtengala!
                            if (someType.IsEnum)
                                prop.SetValue(newObj, Enum.ToObject(someType, propValue));
                            // Si el valor del reader no es igual al de la propiedad, intente convertirlo.
                            else
                                prop.SetValue(newObj, Convert.ChangeType(propValue, someType));
                        }
                        else
                            prop.SetValue(newObj, propValue);
                    }

                    //Se añade el objeto a la lista.
                    list.Add(newObj);
                }

                return list;
            }
            catch (InvalidCastException)
            {
                throw new InvalidCastException("La columna " + currentColumn + " no es de tipo " + currentType + ". Es de tipo " + typeOfObj + ".");
            }
            catch (FormatException)
            {
                throw new FormatException("La columna " + currentColumn + " no es de tipo " + currentType + ". Es de tipo " + typeOfObj + ".");
            }
            finally
            {
                //Avanza el reader a otro resultado si es que existe. Sino, cierra el reader.
                if (!reader.NextResult())
                    reader.Dispose();
            }

        }

        private static List<T> _GetListFromReaderPrimitive<T>(this IDataReader reader)
        {
            var thisType = typeof(T);

            //Control de errores
            var column = string.Empty;
            var currentType = thisType.ToString();
            var typeOfObj = string.Empty;

            //Creamos la lista
            var list = new List<T>();

            //Traiga el valor de un nullable si es que lo es, o simplemente tenga el typeof de la T.
            var someType = Nullable.GetUnderlyingType(thisType) ?? thisType;
            //Permite saber si es de tipo nullable. Nullable<T> isNullable....
            var isNullable = thisType.IsGenericType && thisType.GetGenericTypeDefinition() == typeof(Nullable<>);

            try
            {
                while (reader.Read())
                {
                    column = reader.GetName(0);
                    column = string.IsNullOrEmpty(column) ? "anonima" : column;

                    var prop = reader[0];
                    typeOfObj = prop.GetType().Name;

                    if (!(prop is DBNull))
                    {
                        // Si el valor del reader no es igual a la T primitiva, intente convertirlo.
                        if (prop.GetType() != someType)
                        {
                            if (someType.IsEnum)
                                list.Add((T)Enum.ToObject(someType, prop));
                            else
                                list.Add((T)Convert.ChangeType(prop, someType));
                        }
                        else
                            list.Add((T)prop);
                    }
                    // Si es nulo el campo de la base de datos y si el campo es de tipo nullable o por defecto acepta nullables....
                    else if (isNullable || default(T) == null)
                        list.Add(default(T));
                }

                return list;
            }
            catch (InvalidCastException)
            {
                throw new InvalidCastException("La columna " + column + " no es de tipo " + currentType + ". Es de tipo " + typeOfObj + ".");
            }
            catch (FormatException)
            {
                throw new FormatException("La columna " + column + " no es de tipo " + currentType + ". Es de tipo " + typeOfObj + ".");
            }
            finally
            {
                //Avanza el reader a otro resultado si es que existe. Sino, cierra el reader.
                if (!reader.NextResult())
                    reader.Dispose();
            }
        }

        /// <summary>
        /// Permite obtener el valor de la columna del DataAnnotation ColumnAs.
        /// </summary>
        /// <param name="property">Propiedad del objeto.</param>
        /// <returns></returns>
        private static string GetColumnAsValue(PropertyInfo property)
        {
            var columnAs = Attribute.GetCustomAttribute(property, typeof(ColumnAsAttribute)) as ColumnAsAttribute;

            return columnAs?.ColumnName;
        }

        #endregion

        /// <summary>
        /// Permite obtener las columnas de un DataReader.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static IEnumerable<string> Keys(this IDataReader reader)
        {
            var length = reader.FieldCount;

            for (var i = 0; i < length; i++)
            {
                yield return reader.GetName(i);
            }
        }
    }
}
