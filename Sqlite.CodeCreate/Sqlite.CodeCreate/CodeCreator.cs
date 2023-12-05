using Microsoft.Data.Sqlite;

namespace Sqlite.CodeCreate
{
    public class CodeCreator
    {
        #region Private references
        SqliteConnection _connection;
        #endregion

        /// <summary>
        /// Creates a new instance of the code creator.  
        /// This class is used to generate c# classes that will interact with the database 
        /// provided in the connection.
        /// </summary>
        /// <param name="dataConnection">Connection to the database</param>
        public CodeCreator(SqliteConnection dataConnection) 
        { 
            this._connection = dataConnection;
        }
    }
}
