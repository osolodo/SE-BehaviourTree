using IngameScript.DefaultNodes.AbstractNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace IngameScript.DefaultNodes.ControlNodes
{
    public class SequenceNode : ControlNode
    {
        private int index = 0;
        override public BehaviourTreeReturnType Tick()
        {
            BehaviourTreeReturnType child_status;

            while (index < children.Count)
            {
                child_status = children[index].Tick();

                if (child_status == BehaviourTreeReturnType.SUCCESS)
                {
                    index++;
                }
                else if (child_status == BehaviourTreeReturnType.RUNNING)
                {
                    // keep same index
                    return child_status;
                }
                else if (child_status == BehaviourTreeReturnType.FAILURE)
                {
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
