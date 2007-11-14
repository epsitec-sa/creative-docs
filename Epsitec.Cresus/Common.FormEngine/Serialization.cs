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
			return form.Serialize();
		}

		static public FormDescription DeserializeForm(string xml, ResourceManager manager)
		{
			FormDescription form = new FormDescription();
			form.Deserialize(xml);
			return form;
		}
	}
}
