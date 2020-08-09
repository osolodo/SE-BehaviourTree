using System;
using System.Collections.Generic;
using System.Text;

namespace IngameScript
{
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
}
