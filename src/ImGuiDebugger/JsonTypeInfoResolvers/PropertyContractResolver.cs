namespace ImGuiDebugger.JsonTypeInfoResolvers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

using Divine.Extensions;

internal sealed class PropertyContractResolver : DefaultJsonTypeInfoResolver
{
    private readonly IEnumerable<Type> Types;

    private readonly Dictionary<Type, JsonPolymorphismOptions?> Cache = new();

    public PropertyContractResolver()
    {
        Types = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly())!.Assemblies.First(x => x.GetName().Name is "Divine").GetTypes();
    }

    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        var typeInfo = base.GetTypeInfo(type, options);

        if (typeInfo.Kind is JsonTypeInfoKind.Object)
        {
            if (!Cache.TryGetValue(type, out var polymorphismOptions))
            {
                var types = Types.Where(type.IsAssignableFrom).ToArray();
                if (types.Length > 1)
                {
                    polymorphismOptions = new()
                    {
                        TypeDiscriminatorPropertyName = "$type",
                        IgnoreUnrecognizedTypeDiscriminators = true,
                        UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
                    };

                    var derivedTypes = polymorphismOptions.DerivedTypes;
                    types.ForEach(x => derivedTypes.Add(new(x, x.Name)));
                }

                Cache[type] = polymorphismOptions;
            }

            if (polymorphismOptions is not null)
            {
                typeInfo.PolymorphismOptions = polymorphismOptions;
            }
        }

        foreach (var property in typeInfo.Properties.ToArray())
        {
            if (property.Name is "KeyValue")
            {
                typeInfo.Properties.Remove(property);
            }
        }

        return typeInfo;
    }
}