﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyDebug.CommandLine
{
    /// <summary>
    /// Unifies raw input into CLI in the format of "objectName.functionName args**"
    /// </summary>
    public class ParsedCommand
    {
        /// <summary>
        /// Object that the command is being applied to. Null if global/unspecified
        /// </summary>
        public string objectName = "";

        /// <summary>
        /// Name of the function/method being called
        /// </summary>
        public string functionName = "";

        /// <summary>
        /// Arguments passed through to the command. Be careful - it can be null.
        /// </summary>
        public string[] args;

        public bool containsSeparator = false;

        public static ParsedCommand Empty { get { return new ParsedCommand(); } }

        public override bool Equals(object obj)
        {
            return obj is ParsedCommand info &&
                   objectName == info.objectName &&
                   functionName == info.functionName &&
                   EqualityComparer<object[]>.Default.Equals(args, info.args);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(objectName, functionName, args);
        }

        public static bool operator ==(ParsedCommand a, ParsedCommand b)
        {
            return a.objectName == b.objectName && 
                a.functionName == b.functionName && 
                ((a.args == null && b.args == null) || 
                (a.args != null && b.args != null && a.args.SequenceEqual(b.args)));
        }
        public static bool operator !=(ParsedCommand a, ParsedCommand b)
        {
            return !(a == b);
        }
        public string Serialize()
        {
            return $"{objectName}.{functionName}".Trim();
        }
    }
}
