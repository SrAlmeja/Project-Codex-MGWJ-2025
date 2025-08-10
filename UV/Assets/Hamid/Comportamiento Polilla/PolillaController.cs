using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolillaController : MonoBehaviour
{
    [Header("Configuración")]
    public float velocidadEmerger = 1f;
    public float velocidadDesaparecer = 1f;
    public float tiempoAntesDisparo = 1.5f;

    [Header("Prefabs")]
    public GameObject polillaPrefab;
    public GameObject surcoPrefab;
    public GameObject proyectilPrefab;

    [Header("Disparo")]
    public Transform bocaDisparo;

    private Transform jugador;
    private SpriteRenderer spriteRenderer;
    private Collider2D polillaCollider;
    private Vector3 posicionInicio;

    // Referencia al surco actual usado por esta polilla
    private GameObject surcoActual;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        polillaCollider = GetComponent<Collider2D>();

        GameObject jugadorGO = GameObject.FindGameObjectWithTag("Player");
        if (jugadorGO != null)
            jugador = jugadorGO.transform;
        else
            Debug.LogError("No se encontró al jugador con el tag 'Player'.");
    }

    public void SetJugador(Transform jugadorTransform)
    {
        jugador = jugadorTransform;
    }

    public void EmergerDesdeSurco(GameObject surco)
    {
        surcoActual = surco; // Guardamos la referencia del surco actual
        StartCoroutine(AnimEmerger(surco));
    }

    private IEnumerator AnimEmerger(GameObject surco)
    {
        Vector3 targetPos = transform.position;
        posicionInicio = targetPos + new Vector3(0, -1f, 0); // empieza debajo del surco
        transform.position = posicionInicio;
        spriteRenderer.color = new Color(1, 1, 1, 0);
        if (polillaCollider != null) polillaCollider.enabled = true;

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * velocidadEmerger;
            transform.position = Vector3.Lerp(posicionInicio, targetPos, t);
            spriteRenderer.color = new Color(1, 1, 1, t);
            if (surco != null)
                surco.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1 - t);
            yield return null;
        }

        yield return new WaitForSeconds(tiempoAntesDisparo);
        DispararProyectil();
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(AnimDesaparecer());
    }

    private void DispararProyectil()
    {
        /*
        if (jugador == null) return;

        Vector3 direccion = (jugador.position - transform.position).normalized;
        GameObject proyectil = Instantiate(proyectilPrefab, bocaDisparo.position, Quaternion.identity);
        Rigidbody2D rb = proyectil.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.velocity = direccion * 8f;
        }
        else
        {
            Debug.LogError("El proyectil no tiene Rigidbody2D.");
        }
        */

        if (jugador == null) return;

        Vector3 direccion = (jugador.position - transform.position).normalized;

        GameObject proyectil = Instantiate(proyectilPrefab, bocaDisparo.position, Quaternion.identity);

        // Calcular rotación para alinear el eje Y del proyectil hacia el jugador
        float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg - 90f;
        proyectil.transform.rotation = Quaternion.Euler(0, 0, angulo);

        Rigidbody2D rb = proyectil.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direccion * 8f;
        }
        else
        {
            Debug.LogError("El proyectil no tiene Rigidbody2D.");
        }
    }

    private IEnumerator AnimDesaparecer()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(0, -1f, 0);

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * velocidadDesaparecer;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            spriteRenderer.color = new Color(1, 1, 1, 1 - t);
            if (surcoActual != null)
                surcoActual.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, t);
            yield return null;
        }

        if (polillaCollider != null) polillaCollider.enabled = false;
        spriteRenderer.color = new Color(1, 1, 1, 0); // invisible

        if (surcoActual != null)
        {
            StartCoroutine(DesvanecerYSuprimirSurco(surcoActual, 0.5f));
            surcoActual = null;
        }

        StartCoroutine(ReaparecerConDelay(2f, 4f));
    }

    private IEnumerator ReaparecerConDelay(float min, float max)
    {
        yield return new WaitForSeconds(Random.Range(min, max));
        Reaparecer();
    }

    private void Reaparecer()
    {
        if (jugador == null) return;

        int cantidadDeSurcos = 5;
        float radioMaximo = 6f;
        float radioMinimo = 3f;

        List<GameObject> surcosInstanciados = new List<GameObject>();

        for (int i = 0; i < cantidadDeSurcos; i++)
        {
            Vector2 direccion = Random.insideUnitCircle.normalized;
            float distancia = Random.Range(radioMinimo, radioMaximo);
            Vector3 offset = new Vector3(direccion.x, direccion.y, 0f) * distancia;

            Vector3 posicionSurco = jugador.position + offset;
            GameObject surco = Instantiate(surcoPrefab, posicionSurco, Quaternion.identity);
            surcosInstanciados.Add(surco);
        }

        int indexAleatorio = Random.Range(0, surcosInstanciados.Count);
        GameObject surcoElegido = surcosInstanciados[indexAleatorio];

        // Teletransportar esta misma polilla a la nueva posición
        transform.position = surcoElegido.transform.position;
        spriteRenderer.color = new Color(1, 1, 1, 0);
        if (polillaCollider != null) polillaCollider.enabled = true;

        EmergerDesdeSurco(surcoElegido);

        for (int i = 0; i < surcosInstanciados.Count; i++)
        {
            if (i != indexAleatorio)
                Destroy(surcosInstanciados[i], 1f); // eliminar surcos no usados
        }
    }

    private IEnumerator DesvanecerYSuprimirSurco(GameObject surco, float duracion = 0.5f)
    {
        if (surco == null) yield break;

        SpriteRenderer sr = surco.GetComponent<SpriteRenderer>();
        if (sr == null) yield break;

        Color colorInicial = sr.color;
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, tiempo / duracion);
            sr.color = new Color(colorInicial.r, colorInicial.g, colorInicial.b, alpha);
            yield return null;
        }

        Destroy(surco);
    }
}
