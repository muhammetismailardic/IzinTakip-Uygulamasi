using IzinTakip.Core.DataAccess;
using IzinTakip.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace IzinTakip.DataAccess.Abstract
{
    public interface IEmployeeDal : IEntityRepository<Employee>
    {
    }
}
