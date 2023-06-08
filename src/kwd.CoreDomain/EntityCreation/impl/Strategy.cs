using System.Reflection;

namespace kwd.CoreDomain.EntityCreation.impl;

/// <summary>
/// 
/// </summary>
/// <param name="Static"></param>
/// <param name="Constructor"></param>
public record Strategy(MethodInfo? Static, ConstructorInfo? Constructor);