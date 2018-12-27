using System;
using System.Diagnostics;

/// <summary>
/// Adds a define based on presence of specified type in the project.
/// </summary>
/// <remarks>
/// Unity's conditional compilation utility (<see href="https://github.com/Unity-Technologies/ConditionalCompilationUtility"/>)
/// uses this attribute to manage the project defines.
/// </remarks>
[Conditional("UNITY_CCU"), AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class OptionalDependencyAttribute : Attribute
{
    public string dependentClass;
    public string define;

    public OptionalDependencyAttribute (string dependentClass, string define)
    {
        this.dependentClass = dependentClass;
        this.define = define;
    }
}
