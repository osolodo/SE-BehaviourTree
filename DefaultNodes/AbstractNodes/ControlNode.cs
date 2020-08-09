using System;
using System.Collections.Generic;
using System.Text;

namespace IngameScript.DefaultNodes.AbstractNodes
{
    public abstract class ControlNode : TreeNode
    {
        protected List<TreeNode> children;

        public void SetChildren(List<TreeNode> children)
        {
            this.children = children;
        }
    }
}
