using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Comparers
{
	/// <summary>
	///	Compare deux connections pour minimiser les croisements.
	/// </summary>
	public class Connection : IComparer<EntitiesEditor.ObjectConnection>
	{
		public int Compare(EntitiesEditor.ObjectConnection obj1, EntitiesEditor.ObjectConnection obj2)
		{
			if (obj1.Field.Route == EntitiesEditor.Field.RouteType.Bb)
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

			if (obj1.Field.Route == EntitiesEditor.Field.RouteType.Bt)
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

			if (obj1.Field.Route == EntitiesEditor.Field.RouteType.C)
			{
				if (obj1.IsRightDirection != obj2.IsRightDirection)
				{
					return obj1.IsRightDirection ? -1 : 1;
				}

				return obj2.Points[0].Y.CompareTo(obj1.Points[0].Y);
			}

			if (obj1.Field.Route == EntitiesEditor.Field.RouteType.D)
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
