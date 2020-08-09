using IngameScript.DefaultNodes.AbstractNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace IngameScript.DefaultNodes
{
    class ReactiveFallbackNode : ControlNode
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
                    // continue the while loop
                    index++;
                }
                else if (child_status == BehaviourTreeReturnType.SUCCESS)
                {
                    // Suspend execution and return SUCCESS.
                    //HaltAllChildren();
                    return child_status;
                }
            }
            // all the children returned FAILURE. Return FAILURE too.
            //HaltAllChildren();
            return BehaviourTreeReturnType.FAILURE;
        }
    }

}
