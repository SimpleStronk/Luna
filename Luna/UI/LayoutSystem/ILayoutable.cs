using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luna.UI.LayoutSystem
{
    internal interface ILayoutable
    {
        public LUIVA.Layout GetLayout();

        public void SetLayout(LUIVA.Layout layout);

        public UITransform GetTransform();

        public IEnumerable<ILayoutable> GetChildren();

        public int GetChildCount();

        public string GetTag();
    }
}
