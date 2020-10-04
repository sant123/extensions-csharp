using System.Linq;

namespace System.Globalization
{
    /// <summary>
    /// Añade más métodos al namespace System.Globalization.
    /// </summary>
    public static class ECulture
    {
        /// <summary>
        /// Permite obtener los meses de una cultura. Ej: es-ES = España.
        /// </summary>
        /// <param name="culture">Cultura.</param>
        /// <returns></returns>
        public static string[] GetMonthsFromCulture(string culture)
        {
            return new CultureInfo(culture).DateTimeFormat.MonthNames.Where(c => c.Length > 0).Select(c => c.Substring(0, 1).ToUpper() + c.Substring(1, c.Length - 1)).ToArray();
        }
    }
}
