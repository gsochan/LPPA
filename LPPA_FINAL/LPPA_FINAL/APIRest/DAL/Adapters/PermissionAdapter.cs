using APIRest.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIRest.DAL.Adapters
{
    public class PermissionAdapter
    {
        #region Singleton
        private readonly static PermissionAdapter _instance = new PermissionAdapter();

        public static PermissionAdapter Current
        {
            get
            {
                return _instance;
            }
        }

        private PermissionAdapter()
        {
            //Implement here the initialization code
        }
        #endregion

        public Permission Adapt(object[] values)
        {
            return new Permission()
            {
                Id = int.Parse(values[0].ToString()),
                Name = values[1].ToString()
            };
        }


    }
}
