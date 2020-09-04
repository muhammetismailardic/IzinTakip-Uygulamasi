using IzinTakip.Core.DataAccess.EntityFramework;
using IzinTakip.DataAccess.Abstract;
using IzinTakip.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace IzinTakip.DataAccess.Concrete.EntityFramework
{
    public class EfEmployeeDal : EfEntityRepositoryBase<Employee, IzinTakipContext>, IEmployeeDal
    {
    }
}
