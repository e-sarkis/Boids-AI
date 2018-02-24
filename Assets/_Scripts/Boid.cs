using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Boid : MonoBehaviour 
{
	public float _moveSpeed;

	Vector3 _separation;	
	Vector3 _alignment;


	void Update () 
	{
		transform.Translate(Vector3.forward * _moveSpeed * Time.deltaTime);
	}
}
