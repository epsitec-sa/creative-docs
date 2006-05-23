using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Comparer
{
	/// <summary>
	///	Compare deux cultures d'après leurs noms.
	/// </summary>
	public class CultureName : IComparer<Viewers.Strings.CultureInfo>
	{
		public int Compare(Viewers.Strings.CultureInfo a, Viewers.Strings.CultureInfo b)
		{
			return a.Name.CompareTo(b.Name);
		}
	}
}
