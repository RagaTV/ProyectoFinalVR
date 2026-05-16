using UnityEngine;

public class StartDayButton : MonoBehaviour
{
    [Header("Referencias del Sistema")]
    [SerializeField] private DayNightCycle dayNightCycle;
    [SerializeField] private BoardController boardController;

    public void PresionarBotonIniciarDia()
    {
        if (dayNightCycle != null && dayNightCycle.jornadaActiva)
        {
            Debug.LogWarning("La jornada ya está activa. No puedes iniciarla otra vez.");
            return;
        }

        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayBotonUI(0.6f);
        }

        if (dayNightCycle != null)
        {
            dayNightCycle.IniciarNuevaJornada();
        }

        if (boardController != null)
        {
            boardController.ResetearContadorClientes();
            boardController.SolicitarNuevaMision();
        }
    }

}