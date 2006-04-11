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

			this.array = new MyWidgets.StringArray(this);
			this.array.Columns = 3;
			this.array.SetColumnsRelativeWidth(0, 0.2);
			this.array.SetColumnsRelativeWidth(1, 0.4);
			this.array.SetColumnsRelativeWidth(2, 0.4);
			this.array.CellsQuantityChanged += new EventHandler(this.HandleArrayCellsQuantityChanged);
			this.array.CellsContentChanged += new EventHandler(this.HandleArrayCellsContentChanged);
			this.array.SelectedRowChanged += new EventHandler(this.HandleArraySelectedRowChanged);

			this.primaryEdit = new TextFieldMulti(this);
			this.primaryEdit.TextChanged += new EventHandler(this.HandleEditTextChanged);

			this.secondaryEdit = new TextFieldMulti(this);
			this.secondaryEdit.TextChanged += new EventHandler(this.HandleEditTextChanged);

			this.UpdateCultures();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.secondaryCulture.ComboClosed -= new EventHandler(this.HandleSecondaryCultureComboClosed);
				this.array.CellsQuantityChanged -= new EventHandler(this.HandleArrayCellsQuantityChanged);
				this.array.CellsContentChanged -= new EventHandler(this.HandleArrayCellsContentChanged);
				this.array.SelectedRowChanged -= new EventHandler(this.HandleArraySelectedRowChanged);
				this.primaryEdit.TextChanged -= new EventHandler(this.HandleEditTextChanged);
				this.secondaryEdit.TextChanged -= new EventHandler(this.HandleEditTextChanged);
			}
		}


		protected void UpdateCultures()
		{
			//	Met à jour les widgets pour les cultures.
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
			//	Construit l'index en fonction des ressources primaires.
			this.labelsIndex.Clear();

			foreach (ResourceBundle.Field field in this.primaryBundle.Fields)
			{
				this.labelsIndex.Add(field.Name);
			}
		}

		protected void UpdateArray()
		{
			//	Met à jour tout le contenu du tableau.
			int first = this.array.FirstVisibleRow;
			int total = System.Math.Min(this.labelsIndex.Count, this.array.LineCount);

			this.array.TotalRows = this.labelsIndex.Count;

			for ( int i=0 ; i<total ; i++ )
			{
				if (first+i < this.labelsIndex.Count)
				{
					ResourceBundle.Field primaryField = this.primaryBundle[this.labelsIndex[first+i]];
					ResourceBundle.Field secondaryField = this.secondaryBundle[this.labelsIndex[first+i]];

					this.array.SetLineString(0, first+i, primaryField.Name);
					this.UpdateArrayField(1, first+i, primaryField);
					this.UpdateArrayField(2, first+i, secondaryField);
				}
				else
				{
					this.array.SetLineString(0, first+i, "");
					this.array.SetLineString(1, first+i, "");
					this.array.SetLineString(2, first+i, "");
					this.array.SetLineState(1, first+i, MyWidgets.StringList.CellState.Normal);
					this.array.SetLineState(2, first+i, MyWidgets.StringList.CellState.Normal);
				}
			}
		}

		protected void UpdateArrayField(int row, int column, ResourceBundle.Field field)
		{
			if (field != null)
			{
				string text = field.AsString;
				if (text != null && text != "")
				{
					this.array.SetLineString(row, column, text);
					this.array.SetLineState(row, column, MyWidgets.StringList.CellState.Normal);
					return;
				}
			}

			this.array.SetLineString(row, column, "");
			this.array.SetLineState(row, column, MyWidgets.StringList.CellState.Warning);
		}


		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.primaryCulture == null )  return;

			Rectangle box = this.Client.Bounds;
			box.Deflate(10);
			Rectangle rect;

			//	Il faut obligatoirement s'occuper d'abord de this.array, puisque les autres
			//	widgets dépendent des largeurs relatives de ses colonnes.
			rect = box;
			rect.Top -= 20+5;
			rect.Bottom += 47+5;
			this.array.Bounds = rect;

			rect = box;
			rect.Bottom = rect.Top-20;
			rect.Left += this.array.GetColumnsAbsoluteWidth(0);
			rect.Width = this.array.GetColumnsAbsoluteWidth(1)+1;
			this.primaryCulture.Bounds = rect;
			rect.Left = rect.Right-1;
			rect.Width = this.array.GetColumnsAbsoluteWidth(2);
			this.secondaryCulture.Bounds = rect;

			rect = box;
			rect.Top = rect.Bottom+47;
			rect.Left += this.array.GetColumnsAbsoluteWidth(0);
			rect.Width = this.array.GetColumnsAbsoluteWidth(1)+1;
			this.primaryEdit.Bounds = rect;
			rect.Left = rect.Right-1;
			rect.Width = this.array.GetColumnsAbsoluteWidth(2);
			this.secondaryEdit.Bounds = rect;
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
			this.UpdateArray();
		}

		void HandleArrayCellsContentChanged(object sender)
		{
			this.UpdateArray();
		}

		void HandleArraySelectedRowChanged(object sender)
		{
			int sel = this.array.SelectedRow;
			string text;

			text = this.array.GetLineString(1, sel);
			if (text == null)
			{
				this.primaryEdit.Text = "";
			}
			else
			{
				this.primaryEdit.Text = text;
			}

			text = this.array.GetLineString(2, sel);
			if (text == null)
			{
				this.secondaryEdit.Text = "";
			}
			else
			{
				this.secondaryEdit.Text = text;
			}
		}

		void HandleEditTextChanged(object sender)
		{
			TextFieldMulti edit = sender as TextFieldMulti;
			int sel = this.array.SelectedRow;
			int column = (edit == this.primaryEdit) ? 1 : 2;
			MyWidgets.StringList.CellState state = (edit.Text == "") ? MyWidgets.StringList.CellState.Warning : MyWidgets.StringList.CellState.Normal;

			this.array.SetLineString(column, sel, edit.Text);
			this.array.SetLineState(column, sel, state);
		}


		protected Module					module;
		protected List<string>				labelsIndex;
		protected bool						ignoreChange = false;

		protected TextField					primaryCulture;
		protected TextFieldCombo			secondaryCulture;
		protected ResourceBundle			primaryBundle;
		protected ResourceBundle			secondaryBundle;
		protected MyWidgets.StringArray		array;
		protected TextFieldMulti			primaryEdit;
		protected TextFieldMulti			secondaryEdit;
	}
}
