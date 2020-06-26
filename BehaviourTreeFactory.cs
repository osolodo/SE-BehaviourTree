using System;
using System.Collections.Generic;
using System.Text;

namespace IngameScript
{
    partial class BehaviourTreeFactory
    {
        /*  Object createObjectBy(Type clazz){
                // .. do construction work here
                Object theObject = Activator.CreateInstance(clazz);
                return theObject;
            }
        */
        Dictionary<string, Func<TreeNode>> registeredNodes;
        Dictionary<string, Func<Dictionary<string, string>, BehaviourTreeReturnType>> registeredSimpleConditions;
        Dictionary<string, Func<Dictionary<string, string>, BehaviourTreeReturnType>> registeredSimpleActions;
        readonly HashSet<String> registeredNames;

        public BehaviourTreeFactory()
        {
            registeredNodes = new Dictionary<string, Func<TreeNode>>();
            registeredSimpleConditions = new Dictionary<string, Func<Dictionary<string, string>, BehaviourTreeReturnType>>();
            registeredSimpleActions = new Dictionary<string, Func<Dictionary<string, string>, BehaviourTreeReturnType>>();
            registeredNames = new HashSet<string>();

            RegisterDefaultNodes();
        }

        protected void RegisterDefaultNodes()
        {
            RegisterNodeType<RootNode>("root");
            RegisterNodeType<BehaviorTreeNode>("BehaviorTree");
            RegisterNodeType<SequenceNode>("Sequence");
        }

        public void RegisterNodeType<TreeNodeExtension>(string name) where TreeNodeExtension : TreeNode, new()
        {
            registeredNames.Add(name);
            registeredNodes.Add(name, () => { return new TreeNodeExtension(); });
        }

        public void RegisterSimpleCondition(string name, Func<Dictionary<string, string>, BehaviourTreeReturnType> function)
        {
            registeredNames.Add(name);
            registeredSimpleConditions.Add(name, function);
        }

        public void RegisterSimpleAction(string name, Func<Dictionary<string, string>, BehaviourTreeReturnType> function)
        {
            registeredNames.Add(name);
            registeredSimpleActions.Add(name, function);
        }

        //TODO: Do I need to add registerSimpleAction overloads with input values for the function? Can I?
    }
}
