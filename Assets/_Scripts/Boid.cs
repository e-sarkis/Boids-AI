using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Boid : MonoBehaviour 
{
	public float moveSpeed;			// Movement speed in editor units
	public float rotationPercentage; 	// % of move speed for rotation speed
	
	public float avoidanceProximity; // Min distance from neighbough for corrective steering

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
		UpdateTrajectory();
		// Translate along Z-axis
		transform.Translate(0, 0, Time.deltaTime * moveSpeed);
	}

	void UpdateTrajectory()
	{
		Vector3 avoidance 	= Vector3.zero; // Avoidance vector for collision intelligence.
		Vector3 groupCenter	= Vector3.zero; // The center of the group.

		float groupSpeed 	= 0f; 	// Total speed of neighbour group.
		int groupSize 		= 0;	// Total Boids in neighbour group.

		_parentBoidsController.allBoids.Remove(gameObject); // Remove self from List before iteration.
		foreach (GameObject boidObject in _parentBoidsController.allBoids)
		{
			float distanceToCurrentNeighbour = Vector3.Distance(boidObject.transform.position, transform.position);
			if (distanceToCurrentNeighbour <= minDistanceToNeighbour)
			{
				groupCenter += boidObject.transform.position; // Add neighbour positions for averaging.
				groupSize++;

				Boid otherBoid = boidObject.GetComponent<Boid>();
				if (otherBoid)
				{
					groupSpeed += otherBoid.moveSpeed;
				}

				// Account for correction if experiencing proximity intrusion.
				if (distanceToCurrentNeighbour < avoidanceProximity)
				{
					avoidance += transform.position - boidObject.transform.position;
				}
			}
		}
		_parentBoidsController.allBoids.Add(gameObject); // Add this back to List

		if (groupSize > 0)
		{
			// Calculate average and add Vector to Goal.
			groupCenter = (groupCenter / groupSize) + (_parentBoidsController.GetGoalPosition() - transform.position);
			
			/// Rudimentary accounting for movement speed variance.
			//moveSpeed = groupSpeed / groupSize;

			Vector3 direction = (groupCenter + avoidance) - transform.position;
			if (direction != Vector3.zero)
			{
				transform.rotation = Quaternion.Slerp(transform.rotation,
													Quaternion.LookRotation(direction),
													rotationPercentage * moveSpeed * Time.deltaTime);
			}
		}
	}
}
