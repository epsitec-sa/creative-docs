using System.Collections.Generic;

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

namespace Epsitec.Common.FormEngine
{
	/// <summary>
	/// Procédures de sérialisation et désérialisation de masques de saisie.
	/// </summary>
	public static class Serialization
	{
		static public string SerializeForm(FormDescription form)
		{
			//	Retourne la chaîne qui contient la sérialisation d'un masque de saisie.
			return form.Serialize();
		}

		static public FormDescription DeserializeForm(string xml)
		{
			//	Retourne le masque de saisie désérialisé à partir d'une chaîne.
			FormDescription form = new FormDescription();
			form.Deserialize(xml);
			return form;
		}
	}
}
