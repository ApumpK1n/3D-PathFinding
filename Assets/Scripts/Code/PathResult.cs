using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pumpkin
{
	public class PathRequest
	{
		internal Vector3 from;
		internal Vector3 to;
		internal List<Vector3> path;
		internal bool isCalulated;
		internal bool isCalculating;

		public PathRequest()
		{
			path = new List<Vector3>();
		}

		public List<Vector3> Path
		{
			get { return path; }
		}

		public void Reset()
		{
			isCalulated = false;
			isCalculating = false;
			path.Clear();
		}
	}
}
