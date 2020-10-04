using System.Linq;

namespace System
{
    /// <summary>
    /// Añade extensiones a los tipos String.
    /// </summary>
    public static class EString
    {
        /// <summary>
        /// Permite obtener n palabras de un String
        /// </summary>
        /// <param name="input"></param>
        /// <param name="numWords">Número de palabras a obtener.</param>
        /// <param name="separator">Opcional, separador por el cual diferenciar una palabra.</param>
        /// <param name="union">Opcional, carácter que permite unir las palabras.</param>
        /// <returns></returns>
        public static string Words(this string input, uint numWords, string separator = " ", string union = " ")
        {
            return input == null ? null : string.Join(union, input.Split(new[] { separator }, StringSplitOptions.None).Take((int)numWords).ToArray());
        }

        /// <summary>
        /// Permite obtener n carácteres de un String
        /// </summary>
        /// <param name="input"></param>
        /// <param name="chars">Número de carácteres a obtener.</param>
        /// <param name="symbol">Opcional, agrega al final un string en caso de que la palabra sea mayor al número de carácteres a obtener.</param>
        /// <returns></returns>
        public static string Characters(this string input, uint chars, string symbol = "...")
        {
            if (input == null || input.Length <= (int)chars) return input;

            input = input.Substring(0, (int)chars);

            while (input[input.Length - 1] == ' ')
            {
                input = input.Substring(0, input.Length - 1);
            }

            return input + symbol;
        }
    }
}