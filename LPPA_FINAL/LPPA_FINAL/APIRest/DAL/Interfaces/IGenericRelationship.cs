using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIRest.DAL.Contracts
{
    /// <summary>
    /// Esta interfaz sirve para vincular un objeto con una lista de objetos contenida dentro de él para poder persistirlo
    /// </summary>
    /// <typeparam name="T">Clase del objeto contenedor</typeparam>
    /// <typeparam name="U">Clase de los objetos hijos</typeparam>
    public interface IGenericRelationship<T, U>
    {
        /// <summary>
        /// Agrego una relación de tipo 1 a *, T elemento origen, U es el destino
        /// </summary>
        /// <param name="parent">Este objeto contiene una coleccion de objetos hijos</param>
        /// <param name="child">Este es el objeto que se agrega al padre</param>
        void Join(T parent, U child);

        /// <summary>
        /// Elimino una relación de tipo 1 a *, T elemento origen, U es el destino
        /// </summary>
        /// <param name="parent">Este objeto contiene una coleccion de objetos hijos</param>
        /// <param name="child">Este es el objeto que se elimina del padre</param>
        void Unlink(T parent, U child);

        /// <summary>
        /// Elimino todas las relaciónes de tipo 1 a *, del elemento T
        /// </summary>
        /// <param name="parent">Este objeto contiene una coleccion de objetos hijos</param>
        void UnlinkAll(T parent);

        /// <summary>
        /// Obtener elementos destino a partir de un elemento origen T
        /// </summary>
        /// <param name="parent">Este objeto contiene una coleccion de objetos hijos</param>
        /// <returns>Devuelve la lista de objetos hijos</returns>
        List<U> GetChildren(T parent);
    }
}
