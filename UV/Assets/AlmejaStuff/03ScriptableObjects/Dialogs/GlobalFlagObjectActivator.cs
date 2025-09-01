using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalFlagObjectActivator : MonoBehaviour
{
    [Header("Trigger (flag origen)")]
    [Tooltip("Cuando este flag sea TRUE, se intentará activar el objeto en la otra escena.")]
    public GlobalBoolFlag triggerFlag;      // Flag #1

    [Header("Target Scene & Object (ya debe estar cargada por otro lado)")]
    [Tooltip("Nombre de la escena (debe estar cargada por alguien más, NO se carga aquí).")]
    public string targetSceneName;

    [Tooltip("Ruta jerárquica dentro de esa escena. Ej: Root/Sub/ObjetoObjetivo")]
    public string targetObjectPath;

    [Tooltip("Activa toda la cadena de padres antes de activar el objeto.")]
    public bool ensureParentsActive = true;

    [Header("Flag a setear cuando el objeto quede activo")]
    public GlobalBoolFlag completionFlag;   // Flag #2

    [Header("Opcional")]
    public bool runOnlyOnce = true;

    private bool _done;
    private bool _flagWentTrue;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        if (triggerFlag == null)
        {
            Debug.LogWarning($"{name}: 'triggerFlag' no asignado.");
            return;
        }

        if (triggerFlag.Value)
        {
            _flagWentTrue = true;
            TryActivate(); // por si la escena ya está cargada
        }
        else
        {
            StartCoroutine(WaitForFlagTrue());
        }
    }

    private IEnumerator WaitForFlagTrue()
    {
        while (!triggerFlag.Value) yield return null;
        _flagWentTrue = true;
        TryActivate(); // intenta apenas el flag se vuelve TRUE
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // No cargamos escenas aquí; solo reaccionamos si otro código ya la cargó
        if (_flagWentTrue) TryActivate();
    }

    private void TryActivate()
    {
        if (_done && runOnlyOnce) return;
        if (string.IsNullOrEmpty(targetSceneName) || string.IsNullOrEmpty(targetObjectPath))
        {
            Debug.LogWarning($"{name}: Falta 'targetSceneName' o 'targetObjectPath'.");
            return;
        }

        var scene = SceneManager.GetSceneByName(targetSceneName);
        if (!scene.IsValid() || !scene.isLoaded)
        {
            // La escena aún no está cargada por terceros; esperamos al evento sceneLoaded
            return;
        }

        var target = FindInSceneByPath(scene, targetObjectPath);
        if (target == null)
        {
            Debug.LogWarning($"{name}: No encontré '{targetObjectPath}' en '{targetSceneName}'.");
            return;
        }

        if (ensureParentsActive) SetParentsActive(target.transform, true);
        target.SetActive(true);

        if (target.activeInHierarchy && completionFlag != null)
        {
            completionFlag.SetTrue(); // Flag #2  TRUE
        }

        _done = true;
    }

    // --- Helpers ---

    private GameObject FindInSceneByPath(Scene scene, string path)
    {
        string[] parts = path.Split('/');
        if (parts.Length == 0) return null;

        GameObject rootGO = null;
        foreach (var root in scene.GetRootGameObjects())
        {
            if (root.name == parts[0]) { rootGO = root; break; }
        }
        if (rootGO == null) return null;

        Transform current = rootGO.transform;
        for (int i = 1; i < parts.Length; i++)
        {
            current = current.Find(parts[i]); // encuentra inactivos también
            if (current == null) return null;
        }
        return current.gameObject;
    }

    private void SetParentsActive(Transform t, bool active)
    {
        var stack = new System.Collections.Generic.Stack<Transform>();
        for (var cur = t; cur != null; cur = cur.parent) stack.Push(cur);
        while (stack.Count > 0) stack.Pop().gameObject.SetActive(active);
    }
}
