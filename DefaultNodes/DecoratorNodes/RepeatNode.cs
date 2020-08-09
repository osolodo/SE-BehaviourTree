using System;
using System.Collections.Generic;
using System.Text;

namespace IngameScript.DefaultNodes.DecoratorNodes
{
    class RepeatNode : DecoratorNode
    {
        private int tryIndex = 0;
        private int numCycles = 0;

        override public void Init(Dictionary<string, string> attributes)
        {
            base.Init(attributes);
            string numCyclesStr;
            attributes.TryGetValue("attempts", out numCyclesStr);
            numCycles = int.Parse(numCyclesStr);
        }

        override public BehaviourTreeReturnType Tick()
        {
            BehaviourTreeReturnType child_status;

            while (tryIndex < numCycles || numCycles == -1)
            {
                child_status = child.Tick();

                switch (child_status)
                {
                    case BehaviourTreeReturnType.SUCCESS:
                        tryIndex++;
                        break;
                    case BehaviourTreeReturnType.FAILURE:
                        tryIndex = 0;
                        return BehaviourTreeReturnType.FAILURE;
                    default:
                        return BehaviourTreeReturnType.RUNNING;
                }
            }
            //HaltAllChildren();
            tryIndex = 0;
            return BehaviourTreeReturnType.SUCCESS;
        }
    }
}
