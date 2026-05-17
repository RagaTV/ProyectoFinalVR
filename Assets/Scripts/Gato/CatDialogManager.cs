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
            DecirFrase("Inicio");
        }
        
        // Lanza una frase aleatoria cada 60 segundos aproximadamente
        InvokeRepeating("LanzarDialogoAleatorio", 20f, 60f);
    }

    public void DecirFrase(string categoria) {
        foreach (var grupo in baseDeDatos) {
            if (grupo.categoria.ToLower() == categoria.ToLower()) {
                if (grupo.frasesYAudios.Length > 0) {
                    // Selecciona un índice aleatorio del grupo
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
        textoDialogo.text = ""; // Limpia el texto anterior
        contenedorDialogo.SetActive(true);

        float velocidadEscrituraDinamica = 0.05f; // Valor por defecto si no hay audio

        // Si hay un audio asignado, se reproduce y calculamos la velocidad exacta
        if (elemento.audioFrase != null && miAudioSource != null) {
            miAudioSource.clip = elemento.audioFrase;
            miAudioSource.Play();

            // Evitamos división por cero si la frase está vacía por error
            if (elemento.textoFrase.Length > 0) {
                velocidadEscrituraDinamica = elemento.audioFrase.length / elemento.textoFrase.Length;
            }
        }

        // Efecto letra por letra sincronizado con la duración del audio
        foreach (char letra in elemento.textoFrase.ToCharArray()) {
            textoDialogo.text += letra;
            yield return new WaitForSeconds(velocidadEscrituraDinamica);
        }

        // Si el audio aún se está reproduciendo por pequeños desfases de frames, esperamos a que termine
        if (miAudioSource.isPlaying) {
            yield return new WaitWhile(() => miAudioSource.isPlaying);
        }

        // Espera extra con el bocadillo completo antes de cerrarlo
        yield return new WaitForSeconds(tiempoVisibleDespuesDeEscribir);
        contenedorDialogo.SetActive(false);
    }
}