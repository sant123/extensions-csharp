using System.Linq;

namespace System.Collections.Generic
{
    /// <summary>
    /// Añade extensiones a los tipos IEnumerable.
    /// </summary>
    public static class ECollection
    {
        #region OrderByPropertyName

        /// <summary>
        /// Orden para las listas.
        /// </summary>
        public enum SortDirection
        {
            /// <summary>
            /// Ascendente.
            /// </summary>
            Ascending,
            /// <summary>
            /// Descendente.
            /// </summary>
            Descending
        }

        /// <summary>
        /// Permite agrupar de a N elementos.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="num">Número de elementos para cada grupo.</param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> GroupByIndex<T>(this IEnumerable<T> list, int num)
        {
            return list.Select((value, index) => new { PairNum = index / num, value })
                .GroupBy(pair => pair.PairNum)
                .Select(grp => grp.Select(g => g.value));
        }

        /// <summary>
        /// Permite ordenar una lista por el nombre de la propiedad.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="propertyName">Nombre de la propiedad para ordenar.</param>
        /// <param name="sortDirection">Si ordena de manera ascendente o descendente.</param>
        /// <returns></returns>
        public static IEnumerable<T> OrderByPropertyName<T>(this IEnumerable<T> items, string propertyName, SortDirection sortDirection = SortDirection.Ascending)
        {
            var propInfo = typeof(T).GetProperty(propertyName);

            switch (sortDirection)
            {
                case SortDirection.Ascending:
                    return items.OrderBy(x => propInfo.GetValue(x, null));
                case SortDirection.Descending:
                    return items.OrderByDescending(x => propInfo.GetValue(x, null));
                default:
                    return null;
            }
        }

        #endregion

        #region DistinctBy

        /// <summary>
        /// Permite hacer un DistinctBy por propiedad.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector">Una expresión para realizar el Distinct.</param>
        /// <param name="lastCoincidence">Opcional, permite traer la última coincidencia del Distinct.</param>
        /// <returns></returns>
        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector, bool lastCoincidence = false)
        {
            return lastCoincidence ? source.GroupBy(selector).Select(x => x.Last()) : source.GroupBy(selector).Select(x => x.First());
        }

        #endregion

        #region Slice

        /// <summary>
        /// Devuelve una copia de una parte del array dentro de un nuevo array. Más información: https://developer.mozilla.org/es/docs/Web/JavaScript/Referencia/Objetos_globales/Array/slice
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="inicio">
        /// Indice donde empieza la extracción.
        /// Un número negativo indica un desplazamiento desde el final del array. 
        /// Slice(-2) extrae los dos últimos elementos del array.
        /// </param>
        /// <returns></returns>
        public static IEnumerable<T> Slice<T>(this IEnumerable<T> collection, int inicio)
        {
            return inicio < 0 ? collection.Reverse().Take(Math.Abs(inicio)).Reverse() : collection.Skip(inicio);
        }

        /// <summary>
        /// Devuelve una copia de una parte del array dentro de un nuevo array. Más información: https://developer.mozilla.org/es/docs/Web/JavaScript/Referencia/Objetos_globales/Array/slice
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="inicio">
        /// Indice donde empieza la extracción.
        /// Un número negativo indica un desplazamiento desde el final del array. 
        /// Slice(-2) extrae los dos últimos elementos del array.
        /// </param>
        /// <param name="fin">
        /// Índice que marca el fin a la extracción. 
        /// Un número negativo indica un desplazamiento desde el final del array. 
        /// Slice(1, 4) extrae desde el primer elemento hasta el cuarto.
        /// </param>
        /// <returns></returns>
        public static IEnumerable<T> Slice<T>(this IEnumerable<T> collection, int inicio, int fin)
        {
            var fromNegative = inicio < 0;
            var toNegative = fin < 0;

            if (fromNegative)
            {
                //from es negativo.
                return toNegative ? collection._BothNegative(inicio, fin) : collection._FromNegativeToPositive(inicio, fin);
            }

            if (toNegative)
            {
                //from es positivo.
                //to es negativo.
                return collection._FromPositiveToNegative(inicio, fin);
            }

            return collection.Take(fin).Skip(inicio);
        }

        private static IEnumerable<T> _FromPositiveToNegative<T>(this IEnumerable<T> collection, int inicio, int fin)
        {
            return collection.Skip(inicio).Reverse().Skip(Math.Abs(fin)).Reverse();
        }

        private static IEnumerable<T> _FromNegativeToPositive<T>(this IEnumerable<T> collection, int inicio, int fin)
        {
            var from = Math.Abs(inicio);

            var index1 = collection.Reverse().Skip(from - 1).Count() - 1;
            var index2 = collection.Skip(fin - 1).Count() - 1;

            return collection.Skip(index1).Reverse().Skip(index2).Reverse();
        }

        private static IEnumerable<T> _BothNegative<T>(this IEnumerable<T> collection, int inicio, int fin)
        {
            return collection.Reverse().Take(Math.Abs(inicio)).Skip(Math.Abs(fin)).Reverse();
        }

        #endregion

        #region LeftJoin

        /// <summary>
        /// Permite realizar un LeftJoin sobre una colección.
        /// </summary>
        /// <typeparam name="TOuter"></typeparam>
        /// <typeparam name="TInner"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="outer"></param>
        /// <param name="inner"></param>
        /// <param name="outerKeySelector"></param>
        /// <param name="innerKeySelector"></param>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> LeftJoin<TOuter, TInner, TKey, TResult>(
            this IEnumerable<TOuter> outer,
            IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, TInner, TResult> resultSelector)
        {
            var comparer = EqualityComparer<TKey>.Default;
            var innerLookup = inner.ToLookup(innerKeySelector, comparer);

            foreach (var outerElement in outer)
            {
                var outerKey = outerKeySelector(outerElement);
                var innerElements = innerLookup[outerKey];

                if (innerElements.Any())
                    foreach (var innerElement in innerElements)
                        yield return resultSelector(outerElement, innerElement);
                else
                    yield return resultSelector(outerElement, default(TInner));
            }
        }

        #endregion

        #region RightJoin

        /// <summary>
        /// Permite realizar un RightJoin sobre una colección.
        /// </summary>
        /// <typeparam name="TOuter"></typeparam>
        /// <typeparam name="TInner"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="outer"></param>
        /// <param name="inner"></param>
        /// <param name="outerKeySelector"></param>
        /// <param name="innerKeySelector"></param>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> RightJoin<TOuter, TInner, TKey, TResult>
            (this IEnumerable<TOuter> outer,
            IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, TInner, TResult> resultSelector)
        {
            var comparer = EqualityComparer<TKey>.Default;
            var outerLookup = outer.ToLookup(outerKeySelector, comparer);

            foreach (var innerElement in inner)
            {
                var innerKey = innerKeySelector(innerElement);
                var outerElements = outerLookup[innerKey];

                if (outerElements.Any())
                    foreach (var outerElement in outerElements)
                        yield return resultSelector(outerElement, innerElement);
                else
                    yield return resultSelector(default(TOuter), innerElement);
            }
        }

        #endregion

        #region Xor

        /// <summary>
        /// Permite sacar las diferencias de dos colecciones mediante una llave.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> Xor<TSource, TKey>(
            this IEnumerable<TSource> first,
            IEnumerable<TSource> second,
            Func<TSource, TKey> keySelector)
        {
            return first.Where(x => !second.Any(y => keySelector(x).Equals(keySelector(y))));
        }

        #endregion
    }
}