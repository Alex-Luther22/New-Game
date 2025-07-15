using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;
    
    [Header("Tutorial Settings")]
    public bool showTutorial = true;
    public List<TutorialStep> tutorialSteps = new List<TutorialStep>();
    public int currentStepIndex = 0;
    
    [Header("UI Elements")]
    public GameObject tutorialPanel;
    public Text tutorialText;
    public Text stepCounter;
    public Button nextButton;
    public Button prevButton;
    public Button skipButton;
    public GameObject highlightOverlay;
    public GameObject arrow;
    
    [Header("Highlight Settings")]
    public Material highlightMaterial;
    public Color highlightColor = Color.yellow;
    public float highlightPulseSpeed = 2f;
    
    [Header("Audio")]
    public AudioClip tutorialSound;
    public AudioClip completeSound;
    
    private bool isTutorialActive = false;
    private bool isStepCompleted = false;
    private GameObject currentHighlightedObject;
    private TutorialStep currentStep;
    
    // Eventos
    public System.Action<int> OnTutorialStepChanged;
    public System.Action OnTutorialCompleted;
    public System.Action OnTutorialSkipped;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        SetupTutorial();
        
        if (showTutorial && ShouldShowTutorial())
        {
            StartTutorial();
        }
    }
    
    void SetupTutorial()
    {
        CreateTutorialSteps();
        
        // Configurar botones
        nextButton.onClick.AddListener(NextStep);
        prevButton.onClick.AddListener(PreviousStep);
        skipButton.onClick.AddListener(SkipTutorial);
        
        // Ocultar panel inicialmente
        tutorialPanel.SetActive(false);
        highlightOverlay.SetActive(false);
        arrow.SetActive(false);
    }
    
    void CreateTutorialSteps()
    {
        // Paso 1: Bienvenida
        tutorialSteps.Add(new TutorialStep
        {
            title = "¡Bienvenido a Football Master!",
            description = "Aprende los controles básicos para dominar el campo.",
            targetObject = null,
            action = TutorialAction.ShowMessage,
            isCompleted = false
        });
        
        // Paso 2: Movimiento
        tutorialSteps.Add(new TutorialStep
        {
            title = "Movimiento del Jugador",
            description = "Desliza tu dedo en la pantalla para mover al jugador.",
            targetObject = null,
            action = TutorialAction.WaitForInput,
            inputType = InputType.Movement,
            isCompleted = false
        });
        
        // Paso 3: Pase
        tutorialSteps.Add(new TutorialStep
        {
            title = "Realizar Pases",
            description = "Toca la pantalla para hacer un pase corto.",
            targetObject = null,
            action = TutorialAction.WaitForInput,
            inputType = InputType.Pass,
            isCompleted = false
        });
        
        // Paso 4: Disparo
        tutorialSteps.Add(new TutorialStep
        {
            title = "Disparar al Arco",
            description = "Desliza hacia la portería para disparar.",
            targetObject = null,
            action = TutorialAction.WaitForInput,
            inputType = InputType.Shoot,
            isCompleted = false
        });
        
        // Paso 5: Trucos
        tutorialSteps.Add(new TutorialStep
        {
            title = "Trucos y Regates",
            description = "Dibuja patrones en la pantalla para hacer trucos.",
            targetObject = null,
            action = TutorialAction.WaitForInput,
            inputType = InputType.Trick,
            isCompleted = false
        });
        
        // Paso 6: Completado
        tutorialSteps.Add(new TutorialStep
        {
            title = "¡Tutorial Completado!",
            description = "¡Excelente! Ya dominas los controles básicos. ¡Hora de jugar!",
            targetObject = null,
            action = TutorialAction.Complete,
            isCompleted = false
        });
    }
    
    bool ShouldShowTutorial()
    {
        // Verificar si es la primera vez que juega
        return !PlayerPrefs.HasKey("TutorialCompleted");
    }
    
    public void StartTutorial()
    {
        if (isTutorialActive) return;
        
        isTutorialActive = true;
        currentStepIndex = 0;
        
        tutorialPanel.SetActive(true);
        
        // Pausar el juego
        Time.timeScale = 0f;
        
        ShowCurrentStep();
        
        AudioManager.Instance?.PlaySFX(tutorialSound);
    }
    
    void ShowCurrentStep()
    {
        if (currentStepIndex >= tutorialSteps.Count) return;
        
        currentStep = tutorialSteps[currentStepIndex];
        
        // Actualizar UI
        tutorialText.text = currentStep.description;
        stepCounter.text = $"{currentStepIndex + 1}/{tutorialSteps.Count}";
        
        // Configurar botones
        prevButton.interactable = currentStepIndex > 0;
        nextButton.interactable = currentStep.action == TutorialAction.ShowMessage;
        
        // Highlight objeto si existe
        if (currentStep.targetObject != null)
        {
            HighlightObject(currentStep.targetObject);
        }
        else
        {
            ClearHighlight();
        }
        
        // Mostrar flecha si es necesario
        if (currentStep.arrowPosition != Vector3.zero)
        {
            ShowArrow(currentStep.arrowPosition);
        }
        else
        {
            arrow.SetActive(false);
        }
        
        // Ejecutar acción del paso
        ExecuteStepAction(currentStep);
        
        // Disparar evento
        OnTutorialStepChanged?.Invoke(currentStepIndex);
    }
    
    void ExecuteStepAction(TutorialStep step)
    {
        switch (step.action)
        {
            case TutorialAction.ShowMessage:
                // Solo mostrar mensaje
                break;
                
            case TutorialAction.WaitForInput:
                // Esperar input específico
                StartCoroutine(WaitForSpecificInput(step.inputType));
                break;
                
            case TutorialAction.HighlightUI:
                // Resaltar elemento de UI
                if (step.targetObject != null)
                {
                    HighlightUIElement(step.targetObject);
                }
                break;
                
            case TutorialAction.Complete:
                // Completar tutorial
                nextButton.GetComponentInChildren<Text>().text = "¡Empezar!";
                nextButton.interactable = true;
                break;
        }
    }
    
    IEnumerator WaitForSpecificInput(InputType inputType)
    {
        nextButton.interactable = false;
        
        while (!isStepCompleted)
        {
            switch (inputType)
            {
                case InputType.Movement:
                    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
                    {
                        CompleteCurrentStep();
                    }
                    break;
                    
                case InputType.Pass:
                    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
                    {
                        CompleteCurrentStep();
                    }
                    break;
                    
                case InputType.Shoot:
                    if (Input.touchCount > 0)
                    {
                        Touch touch = Input.GetTouch(0);
                        if (touch.phase == TouchPhase.Ended && touch.deltaPosition.magnitude > 50f)
                        {
                            CompleteCurrentStep();
                        }
                    }
                    break;
                    
                case InputType.Trick:
                    // Detectar patrón de truco
                    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
                    {
                        // Simulación de detección de truco
                        if (Random.Range(0, 100) < 10) // 10% de probabilidad por frame
                        {
                            CompleteCurrentStep();
                        }
                    }
                    break;
            }
            
            yield return null;
        }
    }
    
    void CompleteCurrentStep()
    {
        isStepCompleted = true;
        currentStep.isCompleted = true;
        
        // Activar botón siguiente
        nextButton.interactable = true;
        
        // Efecto visual de completado
        if (EffectsManager.Instance != null)
        {
            EffectsManager.Instance.PlayAchievementEffect(tutorialPanel.transform.position);
        }
        
        AudioManager.Instance?.PlaySFX(completeSound);
    }
    
    public void NextStep()
    {
        if (currentStepIndex < tutorialSteps.Count - 1)
        {
            currentStepIndex++;
            isStepCompleted = false;
            ShowCurrentStep();
        }
        else
        {
            CompleteTutorial();
        }
    }
    
    public void PreviousStep()
    {
        if (currentStepIndex > 0)
        {
            currentStepIndex--;
            isStepCompleted = false;
            ShowCurrentStep();
        }
    }
    
    public void SkipTutorial()
    {
        OnTutorialSkipped?.Invoke();
        CloseTutorial();
    }
    
    void CompleteTutorial()
    {
        // Marcar tutorial como completado
        PlayerPrefs.SetString("TutorialCompleted", "true");
        
        // Dar recompensa
        if (GameData.Instance?.playerProfile != null)
        {
            GameData.Instance.playerProfile.AddCoins(500);
            GameData.Instance.playerProfile.AddXP(100);
        }
        
        OnTutorialCompleted?.Invoke();
        CloseTutorial();
    }
    
    void CloseTutorial()
    {
        isTutorialActive = false;
        tutorialPanel.SetActive(false);
        ClearHighlight();
        arrow.SetActive(false);
        
        // Reanudar el juego
        Time.timeScale = 1f;
    }
    
    void HighlightObject(GameObject obj)
    {
        ClearHighlight();
        currentHighlightedObject = obj;
        
        if (obj != null)
        {
            // Agregar efecto de resaltado
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                // Crear material de resaltado
                Material[] materials = renderer.materials;
                Material[] newMaterials = new Material[materials.Length + 1];
                
                for (int i = 0; i < materials.Length; i++)
                {
                    newMaterials[i] = materials[i];
                }
                
                newMaterials[materials.Length] = highlightMaterial;
                renderer.materials = newMaterials;
            }
            
            // Activar overlay
            highlightOverlay.SetActive(true);
            
            // Animar pulsación
            StartCoroutine(PulseHighlight(obj));
        }
    }
    
    void HighlightUIElement(GameObject uiElement)
    {
        // Resaltar elemento de UI
        Image image = uiElement.GetComponent<Image>();
        if (image != null)
        {
            StartCoroutine(PulseUIElement(image));
        }
    }
    
    IEnumerator PulseHighlight(GameObject obj)
    {
        while (currentHighlightedObject == obj)
        {
            float alpha = (Mathf.Sin(Time.unscaledTime * highlightPulseSpeed) + 1f) / 2f;
            Color color = highlightColor;
            color.a = alpha;
            
            // Aplicar color al material de resaltado
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null && renderer.materials.Length > 0)
            {
                Material highlightMat = renderer.materials[renderer.materials.Length - 1];
                if (highlightMat != null)
                {
                    highlightMat.color = color;
                }
            }
            
            yield return null;
        }
    }
    
    IEnumerator PulseUIElement(Image image)
    {
        Color originalColor = image.color;
        
        while (currentHighlightedObject == image.gameObject)
        {
            float alpha = (Mathf.Sin(Time.unscaledTime * highlightPulseSpeed) + 1f) / 2f;
            Color color = highlightColor;
            color.a = alpha;
            
            image.color = Color.Lerp(originalColor, color, alpha);
            
            yield return null;
        }
        
        image.color = originalColor;
    }
    
    void ClearHighlight()
    {
        if (currentHighlightedObject != null)
        {
            Renderer renderer = currentHighlightedObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                // Remover material de resaltado
                Material[] materials = renderer.materials;
                if (materials.Length > 1)
                {
                    Material[] newMaterials = new Material[materials.Length - 1];
                    for (int i = 0; i < newMaterials.Length; i++)
                    {
                        newMaterials[i] = materials[i];
                    }
                    renderer.materials = newMaterials;
                }
            }
            
            currentHighlightedObject = null;
        }
        
        highlightOverlay.SetActive(false);
    }
    
    void ShowArrow(Vector3 position)
    {
        arrow.SetActive(true);
        arrow.transform.position = position;
        
        // Animar flecha
        StartCoroutine(AnimateArrow());
    }
    
    IEnumerator AnimateArrow()
    {
        Vector3 originalPosition = arrow.transform.position;
        
        while (arrow.activeInHierarchy)
        {
            float offset = Mathf.Sin(Time.unscaledTime * 3f) * 0.1f;
            arrow.transform.position = originalPosition + Vector3.up * offset;
            yield return null;
        }
    }
    
    // Método para reiniciar tutorial
    public void ResetTutorial()
    {
        PlayerPrefs.DeleteKey("TutorialCompleted");
        
        foreach (TutorialStep step in tutorialSteps)
        {
            step.isCompleted = false;
        }
        
        currentStepIndex = 0;
        isStepCompleted = false;
        isTutorialActive = false;
        
        Debug.Log("Tutorial reset!");
    }
    
    // Método para verificar si el tutorial está activo
    public bool IsTutorialActive()
    {
        return isTutorialActive;
    }
    
    // Método para obtener progreso del tutorial
    public float GetTutorialProgress()
    {
        if (tutorialSteps.Count == 0) return 0f;
        
        int completedSteps = 0;
        foreach (TutorialStep step in tutorialSteps)
        {
            if (step.isCompleted) completedSteps++;
        }
        
        return (float)completedSteps / tutorialSteps.Count;
    }
    
    void OnDestroy()
    {
        // Limpiar
        ClearHighlight();
        StopAllCoroutines();
    }
}

[System.Serializable]
public class TutorialStep
{
    public string title;
    public string description;
    public GameObject targetObject;
    public Vector3 arrowPosition;
    public TutorialAction action;
    public InputType inputType;
    public bool isCompleted;
}

public enum TutorialAction
{
    ShowMessage,
    WaitForInput,
    HighlightUI,
    Complete
}

public enum InputType
{
    Movement,
    Pass,
    Shoot,
    Trick,
    Any
}