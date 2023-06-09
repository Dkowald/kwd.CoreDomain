using System;

namespace kwd.CoreDomain.EntityCreation.impl;

/// <summary>
/// Maintain the set of method strategies found.
/// </summary>
internal interface IEntityMethodStrategies
{
    /// <summary>
    /// Get strategy associated with method factory.
    /// </summary>
    Strategy this[Type methodFactoryType] { get; }
}