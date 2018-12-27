using System;
using UnityEditor;

namespace UnityCommon
{
    [InitializeOnLoad]
    public class ScriptOrderManager
    {
        static ScriptOrderManager ()
        {
            foreach (var monoScript in MonoImporter.GetAllRuntimeMonoScripts())
            {
                if (monoScript.GetClass() == null) continue;

                foreach (var attribute in Attribute.GetCustomAttributes(monoScript.GetClass(), typeof(ScriptOrderAttribute), true))
                {
                    var currentOrder = MonoImporter.GetExecutionOrder(monoScript);
                    var newOrder = ((ScriptOrderAttribute)attribute).ExecutionOrder;
                    if (currentOrder != newOrder)
                        MonoImporter.SetExecutionOrder(monoScript, newOrder);
                }
            }
        }
    }
}
