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

			this.secondaryCulture = new TextFieldCombo(this);
			this.secondaryCulture.IsReadOnly = true;
			this.secondaryCulture.ComboClosed += new EventHandler(this.HandleSecondaryCultureComboClosed);

			this.primaryArray = new MyWidgets.StringArray(this);
			this.primaryArray.CellsQuantityChanged += new EventHandler(this.HandlePrimaryArrayCellsQuantityChanged);

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


		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.primaryCulture == null )  return;

			Rectangle box = this.Client.Bounds;
			box.Deflate(5);
			Rectangle part;
			Rectangle rect;

			part = box;
			part.Bottom = part.Top-22;

			rect = part;
			rect.Width = 200;
			this.primaryCulture.Bounds = rect;
			rect.Left = part.Left+200;
			rect.Right = part.Right;
			this.secondaryCulture.Bounds = rect;

			part.Top = part.Bottom-5;
			part.Bottom = box.Bottom;

			rect = part;
			rect.Width = 200;
			this.primaryArray.Bounds = rect;
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

		void HandlePrimaryArrayCellsQuantityChanged(object sender)
		{
			for ( int i=0 ; i<this.primaryBundle.FieldCount ; i++ )
			{
				ResourceBundle.Field field = this.primaryBundle[i];
				this.primaryArray.SetLineString(i, field.AsString);
			}
		}


		protected Module					module;
		protected TextField					primaryCulture;
		protected TextFieldCombo			secondaryCulture;
		protected ResourceBundle			primaryBundle;
		protected ResourceBundle			secondaryBundle;
		protected MyWidgets.StringArray		primaryArray;
	}
}
