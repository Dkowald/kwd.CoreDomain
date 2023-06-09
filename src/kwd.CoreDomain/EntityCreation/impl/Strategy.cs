using System;
using System.Reflection;

namespace kwd.CoreDomain.EntityCreation.impl;

/// <summary>
/// Describes the use of either a static method or constructor
/// to use when creating an entity. 
/// </summary>
public record Strategy(MethodInfo? Static, ConstructorInfo? Constructor, Type[] Arguments);