namespace System
{
    /// <summary>
    /// Añade extensiones a System.DateTime
    /// </summary>
    public static class EDatetime
    {
        /// <summary>
        /// Permite saber si el string dado es una fecha.
        /// </summary>
        /// <param name="txtDate"></param>
        /// <returns></returns>
        public static bool IsDateTime(this string txtDate)
        {
            DateTime tempDate;

            return DateTime.TryParse(txtDate, out tempDate);
        }
    }
}
