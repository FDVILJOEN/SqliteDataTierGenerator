using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sqlite.CodeCreate
{
    internal class ForeignKey
    {
        public required string SourceTable { get; set; }
        public required string TargetTable { get; set; }
        public List<(string SourceCol, string TartgetCol)> Predicates { get; set; } = new();
    }
}
