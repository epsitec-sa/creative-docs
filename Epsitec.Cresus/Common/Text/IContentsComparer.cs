//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
