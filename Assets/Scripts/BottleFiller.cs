using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class BottleFiller : MonoBehaviour
{
    public XRSocketInteractor socketLlenado;
    public XRSocketInteractor socketEntrega; 
    public MeshRenderer liquidoCaldero; 

    public Color colorAguaOriginal = new Color(0.2f, 0.5f, 1f, 0.5f); 


    public void HabilitarLlenado()
    {
        if (socketLlenado != null)
        {
            socketLlenado.socketActive = true;
            Debug.Log("<color=#00FF00>[SISTEMA]</color> ¡Poción lista! Socket de botella activado.");
        }
    }

    public void AlPonerBotella(SelectEnterEventArgs args)
    {
        GameObject botella = args.interactableObject.transform.gameObject;
        StartCoroutine(SecuenciaLlenado(botella));
    }

    IEnumerator SecuenciaLlenado(GameObject botella)
    {
        MeshRenderer rendererBotella = botella.GetComponent<MeshRenderer>();
        Color colorObjetivo = liquidoCaldero.material.color;
        Color colorInicial = rendererBotella.material.color;

        float tiempo = 0;
        float duracion = 2.0f;
        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            Color nuevoColor = Color.Lerp(colorInicial, colorObjetivo, tiempo / duracion);
            nuevoColor.a = 1.0f; 
            rendererBotella.material.color = nuevoColor;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        IXRSelectInteractable interactable = botella.GetComponent<IXRSelectInteractable>();

        if (interactable != null)
        {
            // 2. Forzamos la salida del primer socket
            socketLlenado.interactionManager.SelectExit(socketLlenado, interactable);
            socketLlenado.socketActive = false;
            yield return new WaitForSeconds(0.5f);
            // 3. Forzamos la entrada al segundo socket
            socketEntrega.socketActive = true;
            socketEntrega.interactionManager.SelectEnter(socketEntrega, interactable);
        }

        Debug.Log("Botella transportada por código.");

        ResetearCaldero();

        FindObjectOfType<BoardController>().AsignarNuevaMision();
    }

    void ResetearCaldero()
    {
        Cauldron caldero = FindObjectOfType<Cauldron>();
        if (caldero != null)
        {
            caldero.ingredientesEnElLíquido.Clear();
            liquidoCaldero.material.color = colorAguaOriginal;
            socketLlenado.socketActive = false;
            Debug.Log("<color=#6495ED>[CALDERO]</color> Caldero reseteado a color base y lista limpia.");
        }
    }
}