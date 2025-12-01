using TMPro;
using UnityEngine;

public class ScoreText : MonoBehaviour
{
    public static ScoreText Instance { get; private set; }
    public TextMeshPro floatingScore;
    public int duckScore;


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
        if (floatingScore != null)
        {
            floatingScore.text = "+" + duckScore;
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
    private void DisplayScore()
    {
    }
    
}
