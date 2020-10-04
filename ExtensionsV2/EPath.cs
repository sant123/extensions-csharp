using System.Linq;
using System.Text;

namespace System.IO
{
    /// <summary>
    /// Añade extensiones a System.IO.Path
    /// </summary>
    public static class EPath
    {
        /// <summary>
        /// Limpia el string para ser un directorio valido.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static string CleanDirectoryName(this string directory)
        {
            return Path.GetInvalidPathChars().Aggregate(directory, (current, c) => current.Replace(c.ToString(), string.Empty));
        }

        /// <summary>
        /// Limpia el string para ser un archivo valido.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string CleanFileName(this string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
        }

        /// <summary>
        /// Permite obtener un nombre único de una carpeta en el Path especificado añadiendo (0..1...2) al nombre de la carpeta.
        /// </summary>
        /// <param name="path">Directorio a guardar la carpeta.</param>
        /// <param name="folderName">Nombre de la carpeta.</param>
        /// <returns>Path del directorio con la carpeta.</returns>
        public static string GetUniqueFolder(string path, string folderName)
        {
            var sb = new StringBuilder(Path.Combine(path, folderName));
            var aux = sb.ToString();
            var count = 1;

            while (Directory.Exists(aux))
            {
                sb.Clear();
                sb.Append(Path.Combine(path, folderName));
                sb.Append(" (");
                sb.Append(count.ToString());
                sb.Append(")");
                aux = sb.ToString();
                count++;
            }

            return aux;
        }

        /// <summary>
        /// Crea una carpeta única en el Path proporcionado.
        /// </summary>
        /// <param name="path">Directorio a guardar la carpeta.</param>
        /// <param name="folderName">Nombre de la carpeta.</param>
        /// <returns>Path del directorio con la carpeta.</returns>
        public static string CreateUniqueFolder(string path, string folderName)
        {
            var folder = GetUniqueFolder(path, folderName);
            Directory.CreateDirectory(folder);

            return folder;
        }

        /// <summary>
        /// Obtiene el último nombre del directorio proporcionado.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetLastPath(string path)
        {
            return new DirectoryInfo(path).Name;
        }

        /// <summary>
        /// Permite obtener un nombre único de un archivo en el Path especificado añadiendo (0..1...2) al nombre del archivo.
        /// </summary>
        /// <param name="path">Path del destino del archivo.</param>
        /// <param name="fileName">Nombre del archivo.</param>
        /// <returns></returns>
        public static string GetUniqueFileName(string path, string fileName)
        {
            var sb = new StringBuilder(Path.Combine(path, fileName));
            var aux = sb.ToString();
            var count = 1;

            while (File.Exists(aux))
            {
                sb.Clear();

                var sbFile = new StringBuilder();
                sbFile.Append(Path.GetFileNameWithoutExtension(fileName));
                sbFile.Append(" (");
                sbFile.Append(count.ToString());
                sbFile.Append(")");
                sbFile.Append(Path.GetExtension(fileName));

                sb.Append(Path.Combine(path, sbFile.ToString()));
                aux = sb.ToString();
                count++;
            }

            return aux;
        }
    }
}
