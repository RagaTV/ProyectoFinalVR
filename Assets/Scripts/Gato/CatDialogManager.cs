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

    void Start() {
        miAudioSource = GetComponent<AudioSource>();

        if(contenedorDialogo != null) {
            contenedorDialogo.SetActive(false);
        }

        float retrasoAleatorioInicial = 20f; 
        bool jugarSaludoInicial = true;

        if (SaveManager.Instance != null && SaveManager.Instance.datosActuales != null) {
            int diaActual = SaveManager.Instance.datosActuales.diaActual;

            bool jornadaInactiva = GameManager.Instance != null && !GameManager.Instance.misionEnProgreso;

            if (diaActual < 3) {
                jugarSaludoInicial = false; 
                retrasoAleatorioInicial = 240f;

                // ------
                if (jornadaInactiva)
                {
                    if (diaActual == 1)
                    {
                        
                        DecirFrase("NuevaPartida"); 
                        Debug.Log("<color=cyan>[GATO]</color> Activado diálogo introductorio de Nueva Partida (Día 1).");
                    }
                    else if (diaActual == 2)
                    {
                        
                        DecirFrase("SegundoDia"); 
                        Debug.Log("<color=cyan>[GATO]</color> Activado diálogo introductorio del Segundo Día (Día 2).");
                    }
                }
            }
            else {
                Debug.Log($"<color=orange>[GATO]</color> Modo Normal Activo (Día {diaActual}).");
            }
        }

        if (jugarSaludoInicial) {
            DecirFrase("Inicio");
        }

        InvokeRepeating("LanzarDialogoAleatorio", retrasoAleatorioInicial, 60f);
        StartCoroutine(BucleMaullidosAleatorios());
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

    public void DetenerDialogoActivo() 
    {
        StopAllCoroutines();
        
        if (miAudioSource != null && miAudioSource.isPlaying) {
            miAudioSource.Stop(); 
        }
        
        if (contenedorDialogo != null) {
            contenedorDialogo.SetActive(false);
        }
        
        Debug.Log("<color=yellow>[GATO]</color> Diálogo interrumpido por el jugador.");
    }

    IEnumerator BucleMaullidosAleatorios()
    {
        yield return new WaitForSeconds(15f);

        while (true)
        {
            float tiempoEsperaAleatorio = Random.Range(30f, 75f);
            yield return new WaitForSeconds(tiempoEsperaAleatorio);

            if (SFXManager.Instance == null || SFXManager.Instance.maullidoGato == null) 
                continue;

            bool esDobleMaullido = Random.value > 0.5f;

            SFXManager.Instance.PlaySFXAtPosition(SFXManager.Instance.maullidoGato, transform.position, 0.6f);
            Debug.Log("<color=orange>[GATO]</color> *Miau* ambiental.");

            if (esDobleMaullido)
            {
                yield return new WaitForSeconds(0.5f);

                SFXManager.Instance.PlaySFXAtPosition(SFXManager.Instance.maullidoGato, transform.position, 0.5f);
                Debug.Log("<color=orange>[GATO]</color> *Miau miau* ¡Soltó el doble maullido consecutivo!");
            }
        }
    }
}