using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Comparers
{
	/// <summary>
	///	Compare deux connections Bt ou Bb, afin de répartir astucieusement le point d'arrivé en haut
	/// ou en bas d'une boîte, pour éviter que deux connections arrivent sur le même point.
	/// Les croisements sont minimisés.
	/// L'ordre obtenu correspond aux points d'arrivées de gauche à droite.
	/// </summary>
	public class ConnectionB : IComparer<EntitiesEditor.ObjectConnection>
	{
		public int Compare(EntitiesEditor.ObjectConnection obj1, EntitiesEditor.ObjectConnection obj2)
		{
			if (obj1.Field.Route == EntitiesEditor.Field.RouteType.Bb)
			{
				if (ConnectionB.IsAttachToRight(obj1) != ConnectionB.IsAttachToRight(obj2))
				{
					return ConnectionB.IsAttachToRight(obj1) ? -1 : 1;
				}

				if (ConnectionB.IsAttachToRight(obj1))
				{
					return obj1.Points[0].Y.CompareTo(obj2.Points[0].Y);
				}
				else
				{
					return obj2.Points[0].Y.CompareTo(obj1.Points[0].Y);
				}
			}
			else
			{
				if (ConnectionB.IsAttachToRight(obj1) != ConnectionB.IsAttachToRight(obj2))
				{
					return ConnectionB.IsAttachToRight(obj1) ? -1 : 1;
				}

				if (ConnectionB.IsAttachToRight(obj1))
				{
					return obj2.Points[0].Y.CompareTo(obj1.Points[0].Y);
				}
				else
				{
					return obj1.Points[0].Y.CompareTo(obj2.Points[0].Y);
				}
			}
		}

		protected static bool IsAttachToRight(EntitiesEditor.ObjectConnection connection)
		{
			//	Retourne la direction effective dans laquelle part la connection.
			//	A ne pas confondre avec Field.IsAttachToRight !
			return connection.Points[0].X < connection.Points[connection.Points.Count-1].X;
		}
	}
}
