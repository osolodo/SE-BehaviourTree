using IngameScript.DefaultNodes;
using IngameScript.DefaultNodes.AbstractNodes;
using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;

namespace IngameScript
{
    partial class BehaviourTreeFactory
    {
        private static readonly Logger logger = new Logger("BehaviourTree_Creation");

        private static readonly System.Text.RegularExpressions.Regex xmlName = new System.Text.RegularExpressions.Regex("(:|[A-Z]|_|[a-z])(:|[A-Z]|_|[a-z]|-|[0-9])*");
        private static readonly System.Text.RegularExpressions.Regex xmlAttributes = new System.Text.RegularExpressions.Regex("([a-zA-Z0-9_]+) *= *\"([a-zA-Z0-9_:{\\. }]+)\"");
        public BehaviourTree CreateTree(string inputString,IMyGridTerminalSystem gridTerminalSystem)
        {
            List<SubTreeNode> subTreeNodes = new List<SubTreeNode>();
            List<TerminalNode> terminalNodes = new List<TerminalNode>();


            logger.Log("Creating Tree");
            logger.IncLvl();
            ParsingNode parsingNode = ParseNode(inputString, subTreeNodes, terminalNodes);
            logger.DecLvl();


            logger.Log("Validating Tree");
            if (parsingNode.open)
            {
                logger.Log("Top level node is not closed!");
            }


            TreeNode treeNode = parsingNode.treeNode;
            if (!(treeNode is RootNode))
            {
                throw new Exception(logger.Error("Top level node must be a root node!"));
            }
            RootNode rootNode = (RootNode)treeNode;
            subTreeNodes.ForEach(subTreeNode =>
            {
                subTreeNode.SetRootNode(rootNode);
            });


            logger.Debug("gridTerminalSystem:" + ((gridTerminalSystem != null) ? "good" : "null"));
            terminalNodes.ForEach(terminalNode =>
            {
                terminalNode.SetGridTerminalSystem(gridTerminalSystem);
            });


            logger.IncLvl();
            treeNode.Validate();
            logger.DecLvl();

            return new BehaviourTree(rootNode);
        }

        private struct ParsingNode
        {
            public TreeNode treeNode;
            public bool open;
            public int endIndex;

            public ParsingNode(TreeNode treeNode, int endIndex) : this(treeNode, true, endIndex) { }
            public ParsingNode(TreeNode treeNode, bool open, int endIndex)
            {
                this.treeNode = treeNode;
                this.open = open;
                this.endIndex = endIndex;
            }
        }

        private ParsingNode ParseNode(string inputString, List<SubTreeNode> subTreeNodes, List<TerminalNode> terminalNodes)
        {
            Dictionary<string, string> attributes = new Dictionary<string, string>();

            int currentIndex = inputString.IndexOf('<');
            logger.Debug("currentIndex:" + currentIndex);
            while (currentIndex < inputString.Length)
            {
                //check for comment
                if (inputString.Substring(currentIndex, 4) == "<!--")
                {
                    logger.Debug("Found a comment, skipping.");
                    //jump to first open < after close of comment
                    currentIndex = inputString.IndexOf('<', inputString.IndexOf("-->") + 3);
                    logger.Debug("currentIndex:" + currentIndex);
                    continue;
                } else
                {
                    break;
                }
            }
            int endOfTag = inputString.IndexOf(">", currentIndex);
            logger.Debug("endOfTag:" + endOfTag);
            bool emptyNode = inputString[endOfTag - 1] == '/';
            //logger.Debug("Empty Node:"+ emptyNode);

            //Get element name
            System.Text.RegularExpressions.Match elementName = xmlName.Match(inputString, currentIndex + 1);
            CheckForSkippedChars(inputString, currentIndex + 1, elementName.Index);

            if (null == elementName)
            {
                throw new Exception(logger.Error("Name of element could not be found."));
            }
            //logger.Debug("Element:" + elementName.Value);
            Func<TreeNode> nodeConstructor;
            Func< Dictionary<string, string>,BehaviourTreeReturnType > simpleNode;
            Func<TerminalNode> terminalNode;
            ParsingNode outputNode;
            TreeNode treeNode;

            //attributes
            currentIndex = elementName.Index + elementName.Length;
            logger.Debug("currentIndex:" + currentIndex);
            System.Text.RegularExpressions.MatchCollection matches = xmlAttributes.Matches(inputString.Substring(currentIndex, endOfTag - currentIndex));
            //logger.Debug("Found " + matches.Count + " attributes");
            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                //logger.Debug("Captures:"+match.Groups.Count.ToString());
                logger.Debug("attribute " + match.Groups[1].Value + ": " + match.Groups[2].Value);
                attributes.Add(match.Groups[1].Value, match.Groups[2].Value);
            }

            //create Node
            if (registeredNodes.TryGetValue(elementName.Value, out nodeConstructor))
            { 
                logger.Debug("Creating complex node: " + elementName.Value);
                treeNode = nodeConstructor.Invoke();
                if (treeNode is SubTreeNode)
                {
                    subTreeNodes.Add((SubTreeNode)treeNode);
                }
            }
            else if (registeredLambdaNodes.TryGetValue(elementName.Value, out simpleNode))
            {
                logger.Debug("Creating lambda node: " + elementName.Value);
                treeNode = new LambdaNode(simpleNode);
            }
            else if (registeredTerminalNodes.TryGetValue(elementName.Value, out terminalNode))
            {
                logger.Debug("Creating terminal node: " + elementName.Value);
                treeNode = terminalNode.Invoke();
            }
            else
            {
                throw new Exception(logger.Error("Element type " + elementName.Value + " not registered."));
            }

            outputNode = new ParsingNode(treeNode, !emptyNode, endOfTag);

            List<ParsingNode> childNodes = new List<ParsingNode>();
            int endOfClosingTag;
            currentIndex = inputString.IndexOf("<", endOfTag);

            while (outputNode.open)
            {
                currentIndex = inputString.IndexOf("<", currentIndex);
                logger.Debug("currentIndex:" + currentIndex);
                if (inputString[currentIndex + 1] == '/')
                {
                    endOfClosingTag = inputString.IndexOf(">", currentIndex);
                    string closingTag = inputString.Substring(currentIndex + 2, endOfClosingTag - currentIndex - 2);
                    logger.Debug("Found Closing tag:" + closingTag);
                    if (closingTag != elementName.Value)
                    {
                        throw new Exception(logger.Error("Closing tag " + closingTag + " does not match tag " + elementName.Value));
                    } else
                    {
                        logger.Debug("Found "+childNodes.Count+" Children");
                        if (childNodes.Count > 0)
                        {
                            if (outputNode.treeNode is ControlNode)
                            {
                                ((ControlNode)outputNode.treeNode).SetChildren(childNodes.ConvertAll<TreeNode>(parserNode => parserNode.treeNode));
                            } else if (outputNode.treeNode is DecoratorNode)
                            {
                                ((DecoratorNode)outputNode.treeNode).SetChild(childNodes[0].treeNode);
                            } else
                            {
                                throw new Exception(logger.Error("Node "+elementName.Value+" is not of a type that can have child nodes"));
                            }
                        }
                        outputNode.endIndex = endOfClosingTag;
                        outputNode.open = false;
                    }
                } else
                {
                    logger.Debug("Finding Children");
                    logger.IncLvl();
                    ParsingNode parsingNode = ParseNode(inputString.Substring(currentIndex),subTreeNodes, terminalNodes);
                    logger.DecLvl();
                    childNodes.Add(parsingNode);
                    logger.Debug("endIndex of found child:" + parsingNode.endIndex);
                    currentIndex += parsingNode.endIndex+1;
                }
            }

            treeNode.Init(attributes);

            return outputNode;
        }

        private bool CheckForSkippedChars(string inputString, int start, int end)
        {
            string str = inputString.Substring(start, end - start).Trim();
            if (str.Length != 0)
            {
                throw new Exception(logger.Error("Unexpected chars " + str + " between " + start + " and " + end));
            }
            return true;
        }
    }
}
