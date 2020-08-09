using IngameScript.DefaultNodes.AbstractNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace IngameScript.DefaultNodes.ControlNodes
{
    public class WhileDoElseNode : ControlNode
    {
        private int index = 0;

        override public void Init(Dictionary<string, string> attributes)
        {
            base.Init(attributes);
            if (children.Count != 3)
            {
                throw new Exception(logger.Error(this.name + " WhileDoElse requires 3 children"));
            }
        }

        override public BehaviourTreeReturnType Tick()
        {
            BehaviourTreeReturnType child_status;

            child_status = children[0].Tick();
            switch (child_status)
            {
                case BehaviourTreeReturnType.SUCCESS:
                    child_status = children[1].Tick();
                    break;
                case BehaviourTreeReturnType.FAILURE:
                    child_status = children[2].Tick();
                    break;
                default:
                    return BehaviourTreeReturnType.RUNNING;
            }
            return child_status;
        }
    }
}
