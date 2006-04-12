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

			this.primaryAbout = new TextField(this);
			this.primaryAbout.TextChanged += new EventHandler(this.HandleAboutTextChanged);

			this.secondaryAbout = new TextField(this);
			this.secondaryAbout.TextChanged += new EventHandler(this.HandleAboutTextChanged);

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
				this.primaryAbout.TextChanged -= new EventHandler(this.HandleAboutTextChanged);
				this.secondaryAbout.TextChanged -= new EventHandler(this.HandleAboutTextChanged);
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
			rect.Bottom += 47+5+20+5;
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
			rect.Top = rect.Bottom+47+20+5;
			rect.Bottom = rect.Top-47;
			rect.Left += this.array.GetColumnsAbsoluteWidth(0);
			rect.Width = this.array.GetColumnsAbsoluteWidth(1)+1;
			this.primaryEdit.Bounds = rect;
			rect.Left = rect.Right-1;
			rect.Width = this.array.GetColumnsAbsoluteWidth(2);
			this.secondaryEdit.Bounds = rect;

			rect = box;
			rect.Top = rect.Bottom+20;
			rect.Bottom = rect.Top-20;
			rect.Left += this.array.GetColumnsAbsoluteWidth(0);
			rect.Width = this.array.GetColumnsAbsoluteWidth(1)+1;
			this.primaryAbout.Bounds = rect;
			rect.Left = rect.Right-1;
			rect.Width = this.array.GetColumnsAbsoluteWidth(2);
			this.secondaryAbout.Bounds = rect;
		}


		protected void SetTextField(AbstractTextField field, string text)
		{
			if (text == null)
			{
				field.Text = "";
			}
			else
			{
				field.Text = text;
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
			this.ignoreChange = true;

			int sel = this.array.SelectedRow;
			if ( sel == -1 )
			{
				this.primaryEdit.Text = "";
				this.secondaryEdit.Text = "";
				this.primaryAbout.Text = "";
				this.secondaryAbout.Text = "";
			}
			else
			{
				string label = this.labelsIndex[sel];

				this.SetTextField(this.primaryEdit, this.primaryBundle[label].AsString);
				this.SetTextField(this.secondaryEdit, this.secondaryBundle[label].AsString);

				this.SetTextField(this.primaryAbout, this.primaryBundle[label].About);
				this.SetTextField(this.secondaryAbout, this.secondaryBundle[label].About);
			}

			this.ignoreChange = false;
		}

		void HandleEditTextChanged(object sender)
		{
			if ( this.ignoreChange )  return;

			TextFieldMulti edit = sender as TextFieldMulti;
			string text = edit.Text;
			int sel = this.array.SelectedRow;
			string label = this.labelsIndex[sel];

			if (edit == this.primaryEdit)
			{
				this.primaryBundle[label].SetStringValue(text);
			}

			if (edit == this.secondaryEdit)
			{
				this.secondaryBundle[label].SetStringValue(text);
			}

			int column = (edit == this.primaryEdit) ? 1 : 2;
			MyWidgets.StringList.CellState state = (text == "") ? MyWidgets.StringList.CellState.Warning : MyWidgets.StringList.CellState.Normal;

			this.array.SetLineString(column, sel, text);
			this.array.SetLineState(column, sel, state);

			this.module.Modifier.IsDirty = true;
		}

		void HandleAboutTextChanged(object sender)
		{
			if ( this.ignoreChange )  return;

			TextField edit = sender as TextField;
			string text = edit.Text;
			int sel = this.array.SelectedRow;
			string label = this.labelsIndex[sel];

			if (edit == this.primaryAbout)
			{
				this.primaryBundle[label].SetAbout(text);
			}

			if (edit == this.secondaryAbout)
			{
				this.secondaryBundle[label].SetAbout(text);
			}

			this.module.Modifier.IsDirty = true;
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
		protected TextField					primaryAbout;
		protected TextField					secondaryAbout;
	}
}
