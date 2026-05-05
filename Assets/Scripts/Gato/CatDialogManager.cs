using UnityEngine;
using TMPro; // Asegúrate de tener instalado TextMeshPro
using System.Collections;

[System.Serializable]
public class DialogoGato {
    public string categoria; 
    [TextArea(3, 10)]
    public string[] frases;
}

public class CatDialogManager : MonoBehaviour {
    // Estas variables DEBEN ser 'public' para que aparezcan en el Inspector
    [Header("Configuracion Visual")]
    public TextMeshProUGUI textoDialogo; 
    public GameObject contenedorDialogo; 

    [Header("Base de Datos de Frases")]
    public DialogoGato[] baseDeDatos;

    [Header("Ajustes")]
    public float tiempoVisible = 4.5f;

    void Start() {
        if(contenedorDialogo != null) {
            contenedorDialogo.SetActive(false);
            DecirFrase("Inicio");
        }
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

    IEnumerator MostrarTexto(string texto) {
        textoDialogo.text = texto;
        contenedorDialogo.SetActive(true);
        yield return new WaitForSeconds(tiempoVisible);
        contenedorDialogo.SetActive(false);
    }
}