using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BKTools.Helpers.Physics
{
	public static class PhysicsUtil
	{
		public static (Vector3 point0, Vector3 point1) GetPoints(this CapsuleCollider coll)
		{
			(Vector3 point0, Vector3 point1) points;
			float offset = coll.height / 2 - coll.radius;
			Vector3 direction = new Vector3 { [coll.direction] = 1 };
			points.point0 = coll.transform.TransformPoint(coll.center + direction * offset);
			points.point1 = coll.transform.TransformPoint(coll.center - direction * offset);
			return points;
		}
	}
}