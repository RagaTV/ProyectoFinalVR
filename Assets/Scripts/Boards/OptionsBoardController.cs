using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;

public class OptionsBoardController : MonoBehaviour
{
    [Header("Referencias de Textos Interactivos")]
    public TextMeshProUGUI textoVolumen;
    public TextMeshProUGUI textoAntialiasingActDes;
    public TextMeshProUGUI textoBorrar;

    [Header("Jugador (Para la altura)")]
    public Transform cameraOffset; 

    // Variables internas
    private float volumenActual = 1f;
    private float alturaOffset = 0f;
    private bool aaActivado = true;
    private bool esperandoConfirmacion = false;

    void Start()
    {
        CargarAjustes();
        ActualizarTextos();
    }

    public void SubirAltura()
    {
        if (cameraOffset != null)
        {
            alturaOffset += 0.05f; 
            alturaOffset = Mathf.Clamp(alturaOffset, -0.5f, 1.5f); 
            
            AplicarAltura();
            SFXManager.Instance.PlaySFX(SFXManager.Instance.aciertoReceta, 0.5f); 
        }
    }

    public void BajarAltura()
    {
        if (cameraOffset != null)
        {
            alturaOffset -= 0.05f; 
            alturaOffset = Mathf.Clamp(alturaOffset, -0.5f, 1.5f); 
            
            AplicarAltura();
            SFXManager.Instance.PlaySFX(SFXManager.Instance.aciertoReceta, 0.5f);
        }
    }

    private void AplicarAltura()
    {
        Vector3 pos = cameraOffset.localPosition;
        pos.y = alturaOffset;
        cameraOffset.localPosition = pos;
        Debug.Log("<color=cyan>[SISTEMA]</color> Altura ajustada a: " + alturaOffset.ToString("F2") + " metros.");
        
        PlayerPrefs.SetFloat("AjusteAltura", alturaOffset);
        PlayerPrefs.Save();
    }

    public void SubirVolumen()
    {
        volumenActual += 0.1f;
        if (volumenActual > 1f) volumenActual = 1f;
        AplicarVolumen();
    }

    public void BajarVolumen()
    {
        volumenActual -= 0.1f;
        if (volumenActual < 0f) volumenActual = 0f;
        AplicarVolumen();
    }

    private void AplicarVolumen()
    {
        AudioListener.volume = volumenActual; 
        PlayerPrefs.SetFloat("VolumenMaestro", volumenActual);
        PlayerPrefs.Save();
        
        ActualizarTextos();
        SFXManager.Instance.PlaySFX(SFXManager.Instance.aciertoReceta, 0.5f);
    }

    public void ToggleAntialiasing()
    {
        aaActivado = !aaActivado;
        
        if (aaActivado) QualitySettings.antiAliasing = 4; 
        else QualitySettings.antiAliasing = 0;           

        PlayerPrefs.SetInt("AA_Activado", aaActivado ? 1 : 0);
        PlayerPrefs.Save();

        ActualizarTextos();
        SFXManager.Instance.PlaySFX(SFXManager.Instance.aciertoReceta, 0.5f);
    }

    public void BotonBorrarProgreso()
    {
        if (!esperandoConfirmacion)
        {
            esperandoConfirmacion = true;
            textoBorrar.text = "<color=red>¿Estás Seguro?</color>";
            textoBorrar.color = Color.red;
            SFXManager.Instance.PlaySFX(SFXManager.Instance.errorIngrediente, 0.5f);
            
            Invoke("CancelarBorrado", 4f); 
        }
        else
        {
            if (SaveManager.Instance != null)
            {
                string ruta = SaveManager.Instance.ObtenerRutaArchivo();
                if (File.Exists(ruta))
                {
                    File.Delete(ruta);
                }
                
                SaveManager.Instance.datosActuales = new DatosGuardado(); 
            }

            Debug.Log("<color=red>[SISTEMA]</color> Partida borrada. Reiniciando juego...");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
        }
    }

    private void CancelarBorrado()
    {
        esperandoConfirmacion = false;
        textoBorrar.text = "Borrar\nProgreso";
        textoBorrar.color = Color.white;
    }

    public void BotonMenuPrincipal()
    {
 
        Debug.Log("<color=cyan>[SISTEMA]</color> Volviendo al Menú Principal...");
        SceneManager.LoadScene("MainMenu");
    }

    private void CargarAjustes()
    {
        alturaOffset = PlayerPrefs.GetFloat("AjusteAltura", 0f);
        if (cameraOffset != null) AplicarAltura();

        volumenActual = PlayerPrefs.GetFloat("VolumenMaestro", 1f);
        AudioListener.volume = volumenActual;

        aaActivado = PlayerPrefs.GetInt("AA_Activado", 1) == 1;
        QualitySettings.antiAliasing = aaActivado ? 4 : 0;
    }

    private void ActualizarTextos()
    {
        textoVolumen.text = $"Volumen General: {Mathf.RoundToInt(volumenActual * 100)}%"; 
        
        if (aaActivado) textoAntialiasingActDes.text = "Desactivar";
        else textoAntialiasingActDes.text = "Activar";
    }
}