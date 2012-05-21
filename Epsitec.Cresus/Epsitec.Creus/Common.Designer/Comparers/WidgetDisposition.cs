using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Comparers
{
	/// <summary>
	///	Compare deux widgets pour permettre de les trier selon leurs positions géographiques.
	///	Le premier sera en haut à gauche et le dernier en bas à droite.
	/// </summary>
	public class WidgetDisposition : IComparer<Widget>
	{
		public int Compare(Widget obj1, Widget obj2)
		{
			Point c1 = obj1.ActualBounds.Center;
			Point c2 = obj2.ActualBounds.Center;

			int comp = c2.Y.CompareTo(c1.Y);  // de haut en bas !
			if (comp == 0)
			{
				comp = c1.X.CompareTo(c2.X);  // de gauche à droite
			}
			return comp;
		}
	}
}
