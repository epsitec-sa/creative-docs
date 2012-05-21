//	Copyright © 2003-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// Définition des poignées d'un rectangle. Il y en a à chaque sommet, au
	/// milieu des côtés et au centre de gravité.
	/// </summary>
	public enum GripId
	{
		None = -1,

		VertexBottomLeft,
		VertexBottomRight,
		VertexTopRight,
		VertexTopLeft,

		EdgeBottom,
		EdgeTop,
		EdgeLeft,
		EdgeRight,

		Body,
	}
}
