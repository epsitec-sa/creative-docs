//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// L'interface ISerializableAsText permet de sérialiser et désérialiser
	/// très simplement dans un format lisible le contenu d'objets.
	/// </summary>
	public interface ISerializableAsText
	{
		void SerializeToText(System.Text.StringBuilder buffer);
		void DeserializeFromText(Context context, string text, int pos, int length);
	}
}
