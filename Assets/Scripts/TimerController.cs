using UnityEngine;
using UnityEngine.UI;
using TMPro; // Make sure you have TextMeshPro installed and included

public class TimerController : MonoBehaviour
{
    [Header("Timer Settings")]
    public float timerDuration = 60f; // Duration of the timer in seconds

    [Header("UI References")]
    public Slider timerSlider; // Reference to the UI slider
    public TextMeshProUGUI timerText; // Reference to the TextMeshProUGUI text
	public Color normalTextColor;
	public Color lowTimeTextColor;

	[Header("Other References")]
	[SerializeField] private GameManager gM;
	[SerializeField] private MenuManager menuManager;

    private float timeRemaining;
    private bool timerRunning = false;

    private void Start()
    {
        // Initialize the timer
        // ResetTimer();
        // StartTimer();
    }

    private void Update()
    {
        if (timerRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerUI();
            }
            else
            {
                timeRemaining = 0;
                timerRunning = false;
                UpdateTimerUI();
                OnTimerEnd();
            }
        }
    }

    // Resets the timer to the full duration
    public void ResetTimer()
    {
        timeRemaining = timerDuration;
        UpdateTimerUI();
    }

    // Starts the timer
    public void StartTimer()
    {
        timerRunning = true;
    }

    // Stops the timer
    public void StopTimer()
    {
        timerRunning = false;
    }

	// Adds time to the timer
	public void AddTime(float timeToAdd)
	{
		if (!timerRunning) return;
		timeRemaining += timeToAdd;
		if (timeRemaining > timerDuration) timerDuration = timeRemaining;
	}

    // Updates the slider and text UI elements
    private void UpdateTimerUI()
    {
        if (timerSlider != null)
        {
            timerSlider.value = timeRemaining / timerDuration;
        }

        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60f);
            timerText.text = $"{minutes:00}:{seconds:00}";

			if (timeRemaining > 10)
				timerText.color = normalTextColor;
			else
				timerText.color = lowTimeTextColor;
        }
    }

    // Event triggered when the timer ends
    private void OnTimerEnd()
    {
        Debug.Log("Timer has ended!");
        menuManager.ShowFreeplayModeOverPanel();
    }
}
