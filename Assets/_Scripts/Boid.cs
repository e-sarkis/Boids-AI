using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoidType
{
	AUTONOMOUS,
	MANAGED
};

public class Boid : MonoBehaviour 
{
	// Reference to BoidsController object for Managed Boid objects
	[HideInInspector] public BoidsController parentBoidsController;
	public BoidType boidType;
	public float moveSpeed;				// Movement speed in editor units.
	public float rotationPercentage;	// % of move speed for rotation speed.
	public float separationProximity;	// Min distance from neighbough for corrective steering.
	public float neighbourDetectRadius;	// Boid objects within radius will comprise neigbour group

	// Autonomous Boid objects home in on initial spawn location after exceeding this.
	public float autonomousTravelRadius;

	private Vector3 _separation	= Vector3.zero; // Avoidance vector for separation.
	private Vector3 _alignment 	= Vector3.zero; // Alignment vector for positioning within group.
	private Vector3 _cohesion	= Vector3.zero; // Direction vector for _cohesion.
	private Vector3 _groupAverageHeading;
	private Vector3 _groupAveragePosition;		// Averaged position of all neighbours.
	private Vector3	_spawnLocation;				// The spawn location of this Boid.
	private int _groupSize 		= 0;			// Total Boids in neighbour group.
	private float _groupSpeed 	= 0f; 			// Total speed of neighbour group.
	private float 	_distanceToCurrentNeighbour;

	private TrajectoryUpdate trajectoryUpdate; // Delegate used to update trajectory of this Boid.

	void Awake()
	{
		_spawnLocation = transform.position;
		SetTrajectoryUpdate();
	}

	void Update() 
	{
		trajectoryUpdate();
		// Translate along Z-axis in calculated trajectory
		transform.Translate(0, 0, Time.deltaTime * moveSpeed);
	}

	delegate void TrajectoryUpdate();

	void SetTrajectoryUpdate()
	{
		switch (boidType)
		{
			case BoidType.AUTONOMOUS:
				trajectoryUpdate = AutonomousUpdateTrajectory;
				break;
			case BoidType.MANAGED:
				trajectoryUpdate = ManagedUpdateTrajectory;
				break;
			default:
				break;
		}
	}

	// Leaderless. Stays within travel radius and has no consideration for goal position.
	void AutonomousUpdateTrajectory()
	{
		_separation	= Vector3.zero;
		_alignment	= Vector3.zero;

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
				_alignment += gameObj.transform.position; // Add neighbour positions for averaging.
				_groupSize++;

				Boid otherBoid = gameObj.GetComponent<Boid>();
				if (otherBoid)
				{
					_groupSpeed += otherBoid.moveSpeed;
				}
				// Account for _separation correction if experiencing proximity intrusion.
				if (_distanceToCurrentNeighbour < separationProximity)
				{
					_separation += transform.position - gameObj.transform.position;
				}
			}
		}

		if (_groupSize > 0)
		{
			// Calculate center.
			_alignment = _alignment / _groupSize;
			
			// Rudimentary accounting for movement speed variance.
			//moveSpeed = _groupSpeed / _groupSize;

			_cohesion = (_alignment + _separation) - transform.position;
			if (_cohesion != Vector3.zero)
			{
				transform.rotation = Quaternion.Slerp(transform.rotation,
													Quaternion.LookRotation(_cohesion),
													rotationPercentage * moveSpeed * Time.deltaTime);
			}
		}
	}

	// Managed by BoidsManager. Homes in on goal position while accounting for neighbours.
	void ManagedUpdateTrajectory()
	{
		_separation	= Vector3.zero;
		_alignment	= Vector3.zero;

		parentBoidsController.allBoidGameObjects.Remove(gameObject); // Remove self from List before iteration.
		foreach (GameObject gameObj in parentBoidsController.allBoidGameObjects)
		{
			_distanceToCurrentNeighbour = Vector3.Distance(gameObj.transform.position, transform.position);
			if (_distanceToCurrentNeighbour <= neighbourDetectRadius)
			{
				_alignment += gameObj.transform.position; // Add neighbour positions for averaging.
				_groupSize++;

				Boid otherBoid = gameObj.GetComponent<Boid>();
				if (otherBoid)
				{
					_groupSpeed += otherBoid.moveSpeed;
				}

				// Account for _separation correction if experiencing proximity intrusion.
				if (_distanceToCurrentNeighbour < separationProximity)
				{
					_separation += transform.position - gameObj.transform.position;
				}
			}
		}
		parentBoidsController.allBoidGameObjects.Add(gameObject); // Add this back to List

		if (_groupSize > 0)
		{
			// Calculate center and add Vector to Goal.
			_alignment = (_alignment / _groupSize) + (parentBoidsController.GetGoalPosition() - transform.position);
			
			// Rudimentary accounting for movement speed variance.
			//moveSpeed = _groupSpeed / _groupSize;

			_cohesion = (_alignment + _separation) - transform.position;
			if (_cohesion != Vector3.zero)
			{
				transform.rotation = Quaternion.Slerp(transform.rotation,
													Quaternion.LookRotation(_cohesion),
													rotationPercentage * moveSpeed * Time.deltaTime);
			}
		}
	}
}
