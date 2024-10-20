using System;
using System.Collections;
using System.Collections.Generic;
using Mehroz;
using UnityEngine;

public enum GameDifficulty
{
	Easy,
	Medium,
	Hard
}

/// <summary>
/// This will manage all possible stages a player can encounter:
/// - starting at 0, collect enough asteroids with positive integers to reach GreaterThanValueGoal
/// - starting at 0, collect enough asteroids with positive fractions to reach GreaterThanValueGoal
/// - starting at 0, collect enough asteroids with negative integers to reach LessThanValueGoal
/// - starting at 0, collect enough asteroids with negative fractions to reach LessThanValueGoal
/// - starting at 0, collect enough asteroids with pos/neg integers to reach BetweenTwoValuesGoal
/// - starting at 0, collect enough asteroids with pos/neg fractions to reach BetweenTwoValuesGoal 
/// </summary>

public class GameManager : MonoBehaviour
{
	[Header("References and Managers")]
	[SerializeField] private HUDController hud;
	[SerializeField] private MenuManager menuManager;
	[SerializeField] private TimerController timer;

	[Header("Player Settings")]
	[SerializeField] private Player playerPrefab;
	[SerializeField] private Vector2 playerSpawnPos;
	[SerializeField] private Inventory inventory;

	[Header("Asteroid Settings")]
	[SerializeField] private Asteroid asteroidPrefab;
	[SerializeField] private Vector2 asteroidSpawnRangeX;
	[SerializeField] private Vector2 asteroidSpawnRangeY;
	[SerializeField] private LayerMask asteroidLayer;

	[Header("Current Level Settings")]
	public bool AreFractionsBeingReduced = true;
	public int NumberOfClassroomModeRounds = 5; // Number of rounds to set classroom mode to
	public bool IsFreeplayMode;
	public GameDifficulty difficulty = GameDifficulty.Easy;


	[Header("Next Level References")]
	[SerializeField] private GameObject nextLevelTrigger;
	[SerializeField] private float nextRoundCameraOffsetAmount = 7.0f;
	[SerializeField] private GameObject environmentToMoveOnNextRound;

	public delegate bool CurrentGoal(Fraction fraction);
	public CurrentGoal currentGoalCheck;

	private int currentSetNumberOfRounds = -1; // -1 means infinite rounds
	private int currentRound = 0;
	private Fraction currentGoal1;
	private Fraction currentGoal2;
	private Vector2 spawnOffset = Vector2.zero;

	private void Start()
	{
		nextLevelTrigger.SetActive(false);
		timer.gameObject.SetActive(false);
	}

	public void StartGameClassroomMode()
	{
		Player player = Instantiate(playerPrefab, playerSpawnPos, Quaternion.identity);
		currentSetNumberOfRounds = NumberOfClassroomModeRounds;
		IsFreeplayMode = false;

		CreateNextRound(true);
	}

	public void StartGameFreeplayMode()
	{
		Player player = Instantiate(playerPrefab, playerSpawnPos, Quaternion.identity);
		currentSetNumberOfRounds = -1;
		IsFreeplayMode = true;
		timer.gameObject.SetActive(true);
		timer.ResetTimer();
		timer.StartTimer();

		CreateNextRound(true);
	}

	/// <summary>
	/// This function is called every time we'd like to create a new round,
	/// which includes moving the camera, creating a new goal, and spawning in asteroids
	/// </summary>
	/// <param name="isFirstRound">Whether this is our first round, if so our camera won't move</param>
	public void CreateNextRound(bool isFirstRound = false)
	{
		// Move next level trigger
		nextLevelTrigger.SetActive(false);
		Vector2 newPos = new Vector2(
			0,
			nextLevelTrigger.transform.position.y + nextRoundCameraOffsetAmount
		);
		if (!isFirstRound) nextLevelTrigger.transform.position = newPos;

		// Reset stats
		if (currentRound == currentSetNumberOfRounds)
		{
			// We've reached our goal in classroom mode
			menuManager.ShowClassroomModeOverPanel();
		}
		currentRound++;
		inventory.ResetScore();
		hud.UpdateRoundText(currentRound);
		if (!isFirstRound) timer.AddTime(10);
		if (currentRound > 4)
		{
			difficulty = GameDifficulty.Medium;
			if (currentRound > 8)
			{
				difficulty = GameDifficulty.Hard;
			}
		}
		hud.UpdateDifficultyText(difficulty);

		// Delete current asteroids
		Asteroid[] currentAsteroids = FindObjectsByType<Asteroid>(FindObjectsSortMode.None);
		foreach (Asteroid asteroid in currentAsteroids)
		{
			Destroy(asteroid.gameObject);
		}

		// Spawn next asteroids
		if (!isFirstRound) spawnOffset.y += nextRoundCameraOffsetAmount;

		int goalNumeratorVal = 0;
		int goalDenominatorVal = 0;
		int numberOfAsteroidsToCollect = 0;
		int minimumAsteroidCount = 0;
		int maximumAsteroidCount = 0;
		bool shouldReduceFraction = false;
		bool isValidList;
		do
		{
			switch (difficulty)
			{
				case GameDifficulty.Easy:

					goalNumeratorVal = UnityEngine.Random.Range(2, 6+1);
					numberOfAsteroidsToCollect = UnityEngine.Random.Range(2, 3+1);
					minimumAsteroidCount = 2;
					maximumAsteroidCount = numberOfAsteroidsToCollect + UnityEngine.Random.Range(1, 2+1);
					shouldReduceFraction = false;

					if (IsFreeplayMode)
					{
						goalDenominatorVal = UnityEngine.Random.Range(1, 4+1);
					}
					else
					{
						goalDenominatorVal = UnityEngine.Random.Range(2, 4+1);
					}
					break;

				case GameDifficulty.Medium:

					goalNumeratorVal = UnityEngine.Random.Range(3, 10+1);
					goalDenominatorVal = UnityEngine.Random.Range(2, 5+1);
					numberOfAsteroidsToCollect = UnityEngine.Random.Range(2, 4+1);
					minimumAsteroidCount = 2;
					maximumAsteroidCount = numberOfAsteroidsToCollect + UnityEngine.Random.Range(1, 3+1);
					shouldReduceFraction = true;
					break;

				case GameDifficulty.Hard:

					goalNumeratorVal = UnityEngine.Random.Range(4, 15+1);
					goalDenominatorVal = UnityEngine.Random.Range(2, 6+1);
					numberOfAsteroidsToCollect = UnityEngine.Random.Range(3, 6+1);
					minimumAsteroidCount = 3;
					maximumAsteroidCount = numberOfAsteroidsToCollect + UnityEngine.Random.Range(1, 4+1);
					shouldReduceFraction = true;
					break;

			}

			AreFractionsBeingReduced = shouldReduceFraction;

			isValidList = CreateExactValueGoal(goalNumeratorVal,
									goalDenominatorVal,
									numberOfAsteroidsToCollect,
									minimumAsteroidCount,
									maximumAsteroidCount,
									shouldReduceFraction);
		}
		while (!isValidList);

		if (!isFirstRound)
		{
			newPos = new Vector2(
				0,
				environmentToMoveOnNextRound.transform.position.y + nextRoundCameraOffsetAmount
			);
			StartCoroutine(LerpEnvironmentPosition(newPos, 1f));
		}
	}

	/// <summary>
	/// Move the environment up to the next round across several frames
	/// </summary>
	/// <param name="targetPosition">final position to move to</param>
	/// <param name="duration">how long lerping will take place</param>
	/// <returns></returns>
	IEnumerator LerpEnvironmentPosition(Vector2 targetPosition, float duration)
    {
        float time = 0;
        Vector2 startPosition = environmentToMoveOnNextRound.transform.position;

        while (time < duration)
        {
            environmentToMoveOnNextRound.transform.position = Vector2.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        environmentToMoveOnNextRound.transform.position = targetPosition;
    }

	/// <summary>
	/// This goal creates an environment where you need to obtain asteroids greater than some goal (THIS IS UNFINISHED)
	/// </summary>
	/// <param name="denominatorVal">value of the denominator</param>
	// private void CreateGreaterThanValueGoal(int denominatorVal)
	// {
	// 	// starting at 0, collect enough asteroids with positive integers to reach GreaterThanValueGoal
	// 	int goal = UnityEngine.Random.Range(5, 15);
	// 	currentGoal = goal;
	// 	currentGoal = new Fraction(goal, 1);
	// 	List<int> numbersVisible = new List<int>();

	// 	int goalLeft = goal;
	// 	while (goalLeft > 0 || numbersVisible.Count < 5)
	// 	{
	// 		int value = UnityEngine.Random.Range(1, 5);
	// 		numbersVisible.Add(value);
	// 		goalLeft -= value;
	// 	}

	// 	foreach (int number in numbersVisible)
	// 	{
	// 		Vector2 spawnPos = Vector2.zero;
	// 		Collider2D col;
	// 		do
	// 		{
	// 			spawnPos = new Vector2(UnityEngine.Random.Range(asteroidSpawnRangeX.x, asteroidSpawnRangeX.y), UnityEngine.Random.Range(asteroidSpawnRangeY.x, asteroidSpawnRangeY.y));
	// 			col = Physics2D.OverlapCircle(spawnPos, 1f, asteroidLayer);
	// 		}
	// 		while (col != null);

	// 		Asteroid asteroid = Instantiate(asteroidPrefab,
	// 			spawnPos,
	// 			Quaternion.identity);

	// 		asteroid.Numerator = number;
	// 		asteroid.Denominator = 1;
	// 		asteroid.Fraction = new Fraction(number, 1);
	// 	}

	// 	currentGoalCheck = CheckGreaterThanValueGoal;

	// 	hud.UpdateGoalText($"Goal: collect at least {goal} kg");
	// }

	/// <summary>
	/// This goal creates an environment where you need to obtain an exact amount of asteroids
	/// </summary>
	/// <param name="numeratorGoalVal">value of goal's numerator</param>
	/// <param name="denominatorVal">value of goal's denominator</param>
	/// <param name="numberOfAsteroidsToCollect">how many asteroids should the player collect to pass the round</param>
	/// <param name="minimumAsteroidCount">there should be at least this many asteroids instantiated this round</param>
	/// <param name="maximumAsteroidCount">the highest number of asteroids we want instantiated this round</param>
	/// <param name="shouldReduceFraction">should we reduce fractions in this round</param>
	/// <returns></returns>
	public bool CreateExactValueGoal(int numeratorGoalVal,
								int denominatorVal,
								int numberOfAsteroidsToCollect = 5,
								int minimumAsteroidCount = 5,
								int maximumAsteroidCount = -1,
								bool shouldReduceFraction = true
								)
	{
		// Parameter checks
		if (maximumAsteroidCount == -1) maximumAsteroidCount = UnityEngine.Random.Range(minimumAsteroidCount, minimumAsteroidCount+5);
		if (maximumAsteroidCount < minimumAsteroidCount) maximumAsteroidCount = minimumAsteroidCount;
		if (minimumAsteroidCount <= 0) minimumAsteroidCount = 0;
		if (numberOfAsteroidsToCollect > maximumAsteroidCount) numberOfAsteroidsToCollect = maximumAsteroidCount;

		// Set up current goal
		currentGoal1 = new Fraction(numeratorGoalVal, denominatorVal);

		// Create list of numbers that can be collected
		// We will first create a list of numerators that will later then be divided by the denominatorVal
		// We will first reach numberOfAsteroidsToCollect, then add more once we've reached this goal
		List<int> numbersVisible = new List<int>();
		int goalLeft = numeratorGoalVal;

		int[] goalNumbers;
		goalNumbers = GetNumbers(numberOfAsteroidsToCollect, numeratorGoalVal);
		if (goalNumbers == null)
		{
			// Getting a list of numbers with these parameters is impossible, try again
			return false;
		}
		
		numbersVisible.AddRange(GetNumbers(numberOfAsteroidsToCollect, numeratorGoalVal));

		if (numberOfAsteroidsToCollect < maximumAsteroidCount)
		{
			// We still need to add asteroids to reach our maximumAsteroidCount
			int asteroidsLeft = maximumAsteroidCount - numberOfAsteroidsToCollect;
			for (int i = 0; i < asteroidsLeft; i++)
			{
				int value;
				do
				{
					value = UnityEngine.Random.Range(1, 6);
				}
				while (value == numeratorGoalVal);

				numbersVisible.Add(value);
			}
		}

		Debug.Log($"Our list of numerators is {string.Join( ",", numbersVisible)}");

		foreach (int number in numbersVisible)
		{
			// Convert to fraction
			Fraction fraction = new Fraction(number, denominatorVal, shouldReduceFraction);

			// Spawn asteroid
			Vector2 spawnPos;
			Collider2D col;
			do
			{
				spawnPos = spawnOffset + new Vector2(UnityEngine.Random.Range(asteroidSpawnRangeX.x, asteroidSpawnRangeX.y),
										UnityEngine.Random.Range(asteroidSpawnRangeY.x, asteroidSpawnRangeY.y));
				col = Physics2D.OverlapCircle(spawnPos, 1f, asteroidLayer);
			}
			while (col != null);

			Asteroid asteroid = Instantiate(asteroidPrefab,
				spawnPos,
				Quaternion.identity);

			asteroid.Fraction = fraction;
		}

		currentGoalCheck = CheckExactValueGoal;
		hud.UpdateGoalText($"Goal: collect exactly this many kilograms:", currentGoal1);

		return true;
	}

	/// <summary>
	/// Goes with CreateExactValueGoal()
	/// </summary>
	/// <param name="currentScore">current inventory player holds</param>
	/// <returns>whether goal has been reached</returns>
	private bool CheckExactValueGoal(Fraction currentScore)
	{
		if (currentScore == currentGoal1)
			return true;
		
		return false;
	}

	// https://stackoverflow.com/questions/472013/generate-a-series-of-random-numbers-that-add-up-to-n-in-c-sharp
	private int[] GetNumbers(int count, int total)
	{
		const int LOWERBOUND = 1; // can be changed
		const int UPPERBOUND = 6; // can be changed

		int[] result = new int[count];
		int currentsum = 0;
		int low, high, calc;

		if((UPPERBOUND * count) < total ||
			(LOWERBOUND * count) > total ||
			UPPERBOUND < LOWERBOUND)
		{
			Debug.Log($"Not possible. count={count}, total={total}");
			return null;
		}

		for (int i = 0; i < count; i++)
		{
			calc = (total - currentsum) - (UPPERBOUND * (count - 1 - i));
			low = calc < LOWERBOUND ? LOWERBOUND : calc;
			calc = (total - currentsum) - (LOWERBOUND * (count - 1 - i));
			high = calc > UPPERBOUND ? UPPERBOUND : calc;

			result[i] = UnityEngine.Random.Range(low, high + 1);

			currentsum += result[i];
		}
		return result;
	}

	/// <summary>
	/// Check if our current inventory matches the goal needed to pass this round
	/// </summary>
	/// <param name="currentScore"></param>
	/// <returns></returns>
	public bool CheckIfGoalReached(Fraction currentScore)
	{
		if ((bool) currentGoalCheck?.Invoke(currentScore))
		{
			nextLevelTrigger.SetActive(true);
			return true;
		}

		nextLevelTrigger.SetActive(false);
		return false;
	}

	/// <summary>
	/// Randomly choose between different goals
	/// </summary>
	public void ChooseRandomGoal()
	{
		// TODO: add in a way to randomly choose between goals
	}

	// Example unused goal
	private bool CheckGreaterThanValueGoal(Fraction currentScore)
	{
		if (currentScore >= currentGoal1)
			return true;
		
		return false;
	}

	// Example unused goal
	private bool LessThanValueGoal(Fraction value, Fraction currentScore)
	{
		if (currentScore <= value)
			return true;

		return false;
	}

	// Example unused goal
	private bool BetweenTwoValuesGoal(Fraction value1, Fraction value2, Fraction currentScore)
	{
		if (currentScore >= value1 && currentScore <= value2)
			return true;

		return false;
	}
}
