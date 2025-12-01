using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Abstract base class for all duck types
/// Provides common functionality and enforces consistent interface
/// </summary>
public abstract class BaseDuck : MonoBehaviour
{
    [Header("Base Duck Properties")]
    [SerializeField] protected int pointValue = 1;
    [SerializeField] protected float lifetime = 5f;
    [SerializeField] protected float moveSpeed = 0f; // For future moving ducks
    
    [Header("Visual Feedback")]
    [SerializeField] protected ParticleSystem destroyEffect;
    [SerializeField] protected AudioClip clickSound;
    [SerializeField] public GameObject duckLifeAlert;
    [SerializeField] private bool alarmActive = false;
    
    // Protected properties accessible to child classes
    protected float currentLifetime;
    protected bool isClicked = false;
    protected bool isInitialized = false;
    
    // Public properties for external access
    public int PointValue => pointValue;
    public bool IsClicked => isClicked;
    
    #region Unity Lifecycle
    
    protected virtual void Start()
    {
        Initialize();
        
        // Auto-fit collider to sprite bounds
        AutoFitCollider();
    }
    
    /// <summary>
    /// Automatically fit the BoxCollider2D to the sprite bounds
    /// </summary>
    private void AutoFitCollider()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        
        if (spriteRenderer != null && boxCollider != null && spriteRenderer.sprite != null)
        {
            // Set collider size to match sprite bounds exactly
            Vector2 spriteSize = spriteRenderer.sprite.bounds.size;
            boxCollider.size = spriteSize;
            
            // Center the collider
            boxCollider.offset = Vector2.zero;
            
            Debug.Log($"Auto-fitted collider for {gameObject.name}: size = {boxCollider.size}");
        }
        else
        {
            Debug.LogWarning($"Could not auto-fit collider for {gameObject.name} - missing components");
        }
    }
    
    /// <summary>
    /// Called every frame by Unity - this is where we handle all duck behaviour
    /// 
    /// Update() is one of Unity's most important methods:
    /// - Called once per frame (typically 60 times per second)
    /// - Used for continuous updates like movement, timers, input
    /// - Should be efficient since it runs so frequently
    /// </summary>
    protected virtual void Update()
    {
        // Safety check: Don't do anything if duck isn't properly set up yet
        // This prevents errors during the brief moment between object creation and initialization
        if (!isInitialized) return;
        
        // Handle duck lifetime countdown and expiration
        // This makes ducks disappear after their time is up
        HandleLifetime();
        
        // Handle duck movement (if any)
        // Currently not implemented but ready for future moving ducks
        HandleMovement();
        
        // Check if player has clicked on this duck
        // Uses Unity's new Input System for better input handling
        HandleClickDetection();
    }
    /// <summary>
    /// Handle mouse click detection using new Input System
    /// </summary>
    private void HandleClickDetection()
    {
        if (isClicked) return;
        
        // Check if mouse button was pressed this frame
        if (Mouse.current?.leftButton.wasPressedThisFrame == true)
        {
            // Cast ray from mouse position to check if we hit this duck
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            
            Collider2D hitCollider = Physics2D.OverlapPoint(worldPos);
            if (hitCollider != null && hitCollider.gameObject == gameObject)
            {
                isClicked = true;
                // Disable collider to prevent further clicks
                Collider2D col = GetComponent<Collider2D>();
                if (col != null) col.enabled = false;
                OnClicked();
            }
        }
    }
    
    // Keep OnMouseDown as backup for older Unity versions
    protected virtual void OnMouseDown()
    {
        if (!isClicked && isInitialized)
        {
            isClicked = true;
            // Disable collider to prevent further clicks
            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;
            OnClicked();
        }
    }
    
    #endregion
    
    #region Initialization
    
    /// <summary>
    /// Initialise duck with custom properties
    /// </summary>
    public virtual void Initialize(float customLifetime = -1, int customPointValue = -1)
    {
        currentLifetime = customLifetime > 0 ? customLifetime : lifetime;
        if (customPointValue > 0) pointValue = customPointValue;
        
        isInitialized = true;
        OnDuckSpawned();
    }
    
    #endregion
    
    #region Core Behaviors
    
    /// <summary>
    /// Handle duck lifetime countdown
    /// </summary>
    protected virtual void HandleLifetime()
    {
        currentLifetime -= Time.deltaTime;
        
        // Visual feedback as lifetime gets low
        if (currentLifetime <= 1f)
        {
            OnLifetimeLow();
        }
        
        if (currentLifetime <= 0 && !isClicked)
        {
            OnLifetimeExpired();
            DestroyDuck();
        }
    }
    
    /// <summary>
    /// Handle duck movement (override in child classes)
    /// </summary>
    protected virtual void HandleMovement()
    {
        // Base implementation - no movement
        // Override in child classes for moving ducks
    }
    
    /// <summary>
    /// Common destruction logic with effects
    /// </summary>
    protected virtual void DestroyDuck()
    {
        // Play destruction effects
        if (destroyEffect != null)
        {
            // Create effect at duck position
            ParticleSystem effect = Instantiate(destroyEffect, transform.position, transform.rotation);
            Destroy(effect.gameObject, effect.main.duration);
           
        }
        
        // Play sound effect
        if (clickSound != null)
        {
            AudioSource.PlayClipAtPoint(clickSound, transform.position);
        }
        if (duckLifeAlert != null)
        {
            //duckLifeAlert.GetComponent<DuckLufeAlert>().alertActive = false;
            //.Instance.DestroyAlert()
            duckLifeAlert.GetComponent<DuckLufeAlert>().DestroyAlert();
        }
        // Remove duck from scene
        Destroy(gameObject);
    }
    
    #endregion
    
    #region Abstract Methods - Must be implemented by child classes
    
    /// <summary>
    /// Handle duck click behaviour - specific to each duck type
    /// </summary>
    protected abstract void OnClicked();
    
    /// <summary>
    /// Handle what happens when duck lifetime expires naturally
    /// </summary>
    protected abstract void OnLifetimeExpired();
    
    #endregion
    
    #region Virtual Methods - Can be overridden by child classes
    
    /// <summary>
    /// Called when duck is first spawned
    /// </summary>
    protected virtual void OnDuckSpawned()
    {
        // Default implementation - can be overridden
        Debug.Log($"{GetType().Name} spawned at position {transform.position} with {currentLifetime}s lifetime");
    }

    /// <summary>
    /// Called when duck lifetime is getting low (< 1 second)
    /// </summary>
    protected virtual void OnLifetimeLow()
    {
        // Default implementation - visual warning
        // Override for custom low-lifetime effects
        if (duckLifeAlert != null) 
        {
            if (alarmActive == false)
            {
                Debug.Log("About to despawn");
                duckLifeAlert = Instantiate(duckLifeAlert, transform.position, Quaternion.identity);
                alarmActive = true;
            } 
        }

    }
    
    #endregion
    
    #region Debug Helpers
    
    protected virtual void OnDrawGizmos()
    {
        // Draw lifetime indicator in scene view
        if (Application.isPlaying && isInitialized)
        {
            float lifetimePercent = currentLifetime / lifetime;
            Gizmos.color = Color.Lerp(Color.red, Color.green, lifetimePercent);
            Gizmos.DrawWireSphere(transform.position + Vector3.up, 0.5f);
        }
    }
    
    #endregion
}