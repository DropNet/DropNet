using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DropNet.Models
{
    /// <summary>
    /// Response to the Delta API Call
    /// </summary>
    public class DeltaPage
    {
        /// <summary>
        /// A string that encodes the latest information that has been returned. On the next call to GetDelta(), pass in this value.
        /// </summary>
        public string Cursor { get; set; }
        
        /// <summary>
        /// If true, then there are more entries available; you can call GetDelta() again immediately to retrieve those entries. 
        /// If 'false', then wait for at least five minutes (preferably longer) before checking again.
        /// </summary>
        public bool Has_More { get; set; }

        /// <summary>
        /// If true, clear your local state before processing the delta entries. reset is always true on the initial call to 
        /// GetDelta() (i.e. when no cursor is passed in). Otherwise, it is true in rare situations, such as after server or 
        /// account maintenance, or if a user deletes their app folder.
        /// </summary>
        public bool Reset { get; set; }

        /// <summary>
        /// A list of "delta entries" (described below).
        /// </summary>
        public List<DeltaEntry> Entries { get; set; }
    }

    /// <summary>
    /// Class representing a delta to a single file or folder
    /// </summary>
    public class DeltaEntry
    {
        /// <summary>
        /// The path to a file or folder that has a delta.  Dropbox treats file names in a case-insensitive but 
        /// case-preserving way. To facilitate this, the path values above are lower-cased versions of the actual path. 
        /// If present, the MetaData value has the original case-preserved path.
        /// </summary>
        public string Path { get; set; }
        
        /// <summary>
        /// If null: Indicates that there is no file/folder at the given path. To update your local state to match, 
        ///   anything at path and all its children should be deleted. Deleting a folder in your Dropbox will sometimes 
        ///   send down a single deleted entry for that folder, and sometimes separate entries for the folder and all child paths. 
        ///   If your local state doesn't have anything at path, ignore this entry.
        /// If not null:Indicates that there is a file/folder at the given path. You should add the entry to your local path. 
        ///   The metadata value is the same as what would be returned by the /metadata call, except folder metadata doesn't have hash 
        ///   or contents fields. To correctly process delta entries:
        ///     If the new entry includes parent folders that don't yet exist in your local state, create those parent folders in your local state.
        ///     If the new entry is a file, replace whatever your local state has at path with the new entry.
        ///     If the new entry is a folder, check what your local state has at Path. If it's a file, replace it with the new entry.
        ///       If it's a folder, apply the new MetaData to the folder, but do not modify the folder's children.
        /// </summary>
        public MetaData MetaData { get; set; }
    }

}
