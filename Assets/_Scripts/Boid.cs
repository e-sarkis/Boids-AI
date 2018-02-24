using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Boid : MonoBehaviour 
{
	public float moveSpeed;
	public float rotationSpeed;

	public float avoidProximity = 1.0f;

	Vector3 averageHeadingOfGroup;
	Vector3 averagePositionOfGroup;

	// Any Boid objects further than this will be ignored in calculations
	public float minDistanceToNeighbour;

	Vector3 _separation;	
	Vector3 _alignment;

	/// Reference to BoidsController of this Boid.
	public BoidsController _parentBoidsController;

	void Update () 
	{
		// One in 5 chance of readjusting this frame
		// if (Random.Range(0, 5) < 1)
		// {
			UpdateTrajectory();
		// }
		
		//transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
		transform.Translate(0, 0, Time.deltaTime * moveSpeed);
	}

	void UpdateTrajectory()
	{
		Vector3 avoidance 	= Vector3.zero; // Avoidance vector for collision intelligence.
		Vector3 groupCenter	= Vector3.zero; // The center of the group
		float groupSpeed = 0f; // TEMP - Do I need this here?
		int groupSize = 0; // TEMP - total number of neighbours in our group

		// Iterating through all Boid GameObjects
		_parentBoidsController.allBoids.Remove(gameObject); // Remove self from List before iteration
		foreach (GameObject boidObject in _parentBoidsController.allBoids)
		{
			float distanceToCurrentNeighbour = Vector3.Distance(boidObject.transform.position, transform.position);
			if (distanceToCurrentNeighbour <= minDistanceToNeighbour)
			{
				groupCenter += boidObject.transform.position; // Average out center based on nearby relevant neighbour position
				groupSize++;

				if (distanceToCurrentNeighbour < avoidProximity)
				{
					// Account for proximity
					avoidance = avoidance + (transform.position - boidObject.transform.position);
				}

				// TEMP - what does this do?
				Boid otherBoid = boidObject.GetComponent<Boid>();
				groupSpeed = groupSpeed + otherBoid.moveSpeed;
			}
		}
		_parentBoidsController.allBoids.Add(gameObject); // add self back to list

		if (groupSize > 0)
		{
			groupCenter = (groupCenter / groupSize) + (_parentBoidsController.GetGoalPosition() - transform.position);
			moveSpeed = 0f;
			moveSpeed += groupSpeed / groupSize;

			Vector3 direction = (groupCenter + avoidance) - transform.position;
			if (direction != Vector3.zero)
			{
				transform.rotation = Quaternion.Slerp(transform.rotation,
													Quaternion.LookRotation(direction),
													rotationSpeed * Time.deltaTime);
			}
		}
	}
}
