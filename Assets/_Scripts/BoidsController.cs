using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidsController : MonoBehaviour 
{
	/// The prefab GameObject
	public GameObject _prefabBoid;
	/// Total number of boids belonging to this BoidsController
	public int _numBoids;
	/// List of all GameObjects managed by this BoidsController
	public List<GameObject> _allBoids = new List<GameObject>();
	/// Max spawn distance from this BoidsController transform position
	public int _spawnRadius;
	/// Reference to in-scene goal GameObject for Boid objects in this BoidsController
	public GameObject _goalObject;


	/// Spawn the Boids
	void SpawnInitialBoids(int numToSpawn, int spawnRadius)
	{
		Vector3 spawnOffset;
		for(int i = 0; i < numToSpawn; i++)
		{
			spawnOffset = new Vector3(Random.Range(-spawnRadius, spawnRadius),
										Random.Range(-spawnRadius, spawnRadius),
										Random.Range(-spawnRadius, spawnRadius));
			_allBoids.Add(Instantiate(_prefabBoid, transform.position + spawnOffset, Quaternion.identity));
		}
	}

	void Awake()
	{
		SpawnInitialBoids(_numBoids, _spawnRadius);
	}
}
