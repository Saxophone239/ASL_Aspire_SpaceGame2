using System.Collections;
using System.Collections.Generic;
using Mehroz;
using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{
	[SerializeField] private GameManager gM;

	[SerializeField] private TextMeshProUGUI goalText;
	[SerializeField] private MixedNumberDisplay inventoryMixedNumberDisplay;
	[SerializeField] private MixedNumberDisplay goalMixedNumberDisplay;
	[SerializeField] private TextMeshProUGUI roundText;
	[SerializeField] private TextMeshProUGUI difficultyText;

	public void UpdateInventoryFraction(Fraction newFraction)
	{
		inventoryMixedNumberDisplay.UpdateNumber(newFraction);
	}

	public void UpdateGoalText(string newText, Fraction newFraction)
	{
		goalText.text = newText;
		goalMixedNumberDisplay.UpdateNumber(newFraction);
	}

	public void UpdateRoundText(int newRound)
	{
		if (gM.IsFreeplayMode)
			roundText.text = $"Round {newRound}";
		else
			roundText.text = $"Round {newRound}/{gM.NumberOfClassroomModeRounds}";
	}

	public void UpdateDifficultyText(GameDifficulty difficulty)
	{
		difficultyText.text = $"Difficulty: {difficulty}";
	}
}
