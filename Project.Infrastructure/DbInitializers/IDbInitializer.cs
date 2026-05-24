using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.DbInitializers
{
    public interface IDbInitializer
    {
        Task InitializeAsync();
    }
}
