using System;
using System.Collections.Generic;
using System.Text;

namespace IngameScript.DefaultNodes.AbstractNodes
{
    public abstract class TreeNode
    {
        protected string name;
        protected string ID;
        protected static readonly Logger logger = new Logger("BehaviourTree_TreeNode");
        protected Dictionary<string, string> attributes;
        virtual public void Init(Dictionary<string, string> attributes)
        {
            this.attributes = attributes;
            attributes.TryGetValue("name", out name);
            attributes.TryGetValue("ID", out ID);
            if (name == null)
            {
                name = ID;
            }
        }

        public virtual string GetName()
        {
            return name;
        }
        public virtual string GetID()
        {
            return ID;
        }

        public virtual void Validate()
        {

        }

        public virtual BehaviourTreeReturnType Tick()
        {
            throw new Exception(logger.Error("Invalid call to base Tick() method"));
        }
    }
}
