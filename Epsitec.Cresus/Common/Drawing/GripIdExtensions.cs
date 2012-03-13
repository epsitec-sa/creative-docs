//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// The <c>GripIdExtensions</c> class provides utility methods related to the
	/// <see cref="GripId"/> enumeration.
	/// </summary>
	public static class GripIdExtensions
	{
		static public bool IsVertex(this GripId gripId)
		{
			switch (gripId)
			{
				case GripId.VertexBottomLeft:
				case GripId.VertexBottomRight:
				case GripId.VertexTopLeft:
				case GripId.VertexTopRight:
					return true;
				
				default:
					return false;
			}
		}


		public static bool IsLeftVertex(this GripId gripId)
		{
			return (gripId == GripId.VertexTopLeft)
			    || (gripId == GripId.VertexBottomLeft);
		}

		public static bool IsRightVertex(this GripId gripId)
		{
			return (gripId == GripId.VertexTopRight)
			    || (gripId == GripId.VertexBottomRight);
		}

		public static bool IsTopVertex(this GripId gripId)
		{
			return (gripId == GripId.VertexTopLeft)
			    || (gripId == GripId.VertexTopRight);
		}

		public static bool IsBottomVertex(this GripId gripId)
		{
			return (gripId == GripId.VertexBottomLeft)
			    || (gripId == GripId.VertexBottomRight);
		}
	}
}
