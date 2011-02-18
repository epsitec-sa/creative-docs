//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
		void DeserializeFromText(TextContext context, string text, int pos, int length);
	}
}
