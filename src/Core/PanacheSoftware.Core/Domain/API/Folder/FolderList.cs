using System.Collections.Generic;

namespace PanacheSoftware.Core.Domain.API.Folder
{
    public class FolderList
    {
        public FolderList()
        {
            FolderHeaders = new List<FolderHead>();
        }

        public List<FolderHead> FolderHeaders { get; set; }
    }
}
