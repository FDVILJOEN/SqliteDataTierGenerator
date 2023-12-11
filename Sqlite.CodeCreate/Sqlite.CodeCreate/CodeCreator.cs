using Microsoft.Data.Sqlite;
using Sqlite.CodeCreate.Properties;
using System.Security.Cryptography;
using System.Text;

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

        /// <summary>
        /// Gets or sets the path where the output files will be created.
        /// If not set, the current directory will be used.
        /// </summary>
        public string OutputPath { get; set; } = string.Empty;

        /// <summary>
        /// Main Creation Logic.
        /// </summary>
        public void CreateCode()
        {
            if (this._connection.State != System.Data.ConnectionState.Open)
            {
                throw new InvalidOperationException("Database connection needs to be open.");
            }

            CreateTables();
        }

        /// <summary>
        /// Main logic for crerating individual tables.
        /// </summary>
        private void CreateTables()
        {
            //Retrieve all the foreign Key Rows out.
            //Foreign Key Attributes.
            Dictionary<String, ForeignKey> AllForeignKeys = new Dictionary<String, ForeignKey>();
            SqliteCommand ForeignKeyCommand = _connection.CreateCommand();
            ForeignKeyCommand.CommandText = Resources.qry_ForeignKeys;
            SqliteDataReader ForeignKeyReader = ForeignKeyCommand.ExecuteReader();
            while (ForeignKeyReader.Read())
            {
                //Combine the Name and ID Columns for a Unique Value.
                string UniqueVal = ForeignKeyReader.GetString(0) + ForeignKeyReader.GetInt64(1).ToString();

                //Add the Key if it does not exist.
                if (!AllForeignKeys.ContainsKey(UniqueVal))
                {
                    AllForeignKeys.Add(UniqueVal, new ForeignKey() { SourceTable = ForeignKeyReader.GetString(0), TargetTable= ForeignKeyReader.GetString(3) });
                }

                //Add the Predicate.
                AllForeignKeys[UniqueVal].Predicates.Add((ForeignKeyReader.GetString(4), ForeignKeyReader.GetString(5)));
            }
            ForeignKeyReader.Close();


            //Build the collection of Tables and Columns.
            Dictionary<String, TableDefinition> Tables = new();
            SqliteCommand MetaDataCommand = _connection.CreateCommand();
            MetaDataCommand.CommandText = Resources.qry_MetaData;
            SqliteDataReader MetaDataReader = MetaDataCommand.ExecuteReader();
            while (MetaDataReader.Read())
            {
                string TableName = MetaDataReader.GetString(0);
                TableName = char.ToUpper(TableName[0]) + TableName.Substring(1);

                if (TableName.ToLower().Contains("sqlite"))
                {
                    continue;
                }

                if (!Tables.ContainsKey(TableName))
                    Tables.Add(TableName, new TableDefinition() { TableName = TableName});

                bool fieldNullable = (MetaDataReader.GetInt32(6) == 0);
                string fieldName = char.ToUpper(MetaDataReader.GetString(2)[0]) + MetaDataReader.GetString(2).Substring(1);

                Tables[TableName].Columns.Add(new ColumnDefinition()
                {
                    Name = fieldName,
                    IsPrimaryKey = MetaDataReader.GetInt32(4) != 0,
                    Nullable = fieldNullable,
                    DataBaseType = MetaDataReader.GetString(3),
                });

                //Set nullability of the FK on the column if it exists.
                if (fieldNullable)
                {
                    foreach (ForeignKey fk in AllForeignKeys.Values)
                    {
                        if (fk.SourceTable == TableName)
                        {
                            foreach (var pred in fk.Predicates)
                            {
                                if (pred.SourceCol.ToLower() == fieldName.ToLower())
                                {
                                    fk.IsNullable = true;
                                }
                            }
                        }
                    }
                }

            }
            MetaDataReader.Close();

            foreach (TableDefinition table in Tables.Values)
            {
                string className = table.TableName;
                StringBuilder fileContent = new StringBuilder();

                //Using Clauses
                fileContent.AppendLine("using Sqlite.CodeCreate;");
                fileContent.AppendLine("using Microsoft.Data.Sqlite;");

                fileContent.AppendLine(string.Empty);

                //Namespace
                fileContent.AppendLine("namespace Sqlite.CodeCreate");
                fileContent.AppendLine("{");

                //Class
                List<String> KeyFields = new List<string>();

                foreach (var field in table.Columns)
                {
                    if (field.IsPrimaryKey)
                    {
                        KeyFields.Add("\"" + field.Name + "\"");
                    }
                }

                fileContent.AppendLine("    public class " + className + ": DataEntityBase<" + className + ">, IEntityCreator<" + className + ">");
                fileContent.AppendLine("    {");

                //Key Fields Collection.
                fileContent.AppendLine("");
                fileContent.AppendLine("        ///<summary>");
                fileContent.AppendLine("        ///Definition of Key Fields");
                fileContent.AppendLine("        ///</summary>");
                fileContent.AppendLine("        static " + className + "() {");
                fileContent.AppendLine("            KeyFields = new([" + String.Join(',', KeyFields.ToArray()) + "]);");
                fileContent.AppendLine("        }");
                fileContent.AppendLine("");


                



        //Collection to keep the assignemnt statements from reader to actual fields.
        List<String> AssignmentStatements = new();

                List<String> CrudNameValuePairs = new();


                //Attributes - Corresponding to Field Names.

                int index = 0;
                foreach (ColumnDefinition columnDefinition in table.Columns)
                {
                    fileContent.AppendLine("");

                    //Comments for Attribute.
                    fileContent.AppendLine("        ///<summary>");
                    fileContent.AppendLine("        ///Gets / Sets the value for " + columnDefinition.Name);

                    if (columnDefinition.IsPrimaryKey)
                    {
                        fileContent.AppendLine("        ///This column forms part of the Primary Key");
                    }
                    if (columnDefinition.Nullable)
                    {
                        fileContent.AppendLine("        ///This columns is Nullable");
                    }
                    else
                    {
                        fileContent.AppendLine("        ///This columns is not Nullable");
                    }

                    fileContent.AppendLine("        ///The underlying data type is " + columnDefinition.DataBaseType);

                    fileContent.AppendLine("        ///<summary>");

                    //Adding the property on the Instance for each field.
                    string DataType = columnDefinition.ClassDataType;
                    fileContent.AppendLine("        public " + DataType + " " + columnDefinition.Name + " { get; set; }" + columnDefinition.DeclarationPostFix);

                    //Storing the assignment of this field value from the reader.
                    string methodName = columnDefinition.ReaderMethod;
                    AssignmentStatements.Add("            result." + columnDefinition.Name + " = reader." + methodName + "(" + index.ToString() + ");");

                    //CRUD Name Value Pairs
                    CrudNameValuePairs.Add("(\"" + columnDefinition.Name + "\", this." + columnDefinition.Name + ")");

                    index++;
                }

                fileContent.AppendLine("");

                foreach (ForeignKey FKEntry in AllForeignKeys.Values)
                {
                    if (FKEntry.SourceTable.ToLower() == className.ToLower())
                    {
                        string FKTable = FKEntry.TargetTable;
                        FKTable = char.ToUpper(FKTable[0]) + FKTable.Substring(1);

                        List<string> parameters = new List<string>();

                        foreach (var pred in FKEntry.Predicates)
                        {
                            parameters.Add("(\"" + pred.TartgetCol + "\",this." + pred.SourceCol + ")");
                        }

                        fileContent.AppendLine("        ///<summary>");
                        fileContent.AppendLine("        ///Returns the parent " + FKTable);
                        fileContent.AppendLine("        ///</summary>");

                        if (FKEntry.IsNullable)
                        {
                            fileContent.AppendLine("        public " + FKTable + "? FK_" + FKTable);
                        }
                        else
                        {
                            fileContent.AppendLine("        public " + FKTable + " FK_" + FKTable);
                        }

                        fileContent.AppendLine("        {");
                        fileContent.AppendLine("            get");
                        fileContent.AppendLine("            {");

                        if (FKEntry.IsNullable)
                        {
                            fileContent.AppendLine("                return " + FKTable + ".Scalar(" + string.Join(',', parameters) + ");");
                        }
                        else
                        {
                            fileContent.AppendLine("                return " + FKTable + ".ScalarStrict(" + string.Join(',', parameters) + ");");
                        }

                        fileContent.AppendLine("            }");
                        fileContent.AppendLine("        }");
                        fileContent.AppendLine("");
                    }

                    if (FKEntry.TargetTable.ToLower() == className.ToLower())
                    {
                        string FKTable = FKEntry.SourceTable;
                        FKTable = char.ToUpper(FKTable[0]) + FKTable.Substring(1);

                        List<string> parameters = new List<string>();

                        foreach (var pred in FKEntry.Predicates)
                        {
                            parameters.Add("(\"" + pred.SourceCol + "\",this." + pred.TartgetCol + ")");
                        }


                        fileContent.AppendLine("        ///<summary>");
                        fileContent.AppendLine("        ///Returns the Collection of child " + FKTable + " records.");
                        fileContent.AppendLine("        ///</summary>");
                        fileContent.AppendLine("        public IEnumerable<" + FKTable + "> FK_" + FKTable);
                        fileContent.AppendLine("        {");
                        fileContent.AppendLine("            get");
                        fileContent.AppendLine("            {");
                        fileContent.AppendLine("                return " + FKTable + ".Collection(" + string.Join(',', parameters) + ");");
                        fileContent.AppendLine("            }");
                        fileContent.AppendLine("        }");
                        fileContent.AppendLine("");
                    }
                }

                fileContent.AppendLine("");

                fileContent.AppendLine("        ///<summary>");
                fileContent.AppendLine("        ///Creates an instance of the " + className + " type from a database record.");
                fileContent.AppendLine("        ///</summary>");
                fileContent.AppendLine("        public static " + className + " Instance(SqliteDataReader reader)");
                fileContent.AppendLine("        {");
                fileContent.AppendLine("            " + className + " result = new();");
                
                foreach (string assignment in  AssignmentStatements)
                {
                    fileContent.AppendLine(assignment);
                }

                fileContent.AppendLine("            return result;");
                fileContent.AppendLine("        }");

                fileContent.AppendLine("");

                //AddUpdate Method
                fileContent.AppendLine("        public void AddUpdate()");
                fileContent.AppendLine("        {");
                fileContent.AppendLine("            Upsert(" + string.Join(',', CrudNameValuePairs.ToArray()) + ");");
                fileContent.AppendLine("        }");

                fileContent.AppendLine("");

                //AddUpdate Method
                fileContent.AppendLine("        public void Insert()");
                fileContent.AppendLine("        {");
                fileContent.AppendLine("            Insert(" + string.Join(',', CrudNameValuePairs.ToArray()) + ");");
                fileContent.AppendLine("        }");

                fileContent.AppendLine("");

                //AddUpdate Method
                fileContent.AppendLine("        public void Update()");
                fileContent.AppendLine("        {");
                fileContent.AppendLine("            Update(" + string.Join(',', CrudNameValuePairs.ToArray()) + ");");
                fileContent.AppendLine("        }");


                //End of Class
                fileContent.AppendLine("    }");

                //End of Namespace
                fileContent.AppendLine("}");

                string outputFileName = Path.Combine(OutputPath, className + ".cs");
                File.WriteAllText(outputFileName, fileContent.ToString());
            }
        }
    }
}
