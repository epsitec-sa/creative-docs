//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// The <c>GripIds</c> class provides explicit utility methods related to the
	/// <see cref="GripId"/> enumeration.
	/// </summary>
	public static class GripIds
	{
		public static IEnumerable<GripId> All()
		{
			yield return GripId.Body;
			yield return GripId.VertexBottomLeft;
			yield return GripId.VertexBottomRight;
			yield return GripId.VertexTopRight;
			yield return GripId.VertexTopLeft;
			yield return GripId.EdgeBottom;
			yield return GripId.EdgeRight;
			yield return GripId.EdgeTop;
			yield return GripId.EdgeLeft;
		}
	}
}
