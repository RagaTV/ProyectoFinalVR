using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para recargar escenas

public class BotonEmergencia : MonoBehaviour
{
    public void ReiniciarInstanciaDia()
    {
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlaySFX(SFXManager.Instance.errorIngrediente, 1f);
        }

        string escenaActual = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(escenaActual);
    }
}