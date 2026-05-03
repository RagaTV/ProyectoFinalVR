using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class Ingredient : MonoBehaviour
{
    [Header("Configuración de Datos")]
    public IngredientData data; 
    
    [Header("Configuración de Prefabs")]
    public GameObject ingredientPrefab;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private bool hasRespawned = false; 
    private XRGrabInteractable grabInteractable;

    void Awake()
    {
        // IMPORTANTE: Guardamos la posición solo si NO somos un clon recién movido
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrab);
    }

    void OnDisable()
    {
        if (grabInteractable != null)
            grabInteractable.selectEntered.RemoveListener(OnGrab);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        // LA MAGIA DE VR: Verificamos quién está agarrando el objeto
        // Si es un Socket (el estante), abortamos y no hacemos nada
        if (args.interactorObject is XRSocketInteractor)
        {
            return; 
        }

        // Si pasamos el filtro anterior, significa que fue tu mano (Direct o Ray Interactor)
        if (!hasRespawned)
        {
            hasRespawned = true;
            StartCoroutine(RespawnSequence()); 
        }
        
        StopCoroutine("CleanupTimer"); 
    }

    IEnumerator RespawnSequence()
    {
        yield return new WaitForSeconds(2f);
        
        if (ingredientPrefab != null)
        {
            // 3. Spawneamos el nuevo objeto en la posición del estante
            GameObject nuevo = Instantiate(ingredientPrefab, initialPosition, initialRotation);
            
            // 4. Limpiamos el nombre para evitar (Clone)(Clone)
            nuevo.name = ingredientPrefab.name;

            // 5. Nos aseguramos de que el nuevo objeto sea "virgen"
            Ingredient scriptNuevo = nuevo.GetComponent<Ingredient>();
            if(scriptNuevo != null)
            {
                scriptNuevo.hasRespawned = false; // El nuevo sí puede spawnear cuando lo agarren
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Suelo"))
        {
            StartCoroutine("CleanupTimer");
        }
    }

    IEnumerator CleanupTimer()
    {
        yield return new WaitForSeconds(10f);
        if (!grabInteractable.isSelected)
        {
            Destroy(gameObject);
        }
    }
}