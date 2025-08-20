using System.Threading.Tasks;
using UnityEngine;

public static class AsyncOperationExtensions
{
    public static Task AsTask(this AsyncOperation operation)
    {
        var tcs = new TaskCompletionSource<object>();
        operation.completed += (op) => tcs.SetResult(null);
        return tcs.Task;
    }
}
