using System.Collections.Generic;

namespace DropNet.Models
{
    /// <summary>
    /// Private class used to deal with the DropBox API returning two different types in a given list
    /// </summary>
    internal class DeltaPageInternal
    {
        public string Cursor { get; set; }
        public bool Has_More { get; set; }
        public bool Reset { get; set; }
        public List<List<string>> Entries { get; set; }
    }
}