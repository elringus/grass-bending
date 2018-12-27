using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UnityCommon
{
    public static class ReflectionUtils
    {
        public static IEnumerable<Type> ExportedDomainTypes { get { return cachedDomainTypes ?? (cachedDomainTypes = GetExportedDomainTypes()); } }

        private static IEnumerable<Type> cachedDomainTypes;

        public static bool IsDynamicAssembly (Assembly assembly)
        {
            #if NET_4_6 || NET_STANDARD_2_0
            return assembly.IsDynamic;
            #else
            return assembly is System.Reflection.Emit.AssemblyBuilder;
            #endif
        }

        public static IEnumerable<Assembly> GetDomainAssemblies (bool excludeDynamic = true)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            return excludeDynamic ? assemblies.Where(a => !IsDynamicAssembly(a)) : assemblies;
        }

        public static IEnumerable<Type> GetExportedDomainTypes ()
        {
            return GetDomainAssemblies().SelectMany(a => a.GetExportedTypes());
        }
    }
}
