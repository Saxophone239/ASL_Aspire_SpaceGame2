using System.Collections;
using System.Collections.Generic;
using Mehroz;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAsteroid : MonoBehaviour
{
	public Fraction Fraction;
	public Image Image { get; private set; }

	// private TextMeshProUGUI text;
	private MixedNumberDisplay mixedNumberDisplay;

	private void Awake()
	{
		Image = GetComponent<Image>();
		mixedNumberDisplay = GetComponentInChildren<MixedNumberDisplay>(true);
	}

	private void Start()
	{
		mixedNumberDisplay.UpdateNumber(Fraction, true);
	}
}
