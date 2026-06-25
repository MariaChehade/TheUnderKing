using System.Collections;
using UnityEngine;

public class Trigger1 : MonoBehaviour
{
    [SerializeField] Transform destino;

    [Header("Câmeras")]
    [SerializeField] GameObject cameraAtivar;
    [SerializeField] GameObject cameraDesativar;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(SubirPlayer(collision.transform));
        }
    }

    IEnumerator SubirPlayer(Transform player)
    {
        Vector3 inicio = player.position;
        Vector3 fim = destino.position;

        float tempo = 1f;
        float t = 0;

        while (t < tempo)
        {
            player.position = Vector3.Lerp(inicio, fim, t / tempo);
            t += Time.deltaTime;
            yield return null;
        }

        player.position = fim;

        cameraAtivar.SetActive(true);
        cameraDesativar.SetActive(false);
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.instance.EntrarNaCaverna();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.instance.SairDaCaverna();
        }
    }
}