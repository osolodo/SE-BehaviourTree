using IngameScript.DefaultNodes;
using IngameScript.DefaultNodes.AbstractNodes;
using IngameScript.DefaultNodes.ControlNodes;
using IngameScript.DefaultNodes.DecoratorNodes;
using IngameScript.DefaultNodes.TerminalNodes;
using Sandbox.ModAPI.Ingame;
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
        Dictionary<string, Func<Dictionary<string, string>, BehaviourTreeReturnType>> registeredLambdaNodes;
        Dictionary<string, Func<TerminalNode>> registeredTerminalNodes;
        readonly HashSet<string> registeredNames;

        public BehaviourTreeFactory()
        {
            registeredNodes = new Dictionary<string, Func<TreeNode>>();
            registeredLambdaNodes = new Dictionary<string, Func<Dictionary<string, string>, BehaviourTreeReturnType>>();
            registeredTerminalNodes = new Dictionary<string, Func<TerminalNode>>();
            registeredNames = new HashSet<string>();

            RegisterDefaultNodes();
        }

        protected void RegisterDefaultNodes()
        {
            RegisterNodeType<RootNode>("root");
            RegisterNodeType<BehaviorTreeNode>("BehaviorTree");
            RegisterNodeType<SubTreeNode>("SubTree");

            //Control nodes
            RegisterNodeType<FallbackNode>("Fallback");
            RegisterNodeType<ReactiveFallbackNode>("ReactiveFallback");
            RegisterNodeType<SequenceNode>("Sequence");
            RegisterNodeType<ReactiveSequenceNode>("ReactiveSequence");
            RegisterNodeType<SequenceStarNode>("SequenceStar");
            RegisterNodeType<ParallelNode>("Parallel");
            RegisterNodeType<IfThenElseNode>("IfThenElse");
            RegisterNodeType<WhileDoElseNode>("WhileDoElse");

            //Decorator Nodes
            RegisterNodeType<InverterNode>("Inverter");
            RegisterNodeType<ForceSuccessNode>("ForceSuccess");
            RegisterNodeType<ForceFailureNode>("ForceFailure");
            RegisterNodeType<RepeatNode>("Repeat");
            RegisterNodeType<RetryNode>("Retry");
            RegisterNodeType<TimeoutNode>("Timeout");
        }

        public void RegisterNodeType<TreeNodeExtension>(string name) where TreeNodeExtension : TreeNode, new()
        {
            registeredNames.Add(name);
            registeredNodes.Add(name, () => new TreeNodeExtension());
        }

        public void RegisterLambdaNodes(string name, Func<Dictionary<string, string>, BehaviourTreeReturnType> function)
        {
            registeredNames.Add(name);
            registeredLambdaNodes.Add(name, function);
        }

        public void RegisterTerminalNode<TreeNodeExtension>(string name) where TreeNodeExtension : TerminalNode, new()
        {
            registeredNames.Add(name);
            registeredTerminalNodes.Add(name, () => new TreeNodeExtension());
        }

        public void RegisterLambdaTerminalNode(string name, Func<List<IMyTerminalBlock>, Dictionary<string, string>, BehaviourTreeReturnType> function)
        {
            registeredNames.Add(name);
            registeredTerminalNodes.Add(name, () =>
            {
                return new LambdaTerminalNode(function);
            });
        }

        //TODO: Do I need to add registerSimpleAction overloads with input values for the function? Can I?
    }
}
