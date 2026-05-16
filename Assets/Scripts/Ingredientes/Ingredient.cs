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
        if (args.interactorObject is XRSocketInteractor)
        {
            return; 
        }

        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlaySFXAtPosition(SFXManager.Instance.agarrarObjeto, transform.position, 0.7f);
        }

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
            GameObject nuevo = Instantiate(ingredientPrefab, initialPosition, initialRotation);
            
            nuevo.name = ingredientPrefab.name;

            Ingredient scriptNuevo = nuevo.GetComponent<Ingredient>();
            if(scriptNuevo != null)
            {
                scriptNuevo.hasRespawned = false; 
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