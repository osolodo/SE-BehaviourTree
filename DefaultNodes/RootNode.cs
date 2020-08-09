using IngameScript.DefaultNodes.AbstractNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace IngameScript
{
    public class RootNode : ControlNode
    {
        private string main_tree_to_execute;
        private TreeNode main_node;
        private Dictionary<string, TreeNode> subTrees;
        override public void Init(Dictionary<string, string> attributes)
        {
            base.Init(attributes);
            if (children == null || children.Count == 0)
            {
                throw new Exception(logger.Error("root node does not contain any children"));
            }
            if (!attributes.TryGetValue("main_tree_to_execute", out main_tree_to_execute))
            {
                if (children.Count == 1)
                {
                    main_node = children[0];
                }
                else
                {
                    throw new Exception(logger.Error("root node does not contain attribute:main_tree_to_execute"));
                }
            }
            subTrees = new Dictionary<string, TreeNode>();
            children.ForEach((TreeNode treeNode) =>
            {
                subTrees.Add(treeNode.GetID(), treeNode);
            });

            if(!subTrees.TryGetValue(main_tree_to_execute,out main_node))
            {
                throw new Exception(logger.Error("Main Tree not found: " + main_tree_to_execute));
            }
        }
        override public void Validate()
        {

        }

        override public BehaviourTreeReturnType Tick()
        {
            return main_node.Tick();
        }

        public BehaviourTreeReturnType TickSubTree(string subTreeID)
        {
            TreeNode subTree;
            if (!subTrees.TryGetValue(subTreeID, out subTree))
            {
                throw new Exception(logger.Error("SubTree not found: " + subTreeID));
            }
            return subTree.Tick();
        }
    }

}
