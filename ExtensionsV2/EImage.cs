using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace System.Drawing
{
    /// <summary>
    /// Añade extensiones a los tipos Image.
    /// </summary>
    public static class EImage
    {
        /// <summary>
        /// Permite redimensionar una imagen.
        /// </summary>
        /// <param name="img"></param>
        /// <param name="size">Un objeto con alto y ancho.</param>
        /// <returns></returns>
        public static Image ResizeImage(this Image img, Size size)
        {
            var sourceWidth = img.Width;
            var sourceHeight = img.Height;

            var nPercentW = size.Width / (float)sourceWidth;
            var nPercentH = size.Height / (float)sourceHeight;

            var nPercent = nPercentH < nPercentW ? nPercentH : nPercentW;

            var destWidth = (int)(sourceWidth * nPercent);
            var destHeight = (int)(sourceHeight * nPercent);

            var b = new Bitmap(destWidth, destHeight);
            var g = Graphics.FromImage(b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(img, 0, 0, destWidth, destHeight);
            g.Dispose();

            return b;
        }

        /// <summary>
        /// Corta la imagen con el rectangulo dado.
        /// </summary>
        /// <param name="img"></param>
        /// <param name="cropArea"></param>
        /// <returns></returns>
        public static Image CropImage(this Image img, Rectangle cropArea)
        {
            var bmp = new Bitmap(cropArea.Width, cropArea.Height);

            using (var g = Graphics.FromImage(bmp))
            {
                g.DrawImage(img, 0, 0, cropArea, GraphicsUnit.Pixel);
            }

            return bmp;
        }

        /// <summary>
        /// Corta la imagen con el rectangulo dado y la pone sobre la imagen original.
        /// </summary>
        /// <param name="img"></param>
        /// <param name="cropArea">Ancho, alto, x, y.</param>
        /// <returns></returns>
        public static Image CropImageWithPreview(this Image img, Rectangle cropArea)
        {
            var bmp = new Bitmap(img);

            using (var g = Graphics.FromImage(bmp))
            {
                g.DrawImage(img, 0, 0, cropArea, GraphicsUnit.Pixel);
            }

            return bmp;
        }

        /// <summary>
        /// Permite convertir una imagen a un stream.
        /// </summary>
        /// <param name="image">Imagen</param>
        /// <param name="format">Formato de la imagen</param>
        /// <returns></returns>
        public static MemoryStream ToStream(this Image image, ImageFormat format)
        {
            var stream = new MemoryStream();
            //  Se guarda la imagen en un Stream de acuerdo a la extensión del archivo.
            image.Save(stream, format);
            //  Permite leer el Stream desde el principio.
            stream.Position = 0;

            return stream;
        }

        /// <summary>
        /// Permite obtener el formato de una imagen de acuerdo a la extensión.
        /// </summary>
        /// <param name="fileName">Nombre de la imagen. (imagen.png)</param>
        /// <returns></returns>
        public static ImageFormat GetImageFormat(this string fileName)
        {
            var extension = Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(extension))
                return null;

            switch (extension.ToLower())
            {
                case @".bmp":
                    return ImageFormat.Bmp;

                case @".gif":
                    return ImageFormat.Gif;

                case @".ico":
                    return ImageFormat.Icon;

                case @".jpg":
                case @".jpeg":
                    return ImageFormat.Jpeg;

                case @".png":
                    return ImageFormat.Png;

                case @".tif":
                case @".tiff":
                    return ImageFormat.Tiff;

                case @".wmf":
                    return ImageFormat.Wmf;

                default:
                    return null;
            }
        }

        /// <summary>
        /// Crea una imagen desde una base64. El formato svg aún no esta soportado.
        /// </summary>
        /// <param name="base64">String base64</param>
        /// <returns></returns>
        public static Image ToImage(this string base64)
        {
            if (base64.Contains("data:image"))
                base64 = base64.Split(',')[1];

            var data = System.Convert.FromBase64String(base64);
            var ms = new MemoryStream(data);
            var img = Image.FromStream(ms);

            return img;
        }

        /// <summary>
        /// Permite obtener una extensión de acuerdo al formato de imagen. Los tipos Emf y Exif no estan soportados.
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string GetExtensionFromFormat(this ImageFormat format)
        {
            if (Equals(format, ImageFormat.Bmp))
                return @".bmp";
            if (Equals(format, ImageFormat.Gif))
                return @".gif";
            if (Equals(format, ImageFormat.Icon))
                return @".ico";
            if (Equals(format, ImageFormat.Jpeg))
                return @".jpg";
            if (Equals(format, ImageFormat.Png))
                return @".png";
            if (Equals(format, ImageFormat.Tiff))
                return @".tiff";
            if (Equals(format, ImageFormat.Wmf))
                return @".wmf";
            return null;
        }

        /// <summary>
        /// Permite convertir una Imagen a una Base64.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="format">Formato de la imagen.</param>
        /// <param name="extension">(Opcional) Permite establecer la extension en la base64. e.g: (data:image/{png};base64,)</param>
        /// <returns></returns>
        public static string ToBase64(this Image image, ImageFormat format, string extension = null)
        {
            var formatBase64 = "data:image/{format};base64,";
            var sb = new StringBuilder();
            extension = extension ?? GetExtensionFromFormat(format).Substring(1);

            using (var ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                var imageBytes = ms.ToArray();

                sb.Append(formatBase64.Replace("{format}", extension));
                // Convert byte[] to Base64 String
                sb.Append(Convert.ToBase64String(imageBytes));
                return sb.ToString();
            }
        }
    }
}
