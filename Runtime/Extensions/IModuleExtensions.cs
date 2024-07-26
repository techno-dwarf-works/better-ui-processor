using System.Collections.Generic;
using System.Linq;
using Better.UIProcessor.Runtime.Modules;

namespace Better.UIProcessor.Runtime.Extensions
{
    public static class IModuleExtensions
    {
        internal static bool Link(this IEnumerable<Module> self, UIProcessor processor, bool fallbackUnlink = true)
        {
            var modules = self.ToArray();
            var linkedModules = modules.Where(m => m.Link(processor)).ToArray();
            var result = linkedModules.Length == modules.Length;

            if (!result && fallbackUnlink)
            {
                foreach (var linkedModule in linkedModules)
                {
                    linkedModule.Unlink(processor);
                }
            }

            return result;
        }

        internal static bool Link(this Module self, IEnumerable<UIProcessor> processors, bool fallbackUnlink = true)
        {
            var processorsArray = processors.ToArray();
            var linkedProcessors = processorsArray.Where(self.Link).ToArray();
            var result = linkedProcessors.Length == processorsArray.Length;

            if (!result && fallbackUnlink)
            {
                foreach (var linkedProcessor in linkedProcessors)
                {
                    self.Unlink(linkedProcessor);
                }
            }

            return result;
        }

        internal static bool Unlink(this IEnumerable<Module> self, UIProcessor processor, bool fallbackLink = true)
        {
            var modules = self.ToArray();
            var unlinkedModules = modules.Where(m => m.Unlink(processor)).ToArray();
            var result = unlinkedModules.Length == modules.Length;

            if (!result && fallbackLink)
            {
                foreach (var linkedModule in unlinkedModules)
                {
                    linkedModule.Link(processor);
                }
            }

            return result;
        }

        internal static bool Unlink(this Module self, IEnumerable<UIProcessor> processors, bool fallbackLink = true)
        {
            var processorsArray = processors.ToArray();
            var unlinkedProcessors = processorsArray.Where(self.Unlink).ToArray();
            var result = unlinkedProcessors.Length == processorsArray.Length;

            if (!result && fallbackLink)
            {
                foreach (var linkedProcessor in unlinkedProcessors)
                {
                    self.Link(linkedProcessor);
                }
            }

            return result;
        }
    }
}