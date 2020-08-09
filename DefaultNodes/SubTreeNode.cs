using System;
using System.Collections.Generic;
using System.Text;

namespace IngameScript.DefaultNodes
{
    class SubTreeNode : LeafNode
    {
        private RootNode rootNode;
        public void SetRootNode(RootNode rootNode)
        {
            this.rootNode = rootNode;
        }

        override public BehaviourTreeReturnType Tick()
        {
            return rootNode.TickSubTree(this.ID);
        }
    }
}
