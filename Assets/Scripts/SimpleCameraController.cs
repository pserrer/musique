using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraController : MonoBehaviour
{

	public float Speed = 1f;

	private Camera _camera;

	// Use this for initialization
	void Start ()
	{
		_camera = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		var horizontal = Input.GetAxis("Horizontal");
		if (Math.Abs(horizontal) > 0.01)
		{
			_camera.transform.Rotate(Vector3.up, Time.deltaTime * Speed * horizontal);
		}
	}
}
