using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sqlite.CodeCreate
{
    public class DataEntityBase<T> where T : IEntityCreator<T>
    {
        /// <summary>
        /// Returns the first (or only) item from a table, based on the predicates supplied.
        /// </summary>
        /// <param name="predicates"></param>
        /// <returns></returns>
        public static T? Scalar(params (string Key, object Val)[] predicates)
        {
            if (Context.Connection is not null)
            {
                List<String> PredicateStatements = new List<String>();

                foreach (var pred in predicates)
                {
                    object Val = pred.Val;

                    if (Val is string)
                    {
                        Val = "'" + Val.ToString() + "'";
                    }

                    PredicateStatements.Add("[" + pred.Key + "] = " + pred.Val);
                }

                SqliteCommand cmd = Context.Connection.CreateCommand();

                cmd.CommandText = "Select * from [" + typeof(T).Name + "]";

                if (PredicateStatements.Count > 0)
                {
                    cmd.CommandText += " WHERE " + string.Join(" AND ", PredicateStatements.ToArray());
                }

                SqliteDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return T.Instance(reader);
                }
            }
            return default(T);
        }

        public static T ScalarStrict(params (string Key, object Val)[] predicates)
        {
            T? result = Scalar(predicates);
            if (result != null)
            {
                return result;
            }
            throw new Exception("Foreign Key defined as non-nullable, but no parent found.");
        }

        /// <summary>
        /// Returns a collection of items from a table, based on the predicates supplied.
        /// </summary>
        /// <param name="predicates"></param>
        /// <returns></returns>
        public static IEnumerable<T> Collection(params (string Key, object Val)[] predicates)
        {
            if (Context.Connection is not null)
            {
                List<String> PredicateStatements = new List<String>();

                foreach (var pred in predicates)
                {
                    object Val = pred.Val;

                    if (Val is string)
                    {
                        Val = "'" + Val.ToString() + "'";
                    }

                    PredicateStatements.Add("[" + pred.Key + "] = " + pred.Val);
                }

                SqliteCommand cmd = Context.Connection.CreateCommand();

                cmd.CommandText = "Select * from [" + typeof(T).Name + "]";

                if (PredicateStatements.Count > 0)
                {
                    cmd.CommandText += " WHERE " + string.Join(" AND ", PredicateStatements.ToArray());
                }

                SqliteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    yield return T.Instance(reader);
                }
            }
        }
    }
}
