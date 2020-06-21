using System;
using System.Collections.Generic;
using System.Text;

namespace IngameScript
{
    partial class BehaviourTreeFactory
    {
        /*  Object createObjectBy(Type clazz){
                // .. do construction work here
                Object theObject = Activator.CreateInstance(clazz);
                return theObject;
            }
        */
        Dictionary<string, Type> registeredNodes;
        Dictionary<string, Func<BehaviourTreeReturnType>> registeredSimpleConditions;
        Dictionary<string, Func<BehaviourTreeReturnType>> registeredSimpleActions;
        HashSet<String> registeredNames;

        public BehaviourTreeFactory()
        {
            registeredNodes = new Dictionary<string, Type>();
            registeredSimpleConditions = new Dictionary<string, Func<BehaviourTreeReturnType>>();
            registeredSimpleActions = new Dictionary<string, Func<BehaviourTreeReturnType>>();
            registeredNames = new HashSet<string>();

            //TODO: register default nodes
        }

        public void registerNodeType<T>(string name)
        {
            registeredNames.Add(name);
            registeredNodes.Add(name, typeof(T));
        }

        public void registerSimpleCondition(string name, Func<BehaviourTreeReturnType> function)
        {
            registeredNames.Add(name);
            registeredSimpleConditions.Add(name, function);
        }

        public void registerSimpleAction(string name, Func<BehaviourTreeReturnType> function)
        {
            registeredNames.Add(name);
            registeredSimpleActions.Add(name, function);
        }

        //TODO: Do I need to add registerSimpleAction overloads with input values for the function? Can I?
    }
}
