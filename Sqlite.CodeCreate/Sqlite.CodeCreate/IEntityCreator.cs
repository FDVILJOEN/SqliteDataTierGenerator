using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sqlite.CodeCreate
{
    public interface IEntityCreator<T>
    {
        static abstract T Instance(SqliteDataReader reader);
    }
}
