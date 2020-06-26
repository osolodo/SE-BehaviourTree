using System;
using System.Collections.Generic;
using System.Text;

namespace IngameScript
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
    class ForceFailureNode : DecoratorNode
    {
        override public BehaviourTreeReturnType Tick()
        {
            if (child.Tick() == BehaviourTreeReturnType.RUNNING)
            {
                return BehaviourTreeReturnType.RUNNING;
            }
            return BehaviourTreeReturnType.FAILURE;
        }
    }
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

            while(tryIndex < numCycles || numCycles == -1)
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
    class RetryNode : DecoratorNode
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
                        tryIndex = 0;
                        return BehaviourTreeReturnType.SUCCESS;
                    case BehaviourTreeReturnType.FAILURE:
                        tryIndex++;
                        break;
                    default:
                        return BehaviourTreeReturnType.RUNNING;
                }
            }
            //HaltAllChildren();
            tryIndex = 0;
            return BehaviourTreeReturnType.FAILURE;
        }
    }
    class TimeoutNode : DecoratorNode
    {
        private int msec = 0;
        private bool timerStarted = false;
        private DateTime start;

        override public void Init(Dictionary<string, string> attributes)
        {
            base.Init(attributes);
            string msecStr;
            attributes.TryGetValue("msec", out msecStr);
            msec = int.Parse(msecStr);
        }

        override public BehaviourTreeReturnType Tick()
        {
            BehaviourTreeReturnType child_status;

            if(!timerStarted)
            {
                start = DateTime.UtcNow;
            } else if(msec > 0 && DateTime.UtcNow.Subtract(start).TotalMilliseconds > msec)
            {
                return BehaviourTreeReturnType.FAILURE;
            }
            child_status = child.Tick();
            if (child_status != BehaviourTreeReturnType.RUNNING)
            {
                timerStarted = false;
            }
            return child_status;
        }
    }
}
