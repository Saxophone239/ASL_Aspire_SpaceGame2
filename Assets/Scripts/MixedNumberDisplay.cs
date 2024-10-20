using System;
using System.Collections;
using System.Collections.Generic;
using Mehroz;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MixedNumberDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI wholeNumber;
	[SerializeField] private GameObject fractionHolder;
	[SerializeField] private TextMeshProUGUI numerator;
	[SerializeField] private TextMeshProUGUI denominator;
	[SerializeField] private Image fractionLine;
	[SerializeField] private GameObject signHolder;
	[SerializeField] private GameObject positiveSign;
	[SerializeField] private GameObject negativeSign;

	private RectTransform fractionHolderRectTransform;
	private RectTransform signHolderRectTransform;

	private Vector2 leftAnchorPositionMin = new Vector2(0, 0);
	private Vector2 leftAnchorPositionMax = new Vector2(0.333f, 1);
	private Vector2 leftCenterAnchorPositionMin = new Vector2(0.2f, 0);
	private Vector2 leftCenterAnchorPositionMax = new Vector2(0.5f, 1);
	private Vector2 centerAnchorPositionMin = new Vector2(0.333f, 0);
	private Vector2 centerAnchorPositionMax = new Vector2(0.666f, 1);
	private Vector2 rightCenterAnchorPositionMin = new Vector2(0.5f, 0);
	private Vector2 rightCenterAnchorPositionMax = new Vector2(0.8f, 1);
	private Vector2 rightAnchorPositionMin = new Vector2(0.666f, 0);
	private Vector2 rightAnchorPositionMax = new Vector2(1, 1);

	private void Awake()
	{
		fractionHolderRectTransform = fractionHolder.GetComponent<RectTransform>();
		signHolderRectTransform = signHolder.GetComponent<RectTransform>();
	}

	public void UpdateNumber(Fraction fraction, bool showSignIfPositve = false)
	{
		bool isFractionNegative = IsFractionNegative(fraction);
		float numeratorAbs = Mathf.Abs(fraction.Numerator);
		float denominatorAbs = Mathf.Abs(fraction.Denominator);

		if (numeratorAbs == 0)
		{
			// We have a whole number that should be 0
			wholeNumber.gameObject.SetActive(true);
			fractionHolder.SetActive(false);

			if (showSignIfPositve)
			{
				// We need to show a positive sign and whole number
				positiveSign.SetActive(true);
				negativeSign.SetActive(false);

				ResetTransform(signHolderRectTransform, "left-center");
				ResetTransform(wholeNumber.rectTransform, "right-center");
			}
			else
			{
				// We need to show only the whole number
				positiveSign.SetActive(false);
				negativeSign.SetActive(false);

				ResetTransform(wholeNumber.rectTransform, "center");
			}

			wholeNumber.text = "0";
		}
		else if (numeratorAbs < denominatorAbs)
		{
			// We have a proper fraction
			wholeNumber.gameObject.SetActive(false);
			fractionHolder.SetActive(true);

			if (isFractionNegative)
			{
				// We need to show a negative sign and fraction
				positiveSign.SetActive(false);
				negativeSign.SetActive(true);

				ResetTransform(signHolderRectTransform, "left-center");
				ResetTransform(fractionHolderRectTransform, "right-center");
			}
			else
			{
				if (showSignIfPositve)
				{
					// We need to show a positive sign and fraction
					positiveSign.SetActive(true);
					negativeSign.SetActive(false);

					ResetTransform(signHolderRectTransform, "left-center");
					ResetTransform(fractionHolderRectTransform, "right-center");
				}
				else
				{
					// We need to show only the fraction
					positiveSign.SetActive(false);
					negativeSign.SetActive(false);

					ResetTransform(fractionHolderRectTransform, "center");
				}
			}

			numerator.text = fraction.Numerator.ToString();
			denominator.text = fraction.Denominator.ToString();
		}
		else if (numeratorAbs == denominatorAbs)
		{
			// We have a whole number that should be 1
			wholeNumber.gameObject.SetActive(true);
			fractionHolder.SetActive(false);

			if (isFractionNegative)
			{
				// We need to show a negative sign and whole number
				positiveSign.SetActive(false);
				negativeSign.SetActive(true);

				ResetTransform(signHolderRectTransform, "left-center");
				ResetTransform(wholeNumber.rectTransform, "right-center");
			}
			else
			{
				if (showSignIfPositve)
				{
					// We need to show a positive sign and whole number
					positiveSign.SetActive(true);
					negativeSign.SetActive(false);

					ResetTransform(signHolderRectTransform, "left-center");
					ResetTransform(wholeNumber.rectTransform, "right-center");
				}
				else
				{
					// We need to show only the whole number
					positiveSign.SetActive(false);
					negativeSign.SetActive(false);

					ResetTransform(wholeNumber.rectTransform, "center");
				}
			}

			wholeNumber.text = "1";
		}
		else if (numeratorAbs % denominatorAbs == 0)
		{
			// We have a whole number
			wholeNumber.gameObject.SetActive(true);
			fractionHolder.SetActive(false);

			if (isFractionNegative)
			{
				// We need to show a negative sign and whole number
				positiveSign.SetActive(false);
				negativeSign.SetActive(true);

				ResetTransform(signHolderRectTransform, "left-center");
				ResetTransform(wholeNumber.rectTransform, "right-center");
			}
			else
			{
				if (showSignIfPositve)
				{
					// We need to show a positive sign and whole number
					positiveSign.SetActive(true);
					negativeSign.SetActive(false);

					ResetTransform(signHolderRectTransform, "left-center");
					ResetTransform(wholeNumber.rectTransform, "right-center");
				}
				else
				{
					// We need to show only the whole number
					positiveSign.SetActive(false);
					negativeSign.SetActive(false);

					ResetTransform(wholeNumber.rectTransform, "center");
				}
			}

			wholeNumber.text = (numeratorAbs / denominatorAbs).ToString();
		}
		else
		{
			// We have an improper fraction, we're gonna convert it into a mixed number
			wholeNumber.gameObject.SetActive(true);
			fractionHolder.SetActive(true);

			if (isFractionNegative)
			{
				// We need to show a negative sign, whole number, and fraction
				positiveSign.SetActive(false);
				negativeSign.SetActive(true);

				ResetTransform(signHolderRectTransform, "left");
				ResetTransform(wholeNumber.rectTransform, "center");
				ResetTransform(fractionHolderRectTransform, "right");
			}
			else
			{
				if (showSignIfPositve)
				{
					// We need to show a positive sign, whole number, and fraction
					positiveSign.SetActive(true);
					negativeSign.SetActive(false);

					ResetTransform(signHolderRectTransform, "left");
					ResetTransform(wholeNumber.rectTransform, "center");
					ResetTransform(fractionHolderRectTransform, "right");
				}
				else
				{
					// We need to show only the whole number and fraction
					positiveSign.SetActive(false);
					negativeSign.SetActive(false);
					
					ResetTransform(wholeNumber.rectTransform, "left-center");
					ResetTransform(fractionHolderRectTransform, "right-center");
				}
			}

			wholeNumber.text = (fraction.Numerator / fraction.Denominator).ToString();
			numerator.text = (fraction.Numerator % fraction.Denominator).ToString();
			denominator.text = fraction.Denominator.ToString();
		}
	}
	
	private void ResetTransform(RectTransform transform, string position)
	{
		switch (position)
		{
			case "left":
				transform.anchorMin = leftAnchorPositionMin;
				transform.anchorMax = leftAnchorPositionMax;
				break;
			case "left-center":
				transform.anchorMin = leftCenterAnchorPositionMin;
				transform.anchorMax = leftCenterAnchorPositionMax;
				break;
			case "center":
				transform.anchorMin = centerAnchorPositionMin;
				transform.anchorMax = centerAnchorPositionMax;
				break;
			case "right-center":
				transform.anchorMin = rightCenterAnchorPositionMin;
				transform.anchorMax = rightCenterAnchorPositionMax;
				break;
			case "right":
				transform.anchorMin = rightAnchorPositionMin;
				transform.anchorMax = rightAnchorPositionMax;
				break;
		}

		transform.offsetMin = Vector2.zero;
		transform.offsetMax = Vector2.zero;
	}

	private bool IsFractionNegative(Fraction fraction)
	{
		return fraction.Numerator < 0 ^ fraction.Denominator < 0;
	}
}
