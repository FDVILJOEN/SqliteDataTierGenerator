using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sqlite.CodeCreate
{
    internal class QueryParts
    {
        private List<String> _allColumns = new();
        private List<String> _keyColumns = new();
        private List<String> _nonKeyColumns = new();

        private List<String> _values = new();
        private List<String> _statements = new();
        private List<String> _predicates = new();

        public QueryParts(List<string> KeyFields, (string Key, object? Val)[] values)
        {
            for (int I = 0; I < KeyFields.Count; I++)
            {
                KeyFields[I] = KeyFields[I].ToLower();
            }

            foreach (var item in values)
            {
                bool isKey = KeyFields.Contains(item.Key.ToLower());

                AllColumns.Add(item.Key);

                if (isKey )
                {
                    _keyColumns.Add(item.Key);
                }
                else
                {
                    _nonKeyColumns.Add(item.Key);
                }

                if (item.Val is string)
                {
                    _values.Add("\"" + item.Val.ToString() + "\"");
                }
                else if (item.Val is DateTime)
                {
                    _values.Add(((DateTime)item.Val).ToString("yyyy-MM-dd HH:mm:ss"));
                }
                else
                {
                    if (item.Val is null)
                    {
                        _values.Add("NULL");
                    }
                    else
                    {
                        _values.Add(item.Val.ToString() ?? "");
                    }
                }

                string statement = _allColumns.Last() + " = " + _values.Last();

                if (isKey)
                {
                    _predicates.Add(statement);
                }
                else
                {
                    _statements.Add(statement);
                }
            }
        }

        public List<String> AllColumns => _allColumns;
        public List<string> KeyColumns => _keyColumns;
        public List<string> NonKeyColumns => _nonKeyColumns;
        public List<String> Values => _values;
        public List<String> Statements => _statements;
        public List<String> Predicates => _predicates;
    }
}
