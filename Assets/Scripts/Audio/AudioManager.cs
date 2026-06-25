using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; // pra acessar de qualquer lugar

    [Header("Audio Sources")]
    public AudioSource musicaCastelo;
    public AudioSource musicaCaverna;

    [Header("Configurações")]
    public float duracaoFade = 1.5f; // tempo do fade em segundos
    public float volumeMaximo = 0.6f;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // começa com a música do castelo tocando (ajuste se for diferente)
        musicaCastelo.volume = volumeMaximo;
        musicaCastelo.Play();

        musicaCaverna.volume = 0f;
        musicaCaverna.Play(); // já toca mudo, só sobe o volume depois
    }

    public void EntrarNaCaverna()
    {
        StopAllCoroutines();
        StartCoroutine(Fade(musicaCastelo, 0f));
        StartCoroutine(Fade(musicaCaverna, volumeMaximo));
    }

    public void SairDaCaverna()
    {
        StopAllCoroutines();
        StartCoroutine(Fade(musicaCaverna, 0f));
        StartCoroutine(Fade(musicaCastelo, volumeMaximo));
    }

    IEnumerator Fade(AudioSource source, float volumeAlvo)
    {
        float volumeInicial = source.volume;
        float tempo = 0f;

        while (tempo < duracaoFade)
        {
            tempo += Time.deltaTime;
            source.volume = Mathf.Lerp(volumeInicial, volumeAlvo, tempo / duracaoFade);
            yield return null;
        }

        source.volume = volumeAlvo;
    }
}