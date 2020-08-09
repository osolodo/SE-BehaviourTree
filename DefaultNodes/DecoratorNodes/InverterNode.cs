using System;
using System.Collections.Generic;
using System.Text;

namespace IngameScript.DefaultNodes.DecoratorNodes
{
    class InverterNode : DecoratorNode
    {
        override public BehaviourTreeReturnType Tick()
        {
            switch (child.Tick())
            {
                case BehaviourTreeReturnType.SUCCESS:
                    return BehaviourTreeReturnType.FAILURE;
                case BehaviourTreeReturnType.FAILURE:
                    return BehaviourTreeReturnType.SUCCESS;
                default:
                    return BehaviourTreeReturnType.RUNNING;
            }
        }
    }
}
