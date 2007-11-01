using System.Collections.Generic;

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

namespace Epsitec.Common.FormEngine
{
	public class FormEngine
	{
		public FormEngine()
		{
			//	Constructeur.
		}

		public Widget CreateForm(StructuredData data)
		{
			//	Crée un masque de saisie pour une entité donnée.
			Widget root = new Widget();

#if false
			IList<StructuredData> fields = data.GetValue(Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;
			foreach (StructuredData field in fields)
			{
				Druid id = (Druid) field.GetValue(Res.Fields.Field.CaptionId);
			}
#endif

			StaticText t;

			t = new StaticText(root);
			t.Text = "Coucou";
			t.Dock = DockStyle.Top;

			t = new StaticText(root);
			t.Text = "Tralala";
			t.Dock = DockStyle.Top;

			return root;
		}
	}
}
