using TMPro;
using UnityEngine;

public class DuckLufeAlert : MonoBehaviour
{
    public static DuckLufeAlert Instance { get; private set; }
    
    public bool alertActive;
    void Awake()
    {
        // Singleton pattern implementation
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep audio manager alive across scenes
        }
        else
        {
            // Destroy duplicate instances
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        alertActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (alertActive == false)
            DestroyAlert();
        
      
    }

    public void DestroyAlert()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
    }


}
