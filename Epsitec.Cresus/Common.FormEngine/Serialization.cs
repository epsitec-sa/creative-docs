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
	public class Serialization
	{
		static public string SerializeForm(FormDescription form)
		{
			//	Retourne la chaîne qui contient la sérialisation d'un masque de saisie.
			string xml = form.Serialize();
#if false
			if (xml.StartsWith(Xml.XmlHeader))  // commence par l'en-tête "<?xml ...>" ?
			{
				xml = xml.Substring(Xml.XmlHeader.Length);  // supprime l'en-tête
			}
#endif
			return xml;
		}

		static public FormDescription DeserializeForm(string xml, ResourceManager manager)
		{
			//	Retourne le masque de saisie désérialisé à partir d'une chaîne.
#if false
			if (!xml.StartsWith("<?xml"))  // manque l'en-tête "<?xml ...>" ?
			{
				xml = string.Concat(Xml.XmlHeader, xml);  // ajoute l'en-tête
			}
#endif

			FormDescription form = new FormDescription();
			form.Deserialize(xml);
			return form;
		}
	}
}
