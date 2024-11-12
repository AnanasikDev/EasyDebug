using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace EasyDebug.RuntimeConsole
{
    public enum ConsoleCommandType
    {
        /// <summary>
        /// Command can be identified and called using only name
        /// </summary>
        Global,

        /// <summary>
        /// Command can be identified and called using gameobject's name and name of the command
        /// </summary>
        ObjectRelative
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ConsoleCommand : Attribute
    {
        public string name { get; private set; }
        public ConsoleCommandType type { get; private set; }
        // TODO: add custom prefix (i.e. player, time, etc ~ alias)

        public ConsoleCommand(string name, ConsoleCommandType type)
        {
            this.name = name;
            this.type = type;
        }
    }

    public class RuntimeConsoleEngine
    {
        public BindingFlags access = BindingFlags.Public |
                                     BindingFlags.NonPublic |
                                     BindingFlags.Static |
                                     BindingFlags.Instance;

        List<MethodInfo> methods = new List<MethodInfo>();
        List<ConsoleCommand> commands = new List<ConsoleCommand>();

        public void Init()
        {
            methods = Assembly.GetExecutingAssembly()
                .GetTypes()
                .SelectMany(t => t.GetMethods(access))
                .Where(m => m.GetCustomAttributes<ConsoleCommand>().Any())
                .ToList();

            commands = methods.SelectMany(m => m.GetCustomAttributes<ConsoleCommand>()).ToList();

            Debug.Log("Found " + methods.Count + " commands available:");

            foreach (var method in methods)
            {
                var attr = method.GetCustomAttribute<ConsoleCommand>();
                Debug.Log("Found command: " + method.Name + " with return type = " + method.ReturnType + "; Attribute name is " + attr.name + " of length = " + attr.name.Length);
            }
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

        public void Execute(string query)
        {
            string objectName = query.Split('.')[0];
            //Debug.Log("EXECUTE: objectName = " + objectName + " of length = " + objectName.Length);

            string commandName = objectName == string.Empty ? query.Split(" ")[0] : query.Split(".")[1].Split(" ")[0];
            commandName = commandName.Replace(".", "").Replace(" ", "");
            //Debug.Log("EXECUTE: commandName = " + commandName + " of length = " + commandName.Length);

            if (commandName == string.Empty)
            {
                Debug.LogError("Command name is empty");
                return;
            }

            int index = commands.FindIndex(0, (ConsoleCommand c) => c.name == commandName);
            
            if (index == -1)
            {
                Debug.LogError(string.Format("Command with name {0} could not be found", commandName));
                return;
            }
            
            if (objectName == string.Empty)
            {
                // if no name spacified, try find an object on scene which has that command implemented (with global tag on it)
            }
            var method = methods[index];
            GameObject obj = GameObject.Find(objectName);
            if (obj == null)
            {
                Debug.LogError(string.Format("Object with name {0} not found on the current scene", objectName));
            }

            TryInvokeMethodOnGameObject(obj, method);
        }

        [ConsoleCommand("EngineFuncHEHE", ConsoleCommandType.ObjectRelative)]
        public void EngineFunc()
        {
            Debug.Log("Ok this one works noice");
        }
    }
}