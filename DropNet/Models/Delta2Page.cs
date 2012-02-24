using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DropNet.Models
{
    public class Delta2Page
    {

        public string Cursor { get; set; }
        public bool HasMore { get; set; }
        public bool Reset { get; set; }

        private List<Delta2Entry> _entries = new List<Delta2Entry>();
        public List<Delta2Entry> Entries
        {
            get { return _entries; }
            set { _entries = value; }
        }
    }

    public class Delta2Entry
    {
        public string Path { get; set; }
        public MetaData MetaData { get; set; }
    }  
}
