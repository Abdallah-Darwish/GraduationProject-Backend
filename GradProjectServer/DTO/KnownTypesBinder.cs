using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GradProjectServer.DTO
{
    public class KnownTypesBinder : ISerializationBinder
    {
        readonly Dictionary<string, Type> nameToType;
        readonly Dictionary<Type, string> typeToName;

        public KnownTypesBinder()
        {
            var asm = Assembly.GetExecutingAssembly();
            var types = asm.GetExportedTypes()
                .Where(t => t.IsClass && t.Name.EndsWith("dto", StringComparison.OrdinalIgnoreCase));

            this.nameToType = types.ToDictionary(p => p.Name);
            nameToType.Add("Dictionary<string, object>", typeof(Dictionary<string, object>));

            this.typeToName = nameToType.ToDictionary(p => p.Value, p => p.Key);
        }

        public Type BindToType(string assemblyName, string typeName)
        {
            if (nameToType.TryGetValue(typeName, out var type))
                return type;
            throw new Exception($"Unknown type name {typeName} ({assemblyName})");
        }

        public void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            if (!typeToName.TryGetValue(serializedType, out typeName))
                throw new Exception($"Unknown type {serializedType}");
            assemblyName = null;
        }
    }
}