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

			this.labelsIndex = new List<string>();

			this.primaryCulture = new TextField(this);
			this.primaryCulture.IsReadOnly = true;

			this.secondaryCulture = new TextFieldCombo(this);
			this.secondaryCulture.IsReadOnly = true;
			this.secondaryCulture.ComboClosed += new EventHandler(this.HandleSecondaryCultureComboClosed);

			this.labelsArray = new MyWidgets.StringArray(this);
			this.labelsArray.Name = "Labels";
			this.labelsArray.CellsQuantityChanged += new EventHandler(this.HandleArrayCellsQuantityChanged);

			this.primaryArray = new MyWidgets.StringArray(this);
			this.primaryArray.Name = "Primary";
			this.primaryArray.CellsQuantityChanged += new EventHandler(this.HandleArrayCellsQuantityChanged);

			this.secondaryArray = new MyWidgets.StringArray(this);
			this.secondaryArray.Name = "Secondary";
			this.secondaryArray.CellsQuantityChanged += new EventHandler(this.HandleArrayCellsQuantityChanged);

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

			this.UpdateLabelsIndex();
		}

		protected void UpdateLabelsIndex()
		{
			this.labelsIndex.Clear();

			foreach (ResourceBundle.Field field in this.primaryBundle.Fields)
			{
				this.labelsIndex.Add(field.Name);
			}
		}

		protected void UpdateArrays(string name)
		{
			int first = this.firstIndex;
			int total = System.Math.Min(this.labelsIndex.Count, this.labelsArray.LineCount);

			for ( int i=0 ; i<total ; i++ )
			{
				ResourceBundle.Field primaryField = this.primaryBundle[this.labelsIndex[first+i]];
				ResourceBundle.Field secondaryField = this.secondaryBundle[this.labelsIndex[first+i]];

				if ( name == "Labels" )
				{
					this.labelsArray.SetLineString(i, primaryField.Name);
				}

				if ( name == "Primary" )
				{
					this.primaryArray.SetLineString(i, primaryField.AsString);
				}

				if ( name == "Secondary" )
				{
					if (secondaryField == null)
					{
						this.secondaryArray.SetLineString(i, "");
					}
					else
					{
						this.secondaryArray.SetLineString(i, secondaryField.AsString);
					}
				}
			}
		}


		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.primaryCulture == null )  return;

			Rectangle box = this.Client.Bounds;
			box.Deflate(10);
			Rectangle part;
			Rectangle rect;

			part = box;
			part.Bottom = part.Top-22;

			rect = part;
			rect.Left += this.labelsWidth;
			rect.Width = this.primaryWidth+1;
			this.primaryCulture.Bounds = rect;
			rect.Left = rect.Right-1;
			rect.Right = part.Right;
			this.secondaryCulture.Bounds = rect;

			part.Top = part.Bottom-5;
			part.Bottom = box.Bottom;

			rect = part;
			rect.Width = this.labelsWidth+1;
			this.labelsArray.Bounds = rect;
			rect.Left = rect.Right-1;
			rect.Width = this.primaryWidth+1;
			this.primaryArray.Bounds = rect;
			rect.Left = rect.Right-1;
			rect.Right = part.Right;
			this.secondaryArray.Bounds = rect;
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

		void HandleArrayCellsQuantityChanged(object sender)
		{
			MyWidgets.StringArray array = sender as MyWidgets.StringArray;

			this.UpdateArrays(array.Name);
		}


		protected Module					module;
		protected List<string>				labelsIndex;
		protected int						firstIndex;

		protected TextField					primaryCulture;
		protected TextFieldCombo			secondaryCulture;
		protected ResourceBundle			primaryBundle;
		protected ResourceBundle			secondaryBundle;
		protected MyWidgets.StringArray		labelsArray;
		protected MyWidgets.StringArray		primaryArray;
		protected MyWidgets.StringArray		secondaryArray;
		protected double					labelsWidth = 150;
		protected double					primaryWidth = 200;
	}
}
