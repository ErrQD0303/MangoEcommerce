using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces.Exists
{
    public interface IExistenceCheck
    {
        Task<bool> ExistsAsync(int id);
    }
}