using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Comparers
{
	/// <summary>
	///	Compare deux connections Bt ou Bb, afin de r�partir astucieusement le point d'arriv� en haut
	/// ou en bas d'une bo�te, pour �viter que deux connections arrivent sur le m�me point.
	/// Les croisements sont minimis�s.
	/// L'ordre obtenu correspond aux points d'arriv�es de gauche � droite.
	/// </summary>
	public class ConnectionB : IComparer<EntitiesEditor.ObjectConnection>
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

			return 0;
		}
	}
}
