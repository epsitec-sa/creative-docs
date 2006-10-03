//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>AbstractGroupDescription</c> class provides an abstract class for
	/// types that describe how to divide the items in a collection into groups.
	/// </summary>
	public abstract class AbstractGroupDescription
	{
		public abstract string[] GetGroupNamesForItem(object item, System.Globalization.CultureInfo culture);
		
		public virtual bool NamesMatch(string groupName, string itemName)
		{
			return string.Equals (groupName, itemName, System.StringComparison.Ordinal);
		}
	}
}
