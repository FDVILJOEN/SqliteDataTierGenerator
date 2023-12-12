using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sqlite.CodeCreate
{
    internal class TableDefinition
    {
        public string TableName { get; set; } = string.Empty;
        public List<ColumnDefinition> Columns { get; set; } = new();
    }

    internal class ColumnDefinition
    {
        public string DataBaseType { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsPrimaryKey { get; set; }
        public bool Nullable { get; set; }

        public string ClassDataType
        {
            get
            {
                if (DataBaseType.ToUpper() == "INTEGER")
                {
                    return "long";
                }

                if (DataBaseType.ToUpper() == "DATETIME")
                {
                    return "DateTime";
                }

                if (DataBaseType.ToUpper().Contains("NVARCHAR"))
                {
                    if (Nullable)
                    {
                        return "string?";
                    }
                    else
                    {
                        return "string";
                    }
                }

                if (DataBaseType.ToUpper().Contains("TEXT"))
                {
                    if (Nullable)
                    {
                        return "string?";
                    }
                    else
                    {
                        return "string";
                    }
                }

                if (DataBaseType.ToUpper().Contains("NUMERIC"))
                {
                    return "double";
                }

                throw new Exception("Data Type " + DataBaseType + " not mapped to system type.");

            }
        }

        /// <summary>
        /// Gets the appropriate method to call on the reader for a given data type.
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string ReaderMethod
        {
            get
            {
                if (DataBaseType.ToUpper().Contains("NVARCHAR")) return "GetString";
                if (DataBaseType.ToUpper().Contains("TEXT")) return "GetString";
                if (DataBaseType.ToUpper() == "INTEGER") return "GetInt64";
                if (DataBaseType.ToUpper().Contains("NUMERIC")) return "GetDouble";
                if (DataBaseType.ToUpper() == "DATETIME") return "GetDateTime";

                throw new Exception("Cannot map type " + DataBaseType);
            }
        }

        /// <summary>
        /// Gets the post frix notation for a property declaration
        /// </summary>
        public string DeclarationPostFix
        {
            get
            {
                if (DataBaseType.ToUpper().Contains("NVARCHAR")) return " = String.Empty;";
                if (DataBaseType.ToUpper().Contains("TEXT")) return " = String.Empty;";
                return string.Empty;
            }
        }
    }
}
