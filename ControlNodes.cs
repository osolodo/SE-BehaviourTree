using System;
using System.Collections.Generic;
using System.Text;

namespace IngameScript
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
    public class SequenceStarNode : ControlNode
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
                else
                {
                    // keep same index if running or failure
                    return child_status;
                }
            }
            // all the children returned success. Return SUCCESS too.
            //HaltAllChildren();
            index = 0;
            return BehaviourTreeReturnType.SUCCESS;
        }
    }
    public class ParallelNode : ControlNode
    {
        private int threshold = 0;
        private BehaviourTreeReturnType[] results;
        private int successCount = 0;
        private int failureCount = 0;

        override public void Init(Dictionary<string, string> attributes)
        {
            base.Init(attributes);
            string thresholdStr;
            if (!attributes.TryGetValue("threshold", out thresholdStr))
            {
                threshold = int.Parse(thresholdStr);
            }
            else
            {
                threshold = children.Count;
            }
            results = new BehaviourTreeReturnType[children.Count];
            reset();
        }

        private void reset()
        {
            successCount = 0;
            failureCount = 0;
            for (int i = 0; i < results.Length; i++)
            {
                results[i] = BehaviourTreeReturnType.RUNNING;
            }
        }
        override public BehaviourTreeReturnType Tick()
        {
            if (children.Count < threshold)
            {
                throw new Exception(logger.Error(this.name + " Number of children is less than threshold. Can never suceed."));
            }

            BehaviourTreeReturnType child_status;

            for (int index = 0; index < children.Count; index++)
            {
                //Skip if this child has already produced a result
                if (results[index] != BehaviourTreeReturnType.RUNNING)
                {
                    continue;
                }
                child_status = children[index].Tick();

                switch (child_status)
                {
                    case BehaviourTreeReturnType.SUCCESS:
                        successCount++;
                        if (successCount == threshold)
                        {
                            reset();
                            return BehaviourTreeReturnType.SUCCESS;
                        }
                        results[index] = child_status;
                        break;
                    case BehaviourTreeReturnType.FAILURE:
                        failureCount++;
                        if (failureCount > children.Count - threshold)
                        {
                            reset();
                            return BehaviourTreeReturnType.FAILURE;
                        }
                        results[index] = child_status;
                        break;
                    default:
                        break;
                }
            }
            return BehaviourTreeReturnType.RUNNING;
        }
    }
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
    public class ManualNode : ControlNode
    {
        //TODO: implement manual selection?
        //Could implement this as input from antenna/blackboard?
    }
}
