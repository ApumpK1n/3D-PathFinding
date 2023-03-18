using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Pumpkin
{

	public class RobotMovementController : MonoBehaviour
	{
		[SerializeField] private Transform target;
		[SerializeField] private float maxDistanceRebuildPath = 1;
		[SerializeField] private float acceleration = 1;
		[SerializeField] private float minReachDistance = 2f;
		[SerializeField] private float minFollowDistance = 4f;
		[SerializeField] private float pathPointRadius = 0.2f;
		[SerializeField] private OctTree octree;
		[SerializeField] private LayerMask playerSeeLayerMask = -1;
		[SerializeField] private GameObject playerObject;
		private List<Element> oldPath;
		private List<Element> newPath;
		private Rigidbody rigidbody;
		private Vector3 currentDestination;
		private Vector3 lastDestination;
		private SphereCollider sphereCollider;

		// Use this for initialization
		void Start()
		{
			sphereCollider = GetComponent<SphereCollider>();
			rigidbody = GetComponent<Rigidbody>();
		}

		// Update is called once per frame
		void FixedUpdate()
		{
			//if ((newPath == null /*|| !newPath.isCalculating */) && Vector3.SqrMagnitude(target.transform.position - lastDestination) > maxDistanceRebuildPath && (!CanSeePlayer() || Vector3.Distance(target.position, transform.position) > minFollowDistance) && !octree.IsBuilding)
			//{
			//	lastDestination = target.transform.position;

			//	oldPath = newPath;
			//	newPath = octree.GetPath(transform.position, lastDestination);
			//}

			if (newPath == null && Vector3.SqrMagnitude(target.transform.position - lastDestination) > maxDistanceRebuildPath
				&& Vector3.Distance(target.position, transform.position) > minFollowDistance && !octree.IsBuilding)
			{
				lastDestination = target.transform.position;
				oldPath = newPath;
				newPath = octree.GetPath(transform.position, lastDestination);
			}

			var curPath = Path;

			if (/*!curPath.isCalculating &&*/ curPath != null && curPath.Count > 0)
			{
				if (Vector3.Distance(transform.position, target.position) < minFollowDistance && CanSeePlayer())
				{
					//curPath.Reset();
				}

				currentDestination = curPath[0].Bounds.center + Vector3.ClampMagnitude(rigidbody.position - curPath[0].Bounds.center, pathPointRadius);

				rigidbody.velocity += Vector3.ClampMagnitude(currentDestination - transform.position, 1) * Time.deltaTime * acceleration;
				float sqrMinReachDistance = minReachDistance * minReachDistance;

				Vector3 predictedPosition = rigidbody.position + rigidbody.velocity * Time.deltaTime;
				float shortestPathDistance = Vector3.SqrMagnitude(predictedPosition - currentDestination);
				int shortestPathPoint = 0;

				for (int i = 0; i < curPath.Count; i++)
				{
					float sqrDistance = Vector3.SqrMagnitude(rigidbody.position - curPath[i].Bounds.center);
					if (sqrDistance <= sqrMinReachDistance)
					{
						if (i < curPath.Count)
						{
							curPath.RemoveRange(0, i + 1);
						}
						shortestPathPoint = 0;
						break;
					}

					float sqrPredictedDistance = Vector3.SqrMagnitude(predictedPosition - curPath[i].Bounds.center);
					if (sqrPredictedDistance < shortestPathDistance)
					{
						shortestPathDistance = sqrPredictedDistance;
						shortestPathPoint = i;
					}
				}

				if (shortestPathPoint > 0)
				{
					curPath.RemoveRange(0, shortestPathPoint);
				}
			}
			else
			{
				rigidbody.velocity -= rigidbody.velocity * Time.deltaTime * acceleration;
			}
		}

		private bool CanSeePlayer()
		{
			RaycastHit hit;
			if (Physics.Raycast(new Ray(transform.position, transform.position - target.position), out hit, Vector3.Distance(transform.position, target.position) + 1, playerSeeLayerMask))
			{
				return hit.transform.gameObject == playerObject;
			}
			return false;
		}

		private List<Element> Path
		{
			get
			{
				if ((newPath == null /*|| newPath.isCalculating */) && oldPath != null)
				{
					return oldPath;
				}
				return newPath;
			}
		}

		public bool HasTarget
		{
			get
			{
				return Path != null && Path.Count > 0;

			}
		}

		public Vector3 CurrentTargetPosition
		{
			get
			{
				if (Path != null && Path.Count > 0)
				{
					return currentDestination;
				}
				else
				{
					return target.position;
				}
			}
		}

		private void OnDrawGizmosSelected()
		{
			if (rigidbody != null)
			{
				Gizmos.color = Color.blue;
				Vector3 predictedPosition = rigidbody.position + rigidbody.velocity * Time.deltaTime;
				Gizmos.DrawWireSphere(predictedPosition, sphereCollider.radius);
			}

			if (Path != null)
			{
				var path = Path;
				for (int i = 0; i < path.Count - 1; i++)
				{
					Vector3 pos = path[i].Bounds.center;
					Gizmos.color = Color.yellow;
					Gizmos.DrawWireSphere(pos, minReachDistance);
					Gizmos.color = Color.red;
					Gizmos.DrawRay(pos, Vector3.ClampMagnitude(rigidbody.position - pos, pathPointRadius));
					Gizmos.DrawWireSphere(pos, pathPointRadius);
					Gizmos.DrawLine(pos, path[i+1].Bounds.center);
				}
			}
		}

		public void GetPath()
        {
			octree.GetPath(transform.position, lastDestination);
		}
	}
}

