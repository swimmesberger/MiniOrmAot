// ReSharper disable UnusedAutoPropertyAccessor.Global

using System;

namespace MiniOrmAot.Common; 

public class GenerateMapperAttribute : Attribute {
    public Type[] EntityTypes { get; }
    public PropertyMappingPolicy NamingPolicyType { get; }

    public GenerateMapperAttribute() : this((Type?)null) { }

    public GenerateMapperAttribute(Type? entityType, PropertyMappingPolicy namingPolicyType = PropertyMappingPolicy.Undefined) :
        this(entityType == null ? Type.EmptyTypes : new[] { entityType }, namingPolicyType) { }

    public GenerateMapperAttribute(Type[] entityTypes, PropertyMappingPolicy namingPolicyType = PropertyMappingPolicy.Undefined) {
        EntityTypes = entityTypes;
        NamingPolicyType = namingPolicyType;
    }
}