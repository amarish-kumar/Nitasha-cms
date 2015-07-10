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

        bool HasRight(Rights value);

        void SetRights(int UserId,int RoleId);

        List<string> GetRights(int userId);
    }
}