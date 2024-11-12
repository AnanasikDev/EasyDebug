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

        public void Execute(string query)
        {
            string objectName = query.Split('.')[0];
            Debug.Log("EXECUTE: objectName = " + objectName + " of length = " + objectName.Length);

            string commandName = objectName == string.Empty ? query.Split(" ")[0] : query.Split(".")[1].Split(" ")[0];
            commandName = commandName.Replace(".", "").Replace(" ", "");
            Debug.Log("EXECUTE: commandName = " + commandName + " of length = " + commandName.Length);

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

            method.Invoke(obj, null);
        }
    }
}