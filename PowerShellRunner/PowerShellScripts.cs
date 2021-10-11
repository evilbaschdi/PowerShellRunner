using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EvilBaschdi.Core.Extensions;

namespace PowerShellRunner.Core
{
    // ReSharper disable once UnusedType.Global
    public class PowerShellScripts : IPowerShellScripts
    {
        private readonly IPowerShellScriptsRaw _powerShellScriptsRaw;

        public PowerShellScripts(IPowerShellScriptsRaw powerShellScriptsRaw)
        {
            _powerShellScriptsRaw =
                powerShellScriptsRaw ?? throw new ArgumentNullException(nameof(powerShellScriptsRaw));
        }

        public List<FileInfo> Value
        {
            get
            {
                var powerShellScripts = new ConcurrentBag<FileInfo>();

                Parallel.ForEach(_powerShellScriptsRaw.Value,
                    path => { powerShellScripts.Add(path.FileInfo()); });
                return powerShellScripts.ToList();
            }
        }
    }
}