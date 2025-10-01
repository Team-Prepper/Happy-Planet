using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BKTools.Helpers.Common
{
	public static class CommonUtil
	{
		public static void DestroyAllChildren(this Transform tf)
		{
			foreach (Transform child in tf)
			{
				Object.Destroy(child.gameObject);
			}
		}
	}
}