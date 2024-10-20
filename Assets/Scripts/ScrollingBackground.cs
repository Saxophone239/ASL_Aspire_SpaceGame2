using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    public float xSpeed = 0;
	public float ySpeed = 0;

	[SerializeField] private Renderer bgRenderer;

	private void Awake()
	{
		bgRenderer = GetComponent<Renderer>();
	}

	private void Update()
	{
		bgRenderer.material.mainTextureOffset += new Vector2(xSpeed * Time.deltaTime, ySpeed * Time.deltaTime);
	}
}
