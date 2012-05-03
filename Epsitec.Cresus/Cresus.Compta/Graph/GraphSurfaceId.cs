//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;

namespace Epsitec.Cresus.Compta.Graph
{
	/// <summary>
	/// Cette structure identifie une surface, mais sans en connaître les détails géométriques.
	/// </summary>
	public struct GraphSurfaceId
	{
		public GraphSurfaceId(GraphSurfaceType type)
		{
			this.type  = type;
			this.cubeX = -1;
			this.cubeY = -1;
		}

		public GraphSurfaceId(GraphSurfaceType type, int cubeX, int cubeY)
		{
			this.type  = type;
			this.cubeX = cubeX;
			this.cubeY = cubeY;
		}


		public GraphSurfaceType Type
		{
			get
			{
				return this.type;
			}
		}

		public int CubeX
		{
			get
			{
				return this.cubeX;
			}
		}

		public int CubeY
		{
			get
			{
				return this.cubeY;
			}
		}


		public bool IsEmpty
		{
			get
			{
				return this.Type == GraphSurfaceType.Empty;
			}
		}


		public static bool operator ==(GraphSurfaceId a, GraphSurfaceId b)
		{
			return a.type == b.type && a.cubeX == b.cubeX && a.cubeY == b.cubeY;
		}

		public static bool operator !=(GraphSurfaceId a, GraphSurfaceId b)
		{
			return a.type != b.type || a.cubeX != b.cubeX || a.cubeY != b.cubeY;
		}

		public override bool Equals(object obj)
		{
			return (obj is GraphSurfaceId) && (this == (GraphSurfaceId) obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}


		public static readonly GraphSurfaceId Empty = new GraphSurfaceId (GraphSurfaceType.Empty);


		private GraphSurfaceType	type;
		private int					cubeX;
		private int					cubeY;
	}
}
