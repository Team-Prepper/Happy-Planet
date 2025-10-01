using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BKTools.Helpers.Bits
{
	public static class BitUtil
	{
		public static int OverrideBit(this int self, int bitPosition, bool bitValue)
		{
			if (bitPosition < 0 || bitPosition > 31) throw new System.Exception();

			int mask = 1 << bitPosition;
			self = self | mask;
			if (!bitValue) self = self ^ mask;
			return self;
		}

		public static LayerMask Flag(this LayerMask mask, string layerName, bool value)
		{
			mask.value = mask.value.OverrideBit(LayerMask.NameToLayer(layerName), value);
			return mask;
		}
	}
}