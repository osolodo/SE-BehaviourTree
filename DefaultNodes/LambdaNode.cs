using System;
using System.Collections.Generic;
using System.Text;

namespace IngameScript
{
    class LambdaNode : LeafNode
    {
        protected readonly Func<Dictionary<string, string>,BehaviourTreeReturnType> func;
        public LambdaNode(Func<Dictionary<string, string>, BehaviourTreeReturnType> func)
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
