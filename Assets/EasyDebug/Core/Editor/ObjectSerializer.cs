using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace EasyDebug
{
    internal class ObjectSerializer
    {
        public static ObjectSerializer instance;
        public GameObject obj;

        public BindingFlags access = BindingFlags.Public |
                                     BindingFlags.NonPublic |
                                     BindingFlags.Static |
                                     BindingFlags.Instance;

        public bool onlyScripts = true;


        public ObjectSerializer()
        {
            instance = this;
        }

        public string Serialize()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var script in obj.GetComponents<MonoBehaviour>())
            {
                if (onlyScripts && !IsUserDefined(script.GetType())) continue;

                sb.AppendLine("===== " + script.ToString() + " =====");
                foreach (var field in script.GetType().GetFields(access))
                {
                    sb.AppendLine($"{field.FieldType} {field.Name}: {field.GetValue(script)}");
                }
            }

            return sb.ToString();
        }

        private bool IsUserDefined(System.Type type)
        {
            return type.Namespace == null || !type.Namespace.StartsWith("UnityEngine") ||
                type.Assembly.GetName().Name.StartsWith("Assembly-CSharp");
        }
    }
}