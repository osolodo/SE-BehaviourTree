using IngameScript.DefaultNodes.AbstractNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace IngameScript.DefaultNodes.ControlNodes
{
    public class ReactiveSequenceNode : ControlNode
    {
        override public BehaviourTreeReturnType Tick()
        {
            BehaviourTreeReturnType child_status;

            for (int index = 0; index < children.Count; index++)
            {
                child_status = children[index].Tick();

                if (child_status == BehaviourTreeReturnType.RUNNING)
                {
                    return BehaviourTreeReturnType.RUNNING;
                }
                else if (child_status == BehaviourTreeReturnType.FAILURE)
                {
                    return BehaviourTreeReturnType.FAILURE;
                }
            }
            // all the children returned success. Return SUCCESS too.
            return BehaviourTreeReturnType.SUCCESS;
        }
    }
}
