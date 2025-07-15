using UnityEngine;
using System.Collections.Generic;

public class TouchControlManager : MonoBehaviour
{
    [Header("Touch Settings")]
    public float touchSensitivity = 1.0f;
    public float swipeDeadZone = 50f;
    public float tapTimeThreshold = 0.2f;
    
    [Header("Player Control")]
    public PlayerController playerController;
    public BallController ballController;
    
    private Vector2 fingerStartPos;
    private Vector2 fingerEndPos;
    private float fingerDownTime;
    private bool isTouching = false;
    
    private Dictionary<int, Vector2> activeTouches = new Dictionary<int, Vector2>();
    
    void Update()
    {
        HandleTouchInput();
    }
    
    void HandleTouchInput()
    {
        // Manejo de múltiples toques
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    OnTouchBegan(touch);
                    break;
                    
                case TouchPhase.Moved:
                    OnTouchMoved(touch);
                    break;
                    
                case TouchPhase.Ended:
                    OnTouchEnded(touch);
                    break;
                    
                case TouchPhase.Canceled:
                    OnTouchCanceled(touch);
                    break;
            }
        }
        
        // Manejo de mouse para testing en editor
        if (Application.isEditor)
        {
            HandleMouseInput();
        }
    }
    
    void OnTouchBegan(Touch touch)
    {
        activeTouches[touch.fingerId] = touch.position;
        fingerStartPos = touch.position;
        fingerDownTime = Time.time;
        isTouching = true;
    }
    
    void OnTouchMoved(Touch touch)
    {
        if (activeTouches.ContainsKey(touch.fingerId))
        {
            Vector2 currentPos = touch.position;
            Vector2 startPos = activeTouches[touch.fingerId];
            Vector2 swipeDirection = (currentPos - startPos).normalized;
            float swipeDistance = Vector2.Distance(startPos, currentPos);
            
            // Actualizar movimiento del jugador
            if (swipeDistance > swipeDeadZone)
            {
                playerController.MovePlayer(swipeDirection);
            }
        }
    }
    
    void OnTouchEnded(Touch touch)
    {
        if (activeTouches.ContainsKey(touch.fingerId))
        {
            fingerEndPos = touch.position;
            float touchDuration = Time.time - fingerDownTime;
            
            Vector2 swipeVector = fingerEndPos - fingerStartPos;
            float swipeDistance = swipeVector.magnitude;
            
            // Detectar tipo de gesto
            if (touchDuration < tapTimeThreshold && swipeDistance < swipeDeadZone)
            {
                // Tap simple
                HandleTap();
            }
            else if (swipeDistance > swipeDeadZone)
            {
                // Swipe gesture
                HandleSwipe(swipeVector, touchDuration);
            }
            
            activeTouches.Remove(touch.fingerId);
        }
        
        isTouching = false;
    }
    
    void OnTouchCanceled(Touch touch)
    {
        if (activeTouches.ContainsKey(touch.fingerId))
        {
            activeTouches.Remove(touch.fingerId);
        }
        isTouching = false;
    }
    
    void HandleTap()
    {
        // Tap simple - pase corto o cambio de jugador
        playerController.PerformShortPass();
    }
    
    void HandleSwipe(Vector2 swipeVector, float duration)
    {
        Vector2 swipeDirection = swipeVector.normalized;
        float swipeSpeed = swipeVector.magnitude / duration;
        
        // Determinar tipo de truco basado en dirección y velocidad
        if (swipeSpeed > 1000f) // Swipe rápido
        {
            // Disparo
            HandleShoot(swipeDirection, swipeSpeed);
        }
        else
        {
            // Truco o movimiento especial
            HandleTrick(swipeDirection, swipeSpeed);
        }
    }
    
    void HandleShoot(Vector2 direction, float power)
    {
        float shootPower = Mathf.Clamp(power / 2000f, 0.1f, 1.0f);
        Vector3 shootDirection = new Vector3(direction.x, 0, direction.y).normalized;
        
        playerController.Shoot(shootDirection, shootPower);
    }
    
    void HandleTrick(Vector2 direction, float speed)
    {
        // Diferentes trucos basados en dirección
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            // Horizontal - regate lateral
            if (direction.x > 0)
                playerController.PerformTrick(TrickType.StepOverRight);
            else
                playerController.PerformTrick(TrickType.StepOverLeft);
        }
        else
        {
            // Vertical - túnel o ruleta
            if (direction.y > 0)
                playerController.PerformTrick(TrickType.Nutmeg);
            else
                playerController.PerformTrick(TrickType.Roulette);
        }
    }
    
    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            fingerStartPos = Input.mousePosition;
            fingerDownTime = Time.time;
            isTouching = true;
        }
        
        if (Input.GetMouseButtonUp(0) && isTouching)
        {
            fingerEndPos = Input.mousePosition;
            float touchDuration = Time.time - fingerDownTime;
            
            Vector2 swipeVector = fingerEndPos - fingerStartPos;
            float swipeDistance = swipeVector.magnitude;
            
            if (touchDuration < tapTimeThreshold && swipeDistance < swipeDeadZone)
            {
                HandleTap();
            }
            else if (swipeDistance > swipeDeadZone)
            {
                HandleSwipe(swipeVector, touchDuration);
            }
            
            isTouching = false;
        }
    }
    
    // Método para obtener la posición del toque en el mundo
    public Vector3 GetWorldTouchPosition(Vector2 screenPos)
    {
        Camera cam = Camera.main;
        Ray ray = cam.ScreenPointToRay(screenPos);
        
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.point;
        }
        
        return Vector3.zero;
    }
}