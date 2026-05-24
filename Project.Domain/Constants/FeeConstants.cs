using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.Constants
{
    public static class FeeConstants
    {
        public const int MaxInstallments = 12;
        public const decimal LateFinePerDay = 10;
        public const int GracePeriodDays = 5;
    }
}
