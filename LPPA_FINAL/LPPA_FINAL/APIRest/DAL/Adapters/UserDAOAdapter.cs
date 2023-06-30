using APIRest.DAL.Domains;
using APIRest.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIRest.DAL.Adapters
{
    public class UserDAOAdapter
    {
        #region Singleton
        private readonly static UserDAOAdapter _instance = new UserDAOAdapter();

        public static UserDAOAdapter Current
        {
            get
            {
                return _instance;
            }
        }

        private UserDAOAdapter()
        {
            //Implement here the initialization code
        }
        #endregion

        public UserDAO Adapt(object[] values)
        {
            return new UserDAO()
            {
                Id = int.Parse(values[0].ToString()),
                Username = values[1].ToString(),
                Salt = values[2].ToString(),
                PasswordHash = values[3].ToString(),
                Email = values[4].ToString(),
                Permissions = new List<Permission>()
            };
        }


    }
}
