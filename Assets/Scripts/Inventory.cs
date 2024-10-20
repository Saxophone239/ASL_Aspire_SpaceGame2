using System.Collections;
using System.Collections.Generic;
using Mehroz;
using UnityEngine;

public class Inventory : MonoBehaviour
{
	public Fraction CurrentValue;

	private List<UIAsteroid> uiAsteroids = new List<UIAsteroid>();

	[SerializeField] private GameManager gameManager;
	[SerializeField] private UIAsteroid uiAsteroidPrefab;
	[SerializeField] private GameObject layoutGroup;
	[SerializeField] private HUDController hud;

	private Player player;

	private void Start()
	{
		CurrentValue = new Fraction(0, 1);
		hud.UpdateInventoryFraction(CurrentValue);
	}

	/// <summary>
	/// Add a UIAsteroid from collecting an Asteroid
	/// </summary>
	/// <param name="a">asteroid that's collected</param>
	public void AddAsteroid(Asteroid a)
	{
		UIAsteroid uIAsteroidAdded = Instantiate(uiAsteroidPrefab, layoutGroup.transform);
		uIAsteroidAdded.Fraction = new Fraction(a.Fraction);
		uIAsteroidAdded.Image.sprite = a.SpriteRenderer.sprite;
		
		uiAsteroids.Add(uIAsteroidAdded);

		CurrentValue = CurrentValue + uIAsteroidAdded.Fraction;
		hud.UpdateInventoryFraction(CurrentValue);
		gameManager.CheckIfGoalReached(CurrentValue);
		Debug.Log($"Collected UI Asteroid, size of our inventory is now {uiAsteroids.Count}");
	}

	#nullable enable
	/// <summary>
	/// Remove a UIAsteroid from our inventory
	/// </summary>
	/// <param name="shouldCheckGoal">upon removing an asteroid, should we make a check that we've achieved our goal</param>
	/// <returns>the UIAsteroid removed, could be null</returns>
	public UIAsteroid? RemoveAsteroid(bool shouldCheckGoal = true)
	{
		UIAsteroid? uIAsteroidRemoved = null;

		if (uiAsteroids.Count > 0)
		{
			uIAsteroidRemoved = uiAsteroids[uiAsteroids.Count - 1];
			uiAsteroids.RemoveAt(uiAsteroids.Count - 1);

			CurrentValue = CurrentValue - uIAsteroidRemoved.Fraction;
			hud.UpdateInventoryFraction(CurrentValue);
			if (shouldCheckGoal) gameManager.CheckIfGoalReached(CurrentValue);
			Destroy(uIAsteroidRemoved.gameObject);
		}

		Debug.Log($"Removed UI Asteroid, size of our inventory is now {uiAsteroids.Count}");
		return uIAsteroidRemoved;
	}

	public void ResetScore()
	{
		while (uiAsteroids.Count != 0)
		{
			RemoveAsteroid(false);
		}
	}
}
