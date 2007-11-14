using System.Collections.Generic;

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

namespace Epsitec.Common.FormEngine
{
	/// <summary>
	/// Proc�dures de s�rialisation et d�s�rialisation de masques de saisie.
	/// </summary>
	public class Serialization
	{
		static public string SerializeForm(FormDescription form)
		{
			//	Retourne la cha�ne qui contient la s�rialisation d'un masque de saisie.
			return form.Serialize();
		}

		static public FormDescription DeserializeForm(string xml, ResourceManager manager)
		{
			//	Retourne le masque de saisie d�s�rialis� � partir d'une cha�ne.
			FormDescription form = new FormDescription();
			form.Deserialize(xml);
			return form;
		}
	}
}
