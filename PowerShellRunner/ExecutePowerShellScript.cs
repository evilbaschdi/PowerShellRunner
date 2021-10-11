using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace PowerShellRunner.Core
{
    public class ExecutePowerShellScript : IExecutePowerShellScript
    {
        /// <inheritdoc />
        public async void RunFor(FileInfo script)
        {
            // there is no non-generic TaskCompletionSource
            var tcs = new TaskCompletionSource<bool>();

            var process = new Process
                          {
                              StartInfo =
                              {
                                  FileName = @"C:\windows\system32\windowspowershell\v1.0\powershell.exe",
                                  Arguments = $"-executionpolicy unrestricted {script.FullName}"
                              },

                              EnableRaisingEvents = true
                          };

            process.Exited += (_, _) =>
                              {
                                  tcs.SetResult(true);
                                  process.Dispose();
                              };

            process.Start();

            var task = tcs.Task;
            await task.ConfigureAwait(true);
            Console.WriteLine(task.Result);
        }
    }
}