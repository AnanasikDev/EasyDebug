using System;

namespace EasyDebug.CommandLine
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

    /// <summary>
    /// A marker for all functions that can be accessed via Command Line
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class Command : Attribute
    {
        public string functionName { get; private set; }
        public ConsoleCommandType accessType { get; private set; }
        public string objectAlias = "";
        // TODO: add optional custom prefix (i.e. player, time, etc ~ alias for gameobject name)

        public Command(string name, ConsoleCommandType type, string objectAlias = "")
        {
            this.functionName = name;
            this.accessType = type;
            this.objectAlias = objectAlias;
        }
        public string Serialize()
        {
            return $"{functionName}, {accessType} with alias {objectAlias}";
        }
    }
}
