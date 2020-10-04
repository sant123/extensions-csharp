using System.Collections.Specialized;
using System.Net;

namespace System.Web
{
    /// <summary>
    /// Añade métodos a System.Web.
    /// </summary>
    public static class EWeb
    {
        /// <summary>
        /// Establece un código de estado y devuelve nulo.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="httpStatusCode"></param>
        /// <returns></returns>
        public static object SetStatus(this HttpResponseBase response, HttpStatusCode httpStatusCode)
        {
            response.StatusCode = (int)httpStatusCode;

            return null;
        }

        /// <summary>
        /// Establece un código de estado y devuelve un objeto T dado.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="httpStatusCode"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T SetStatus<T>(this HttpResponseBase response, HttpStatusCode httpStatusCode, T obj = default(T))
        {
            response.StatusCode = (int)httpStatusCode;

            return obj;
        }

        /// <summary>
        /// Permite obtener un valor desde el QueryString de la solicitud actual.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetQueryString(string key)
        {
            return HttpContext.Current.Request.QueryString.Get(key);
        }

        /// <summary>
        /// Permite obtener un header HTTP del cliente de la solicitud actual.
        /// </summary>
        /// <param name="key">Nombre del header. e.g: X-Requested-With</param>
        /// <returns></returns>
        public static string GetHeader(string key)
        {
            return HttpContext.Current.Request.Headers[key];
        }

        /// <summary>
        /// Permite obtener una cookie de la solicitud actual.
        /// </summary>
        /// <param name="cookieName">El nombre de la cookie.</param>
        /// <returns></returns>
        public static string GetCookie(string cookieName)
        {
            var cookie = HttpContext.Current.Request.Cookies[cookieName];
            return cookie?.Value;
        }

        /// <summary>
        /// Permite crear una cookie a la solicitud actual.
        /// </summary>
        /// <param name="cookieName">El nombre de la cookie.</param>
        /// <param name="cookieValue">El valor de la cookie.</param>
        /// <param name="expires">Opcional, permite establecer la fecha de expiración de la cookie.</param>
        public static void SetCookie(string cookieName, string cookieValue, DateTime? expires = null)
        {
            var cookie = new HttpCookie(cookieName, cookieValue);

            if (expires != null)
                cookie.Expires = expires.Value;

            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        /// <summary>
        /// Permite eliminar una cookie en la solicitud actual.
        /// </summary>
        /// <param name="cookieName">El nombre de la cookie.</param>
        public static void DeleteCookie(string cookieName)
        {
            HttpContext.Current.Response.Cookies[cookieName].Expires = DateTime.Now.AddDays(-1);
        }

        /// <summary>
        /// Permite convertir el querystring de una URL en un NameValueCollection.
        /// </summary>
        /// <param name="url">Url con querystring.</param>
        /// <returns>NameValueCollection(llave, valor) del querystring.</returns>
        public static NameValueCollection GetQueryStringFromUrl(string url)
        {
            return HttpUtility.ParseQueryString(url.Substring(url.IndexOf('?')).Split('#')[0]);
        }

        /// <summary>
        /// Permite agregar una variable de querystring al objeto Uri.
        /// </summary>
        /// <param name="uri">Objeto Uri.</param>
        /// <param name="name">Nombre de la variable.</param>
        /// <param name="value">Valor.</param>
        /// <returns></returns>
        public static Uri AddQuery(this Uri uri, string name, string value)
        {
            var ub = new UriBuilder(uri);

            // decodes urlencoded pairs from uri.Query to HttpValueCollection
            var httpValueCollection = HttpUtility.ParseQueryString(uri.Query);

            if (httpValueCollection.Get(name) != null)
                httpValueCollection.Set(name, value);
            else
                httpValueCollection.Add(name, value);

            // urlencodes the whole HttpValueCollection
            ub.Query = httpValueCollection.ToString();

            return ub.Uri;
        }


        /// <summary>
        /// Permite agregar una variable de querystring a un string.
        /// </summary>
        /// <param name="url">Url a modificar.</param>
        /// <param name="name">Nombre de la variable.</param>
        /// <param name="value">Valor.</param>
        /// <returns></returns>
        public static string AddQuery(string url, string name, string value)
        {
            return AddQuery(new Uri(url), name, value).ToString();
        }
    }
}