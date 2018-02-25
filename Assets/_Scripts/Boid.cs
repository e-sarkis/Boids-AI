using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Boid : MonoBehaviour 
{
	public float moveSpeed;				// Movement speed in editor units.
	public float rotationPercentage;	// % of move speed for rotation speed.
	public float separationProximity;	// Min distance from neighbough for corrective steering.
	public float neighbourDetectRadius;	// Boid objects within radius will comprise neigbour group

	// Autonomous Boid objects home in on initial spawn location after exceeding this.
	public float autonomousTravelRadius;

	private Vector3 separation 	= Vector3.zero; // Avoidance vector for separation.
	private Vector3 alignment	= Vector3.zero; // Alignment vector for positioning within group.
	private Vector3 groupAverageHeading;
	private Vector3 groupAveragePosition;		// Averaged position of all neighbours.
	private float groupSpeed 	= 0f; 			// Total speed of neighbour group.
	private int groupSize 		= 0;			// Total Boids in neighbour group.

	// Reference to BoidsController object for Managed Boid objects
	public BoidsController parentBoidsController;

	private float 	_distanceToCurrentNeighbour;
	private Vector3	_spawnLocation;

	void Update () 
	{
		//ManagedUpdateTrajectory();
		AutonomousUpdateTrajectory();

		// Translate along Z-axis
		transform.Translate(0, 0, Time.deltaTime * moveSpeed);
	}

	// Leaderless, stays within travel radius.
	void AutonomousUpdateTrajectory()
	{
		separation 	= Vector3.zero;
		alignment	= Vector3.zero;

		// Obtain neighbours.
		Collider[] neigbourColliders = Physics.OverlapSphere(transform.position, neighbourDetectRadius);

		List<GameObject> neighboughs = new List<GameObject>();
		for (int i = 0; i < neigbourColliders.Length; i++)
		{
			neighboughs.Add(neigbourColliders[i].gameObject);
		}
		neighboughs.Remove(gameObject); // Remove self from List before iteration.

		foreach (GameObject gameObj in neighboughs)
		{
			float _distanceToCurrentNeighbour = Vector3.Distance(gameObj.transform.position, transform.position);
			if (_distanceToCurrentNeighbour <= neighbourDetectRadius)
			{
				alignment += gameObj.transform.position; // Add neighbour positions for averaging.
				groupSize++;

				Boid otherBoid = gameObj.GetComponent<Boid>();
				if (otherBoid)
				{
					groupSpeed += otherBoid.moveSpeed;
				}
				// Account for separation correction if experiencing proximity intrusion.
				if (_distanceToCurrentNeighbour < separationProximity)
				{
					separation += transform.position - gameObj.transform.position;
				}
			}
		}

		if (groupSize > 0)
		{
			// Calculate center.
			alignment = alignment / groupSize;
			
			// Rudimentary accounting for movement speed variance.
			//moveSpeed = groupSpeed / groupSize;

			Vector3 direction = (alignment + separation) - transform.position;
			if (direction != Vector3.zero)
			{
				transform.rotation = Quaternion.Slerp(transform.rotation,
													Quaternion.LookRotation(direction),
													rotationPercentage * moveSpeed * Time.deltaTime);
			}
		}
	}

	// Managed by BoidsManager, homes in on goal position.
	void ManagedUpdateTrajectory()
	{
		separation 	= Vector3.zero;
		alignment	= Vector3.zero;

		parentBoidsController.allBoids.Remove(gameObject); // Remove self from List before iteration.
		foreach (GameObject gameObj in parentBoidsController.allBoids)
		{
			_distanceToCurrentNeighbour = Vector3.Distance(gameObj.transform.position, transform.position);
			if (_distanceToCurrentNeighbour <= neighbourDetectRadius)
			{
				alignment += gameObj.transform.position; // Add neighbour positions for averaging.
				groupSize++;

				Boid otherBoid = gameObj.GetComponent<Boid>();
				if (otherBoid)
				{
					groupSpeed += otherBoid.moveSpeed;
				}

				// Account for separation correction if experiencing proximity intrusion.
				if (_distanceToCurrentNeighbour < separationProximity)
				{
					separation += transform.position - gameObj.transform.position;
				}
			}
		}
		parentBoidsController.allBoids.Add(gameObject); // Add this back to List

		if (groupSize > 0)
		{
			// Calculate center and add Vector to Goal.
			alignment = (alignment / groupSize) + (parentBoidsController.GetGoalPosition() - transform.position);
			
			// Rudimentary accounting for movement speed variance.
			//moveSpeed = groupSpeed / groupSize;

			Vector3 direction = (alignment + separation) - transform.position;
			if (direction != Vector3.zero)
			{
				transform.rotation = Quaternion.Slerp(transform.rotation,
													Quaternion.LookRotation(direction),
													rotationPercentage * moveSpeed * Time.deltaTime);
			}
		}
	}

	void Awake()
	{
		_spawnLocation = transform.position;
	}
}
