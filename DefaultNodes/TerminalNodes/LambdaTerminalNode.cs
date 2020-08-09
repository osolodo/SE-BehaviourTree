using IngameScript.DefaultNodes.AbstractNodes;
using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;

namespace IngameScript.DefaultNodes.TerminalNodes
{
    class LambdaTerminalNode : TerminalNode
    {
        protected readonly Func<List<IMyTerminalBlock>,Dictionary<string, string>, BehaviourTreeReturnType> func;

        public LambdaTerminalNode(Func<List<IMyTerminalBlock>, Dictionary<string, string>, BehaviourTreeReturnType>  func) : base()
        {
            this.func = func;
        }

        public override void Init(Dictionary<string, string> attributes)
        {
            base.Init(attributes);
        }

        public override BehaviourTreeReturnType Tick()
        {
            base.Tick();
            return func(myTerminalBlocks,attributes);
        }
    }
}
