using System;
using System.Collections.Generic;
using System.Text;

namespace IngameScript.DefaultNodes.DecoratorNodes
{
    class ForceSuccessNode : DecoratorNode
    {
        override public BehaviourTreeReturnType Tick()
        {
            if (child.Tick() == BehaviourTreeReturnType.RUNNING)
            {
                return BehaviourTreeReturnType.RUNNING;
            }
            return BehaviourTreeReturnType.SUCCESS;
        }
    }
}
