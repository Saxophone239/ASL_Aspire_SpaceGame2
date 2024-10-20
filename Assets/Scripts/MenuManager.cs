using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
	[SerializeField] private GameObject mainMenuPanel;
	[SerializeField] private GameObject classroomModeOverPanel;
	[SerializeField] private GameObject freeplayModeOverPanel;
	[SerializeField] private GameObject pauseMenuPanel;

	[SerializeField] private GameManager gameManager;

	private void Start()
	{
		Time.timeScale = 1.0f;

		mainMenuPanel.SetActive(true);
		classroomModeOverPanel.SetActive(false);
		freeplayModeOverPanel.SetActive(false);
		pauseMenuPanel.SetActive(false);
	}

    public void OnBackToMapButtonClick()
	{
		// TODO: change scene back to main map
		Debug.Log("We're back to our main map");
	}

	public void OnClassroomModePlayButtonClick()
	{
		// Set up 5 rounds and classroom mode on GameManager
		gameManager.StartGameClassroomMode();
		mainMenuPanel.SetActive(false);
	}

	public void OnFreeplayModePlayButtonClick()
	{
		// TODO: set up infinite rounds, the timer, and freeplay mode on GameManager
		gameManager.StartGameFreeplayMode();
		mainMenuPanel.SetActive(false);
	}

	public void OnRestartButtonClick()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void OnPauseButtonClick()
	{
		Time.timeScale = 0.0f;
		pauseMenuPanel.SetActive(true);
	}

	public void OnResumeGameButtonClick()
	{
		Time.timeScale = 1.0f;
		pauseMenuPanel.SetActive(false);
	}

	public void ShowClassroomModeOverPanel()
	{
		Time.timeScale = 0.0f;
		classroomModeOverPanel.SetActive(true);
	}

	public void ShowFreeplayModeOverPanel()
	{
		Time.timeScale = 0.0f;
		freeplayModeOverPanel.SetActive(true);
	}
}
