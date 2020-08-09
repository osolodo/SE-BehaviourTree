using System;
using System.Collections.Generic;
using System.Text;

namespace IngameScript.DefaultNodes.DecoratorNodes
{
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

            if (!timerStarted)
            {
                start = DateTime.UtcNow;
            }
            else if (msec > 0 && DateTime.UtcNow.Subtract(start).TotalMilliseconds > msec)
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
