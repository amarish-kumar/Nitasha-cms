using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NITASA.Services.Security
{
    public interface IAclService
    {
        Dictionary<string, string> GetAll();

        List<AccessPermission> GetAllAccessPermission();

        void SetActiveUser(int UserId);
        void RemoveActiveUser(int UserId);
        bool IsActiveUser(int UserId);

        void SetRights(int UserId,int RoleId);
        bool HasRight(Rights value);
    }
}