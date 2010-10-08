//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.WorkflowDesigner.Comparers
{
	/// <summary>
	///	Compare deux connections pour minimiser les croisements.
	/// </summary>
	public class EdgeComparer : IComparer<Objects.ObjectEdge>
	{
		public int Compare(Objects.ObjectEdge obj1, Objects.ObjectEdge obj2)
		{
			if (obj1.Edge.Route == Objects.Edge.RouteType.Bb)
			{
				if (obj1.IsRightDirection != obj2.IsRightDirection)
				{
					return obj1.IsRightDirection ? -1 : 1;
				}

				if (obj1.IsRightDirection)
				{
					return obj1.Points[0].Y.CompareTo(obj2.Points[0].Y);
				}
				else
				{
					return obj2.Points[0].Y.CompareTo(obj1.Points[0].Y);
				}
			}

			if (obj1.Edge.Route == Objects.Edge.RouteType.Bt)
			{
				if (obj1.IsRightDirection != obj2.IsRightDirection)
				{
					return obj1.IsRightDirection ? -1 : 1;
				}

				if (obj1.IsRightDirection)
				{
					return obj2.Points[0].Y.CompareTo(obj1.Points[0].Y);
				}
				else
				{
					return obj1.Points[0].Y.CompareTo(obj2.Points[0].Y);
				}
			}

			if (obj1.Edge.Route == Objects.Edge.RouteType.C)
			{
				if (obj1.IsRightDirection != obj2.IsRightDirection)
				{
					return obj1.IsRightDirection ? -1 : 1;
				}

				return obj2.Points[0].Y.CompareTo(obj1.Points[0].Y);
			}

			if (obj1.Edge.Route == Objects.Edge.RouteType.D)
			{
				if (obj1.IsRightDirection != obj2.IsRightDirection)
				{
					return obj1.IsRightDirection ? -1 : 1;
				}

				return obj1.Points[0].Y.CompareTo(obj2.Points[0].Y);
			}

			return 0;
		}
	}
}
