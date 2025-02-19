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

        public Dictionary<Command, Type> command2script;

        public void Init()
        {
            methods = Assembly.GetExecutingAssembly()
                .GetTypes()
                .SelectMany(t => t.GetMethods(access))
                .Where(m => m.GetCustomAttributes<Command>().Any())
                .ToList();

            commands = methods.SelectMany(m => m.GetCustomAttributes<Command>()).ToList();

            command2script = new Dictionary<Command, Type>();

            var types = Assembly.GetExecutingAssembly()
                .GetTypes();
            foreach (var type in types)
            {
                foreach (var m in type.GetMethods(access).Where(mi => mi.GetCustomAttribute<Command>() != null))
                {
                    try
                    {
                        command2script.Add(m.GetCustomAttribute<Command>(), type);
                    }
                    catch (Exception e)
                    {
                        PipeConsole.Print(m.GetCustomAttribute<Command>().functionName, " is doubled! skipping");
                    }
                }
            }

            foreach (var method in methods)
            {
                var attr = method.GetCustomAttribute<Command>();
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
        /// Checks if there are any available commands on the gameobject.
        /// </summary>
        private bool HasCommands(GameObject gameObject)
        {
            foreach (var script in gameObject.GetComponents<MonoBehaviour>())
            {
                if (script.GetType().GetMethods(access).Where(mi => mi.GetCustomAttributes<Command>().Any()).ToArray().Length > 0)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns all commands runnable on the specific gameobject.
        /// </summary>
        private List<Command> GetCommands(GameObject gameObject)
        {
            List<Command> result = new();
            foreach (var script in gameObject.GetComponents<MonoBehaviour>())
            {
                foreach (var m in script.GetType().GetMethods(access))
                {
                    var cmd = m.GetCustomAttribute<Command>();
                    if (cmd == null || cmd.accessType == ConsoleCommandType.Global) continue;
                    result.Add(cmd);
                }
            }
            return result;
        }

        /// <summary>
        /// Returns all commands runnable on the specific gameobject.
        /// </summary>
        private List<Command> GetGlobalCommands(GameObject gameObject)
        {
            List<Command> result = new();
            foreach (var script in gameObject.GetComponents<MonoBehaviour>())
            {
                foreach (var m in script.GetType().GetMethods(access))
                {
                    var cmd = m.GetCustomAttribute<Command>();
                    if (cmd == null || cmd.accessType != ConsoleCommandType.Global) continue;
                    result.Add(cmd);
                }
            }
            return result;
        }

        /// <summary>
        /// Searches for a component on the GameObject with the specified method and invokes it if found.
        /// </summary>
        public void TryInvokeMethodOnGameObject(GameObject gameObject, MethodInfo methodInfo, object[] args)
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
                    componentMethod.Invoke(component, args); // Pass parameters here if required
                    Debug.Log($"Method '{methodName}' invoked on component '{component.GetType().Name}' attached to '{gameObject.name}'");
                    return;
                }
            }

            Debug.LogWarning($"No component with method '{methodName}' found on GameObject '{gameObject.name}'");
        }

        public void TryInvokeMethodWithArguments(GameObject gameObject, MethodInfo methodInfo, string[] stringArgs)
        {
            try
            {
                ParameterInfo[] argTypes = methodInfo.GetParameters();
                object[] args = new object[stringArgs.Length];
                for (int a = 0; a < stringArgs.Length; a++)
                {
                    args[a] = Convert.ChangeType(stringArgs[a], argTypes[a].ParameterType);
                }

                TryInvokeMethodOnGameObject(gameObject, methodInfo, args);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }

        /// <summary>
        /// Find the gameobject based on its name or alias
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private List<GameObject> GetAllGameobjectsByName(string name)
        {
            List<GameObject> result = new();

            foreach (var command in commands)
            {
                if (command.objectAlias == name)
                {
                    var gameobjects = FindGameobjectsByCommand(command);
                    result = result.Union(gameobjects).ToList();
                    break;
                }
            }

            foreach (var gameObject in GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
            {
                if (gameObject.name == name)
                {
                    result.Add(gameObject);
                }
            }
            return result;
        }

        /// <summary>
        /// Finds all commands which names start with the specified prefix.
        /// </summary>
        /// <param name="prefix">The prefix to search for in the command name.</param>
        public List<string> SuggestFunctions(ParsedCommand parsedCommand)
        {
            var result = new List<string>();

            // if searching for global commands
            if (parsedCommand.objectName == string.Empty)
            {
                foreach (var gameobject in GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
                {
                    foreach (var command in GetGlobalCommands(gameobject))
                    {
                        if (command.accessType == ConsoleCommandType.Global && command.functionName.StartsWith(parsedCommand.functionName, StringComparison.OrdinalIgnoreCase))
                        {
                            result.Add(command.functionName);
                        }
                    }
                }
                return result;
            }

            // object-relative commands only

            List<GameObject> gameobjects = GetAllGameobjectsByName(parsedCommand.objectName); 

            foreach (GameObject gameobject in gameobjects)
            {
                foreach (var command in GetCommands(gameobject))
                {
                    if (command.accessType == ConsoleCommandType.ObjectRelative && command.functionName.StartsWith(parsedCommand.functionName, StringComparison.OrdinalIgnoreCase))
                    {
                        result.Add(command.functionName);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Finds all gameobjects and aliases which names start with the specified prefix.
        /// </summary>
        /// <param name="prefix">The prefix to search for in the gameobject/alias name.</param>
        public List<string> SuggestObjects(ParsedCommand parsedCommand)
        {
            List<string> names = new();

            foreach (var command in commands)
            {
                if (command.objectAlias != string.Empty && command.objectAlias.StartsWith(parsedCommand.objectName, StringComparison.OrdinalIgnoreCase))
                {
                    if (names.Contains(command.objectAlias)) continue;
                    names.Add(command.objectAlias);
                }
            }

            foreach (var gameObject in GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
            {
                if (HasCommands(gameObject) && gameObject.name.StartsWith(parsedCommand.objectName, StringComparison.OrdinalIgnoreCase))
                {
                    if (names.Contains(gameObject.name)) continue;
                    names.Add(gameObject.name);
                }
            }

            return names;
        }

        /// <summary>
        /// Format of a query must be as following (objectName.functionName arg1; arg2; arg...)
        /// objectName can be omitted if command is declared as global (.functionName arg1; arg2; arg...)
        /// args are optional in any case.
        /// </summary>
        /// <param name="query">Completed or non-completed input query from the command line</param>
        /// <returns>Deformatted string into a temporary CommandInfo without connection to real commands</returns>
        internal ParsedCommand ParseInput(string query)
        {
            query = query.TrimStart();
            if (query == string.Empty) return ParsedCommand.Empty;
            ParsedCommand result = new ParsedCommand();
            if (query.Contains('.') == false)
            {
                result.objectName = query;
                return result;
            }

            result.containsSeparator = true;

            result.objectName = query.Split('.')[0];
            result.functionName = result.objectName == string.Empty ? query.Split(" ")[0] : query.Split(".")[1].Split(" ")[0];
            result.functionName = result.functionName.Replace(".", "").Replace(" ", "");

            if (result.functionName.Length > 0)
            {
                int argsIndex = result.objectName.Length + 1 + result.functionName.Length;
                if (query.Length > argsIndex && query[argsIndex] == ' ') // function is written
                {
                    string[] args = query.Substring(argsIndex).Replace(" ", "").Split(";");
                    result.args = args;
                    for (int a = 0; a < args.Length; a++)
                    {
                        Debug.Log($"Arg {a}: {args[a]}");
                    }
                }
            }

            return result;
        }

        public void Execute(string query)
        {
            ParsedCommand parsedCommand = ParseInput(query);
            if (parsedCommand == ParsedCommand.Empty)
            {
                //Debug.LogError("Command Line Query parse failed");
                return;
            }

            int index = commands.FindIndex(0, (Command c) => c.functionName == parsedCommand.functionName);
            
            if (index == -1)
            {
                //Debug.LogError($"Command with name {commandInfo.functionName} could not be found");
                return;
            }

            string objectName = parsedCommand.objectName;
            
            if (parsedCommand.objectName == string.Empty)
            {
                // if no name specified, try find an object on scene which has that command implemented (with global tag on it)
                for (int i = 0; i < commands.Count; i++)
                {
                    if (commands[i].accessType == ConsoleCommandType.Global && commands[i].functionName == parsedCommand.functionName)
                    {
                        var gameobjects = FindGameobjectsByCommand(commands[i]);
                        foreach (var go in gameobjects)
                        {
                            TryInvokeMethodWithArguments(go, methods[i], parsedCommand.args);
                        }
                        return;
                    }
                }
            }

            else // try find alias
            {
                for (int i = 0; i < commands.Count; i++)
                {
                    if (commands[i].objectAlias == parsedCommand.objectName && commands[i].accessType == ConsoleCommandType.ObjectRelative && commands[i].functionName == parsedCommand.functionName)
                    {
                        var gameobjects = FindGameobjectsByCommand(commands[i]);
                        foreach (var go in gameobjects)
                        {
                            TryInvokeMethodWithArguments(go, methods[i], parsedCommand.args);
                        }
                        return;
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

            TryInvokeMethodWithArguments(obj, method, parsedCommand.args);
        }
    }
}