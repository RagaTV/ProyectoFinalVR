using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [Header("Configuración del Pool")]
    [SerializeField] private AudioSource audioSourcePrefab; 
    [SerializeField] private int poolSize = 12; // Subimos a 12 por la cantidad de sonidos simultáneos

    [Header("Música y Ambiente (2D)")]
    public AudioClip musicaInicio;
    public AudioClip musicaAmbiente;
    public AudioClip musicaGanar;
    public AudioClip musicaPerder;
    public AudioClip sonidoBotonUI;

    [Header("Efectos del Laboratorio (3D)")]
    public AudioClip ponerBotellaBandeja;
    public AudioClip ingredienteAlCaldero;
    public AudioClip calderoExplota;
    public AudioClip calderoBurbujeoNormal; // El loop constante
    public AudioClip agarrarObjeto;
    public AudioClip aciertoReceta;
    public AudioClip errorIngrediente;
    public AudioClip recibirMonedas;
    
    [Header("Mascota (3D)")]
    public AudioClip maullidoGato;

    private AudioSource[] audioPool;
    private int currentPoolIndex = 0;
    private AudioSource calderoAmbientalSource; // Source especial para el sonido infinito del caldero

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InicializarPool();
    }


    private void InicializarPool()
    {
        audioPool = new AudioSource[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            AudioSource nuevoSource = Instantiate(audioSourcePrefab, transform);
            audioPool[i] = nuevoSource;
        }

        // Creamos un Source extra exclusivo para el caldero para que no se interrumpa con los clics
        calderoAmbientalSource = Instantiate(audioSourcePrefab, transform);
    }

    public void PlaySFX(AudioClip clip, float volumen = 1f, bool loop = false)
    {
        if (clip == null) return;

        AudioSource source = audioPool[currentPoolIndex];
        source.clip = clip;
        source.volume = volumen;
        source.spatialBlend = 0f; 
        source.loop = loop;
        source.Play();

        currentPoolIndex = (currentPoolIndex + 1) % poolSize;
    }

    public void PlayBotonUI(float volumen = 0.5f)
    {
        if (sonidoBotonUI == null) return;

        AudioSource source = audioPool[currentPoolIndex];
        source.clip = sonidoBotonUI;
        source.volume = volumen;
        source.spatialBlend = 0f; 
        source.loop = false;
        source.Play();

        currentPoolIndex = (currentPoolIndex + 1) % poolSize;
    }

    public void PlaySFXAtPosition(AudioClip clip, Vector3 posicion, float volumen = 1f)
    {
        if (clip == null) return;

        AudioSource source = audioPool[currentPoolIndex];
        source.transform.position = posicion;
        source.clip = clip;
        source.volume = volumen;
        source.spatialBlend = 1f; 
        source.minDistance = 1f; 
        source.maxDistance = 5f; 
        source.loop = false;
        source.Play();

        currentPoolIndex = (currentPoolIndex + 1) % poolSize;
    }

    public void PlayCalderoLoop(Vector3 posicion, float volumen = 0.5f)
    {
        if (calderoBurbujeoNormal == null) return;

        calderoAmbientalSource.transform.position = posicion;
        calderoAmbientalSource.clip = calderoBurbujeoNormal;
        calderoAmbientalSource.volume = volumen;
        calderoAmbientalSource.spatialBlend = 1f; 
        calderoAmbientalSource.loop = true;       
        calderoAmbientalSource.minDistance = 0.5f;
        calderoAmbientalSource.maxDistance = 2f;  
        calderoAmbientalSource.Play();
    }

    public void PlayAmbientMusic(AudioClip clip, float volumen = 0.5f)
    {
        if (clip == null) return;
        
        AudioSource musicSource = audioPool[0]; 
        musicSource.clip = clip;
        musicSource.volume = volumen;
        musicSource.spatialBlend = 0f;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void DetenerTodoElAudio()
    {
        foreach (AudioSource source in audioPool)
        {
            source.Stop();
        }
        if (calderoAmbientalSource.isPlaying)
        {
            calderoAmbientalSource.Stop();
        }
    }
}