using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer
{
	/// <summary>
	/// Permet de représenter les ressources d'un module.
	/// </summary>
	public class Viewer : Widget
	{
		public Viewer(Module module)
		{
			this.module = module;

			this.primaryCulture = new TextField(this);
			this.primaryCulture.IsReadOnly = true;
			this.primaryCulture.Width = 200;
			this.primaryCulture.Anchor = AnchorStyles.Left | AnchorStyles.Top;
			this.primaryCulture.Margins = new Margins(10, 0, 10, 0);

			this.secondaryCulture = new TextFieldCombo(this);
			this.secondaryCulture.Width = 200;
			this.secondaryCulture.Anchor = AnchorStyles.Left | AnchorStyles.Top;
			this.secondaryCulture.Margins = new Margins(10+200+10, 0, 10, 0);

			this.UpdateCultures();
		}

		public void Dispose()
		{
		}


		protected void UpdateCultures()
		{
			ResourceBundleCollection bundles = this.module.Bundles;

			ResourceBundle def = bundles[ResourceLevel.Default];
			this.primaryCulture.Text = Misc.LongCulture(def.Culture.Name);

			string first = "";
			this.secondaryCulture.Items.Clear();
			for ( int b=0 ; b<bundles.Count ; b++ )
			{
				ResourceBundle bundle = bundles[b];
				if ( bundle != def )
				{
					string culture = Misc.LongCulture(bundle.Culture.Name);
					this.secondaryCulture.Items.Add(culture);
					if ( first == "" )
					{
						first = culture;
					}
				}
			}
			this.secondaryCulture.Text = first;
		}


		protected Module					module;
		protected TextField					primaryCulture;
		protected TextFieldCombo			secondaryCulture;
	}
}
