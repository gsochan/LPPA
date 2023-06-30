using APIRest.DAL.Adapters;
using APIRest.DAL.Contracts;
using APIRest.DAL.Herramientas;
using APIRest.DTOs;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace APIRest.DAL
{
    public class DALPrivilege: IGenericRepository<Permission>
    {
        private String connectionString;
        internal DALPrivilege(String oneConnectionString)
        {
            connectionString = oneConnectionString;
        }

        public void Insert(Permission oneObject)
        {
            if (oneObject.Name == null || oneObject.Name.Trim().Length == 0)
                throw new Exception("Faltan completar datos");
            try {
                SqlHelper sqlHelper = new SqlHelper(connectionString);
                string query = $"INSERT INTO Privileges(Description) VALUES (@Description)";
                sqlHelper.ExecuteNonQuery(query, System.Data.CommandType.Text, 
                    new SqlParameter[] { new SqlParameter("@Description", oneObject.Name)
                });
            } catch (Exception) {
                throw new Exception("Error al agregar un nuevo permiso");
            }
        }

        public void Delete(Permission oneObject)
        {
            try {
                SqlHelper sqlHelper = new SqlHelper(connectionString);
                var query = "DELETE FROM Privileges WHERE Id = @Id";
                sqlHelper.ExecuteNonQuery(query, System.Data.CommandType.Text, 
                    new SqlParameter[] { new SqlParameter("@Id", oneObject.Id)
                });
            } catch (Exception) {
                throw new Exception("Error al borrar este permiso");
            }
        }

        public void Update(Permission oneObject)
        {
            if (oneObject.Name == null || oneObject.Name.Trim().Length == 0)
                throw new Exception("Faltan completar datos");
            try {
                SqlHelper sqlHelper = new SqlHelper(connectionString);
                var query = $"UPDATE Privileges SET Description = @Description WHERE Id = @Id";
                sqlHelper.ExecuteNonQuery(query, System.Data.CommandType.Text, 
                    new SqlParameter[] { new SqlParameter("@Id", oneObject.Id),
                                         new SqlParameter("@Description", oneObject.Name)
                });
            } catch (Exception) {
                throw new Exception("Error al modificar este permiso");
            }
        }

        public IEnumerable<Permission> FindAll()
        {
            try {
                List<Permission> allPermissions = new List<Permission>();
                SqlHelper sqlHelper = new SqlHelper(connectionString);
                string query = "SELECT Id, Description FROM Privileges";
                using (var dr = sqlHelper.ExecuteReader(query, System.Data.CommandType.Text)) {
                    while (dr.Read()) {
                        object[] values = new object[dr.FieldCount];
                        dr.GetValues(values);
                        Permission onePermission = PermissionAdapter.Current.Adapt(values);
                        allPermissions.Add(onePermission);
                    }
                }
                return allPermissions;
            } catch (Exception) {
                throw new Exception("Error al listar los permisos");
            }
        }
    }
}
