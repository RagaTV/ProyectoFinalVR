using UnityEngine;
using TMPro;
using System.Collections;

[System.Serializable]
public class FraseConAudio {
    [TextArea(2, 5)]
    public string textoFrase;
    public AudioClip audioFrase;
}

[System.Serializable]
public class DialogoGato {
    public string categoria; 
    public FraseConAudio[] frasesYAudios;
}

[RequireComponent(typeof(AudioSource))]
public class CatDialogManager : MonoBehaviour {
    [Header("Configuracion Visual")]
    public TextMeshProUGUI textoDialogo; 
    public GameObject contenedorDialogo; 

    [Header("Base de Datos de Frases")]
    public DialogoGato[] baseDeDatos;

    [Header("Ajustes de Efecto")]
    public float tiempoVisibleDespuesDeEscribir = 2.5f;

    private AudioSource miAudioSource;
    [SerializeField] private AudioSource audioSourcePrefab;

    void Start() {
        miAudioSource = GetComponent<AudioSource>();

        if(contenedorDialogo != null) {
            contenedorDialogo.SetActive(false);
        }

        // Valores por defecto para el bucle de diálogos aleatorios
        float retrasoAleatorioInicial = 20f; 
        bool jugarSaludoInicial = true;

        // Comprobamos el día actual mediante el SaveManager
        if (SaveManager.Instance != null && SaveManager.Instance.datosActuales != null) {
            int diaActual = SaveManager.Instance.datosActuales.diaActual;

            // Verificamos si la jornada NO está en progreso mediante el GameManager
            bool jornadaInactiva = GameManager.Instance != null && !GameManager.Instance.misionEnProgreso;

            if (diaActual < 3) {
                jugarSaludoInicial = false;         // Bloqueado el "Inicio" estándar en día 1 y 2
                retrasoAleatorioInicial = 240f;     // 4 minutos expresados en segundos para los aleatorios

                // ------
                if (jornadaInactiva)
                {
                    if (diaActual == 1)
                    {
                        // "NuevaPartida" en el Inspector del gato
                        DecirFrase("NuevaPartida"); 
                        Debug.Log("<color=cyan>[GATO]</color> Activado diálogo introductorio de Nueva Partida (Día 1).");
                    }
                    else if (diaActual == 2)
                    {
                        // "SegundoDia" en el Inspector del gato
                        DecirFrase("SegundoDia"); 
                        Debug.Log("<color=cyan>[GATO]</color> Activado diálogo introductorio del Segundo Día (Día 2).");
                    }
                }
            }
            else {
                Debug.Log($"<color=orange>[GATO]</color> Modo Normal Activo (Día {diaActual}).");
            }
        }

        // Ejecutar saludo de Inicio estándar solo del día 3 en adelante
        if (jugarSaludoInicial) {
            DecirFrase("Inicio");
        }

        // Registramos el bucle repetitivo usando el tiempo de retraso calculado
        InvokeRepeating("LanzarDialogoAleatorio", retrasoAleatorioInicial, 60f);
    }

    public void DecirFrase(string categoria) {
        foreach (var grupo in baseDeDatos) {
            if (grupo.categoria.ToLower() == categoria.ToLower()) {
                if (grupo.frasesYAudios.Length > 0) {
                    int indiceAleatorio = Random.Range(0, grupo.frasesYAudios.Length);
                    FraseConAudio seleccion = grupo.frasesYAudios[indiceAleatorio];
                    
                    StopAllCoroutines();
                    StartCoroutine(MostrarTextoYSonido(seleccion));
                }
                return;
            }
        }
    }

    private void LanzarDialogoAleatorio() {
        if (contenedorDialogo != null && !contenedorDialogo.activeSelf) {
            DecirFrase("Aleatorio");
        }
    }

    IEnumerator MostrarTextoYSonido(FraseConAudio elemento) {
        textoDialogo.text = elemento.textoFrase;
        textoDialogo.maxVisibleCharacters = 0; 
        contenedorDialogo.SetActive(true);

        float velocidadEscrituraDinamica = 0.05f; 

        if (elemento.audioFrase != null && miAudioSource != null) {
            miAudioSource.clip = elemento.audioFrase;
            miAudioSource.Play();

            if (elemento.textoFrase.Length > 0) {
                velocidadEscrituraDinamica = (elemento.audioFrase.length / elemento.textoFrase.Length) / 2f;
            }
        }

        textoDialogo.ForceMeshUpdate(); 
        TMP_TextInfo textInfo = textoDialogo.textInfo;
        int totalPaginas = textInfo.pageCount;

        for (int p = 0; p < totalPaginas; p++) {
            textoDialogo.pageToDisplay = p + 1; 

            TMP_PageInfo paginaActual = textInfo.pageInfo[p];
            int primerCaracter = paginaActual.firstCharacterIndex;
            int ultimoCaracter = paginaActual.lastCharacterIndex;

            textoDialogo.maxVisibleCharacters = primerCaracter;

            for (int i = primerCaracter; i <= ultimoCaracter; i++) {
                textoDialogo.maxVisibleCharacters++;
                yield return new WaitForSeconds(velocidadEscrituraDinamica);
            }

            if (p < totalPaginas - 1) {
                yield return new WaitForSeconds(0.5f); 
            }
        }

        if (miAudioSource.isPlaying) {
            yield return new WaitWhile(() => miAudioSource.isPlaying);
        }

        yield return new WaitForSeconds(tiempoVisibleDespuesDeEscribir);
        contenedorDialogo.SetActive(false);
    }
}