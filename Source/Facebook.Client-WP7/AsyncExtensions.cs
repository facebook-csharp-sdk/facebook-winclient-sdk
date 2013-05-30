using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Client
{
    internal static class AsyncExtensions
    {

        public static async Task<object> GetTaskAsync(this FacebookClient client, string path, object parameters)
        {
            var tcs = new TaskCompletionSource<object>();
            client.GetCompleted += (s, e) =>
            {
                if (e.Error != null) tcs.TrySetException(e.Error);
                else if (e.Cancelled) tcs.TrySetCanceled();
                else tcs.TrySetResult(e.GetResultData());
            };
            client.GetAsync(path, parameters);
            return tcs.Task;
        }

    }
}
