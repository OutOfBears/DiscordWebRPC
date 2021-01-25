using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordWebRPC
{
    public static class TaskExtensions
    {
        public static async Task<T> Timeout<T>(this Task<T> task, int timeout = 5000)
        {
            var delayedTask = Task.Delay(timeout);
            var finishedTask = await Task.WhenAny(task, delayedTask).ConfigureAwait(false);

            if (finishedTask != task)
            {
                throw new TimeoutException();
            }

            return task.Result;
        }
    }
}
