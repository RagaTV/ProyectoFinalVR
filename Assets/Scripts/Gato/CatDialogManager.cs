using UnityEngine;
using TMPro;
using System.Collections;

// Esta clase DEBE estar fuera de la clase principal para que Unity la reconozca globalmente
[System.Serializable]
public class DialogoGato {
    public string categoria; 
    [TextArea(3, 10)]
    public string[] frases;
}

public class CatDialogManager : MonoBehaviour {
    [Header("Configuracion Visual")]
    public TextMeshProUGUI textoDialogo; 
    public GameObject contenedorDialogo; 

    [Header("Base de Datos de Frases")]
    public DialogoGato[] baseDeDatos;

    [Header("Ajustes de Efecto")]
    public float velocidadEscritura = 0.05f; 
    public float tiempoVisibleDespuesDeEscribir = 2.5f;

    void Start() {
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
                if (grupo.frases.Length > 0) {
                    string fraseAleatoria = grupo.frases[Random.Range(0, grupo.frases.Length)];
                    StopAllCoroutines();
                    StartCoroutine(MostrarTexto(fraseAleatoria));
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

    IEnumerator MostrarTexto(string fraseCompleta) {
        textoDialogo.text = ""; // Limpia el texto anterior
        contenedorDialogo.SetActive(true);

        // Efecto letra por letra
        foreach (char letra in fraseCompleta.ToCharArray()) {
            textoDialogo.text += letra;
            yield return new WaitForSeconds(velocidadEscritura);
        }

        // Espera antes de cerrar el bocadillo
        yield return new WaitForSeconds(tiempoVisibleDespuesDeEscribir);
        contenedorDialogo.SetActive(false);
    }

}