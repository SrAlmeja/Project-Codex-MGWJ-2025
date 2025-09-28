using UnityEngine;

public class PolillaZoneTrigger : MonoBehaviour
{
    public GameObject polillaPrefab;
    public GameObject montanitaPrefab;
    public float distanciaFrontal = 5f;

    private bool yaActivado = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!yaActivado && other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            //Debug.Log("Ingreso el jugador");
            yaActivado = true;

            Vector3 spawnPosition = other.transform.position + other.transform.up * distanciaFrontal;
            GameObject montanita = Instantiate(montanitaPrefab, spawnPosition, Quaternion.identity);
            GameObject polilla = Instantiate(polillaPrefab, spawnPosition, Quaternion.identity);

            polilla.GetComponent<PolillaController>().EmergerDesdeSurco(montanita);

        }
    }
}
