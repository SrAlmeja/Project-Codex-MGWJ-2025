using System.Threading.Tasks;
using UnityEngine;

public static class TaskExtensions
{
    /// <summary>
    /// Ejecuta un Task sin await y captura cualquier excepción para que no pase desapercibida.
    /// </summary>
    /// <param name="task">El Task a “olvidar” (fire-and-forget).</param>
    public static void Forget(this Task task)
    {
        if (task == null) return;

        // Solo continuamos si el Task falla (OnlyOnFaulted)
        task.ContinueWith(t => { Debug.LogError($"[TaskExtensions] Excepción en Task: {t.Exception.Flatten()}"); }, TaskContinuationOptions.OnlyOnFaulted);
    }
}

