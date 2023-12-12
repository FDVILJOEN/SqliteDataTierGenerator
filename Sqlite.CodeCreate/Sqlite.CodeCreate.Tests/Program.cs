using Microsoft.Data.Sqlite;
using Sqlite.CodeCreate.Tests.Properties;

namespace Sqlite.CodeCreate.Tests
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Unit tests packaged in a Console App
            //.NET 8 has no UT support yet.

            //Dropping DB on Disk.
            string filename = "testdata.db";
            File.WriteAllBytes(filename, Resources.testdata);

            SqliteConnectionStringBuilder sb = new SqliteConnectionStringBuilder();
            sb.DataSource = filename;
            SqliteConnection conn = new SqliteConnection(sb.ToString());
            conn.Open();


            //Definig and clearing output path.
            string outputPath = "C:\\code\\SqliteDataTierGenerator\\Sqlite.CodeCreate\\Sqlite.CodeCreate.Tests\\TestClasses";
            Directory.CreateDirectory(outputPath);
            foreach (string currentFile in Directory.GetFiles(outputPath))
            {
                File.Delete(currentFile);
            }

            //Run Routine.
            CodeCreator cc = new CodeCreator(conn);
            cc.OutputPath = outputPath;
            foreach (String S in cc.CreateCode())
            {
                Console.WriteLine(S);
            }

            //Querying one of the items.
            /*            
                        
            Context.Connection = conn;
            var allAlbums = Albums.Collection();

            foreach (var item in allAlbums)
            {
                Console.WriteLine(item.Title + " by " + item.FK_Artists.Name);

                if (item.Title[0] == '*')
                {
                    item.Title = item.Title.Substring(1);
                }
                else
                {
                    item.Title = '*' + item.Title;
                }

                item.Update();

                item.AddUpdate();
            }

            */

        }
    }
}
