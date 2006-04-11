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
			this.labelsArray.DraggingCellSelectionChanged += new EventHandler(this.HandleArrayDraggingCellSelectionChanged);
			this.labelsArray.DraggingCellSelectionChanged += new EventHandler(this.HandleArrayFinalCellSelectionChanged);

			this.primaryArray = new MyWidgets.StringArray(this);
			this.primaryArray.Name = "Primary";
			this.primaryArray.CellsQuantityChanged += new EventHandler(this.HandleArrayCellsQuantityChanged);
			this.primaryArray.DraggingCellSelectionChanged += new EventHandler(this.HandleArrayDraggingCellSelectionChanged);
			this.primaryArray.DraggingCellSelectionChanged += new EventHandler(this.HandleArrayFinalCellSelectionChanged);

			this.secondaryArray = new MyWidgets.StringArray(this);
			this.secondaryArray.Name = "Secondary";
			this.secondaryArray.CellsQuantityChanged += new EventHandler(this.HandleArrayCellsQuantityChanged);
			this.secondaryArray.DraggingCellSelectionChanged += new EventHandler(this.HandleArrayDraggingCellSelectionChanged);
			this.secondaryArray.DraggingCellSelectionChanged += new EventHandler(this.HandleArrayFinalCellSelectionChanged);

			this.scroller = new VScroller(this);
			this.scroller.IsInverted = true;
			this.scroller.ValueChanged += new EventHandler(this.HandleScrollerValueChanged);

			this.UpdateCultures();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.secondaryCulture.ComboClosed -= new EventHandler(this.HandleSecondaryCultureComboClosed);
				this.labelsArray.CellsQuantityChanged -= new EventHandler(this.HandleArrayCellsQuantityChanged);
				this.labelsArray.DraggingCellSelectionChanged -= new EventHandler(this.HandleArrayDraggingCellSelectionChanged);
				this.labelsArray.DraggingCellSelectionChanged -= new EventHandler(this.HandleArrayFinalCellSelectionChanged);
				this.primaryArray.CellsQuantityChanged -= new EventHandler(this.HandleArrayCellsQuantityChanged);
				this.primaryArray.DraggingCellSelectionChanged -= new EventHandler(this.HandleArrayDraggingCellSelectionChanged);
				this.primaryArray.DraggingCellSelectionChanged -= new EventHandler(this.HandleArrayFinalCellSelectionChanged);
				this.secondaryArray.CellsQuantityChanged -= new EventHandler(this.HandleArrayCellsQuantityChanged);
				this.secondaryArray.DraggingCellSelectionChanged -= new EventHandler(this.HandleArrayDraggingCellSelectionChanged);
				this.secondaryArray.DraggingCellSelectionChanged -= new EventHandler(this.HandleArrayFinalCellSelectionChanged);
				this.scroller.ValueChanged -= new EventHandler(this.HandleScrollerValueChanged);
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

		protected void UpdateArrays(string name)
		{
			//	Met à jour un tableau (donc une colonne).
			int first = this.firstIndex;
			int total = System.Math.Min(this.labelsIndex.Count, this.labelsArray.LineCount);

			for ( int i=0 ; i<total ; i++ )
			{
				if (name == "Labels")
				{
					if (first+i < this.labelsIndex.Count)
					{
						ResourceBundle.Field primaryField = this.primaryBundle[this.labelsIndex[first+i]];
						this.labelsArray.SetLineString(i, primaryField.Name);
						this.labelsArray.SetLineState(i, MyWidgets.StringArray.CellState.Normal);
					}
					else
					{
						this.labelsArray.SetLineString(i, "");
						this.labelsArray.SetLineState(i, MyWidgets.StringArray.CellState.Warning);
					}
				}

				if (name == "Primary")
				{
					this.UpdateArraysField(this.primaryBundle, this.primaryArray, first, i);
				}

				if (name == "Secondary")
				{
					this.UpdateArraysField(this.secondaryBundle, this.secondaryArray, first, i);
				}
			}
		}

		protected void UpdateArraysField(ResourceBundle bundle, MyWidgets.StringArray array, int first, int i)
		{
			if (first+i < this.labelsIndex.Count)
			{
				ResourceBundle.Field field = bundle[this.labelsIndex[first+i]];

				if (field != null)
				{
					string text = field.AsString;
					if (text != null && text != "")
					{
						array.SetLineString(i, text);
						array.SetLineState(i, MyWidgets.StringArray.CellState.Normal);
						return;
					}
				}
			}

			array.SetLineString(i, "");
			array.SetLineState(i, MyWidgets.StringArray.CellState.Warning);
		}

		protected void UpdateScroller()
		{
			this.ignoreChange = true;

			if (this.labelsIndex.Count <= this.labelsArray.LineCount)
			{
				this.scroller.Enable = false;
			}
			else
			{
				this.scroller.Enable = true;
				this.scroller.MinValue = (decimal) 0;
				this.scroller.MaxValue = (decimal) (this.labelsIndex.Count - this.labelsArray.LineCount);
				this.scroller.Value = (decimal) this.firstIndex;
				this.scroller.VisibleRangeRatio = (decimal) this.labelsArray.LineCount / (decimal) this.labelsIndex.Count;
				this.scroller.LargeChange = (decimal) this.labelsArray.LineCount-1;
				this.scroller.SmallChange = (decimal) 1;
			}

			this.ignoreChange = false;
		}


		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.primaryCulture == null )  return;

			Rectangle box = this.Client.Bounds;
			box.Deflate(10);
			box.Width -= this.scroller.Width;
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

			rect = box;
			rect.Left = rect.Right;
			rect.Width = scroller.Width;
			rect.Top -= 22+5;
			this.scroller.Bounds = rect;
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
			this.UpdateScroller();
		}

		void HandleArrayDraggingCellSelectionChanged(object sender)
		{
			MyWidgets.StringArray array = sender as MyWidgets.StringArray;
			int sel = array.CellSelected;
			this.labelsArray.CellSelected = sel;
			this.primaryArray.CellSelected = sel;
			this.secondaryArray.CellSelected = sel;
		}

		void HandleArrayFinalCellSelectionChanged(object sender)
		{
		}

		void HandleScrollerValueChanged(object sender)
		{
			if ( this.ignoreChange )  return;
			this.firstIndex = (int) System.Math.Floor(this.scroller.DoubleValue+0.5);
			this.UpdateArrays("Labels");
			this.UpdateArrays("Primary");
			this.UpdateArrays("Secondary");
		}


		protected Module					module;
		protected List<string>				labelsIndex;
		protected int						firstIndex;
		protected bool						ignoreChange = false;

		protected TextField					primaryCulture;
		protected TextFieldCombo			secondaryCulture;
		protected ResourceBundle			primaryBundle;
		protected ResourceBundle			secondaryBundle;
		protected MyWidgets.StringArray		labelsArray;
		protected MyWidgets.StringArray		primaryArray;
		protected MyWidgets.StringArray		secondaryArray;
		protected VScroller					scroller;
		protected double					labelsWidth = 150;
		protected double					primaryWidth = 200;
	}
}
