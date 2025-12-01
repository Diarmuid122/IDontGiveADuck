using TMPro;
using UnityEngine;

public class TimeText : MonoBehaviour
{
    public static TimeText Instance { get; private set; }
    public TextMeshPro floatingTime;
    public int duckPenalty;


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
        if (floatingTime != null)
        {
            
            floatingTime.text = "-" + duckPenalty + " sec";
            Invoke("DestroyText", 0.5f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DestroyText()
    {
        Destroy(gameObject);
    }
}
