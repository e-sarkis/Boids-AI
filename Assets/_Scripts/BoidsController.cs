using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidsController : MonoBehaviour 
{
	/// The prefab GameObject.
	public GameObject prefabBoid;
	/// Total number of boids belonging to this BoidsController.
	public int numBoids;
	/// List of all GameObjects managed by this BoidsController.
	public List<GameObject> allBoids = new List<GameObject>();
	/// Max spawn distance from this BoidsController transform position.
	public int spawnRadius;
	/// Reference to in-scene goal GameObject for Boid objects in this BoidsController.
	public GameObject goalObject;

	/// Spawn the starting Boids.
	void SpawnInitialBoids(int numToSpawn, int spawnRadius)
	{
		Vector3 spawnOffset;
		for(int i = 0; i < numToSpawn; i++)
		{
			spawnOffset = new Vector3(Random.Range(-spawnRadius, spawnRadius),
										Random.Range(-spawnRadius, spawnRadius),
										Random.Range(-spawnRadius, spawnRadius));
			GameObject boidObject = Instantiate(prefabBoid, transform.position + spawnOffset, Quaternion.identity);
			allBoids.Add(boidObject);
			Boid boid = boidObject.GetComponent<Boid>();
			if (boid)
			{
				boid._parentBoidsController = this;
			}
		}
	}
	
	void Awake()
	{
		SpawnInitialBoids(numBoids, spawnRadius);
	}

	/// Return the position the goal GameObject of this BoidsController.
	/// @return - position of the goal GameObject
	public Vector3 GetGoalPosition()
	{
		return goalObject.transform.position;
	}
}
