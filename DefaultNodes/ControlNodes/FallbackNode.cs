using IngameScript.DefaultNodes.AbstractNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace IngameScript.DefaultNodes
{
    class FallbackNode : ControlNode
    {
        private int index = 0;
        override public BehaviourTreeReturnType Tick()
        {
            BehaviourTreeReturnType child_status;

            while (index < children.Count)
            {
                child_status = children[index].Tick();

                if (child_status == BehaviourTreeReturnType.RUNNING)
                {
                    // Suspend execution and return RUNNING.
                    // At the next tick, _index will be the same.
                    return child_status;
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
                    index = 0;
                    return child_status;
                }
            }
            // all the children returned success. Return SUCCESS too.
            //HaltAllChildren();
            index = 0;
            return BehaviourTreeReturnType.SUCCESS;
        }
    }
}
