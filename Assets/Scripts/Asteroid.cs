using System.Collections;
using System.Collections.Generic;
using Mehroz;
using TMPro;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
	public Fraction Fraction;
	public SpriteRenderer SpriteRenderer { get; private set; }

	private MixedNumberDisplay mixedNumberDisplay;
	new private Collider2D collider;

	private void Awake()
	{
		SpriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
		mixedNumberDisplay = GetComponentInChildren<MixedNumberDisplay>(true);
		collider = GetComponentInChildren<Collider2D>(true);
	}

	private void Start()
	{
		mixedNumberDisplay.UpdateNumber(Fraction, true);
	}

	public void Show(bool shouldShow)
	{
		SpriteRenderer.gameObject.SetActive(shouldShow);
		mixedNumberDisplay.gameObject.SetActive(shouldShow);
		collider.enabled = false;
		
		if (!shouldShow)
		{
			collider.enabled = false;
			Destroy(gameObject, 2);
		}
		else
			StartCoroutine(EnableColliderAfterSomeTime(1));
	}

	public IEnumerator EnableColliderAfterSomeTime(float timeInSeconds)
	{
		yield return new WaitForSeconds(timeInSeconds);
		collider.enabled = true;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Show(false);
	}
}

