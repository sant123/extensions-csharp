using System.Collections.Generic;

namespace System.Data.SqlClient
{
    /// <summary>
    /// Añade extensiones a System.Data.SqlClient
    /// </summary>
    public static class ESql
    {
        /// <summary>
        /// Si el objeto es nulo, retorna un DbNull.Value.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object CheckIfDbNull(this object obj)
        {
            return obj ?? DBNull.Value;
        }

        /// <summary>
        /// Permite ejecutar una consulta SQL con los parámetros dados y retorna el número de filas afectadas.
        /// </summary>
        /// <param name="cnnString">Conexión a la base de datos</param>
        /// <param name="query">Sentencia a ejecutar</param>
        /// <param name="parameters">(Opcional) Parámetros para la consulta</param>
        /// <param name="commandType">(Opcional) Tipo de comando para la consulta</param>
        /// <returns>Número de filas afectadas.</returns>
        public static int ExecuteNonQuery(string cnnString, string query, SqlParameter[] parameters = null, CommandType commandType = CommandType.Text)
        {
            var command = GetCommand(cnnString, query, parameters, commandType);

            try
            {
                return command.ExecuteNonQuery();
            }
            finally
            {
                command.Connection.Dispose();
                command.Dispose();
            }
        }

        /// <summary>
        /// Permite ejecutar una consulta SQL con los parámetros dados y retorna un objeto.
        /// </summary>
        /// <param name="cnnString">Conexión a la base de datos</param>
        /// <param name="query">Sentencia a ejecutar</param>
        /// <param name="parameters">(Opcional) Parámetros para la consulta</param>
        /// <param name="commandType">(Opcional) Tipo de comando para la consulta</param>
        /// <returns>El objeto retornado desde la base de datos.</returns>
        public static object ExecuteScalar(string cnnString, string query, SqlParameter[] parameters = null, CommandType commandType = CommandType.Text)
        {
            var command = GetCommand(cnnString, query, parameters, commandType);

            try
            {
                return command.ExecuteScalar();
            }
            finally
            {
                command.Connection.Dispose();
                command.Dispose();
            }
        }

        /// <summary>
        /// Permite ejecutar una consulta SQL con los parámetros dados y retorna un objeto T.
        /// </summary>
        /// <param name="cnnString">Conexión a la base de datos</param>
        /// <param name="query">Sentencia a ejecutar</param>
        /// <param name="parameters">(Opcional) Parámetros para la consulta</param>
        /// <param name="commandType">(Opcional) Tipo de comando para la consulta</param>
        /// <returns>El objeto retornado desde la base de datos convertido a T.</returns>
        /// <exception cref="FormatException">Cuando el valor no se puede convertir a un tipo. e.g: 123P a int.</exception>
        public static T ExecuteScalar<T>(string cnnString, string query, SqlParameter[] parameters = null, CommandType commandType = CommandType.Text)
        {
            var command = GetCommand(cnnString, query, parameters, commandType);

            try
            {
                return (T)Convert.ChangeType(command.ExecuteScalar(), typeof(T));
            }
            finally
            {
                command.Connection.Dispose();
                command.Dispose();
            }
        }

        /// <summary>
        /// Permite ejecutar una consulta SQL con los parámetros dados y retorna un reader. Este reader deberá cerrarse más adelante con el método Dispose() o dentro de un using.
        /// </summary>
        /// <param name="cnnString">Conexión a la base de datos</param>
        /// <param name="query">Sentencia a ejecutar</param>
        /// <param name="parameters">(Opcional) Parámetros para la consulta</param>
        /// <param name="commandType">(Opcional) Tipo de comando para la consulta</param>
        /// <returns>IDataReader</returns>
        public static IDataReader ExecuteReader(string cnnString, string query, SqlParameter[] parameters = null, CommandType commandType = CommandType.Text)
        {
            //Retorna un reader, al cerrarse el reader cierra la conexión: CommandBehavior.CloseConnection.
            return GetCommand(cnnString, query, parameters, commandType).ExecuteReader(CommandBehavior.CloseConnection);
        }

        /// <summary>
        /// Permite ejecutar una consulta SQL con los parámetros dados y retorna una lista dinámica, estos objetos son construidos de acuerdo a las columnas presentes de la consulta.
        /// </summary>
        /// <param name="cnnString">Conexión a la base de datos</param>
        /// <param name="query">Sentencia a ejecutar</param>
        /// <param name="parameters">(Opcional) Parámetros para la consulta</param>
        /// <param name="commandType">(Opcional) Tipo de comando para la consulta</param>
        /// <returns>Lista dinamyc(ExpandoObject) con los datos de la consulta. Si se quiere acceder por indice e.g : obj["data"], se debe agregar este casting: ((IDictionary&lt;String, Object&gt;)obj)["data"].</returns>
        public static List<dynamic> ExecuteQuery(string cnnString, string query, SqlParameter[] parameters = null, CommandType commandType = CommandType.Text)
        {
            //Obtiene un reader de acuerdo a la conexión, consulta y parámetros.
            using (var reader = ExecuteReader(cnnString, query, parameters, commandType))
            {
                //Obtiene una lista de objetos dinámicos del reader. Cada objeto dinámico se crea a partir de las columnas disponibles del reader.
                var list = reader.GetListFromReader();

                return list;
            }

        }

        /// <summary>
        /// Permite ejecutar una consulta SQL con los parámetros dados y retorna una lista T. Si es un tipo primitivo o una enumeración, mapea solo la primera columna. Datetime y Decimal se incluyen como primitivos.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cnnString">Conexión a la base de datos</param>
        /// <param name="query">Sentencia a ejecutar</param>
        /// <param name="parameters">(Opcional) Parámetros para la consulta</param>
        /// <param name="commandType">(Opcional) Tipo de comando para la consulta</param>
        /// <returns>Lista T con los datos de la consulta</returns>
        public static List<T> ExecuteQuery<T>(string cnnString, string query, SqlParameter[] parameters = null, CommandType commandType = CommandType.Text) where T : new()
        {
            //Obtiene un reader de acuerdo a la conexión, consulta y parámetros.
            using (var reader = ExecuteReader(cnnString, query, parameters, commandType))
            {
                //Obtiene la lista de strings del reader.
                var list = reader.GetListFromReader<T>();

                return list;
            }
        }

        /// <summary>
        /// Permite ejecutar una consulta SQL con los parámetros dados y retorna una lista de strings sobre la primera columna.
        /// </summary>
        /// <param name="cnnString">Conexión a la base de datos</param>
        /// <param name="query">Sentencia a ejecutar</param>
        /// <param name="parameters">(Opcional) Parámetros para la consulta</param>
        /// <param name="commandType">(Opcional) Tipo de comando para la consulta</param>
        /// <returns>Lista T con los datos de la consulta</returns>
        public static List<string> ExecuteStringQuery(string cnnString, string query, SqlParameter[] parameters = null, CommandType commandType = CommandType.Text)
        {
            //Obtiene un reader de acuerdo a la conexión, consulta y parámetros.
            using (var reader = ExecuteReader(cnnString, query, parameters, commandType))
            {
                //Obtiene la lista de strings del reader.
                var list = reader.GetStringListFromReader();

                return list;
            }
        }

        #region Metodos privados

        /// <summary>
        /// Obtiene un SqlCommand con los parámetros dados.
        /// </summary>
        /// <param name="cnnString">Conexión a la base de datos</param>
        /// <param name="query">Sentencia a ejecutar</param>
        /// <param name="parameters">(Opcional) Parámetros para la consulta</param>
        /// <param name="commandType">(Opcional) Tipo de comando para la consulta</param>
        /// <returns></returns>
        private static SqlCommand GetCommand(string cnnString, string query, SqlParameter[] parameters = null, CommandType commandType = CommandType.Text)
        {
            parameters = parameters ?? new SqlParameter[] { };

            var cnn = new SqlConnection(cnnString);
            cnn.Open();

            var command = new SqlCommand(query, cnn) {CommandType = commandType};
            command.Parameters.AddRange(parameters);

            return command;
        }

        #endregion

        /// <summary>
        /// Funcion que convierte la cadena de conexión ADO que viene desde ASP clásico y la convierte en una cadena de conexión sintácticamente válida
        /// para utilizar tecnologías de acceso a datos .Net (ADO .Net, Entity Framework, etc.)    
        /// </summary>
        /// <param name="adoConnString">Recibe la cadena de conexión ADO que viene desde ASP Clásico</param>
        /// <returns></returns>
        public static string AdoCnnToNetCnn(this string adoConnString)
        {
            var connStrBuilder = new SqlConnectionStringBuilder();
            Array parametros = adoConnString.Split(';');

            foreach (string item in parametros)
            {
                var ss = item.Trim().ToLower();

                if (ss.Contains("data source="))
                {
                    connStrBuilder.DataSource = item.Trim().Substring(12);
                }
                if (ss.Contains("uid="))
                {
                    connStrBuilder.UserID = item.Trim().Substring(4);
                }
                if (ss.Contains("user id="))
                {
                    connStrBuilder.UserID = item.Trim().Substring(8);
                }
                if (ss.Contains("password="))
                {
                    connStrBuilder.Password = item.Trim().Substring(9);
                }
                if (ss.Contains("initial catalog="))
                {
                    connStrBuilder.InitialCatalog = item.Trim().Substring(16);
                }
                if (ss.Contains("application name="))
                {
                    connStrBuilder.ApplicationName = item.Trim().Substring(17);
                }
            }

            return connStrBuilder.ConnectionString;
        }
    }
}
