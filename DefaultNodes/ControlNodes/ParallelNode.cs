using IngameScript.DefaultNodes.AbstractNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace IngameScript.DefaultNodes.ControlNodes
{
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
            Reset();
        }

        private void Reset()
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
                            Reset();
                            return BehaviourTreeReturnType.SUCCESS;
                        }
                        results[index] = child_status;
                        break;
                    case BehaviourTreeReturnType.FAILURE:
                        failureCount++;
                        if (failureCount > children.Count - threshold)
                        {
                            Reset();
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
}
