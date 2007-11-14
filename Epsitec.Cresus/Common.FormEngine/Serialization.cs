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
			string xml = form.Serialize();
#if false
			if (xml.StartsWith(Xml.XmlHeader))  // commence par l'en-t�te "<?xml ...>" ?
			{
				xml = xml.Substring(Xml.XmlHeader.Length);  // supprime l'en-t�te
			}
#endif
			return xml;
		}

		static public FormDescription DeserializeForm(string xml, ResourceManager manager)
		{
			//	Retourne le masque de saisie d�s�rialis� � partir d'une cha�ne.
#if false
			if (!xml.StartsWith("<?xml"))  // manque l'en-t�te "<?xml ...>" ?
			{
				xml = string.Concat(Xml.XmlHeader, xml);  // ajoute l'en-t�te
			}
#endif

			FormDescription form = new FormDescription();
			form.Deserialize(xml);
			return form;
		}
	}
}
