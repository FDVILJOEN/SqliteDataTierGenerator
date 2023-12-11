using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sqlite.CodeCreate
{
    public static class Context
    {
        public static SqliteConnection? Connection { get; set; }
    }
}
