using PanacheSoftware.Core.Domain.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PanacheSoftware.Core.Extensions
{
    public static class ModelExtensions
    {
        public static IEnumerable<FolderStructureModel> Descendants(this FolderStructureModel root)
        {
            var nodes = new Stack<FolderStructureModel>(new[] { root });
            while (nodes.Any())
            {
                FolderStructureModel node = nodes.Pop();
                yield return node;
                foreach (var n in node.Children) nodes.Push(n);
            }
        }
    }
}
