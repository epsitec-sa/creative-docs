//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// L'interface IContentsComparer permet de comparer le contenu de deux
	/// classes.
	/// </summary>
	public interface IContentsComparer
	{
		bool CompareEqualContents(object value);
	}
}
