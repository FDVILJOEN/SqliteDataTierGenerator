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
        /// Key Field Collection.
        /// </summary>
        public static List<string> KeyFields = new();

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

        /// <summary>
        /// Returns a non-nullable parent property.
        /// </summary>
        /// <param name="predicates"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
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

        /// <summary>
        /// Performs a Basic Insert Operation.
        /// </summary>
        /// <param name="values"></param>
        protected void Insert(params (string Key, object? Val)[] values)
        {
            QueryParts parts = new QueryParts(KeyFields, values);           

            if (Context.Connection is not null)
            {
                SqliteCommand cmd = Context.Connection.CreateCommand();
                cmd.CommandText = "INSERT INTO [" + typeof(T).Name + "] (" + String.Join(',', parts.AllColumns.ToArray())  + ") VALUES (" + String.Join(',', parts.Values.ToArray()) + ")";
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Performs a Basic Update Operation.
        /// </summary>
        /// <param name="values"></param>
        protected void Update(params (string Key, object? Val)[] values)
        {
            QueryParts parts = new QueryParts(KeyFields, values);

            if (Context.Connection is not null)
            {
                SqliteCommand cmd = Context.Connection.CreateCommand();
                cmd.CommandText = "Update [" + typeof(T).Name + "] SET " 
                    + String.Join(',', parts.Statements.ToArray()) + " WHERE " 
                    + string.Join(" AND ", parts.Predicates.ToArray());
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Upsert Operation.
        /// </summary>
        /// <param name="values"></param>
        protected void Upsert(params (string Key, object? Val)[] values)
        {
            QueryParts parts = new QueryParts(KeyFields, values);

            if (Context.Connection is not null)
            {
                SqliteCommand cmd = Context.Connection.CreateCommand();

                cmd.CommandText = "INSERT INTO [" + typeof(T).Name + "] (" + String.Join(',', parts.AllColumns.ToArray()) + ") VALUES (" + String.Join(',', parts.Values.ToArray()) + ")";
                cmd.CommandText += " ON CONFLICT(" + string.Join(',', parts.KeyColumns.ToArray()) + ") DO UPDATE SET " + String.Join(',', parts.Statements.ToArray());
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Deletion of Records from the database.
        /// </summary>
        /// <param name="values"></param>
        protected void Delete(params (string Key, object? Val)[] values)
        {
            QueryParts parts = new QueryParts(KeyFields, values);

            if (Context.Connection is not null)
            {
                SqliteCommand cmd = Context.Connection.CreateCommand();
                cmd.CommandText = "DELETE FROM [" + typeof(T).Name + "] WHERE " + string.Join(" AND ", parts.Predicates.ToArray());
                cmd.ExecuteNonQuery();
            }
        }



    }
}
