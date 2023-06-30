using APIRest.DAL.Contracts;
using APIRest.DAL.Domains;
using APIRest.DAL.Herramientas;
using APIRest.DTOs;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace APIRest.DAL
{
    public class DALRelationshipUserPermission : IGenericRelationship<UserDAO, Permission>
    {
        private string connectionString;
        internal DALRelationshipUserPermission(String oneConnectionString)
        {
            connectionString = oneConnectionString;
        }

        public void Join(UserDAO parent, Permission child)
        {
            try {
                SqlHelper sqlHelper = new SqlHelper(connectionString);
                string query = $"INSERT INTO UsersPrivileges(UserID, PrivilegeId) VALUES (@UserID, @PrivilegeId)";
                sqlHelper.ExecuteNonQuery(query, System.Data.CommandType.Text, 
                    new SqlParameter[] { new SqlParameter("@UserID", parent.Id),
                                         new SqlParameter("@PrivilegeId", child.Id)
                });
            } catch (Exception) {
                throw new Exception("Error al asignar un permiso a un usuario");
            }
        }

        public List<Permission> GetChildren(UserDAO parent)
        {
            try {
                IEnumerable<Permission> allPermissions = new DALPrivilege(connectionString).FindAll();

                List<Permission> userPermissions = new List<Permission>();
                SqlHelper sqlHelper = new SqlHelper(connectionString);
                string query = $"SELECT Id, UserId, PrivilegeId FROM UsersPrivileges WHERE UserId = {parent.Id}";
                using (var dr = sqlHelper.ExecuteReader(query, System.Data.CommandType.Text))
                {
                    while (dr.Read())
                    {
                        object[] values = new object[dr.FieldCount];
                        dr.GetValues(values);
                        int thisPermissionId = int.Parse(values[2].ToString());
                        Permission thisPermission = allPermissions.First(item => item.Id == thisPermissionId);

                        userPermissions.Add(thisPermission);
                    }
                }
                return userPermissions.OrderBy(item => item.Id).ToList();
            } catch (Exception) {
                throw new Exception("Hubo un problema al listar los permisos de este usuario");
            }
        }

        public void Unlink(UserDAO parent, Permission child)
        {
            throw new NotImplementedException();
        }

        public void UnlinkAll(UserDAO parent)
        {
            try {
                SqlHelper sqlHelper = new SqlHelper(connectionString);
                var query = "DELETE FROM UsersPrivileges WHERE UserID = @UserID";
                sqlHelper.ExecuteNonQuery(query, System.Data.CommandType.Text, 
                    new SqlParameter[] { new SqlParameter("@UserID", parent.Id)});
            } catch (Exception) {
                throw new Exception("Error al borrar los permisos de un usuario");
            }
        }
    }
}
