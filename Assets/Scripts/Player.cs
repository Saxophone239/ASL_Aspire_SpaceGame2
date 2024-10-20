using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
	public float MoveLerpSpeed;
	public float RotLerpSpeed;
	public float Speed;

	[SerializeField] private SpriteRenderer sprite;
	[SerializeField] private Rigidbody2D rb;
	[SerializeField] private Inventory inventory;
	[SerializeField] private GameManager gameManager;
	[SerializeField] private Asteroid asteroidPrefab;

	private Vector2 previousMovementInput;
	private Quaternion currentRotation;
	private float currentSpeed;

	private void Awake()
	{
		sprite = GetComponent<SpriteRenderer>();
		rb = GetComponent<Rigidbody2D>();
		inventory = FindObjectOfType<Inventory>();
		gameManager = FindObjectOfType<GameManager>();
	}

	private void FixedUpdate()
	{
		currentSpeed = Mathf.Lerp(currentSpeed, Speed * previousMovementInput.magnitude, MoveLerpSpeed * Time.fixedDeltaTime);
		rb.MovePosition(rb.position + previousMovementInput * currentSpeed * Time.fixedDeltaTime);

		if (previousMovementInput != Vector2.zero)
		{
			Quaternion rot = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, previousMovementInput));
			currentRotation = Quaternion.Lerp(currentRotation, rot, RotLerpSpeed * Time.fixedDeltaTime);
			rb.MoveRotation(currentRotation);
		}
	}

	public void HandleMove(InputAction.CallbackContext context)
	{
		previousMovementInput = context.ReadValue<Vector2>();
	}

	public void HandleEject(InputAction.CallbackContext context)
	{
		if (!context.performed) return;

		UIAsteroid mostRecentAsteroid = inventory.RemoveAsteroid();
		if (mostRecentAsteroid != null) SpawnAsteroid(mostRecentAsteroid);
		Debug.Log("Ejected asteroid");
	}

	public void SpawnAsteroid(UIAsteroid a)
	{
		Asteroid justSpawned = Instantiate(asteroidPrefab, this.transform.position, Quaternion.identity);
		justSpawned.Fraction = new Mehroz.Fraction(a.Fraction);
		justSpawned.SpriteRenderer.sprite = a.Image.sprite;
		justSpawned.Show(true);
		Debug.Log("Spawned asteroid");
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Asteroid"))
		{
			inventory.AddAsteroid(other.gameObject.GetComponent<Asteroid>());
			Debug.Log("Collected asteroid");
		}
		if (other.gameObject.CompareTag("NextLevelTrigger"))
		{
			Debug.Log("Creating next round");
			gameManager.CreateNextRound();
		}
	}
}
