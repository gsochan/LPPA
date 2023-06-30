using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIRest.DAL.Contracts
{
    /// <summary>
    /// Esta interfaz sirve para definir los métodos a traves de los cuales se va a persistir la información de los objetos que pertenecen a la clase que implementa esta interfaz
    /// </summary>
    /// <typeparam name="T">Tipo de objeto a persistir</typeparam>
    public interface IGenericRepository<T>
    {
        /// <summary>
        /// Este método sirve para agregar un objeto nuevo a la base de datos
        /// </summary>
        /// <param name="oneObject">El objeto a persistir (si tiene un id, se lo va a omitir)</param>
        void Insert(T oneObject);

        /// <summary>
        /// Este método sirve para modificar un objeto existente
        /// </summary>
        /// <param name="oneObject">El objeto a modificar (respetando el id que se envía)</param>
        void Update(T oneObject);

        /// <summary>
        /// Este método sirve para eliminar un objeto de la base de datos
        /// </summary>
        /// <param name="oneObject">El objeto a eliminar(buscandolo por su id)</param>

        void Delete(T oneObject);

        /// <summary>
        /// Este método devuelve todos los objetos de este tipo que están persistidos en la base de datos
        /// </summary>
        /// <returns>Devuelve una coleccion de objetos</returns>
        IEnumerable<T> FindAll();
    }
}
