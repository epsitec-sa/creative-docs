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
			string xml = form.Serialize();

			if (xml.StartsWith(Xml.XmlHeader))
			{
				xml = xml.Substring(Xml.XmlHeader.Length);
			}

			return xml;
		}

		static public FormDescription DeserializeForm(string xml, ResourceManager manager)
		{
			if (!xml.StartsWith("<?xml"))
			{
				xml = string.Concat(Xml.XmlHeader, xml);
			}

			FormDescription form = new FormDescription();
			form.Deserialize(xml);
			return form;
		}
	}
}
