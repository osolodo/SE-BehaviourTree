﻿using System;
using System.Collections.Generic;
using System.Text;

namespace IngameScript
{
    public class TreeNode
    {
        protected string name;
        protected string ID;
        protected static readonly Logger logger = new Logger("BehaviourTree_TreeNode");
        protected Dictionary<string, string> attributes;
        virtual public void Init(Dictionary<string, string> attributes)
        {
            this.attributes = attributes;
            attributes.TryGetValue("name", out name);
            attributes.TryGetValue("ID", out ID);
            if(name == null)
            {
                name = ID;
            }
        }

        public virtual string GetName()
        {
            return name;
        }

        public virtual void Validate()
        {

        }

        public virtual BehaviourTreeReturnType Tick()
        {
            throw new Exception(logger.Error("Invalid call to base Tick() method"));
        }
    }

    public abstract class DecoratorNode : TreeNode
    {
        protected TreeNode child;

        public void SetChild(TreeNode child)
        {
            this.child = child;
        }
    }

    public abstract class ControlNode : TreeNode
    {
        protected List<TreeNode> children;

        public void SetChildren(List<TreeNode> children)
        {
            this.children = children;
        }
    }

    public abstract class LeafNode : TreeNode
    { }

    public class RootNode : ControlNode
    {
        private string main_tree_to_execute;
        private TreeNode main_node;
        override public void Init(Dictionary<string, string> attributes)
        {
            base.Init(attributes);
            if(children == null || children.Count == 0)
            {
                throw new Exception(logger.Error("root node does not contain any children"));
            }
            if(!attributes.TryGetValue("main_tree_to_execute", out main_tree_to_execute))
            {
                if (children.Count == 1)
                {
                    main_node = children[0];
                } else
                {
                    throw new Exception(logger.Error("root node does not contain attribute:main_tree_to_execute"));
                }
            }
            main_node = children.Find(childNode => main_tree_to_execute == childNode.GetName());
        }
        override public void Validate()
        {

        }

        override public BehaviourTreeReturnType Tick()
        {
            return main_node.Tick();
        }
    }

    class BehaviorTreeNode : DecoratorNode
    {
        override public void Init(Dictionary<string, string> attributes)
        {
            base.Init(attributes);

        }
        override public void Validate()
        {

        }

        override public BehaviourTreeReturnType Tick()
        {
            return child.Tick();
        }
    }

    class SimpleConditionNode : LeafNode
    {
        readonly Func<Dictionary<string, string>,BehaviourTreeReturnType> func;
        public SimpleConditionNode(Func<Dictionary<string, string>,BehaviourTreeReturnType> func)
        {
            this.func = func;
        }
        override public void Validate()
        {

        }
        override public BehaviourTreeReturnType Tick()
        {
            return func(attributes);
        }
    }

    class SimpleActionNode : LeafNode
    {
        readonly Func<Dictionary<string, string>,BehaviourTreeReturnType> func;
        public SimpleActionNode(Func<Dictionary<string, string>, BehaviourTreeReturnType> func)
        {
            this.func = func;
        }
        override public void Validate()
        {

        }

        override public BehaviourTreeReturnType Tick()
        {
            return func(attributes);
        }
    }
}
