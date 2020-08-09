using IngameScript.DefaultNodes.AbstractNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace IngameScript.DefaultNodes.ControlNodes
{
    public class IfThenElseNode : ControlNode
    {
        private int index = 0;

        override public void Init(Dictionary<string, string> attributes)
        {
            base.Init(attributes);
            if (children.Count != 2 && children.Count != 3)
            {
                throw new Exception(logger.Error(this.name + " IfThenElse requires 2 or 3 children"));
            }
        }

        override public BehaviourTreeReturnType Tick()
        {
            BehaviourTreeReturnType child_status;

            if (index == 0)
            {
                child_status = children[0].Tick();
                switch (child_status)
                {
                    case BehaviourTreeReturnType.SUCCESS:
                        index = 1;
                        break;
                    case BehaviourTreeReturnType.FAILURE:
                        if (children.Count == 3)
                        {
                            index = 2;
                        }
                        else
                        {
                            return BehaviourTreeReturnType.FAILURE;
                        }
                        break;
                    default:
                        return BehaviourTreeReturnType.RUNNING;
                }
            }
            // important that this is not an else
            if (index > 0)
            {
                child_status = children[index].Tick();
                if (child_status != BehaviourTreeReturnType.RUNNING)
                {
                    //HaltAllChildren();
                    index = 0;
                }
                return child_status;
            }
            throw new Exception(logger.Error(this.name + " This should never be possible."));
        }
    }
}
