using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Text;

namespace IngameScript.DefaultNodes.AbstractNodes
{
    class TerminalNode : LeafNode
    {
        protected IMyGridTerminalSystem gridTerminalSystem;
        string blockType;
        string terminalSelector;
        TimeSpan refreshInterval = new TimeSpan(0,1,0);
        DateTime lastRefresh;
        protected List<IMyTerminalBlock> myTerminalBlocks = new List<IMyTerminalBlock>();

        public TerminalNode():base()
        {
        }

        override public void Init(Dictionary<string, string> attributes)
        {
            base.Init(attributes);
            if (!attributes.TryGetValue("blockType", out blockType))
            {
                throw new Exception(logger.Error("Block Type not found"));
            }
            if (!attributes.TryGetValue("terminalSelector", out terminalSelector))
            {
                throw new Exception(logger.Error("Terminal selector not found"));
            }
            string refreshIntervalString;
            if (attributes.TryGetValue("refreshInterval", out refreshIntervalString))
            {
                refreshInterval = new TimeSpan(0,0,int.Parse(refreshIntervalString));
            }
            lastRefresh = DateTime.UtcNow.Subtract(refreshInterval);
        }
        public void SetGridTerminalSystem(IMyGridTerminalSystem gridTerminalSystem)
        {
            logger.Debug("SetGridTerminalSystem:" + ((gridTerminalSystem != null) ? "good" : "null"));
            this.gridTerminalSystem = gridTerminalSystem;
            logger.Debug("SetGridTerminalSystem2:" + ((this.gridTerminalSystem != null) ? "good" : "null"));
        }

        private void RefreshTerminalBlocks(string terminalSelector)
        {
            logger.Debug("gridTerminalSystem:" + ((this.gridTerminalSystem != null) ? "good" : "null"));
            logger.Debug("myTerminalBlocks:" + ((myTerminalBlocks != null) ? "good" : "null"));
            this.gridTerminalSystem.SearchBlocksOfName(terminalSelector, myTerminalBlocks, block =>
            {
                Type blockT = block.GetType();
                return blockT.Name == blockType;
            });
            lastRefresh = DateTime.UtcNow;
        }

        override public BehaviourTreeReturnType Tick()
        {
            if (lastRefresh.Add(refreshInterval).CompareTo(DateTime.UtcNow) < 0)
            {
                RefreshTerminalBlocks(terminalSelector);
            }
            return BehaviourTreeReturnType.RUNNING;
        }
    }
}
