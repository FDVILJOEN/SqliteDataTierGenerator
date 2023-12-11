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
        /// <summary>
        /// Add item to Database.
        /// </summary>
        void AddUpdate();

        /// <summary>
        /// Insertingo of record to DB.
        /// </summary>
        void Insert();

        //Update of Record to Database.
        void Update();

        static abstract T Instance(SqliteDataReader reader);
        
    }
}
