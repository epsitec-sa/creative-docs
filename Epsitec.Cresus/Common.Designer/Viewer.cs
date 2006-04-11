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

			this.book = new PaneBook(this);
			this.book.Dock = DockStyle.Fill;

			this.primaryPage = new PanePage();
			this.primaryPage.PaneRelativeSize = 50;
			this.primaryPage.PaneMinSize = 100;
			this.book.Items.Add(this.primaryPage);

			this.secondaryPage = new PanePage();
			this.secondaryPage.PaneRelativeSize = 50;
			this.secondaryPage.PaneMinSize = 100;
			this.book.Items.Add(this.secondaryPage);

			this.primaryCulture = new TextField(this.primaryPage);
			this.primaryCulture.IsReadOnly = true;
			this.primaryCulture.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
			this.primaryCulture.Margins = new Margins(10, 10, 10, 10);

			this.secondaryCulture = new TextFieldCombo(this.secondaryPage);
			this.secondaryCulture.IsReadOnly = true;
			this.secondaryCulture.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
			this.secondaryCulture.Margins = new Margins(10, 10, 10, 10);
			this.secondaryCulture.ComboClosed += new EventHandler(this.HandleSecondaryCultureComboClosed);

			this.primaryArray = new MyWidgets.StringArray(this.primaryPage);
			this.primaryArray.Anchor = AnchorStyles.All;
			this.primaryArray.Margins = new Margins(10, 10, 40, 10);
			this.primaryArray.SizeChanged += new EventHandler(this.HandlePrimaryArraySizeChanged);

			this.UpdateCultures();
		}

		public void Dispose()
		{
		}


		protected void UpdateCultures()
		{
			ResourceBundleCollection bundles = this.module.Bundles;

			this.primaryBundle = bundles[ResourceLevel.Default];
			this.primaryCulture.Text = Misc.LongCulture(this.primaryBundle.Culture.Name);

			this.secondaryBundle = null;
			this.secondaryCulture.Items.Clear();
			for ( int b=0 ; b<bundles.Count ; b++ )
			{
				ResourceBundle bundle = bundles[b];
				if ( bundle != this.primaryBundle )
				{
					this.secondaryCulture.Items.Add(Misc.LongCulture(bundle.Culture.Name));
					if ( this.secondaryBundle == null )
					{
						this.secondaryBundle = bundle;
					}
				}
			}

			if ( this.secondaryBundle == null )
			{
				this.secondaryCulture.Text = "";
			}
			else
			{
				this.secondaryCulture.Text = Misc.LongCulture(this.secondaryBundle.Culture.Name);
			}
		}


		void HandleSecondaryCultureComboClosed(object sender)
		{
			//	Changement de la culture secondaire.
			ResourceBundleCollection bundles = this.module.Bundles;

			for ( int b=0 ; b<bundles.Count ; b++ )
			{
				ResourceBundle bundle = bundles[b];

				if ( Misc.LongCulture(bundle.Culture.Name) == secondaryCulture.Text )
				{
					this.secondaryBundle = bundle;
					break;
				}
			}
		}


		void HandlePrimaryArraySizeChanged(object sender)
		{
			for ( int i=0 ; i<this.primaryBundle.FieldCount ; i++ )
			{
				ResourceBundle.Field field = this.primaryBundle[i];
				this.primaryArray.SetLineString(i, field.AsString);
			}
		}


		protected Module module;
		protected PaneBook					book;
		protected PanePage					primaryPage;
		protected PanePage					secondaryPage;
		protected TextField					primaryCulture;
		protected TextFieldCombo			secondaryCulture;
		protected ResourceBundle			primaryBundle;
		protected ResourceBundle			secondaryBundle;
		protected MyWidgets.StringArray		primaryArray;
	}
}
