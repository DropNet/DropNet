using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DropNet.Models
{
    public class DeltaPage
    {
        public string Cursor { get; set; }
        public bool HasMore { get; set; }
        public List<DeltaEntry> Delta { get; set; }
    }

    public class DeltaEntry
    {
    }
}
