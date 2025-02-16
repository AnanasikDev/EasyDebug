using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace EasyDebug.CommandLine
{
    public class CommandLineEngine
    {
        public BindingFlags access = BindingFlags.Public |
                                     BindingFlags.NonPublic |
                                     BindingFlags.Static |
                                     BindingFlags.Instance;

        public List<MethodInfo> methods = new List<MethodInfo>();
        public List<Command> commands = new List<Command>();

        public Dictionary<Command, Type> command2script = new();

        public void Init()
        {
            methods = Assembly.GetExecutingAssembly()
                .GetTypes()
                .SelectMany(t => t.GetMethods(access))
                .Where(m => m.GetCustomAttributes<Command>().Any())
                .ToList();

            commands = methods.SelectMany(m => m.GetCustomAttributes<Command>()).ToList();

            var types = Assembly.GetExecutingAssembly()
                .GetTypes();
            foreach (var type in types)
            {
                foreach (var m in type.GetMethods(access).Where(mi => mi.GetCustomAttributes<Command>().Any()))
                {
                    Debug.Log($"------ {m.GetCustomAttribute<Command>().GetHashCode()} {m.GetCustomAttribute<Command>().Serialize()} {type}");
                    command2script.Add(m.GetCustomAttribute<Command>(), type);
                }
            }

            Debug.Log("Found " + methods.Count + " commands available:");

            foreach (var method in methods)
            {
                var attr = method.GetCustomAttribute<Command>();
                Debug.Log("Found command: " + method.Name + " with return type = " + method.ReturnType + "; Attribute name is " + attr.functionName + " of length = " + attr.functionName.Length);
            }
        }

        private List<GameObject> FindGameobjectsByCommand(Command command)
        {
            List<GameObject> result = new();
            foreach (var go in GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
            {
                if (go.GetComponents(command2script[command]).Any())
                    result.Add(go);
            }
            return result;
        }

        /// <summary>
        /// Searches for a component on the GameObject with the specified method and invokes it if found.
        /// </summary>
        public void TryInvokeMethodOnGameObject(GameObject gameObject, MethodInfo methodInfo)
        {
            // Get the name of the method we are looking for
            string methodName = methodInfo.Name;

            // Loop through each component attached to the GameObject
            foreach (var component in gameObject.GetComponents<MonoBehaviour>())
            {
                // Check if this component has the specified method
                var componentMethod = component.GetType().GetMethod(methodName, access);
                if (componentMethod != null && componentMethod == methodInfo)
                {
                    // Invoke the method on the component
                    componentMethod.Invoke(component, null); // Pass parameters here if required
                    Debug.Log($"Method '{methodName}' invoked on component '{component.GetType().Name}' attached to '{gameObject.name}'");
                    return;
                }
            }

            Debug.LogWarning($"No component with method '{methodName}' found on GameObject '{gameObject.name}'");
        }

        /// <summary>
        /// Finds all commands which names start with the specified prefix.
        /// </summary>
        /// <param name="prefix">The prefix to search for in the command name.</param>
        public List<Command> GetCommandsStartingWith(string prefix)
        {
            var matchingCommands = new List<Command>();

            foreach (var command in commands)
            {
                if (command != null && command.functionName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    matchingCommands.Add(command);
                }
            }

            return matchingCommands;
        }

        /// <summary>
        /// Format of a query must be as following (objectName.functionName arg1 arg2 arg...)
        /// objectName can be omitted if command is declared as global (.functionName arg1 arg2 arg...)
        /// a function can also be called on all instances of a certain type using syntax (typeName::functionName arg1 arg2 arg...)
        /// args are optional in any case.
        /// </summary>
        /// <param name="query">Completed or non-completed input query from the command line</param>
        /// <returns>Deformatted string into a temporary CommandInfo without connection to real commands</returns>
        internal CommandInfo ParseInput(string query)
        {
            if (query == string.Empty) return CommandInfo.Empty;
            if (query.Contains('.') == false)
            {
                //Debug.LogWarning($"Command Line could not execute command ({query}) as it has no '.' sign in it");
                return CommandInfo.Empty;
            }

            CommandInfo result = new CommandInfo();

            result.objectName = query.Split('.')[0];
            result.functionName = result.objectName == string.Empty ? query.Split(" ")[0] : query.Split(".")[1].Split(" ")[0];
            result.functionName = result.functionName.Replace(".", "").Replace(" ", "");

            return result;
        }

        public void Execute(string query)
        {
            CommandInfo commandInfo = ParseInput(query);
            if (commandInfo == CommandInfo.Empty)
            {
                //Debug.LogError("Command Line Query parse failed");
                return;
            }

            int index = commands.FindIndex(0, (Command c) => c.functionName == commandInfo.functionName);
            
            if (index == -1)
            {
                //Debug.LogError($"Command with name {commandInfo.functionName} could not be found");
                return;
            }

            string objectName = commandInfo.objectName;
            
            if (commandInfo.objectName == string.Empty)
            {
                // if no name specified, try find an object on scene which has that command implemented (with global tag on it)
                for (int i = 0; i < commands.Count; i++)
                {
                    if (commands[i].accessType == ConsoleCommandType.Global && commands[i].functionName == commandInfo.functionName)
                    {
                        var gameobjects = FindGameobjectsByCommand(commands[i]);
                        foreach (var go in gameobjects)
                        {
                            TryInvokeMethodOnGameObject(go, methods[i]);
                        }
                        break;
                    }
                }
            }
            var method = methods[index];
            GameObject obj = GameObject.Find(objectName);
            if (obj == null)
            {
                //Debug.LogError($"Object with name {commandInfo.objectName} not found on the current scene");
                return;
            }

            TryInvokeMethodOnGameObject(obj, method);
        }
    }
}