using IngameScript.DefaultNodes.AbstractNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace IngameScript
{
    public abstract class DecoratorNode : TreeNode
    {
        protected TreeNode child;

        public void SetChild(TreeNode child)
        {
            this.child = child;
        }
    }
}
