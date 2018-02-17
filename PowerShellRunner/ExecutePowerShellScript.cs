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
            //await RunDiceAsync(@"C:\windows\system32\windowspowershell\v1.0\powershell.exe ", Path.Combine(script.Key, script.Value)).ConfigureAwait(true);

            // there is no non-generic TaskCompletionSource
            var tcs = new TaskCompletionSource<bool>();

            var process = new Process
                          {
                              StartInfo =
                              {
                                  FileName = @"C:\windows\system32\windowspowershell\v1.0\powershell.exe",
                                  Arguments = script.FullName
                              },

                              EnableRaisingEvents = true
                          };

            process.Exited += (sender, args) =>
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