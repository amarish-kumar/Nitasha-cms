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

        bool HasRights(Rights value);

        void BindRights();
    }
}
