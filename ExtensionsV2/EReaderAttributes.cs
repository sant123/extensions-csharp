namespace System.Data.SqlClient
{
    /// <summary>
    /// Permite enlazar una columna SQL con un alias envés del nombre de la propiedad.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAsAttribute : Attribute
    {
        /// <summary>
        /// Nombre del alias de la columna.
        /// </summary>
        public string ColumnName { get; private set; }

        /// <summary>
        /// Establece el alias de la columna.
        /// </summary>
        /// <param name="columnName">Nombre del alias de la columna.</param>
        /// <exception cref="InvalidOperationException">Si ColumnName es nulo o vacio.</exception>
        public ColumnAsAttribute(string columnName)
        {
            if (columnName == null)
                throw new InvalidOperationException("ColumnName no puede ser nulo.");

            if (columnName == string.Empty)
                throw new InvalidOperationException("ColumnName no puede estar vacio.");

            ColumnName = columnName;
        }
    }
}
