using ScriptableObjectArchitecture.Examples;
using System.Collections.Generic;
using UnityEngine;

public class LogicaPulga : MonoBehaviour
{
    //[Header("Movimiento")]
    public float speed = 2f;                 // (se sobreescribe con speedOptions)
    public float moveTime = 2f;              // tiempo moviéndose
    public float stopTime = 0.6f;            // tiempo detenido

    //[Header("Velocidades posibles")]
    public float[] speedOptions = new float[] { 1.83f, 3.66f, 5.5f };

    //[Header("Área de movimiento (coordenadas de mundo)")]
    public float minX = -5f, maxX = 5f;
    public float minY = -3f, maxY = 3f;

    //[Header("Vida / Daño recibido por trigger del Player")]
    public int vidaMax = 5;
    public int vidaActual;
    public int danoAlRecibirTriggerDePlayer = 1;
    public float tiempoRojoAlSerGolpeada = 2f;

    // Internos
    private Vector2 currentDirection;
    private float timer;
    private bool isMoving;

    private SpriteRenderer sr;
    private Color colorOriginal;
    private Coroutine flashRutina;

    private static readonly Vector2[] dirs = new Vector2[]
    {
        Vector2.up, Vector2.down, Vector2.left, Vector2.right
    };

    void Start()
    {
        // Vida
        vidaActual = Mathf.Max(1, vidaMax);

        // SpriteRenderer (en este GO o en hijos)
        sr = GetComponent<SpriteRenderer>();
        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();
        colorOriginal = sr ? sr.color : Color.white;

        // Área y movimiento inicial
        transform.position = ClampToBounds(transform.position);
        ChooseNewDirection(true);
        RandomizeSpeed();      // velocidad aleatoria al arrancar
        isMoving = true;
        timer = 0f;
    }

    void Update()
    {
        if (isMoving)
        {
            Vector2 pos = transform.position;
            Vector2 newPos = pos + currentDirection * speed * Time.deltaTime;

            if (!Inside(newPos))
            {
                transform.position = ClampToBounds(newPos);
                BounceInward();
            }
            else
            {
                transform.position = newPos;
            }

            timer += Time.deltaTime;
            if (timer >= moveTime)
            {
                isMoving = false;    // pausa
                timer = 0f;
            }
        }
        else
        {
            // detenido
            timer += Time.deltaTime;
            if (timer >= stopTime)
            {
                ChooseNewDirection(false);
                RandomizeSpeed();    // nueva velocidad al reanudar
                isMoving = true;
                timer = 0f;
            }
        }
    }

    // ======== Daño por trigger con Player ========
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            RecibirDanio(danoAlRecibirTriggerDePlayer);
        }
    }

    public void RecibirDanio(int cantidad)
    {
        vidaActual -= Mathf.Max(1, cantidad);

        // Feedback visual: rojo por N segundos
        if (sr != null)
        {
            if (flashRutina != null) StopCoroutine(flashRutina);
            flashRutina = StartCoroutine(FlashRojoRutina());
            Debug.Log(vidaActual);
        }

        if (vidaActual <= 0)
        {
            // Aquí puedes poner animación/partículas antes de destruir.
            Destroy(gameObject);
        }
    }

    System.Collections.IEnumerator FlashRojoRutina()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(tiempoRojoAlSerGolpeada);
        if (sr != null) sr.color = colorOriginal;
        flashRutina = null;
    }
    // =============================================

    void RandomizeSpeed()
    {
        if (speedOptions != null && speedOptions.Length > 0)
            speed = speedOptions[Random.Range(0, speedOptions.Length)];
    }

    // Elige dirección que mantenga a la pulga dentro del área
    void ChooseNewDirection(bool forceIfCorner)
    {
        Vector2 pos = transform.position;
        List<Vector2> valid = new List<Vector2>();

        foreach (var d in dirs)
        {
            Vector2 test = pos + d * Mathf.Max(0.1f, speed * 0.1f);
            if (Inside(test)) valid.Add(d);
        }

        if (valid.Count > 0)
        {
            currentDirection = valid[Random.Range(0, valid.Count)];
            return;
        }

        // Esquina dura: empuja hacia el centro del rectángulo
        if (forceIfCorner || valid.Count == 0)
        {
            Vector2 center = new Vector2((minX + maxX) * 0.5f, (minY + maxY) * 0.5f);
            Vector2 toCenter = (center - (Vector2)transform.position);

            if (Mathf.Abs(toCenter.x) > Mathf.Abs(toCenter.y))
                currentDirection = new Vector2(Mathf.Sign(toCenter.x), 0f);
            else
                currentDirection = new Vector2(0f, Mathf.Sign(toCenter.y));
        }
    }

    // Si pegó al borde, invierte el eje que salió
    void BounceInward()
    {
        Vector2 p = transform.position;
        Vector2 d = currentDirection;

        if ((p.x <= minX && d.x < 0) || (p.x >= maxX && d.x > 0)) d.x = -d.x;
        if ((p.y <= minY && d.y < 0) || (p.y >= maxY && d.y > 0)) d.y = -d.y;

        currentDirection = (d == Vector2.zero) ? Vector2.up : d;
    }

    bool Inside(Vector2 pos)
    {
        return pos.x >= minX && pos.x <= maxX && pos.y >= minY && pos.y <= maxY;
    }

    Vector2 ClampToBounds(Vector2 pos)
    {
        return new Vector2(Mathf.Clamp(pos.x, minX, maxX), Mathf.Clamp(pos.y, minY, maxY));
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(
            new Vector3((minX + maxX) * 0.5f, (minY + maxY) * 0.5f, 0),
            new Vector3(Mathf.Max(0.01f, maxX - minX), Mathf.Max(0.01f, maxY - minY), 0));
    }
#endif
}
