//	Copyright � 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Graph;
using Epsitec.Cresus.Compta.Search.Data;

using System.Collections.Generic;

namespace Epsitec.Cresus.Compta.Widgets
{
	/// <summary>
	/// Tableau d'une colonne de TextLayout (vus comme des string).
	/// </summary>
	public class StringList : Widget, Epsitec.Common.Widgets.Helpers.IToolTipHost
	{
		public enum CellState
		{
			Normal,
			Warning,
			Modified,
			Disabled,
			Unused,
		}


		public StringList() : base()
		{
			this.graphEngine = new GraphEngine ();

			this.AutoEngage = false;
			this.AutoFocus = true;
			this.AutoDoubleClick = true;

			this.InternalState |= WidgetInternalState.Focusable;
			this.InternalState |= WidgetInternalState.Engageable;

			this.searchLocatorLine = -1;
		}

		public StringList(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		public Cube Cube
		{
			get
			{
				return this.cube;
			}
			set
			{
				this.cube = value;
			}
		}

		public GraphOptions GraphOptions
		{
			get
			{
				return this.graphOptions;
			}
			set
			{
				this.graphOptions = value;
			}
		}


		public Color ColorSelection
		{
			//	Couleur utilis�e pour les lignes s�lectionn�es.
			get
			{
				return this.colorSelection;
			}

			set
			{
				if (this.colorSelection != value)
				{
					this.colorSelection = value;
					this.Invalidate ();
				}
			}
		}

		public Color ColorHilite
		{
			//	Couleur utilis�e pour les lignes mises en �vidence.
			get
			{
				return this.colorHilite;
			}

			set
			{
				if (this.colorHilite != value)
				{
					this.colorHilite = value;
					this.Invalidate ();
				}
			}
		}

		public double LineHeight
		{
			//	Hauteur d'une ligne.
			get
			{
				return this.lineHeight;
			}

			set
			{
				if ( this.lineHeight != value )
				{
					this.lineHeight = value;
					this.UpdateClientGeometry();
				}
			}
		}

		public double RelativeWidth
		{
			//	Largeur relative (dans n'importe quelle unit�) de la colonne.
			get
			{
				return this.relativeWidth;
			}

			set
			{
				if ( this.relativeWidth != value )
				{
					this.relativeWidth = value;
				}
			}
		}

		public ContentAlignment Alignment
		{
			//	Alignement des textes pour la colonne.
			get
			{
				return this.alignment;
			}

			set
			{
				this.alignment = value;
			}
		}

		public TextBreakMode BreakMode
		{
			//	C�sure des textes pour la colonne.
			get
			{
				return this.breakMode;
			}

			set
			{
				this.breakMode = value;
			}
		}

		public int LineCount
		{
			//	Nombre total de ligne en fonction de la hauteur du widget et de la hauteur d'une ligne.
			get
			{
				if (this.cells == null)
				{
					this.UpdateClientGeometry();
				}

				if (this.cells == null)
				{
					return 0;
				}

				return this.cells.Length;
			}
		}

		public void SetLineString(int index, string text)
		{
			//	Sp�cifie le texte contenu dans une ligne.
			if ( this.cells == null )  return;
			if ( index < 0 || index >= this.cells.Length )  return;

			this.forceLeftAlignment[index] = false;
			this.forceRightAlignment[index] = false;

			if (text.StartsWith (StringArray.SpecialContentLeftAlignment))
			{
				text = text.Substring (StringArray.SpecialContentLeftAlignment.Length);
				this.forceLeftAlignment[index] = true;
			}

			if (text.StartsWith (StringArray.SpecialContentRightAlignment))
			{
				text = text.Substring (StringArray.SpecialContentRightAlignment.Length);
				this.forceRightAlignment[index] = true;
			}

			if (this.cells[index].TextLayout.Text != text)
			{
				this.cells[index].TextLayout.Text = text;
				this.Invalidate();
			}
		}

		public string GetLineString(int index)
		{
			//	Retourne le texte contenu dans une ligne.
			if ( this.cells == null )  return null;
			if ( index < 0 || index >= this.cells.Length )  return null;
			return this.cells[index].TextLayout.Text;
		}

		public void SetLineTooltip(int index, string text)
		{
			//	Sp�cifie le tooltip d'une ligne.
			if ( this.cells == null )  return;
			if ( index < 0 || index >= this.cells.Length )  return;

			if (this.cells[index].Tooltip != text)
			{
				this.cells[index].Tooltip = text;
				this.Invalidate();
			}
		}

		public string GetLineTooltip(int index)
		{
			//	Retourne le tooltip d'une ligne.
			if ( this.cells == null )  return null;
			if ( index < 0 || index >= this.cells.Length )  return null;
			return this.cells[index].Tooltip;
		}

		public void SetLineState(int index, CellState state)
		{
			//	Sp�cifie l'�tat d'une ligne.
			if ( this.cells == null )  return;
			if ( index < 0 || index >= this.cells.Length )  return;

			if ( this.cells[index].State != state )
			{
				this.cells[index].State = state;
				this.Invalidate();
			}
		}

		public CellState GetLineState(int index)
		{
			//	Retourne l'�tat d'une ligne.
			if ( this.cells == null )  return CellState.Normal;
			if ( index < 0 || index >= this.cells.Length )  return CellState.Normal;
			return this.cells[index].State;
		}

		public void SetLineColor(int index, Color color)
		{
			//	Sp�cifie la couleur de fond d'une ligne.
			if ( this.cells == null )  return;
			if ( index < 0 || index >= this.cells.Length )  return;

			if ( this.cells[index].Color != color )
			{
				this.cells[index].Color = color;
				this.Invalidate();
			}
		}

		public Color GetLineColor(int index)
		{
			//	Retourne la couleur de fond d'une ligne.
			if ( this.cells == null )  return Color.Empty;
			if ( index < 0 || index >= this.cells.Length )  return Color.Empty;
			return this.cells[index].Color;
		}

		public void SetLineBottomSeparator(int index, bool state)
		{
			if (this.cells == null)  return;
			if (index < 0 || index >= this.cells.Length)  return;

			if (this.cells[index].BottomSeparator != state)
			{
				this.cells[index].BottomSeparator = state;
				this.Invalidate ();
			}
		}

		public bool GetLineBottomSeparator(int index)
		{
			if (this.cells == null)  return false;
			if (index == this.cells.Length)  return true;
			if (index < 0 || index >= this.cells.Length)  return false;
			return this.cells[index].BottomSeparator;
		}

		public bool AllowMultipleSelection
		{
			//	Indique si les s�lections multiples sont possibles.
			//	En mode 'true' la s�lection multiple est forc�e, c'est-�-dire qu'il
			//	n'est pas n�cessaire d'utiliser la touche Ctrl.
			get
			{
				return this.allowMultipleSelection;
			}

			set
			{
				this.allowMultipleSelection = value;
			}
		}

		public int SelectedCell
		{
			//	Cellule s�lectionn�e.
			//	Attention, les cellules sont compt�s ici � partir de z�ro, contraitement �
			//	StringArray qui compte des lignes en tenant compte de la premi�re ligne
			//	visible.
			get
			{
				return this.selectedCell;
			}

			set
			{
				this.selectedCell = value;
				this.UpdateSelectedCell();
			}
		}

		public List<int> SelectedCells
		{
			//	Cellules s�lectionn�es.
			//	Attention, les cellules sont compt�s ici � partir de z�ro, contraitement �
			//	StringArray qui compte des lignes en tenant compte de la premi�re ligne
			//	visible.
			get
			{
				return this.selectedCells;
			}

			set
			{
				this.selectedCells = value;
				this.UpdateSelectedCell();
			}
		}


		public void SetHilitedLines(int firstLine, int countLine)
		{
			//	Met en �vidence un groupe de lignes.
			if (this.hilitedFirstLine != firstLine ||
				this.hilitedCountLine != countLine)
			{
				this.hilitedFirstLine = firstLine;
				this.hilitedCountLine = countLine;

				this.Invalidate ();
			}
		}

		public void SetSearchLocatorLine(int line)
		{
			if (this.searchLocatorLine != line)
			{
				this.searchLocatorLine = line;
				this.Invalidate ();
			}
		}

		public int InsertionPointLine
		{
			get
			{
				return this.insertionPointLine;
			}
			set
			{
				if (this.insertionPointLine != value)
				{
					this.insertionPointLine = value;
					this.Invalidate ();
				}
			}
		}


		public bool IsDynamicToolTips
		{
			//	Faut-il g�n�rer les tooltips dynamiques ?
			get
			{
				return this.isDynamicToolTips;
			}

			set
			{
				this.isDynamicToolTips = value;
			}
		}


		protected override void ProcessMessage(Message message, Point pos)
		{
			if (this.IsEnabled == false)
			{
				return;
			}

			if (message.IsMouseType)
			{
				StringArray array = this.Parent as StringArray;

				if (array != null)
				{
					if (array.ProcessListMessage (this, message, pos))
					{
						return;
					}
				}
				
				if (message.MessageType == MessageType.MouseDown)
				{
					int cell = this.Detect(pos, true);

					if (this.allowMultipleSelection)
					{
						if (message.IsNoModifierPressed)
						{
							this.selectedCells.Clear();
						}

						if (message.IsShiftPressed && this.selectedCells.Count > 0)
						{
							int last = this.selectedCells[this.selectedCells.Count-1];

							if (cell > last)
							{
								for (int i=last+1; i<=cell; i++)
								{
									if (!this.selectedCells.Contains(i))
									{
										this.selectedCells.Add(i);
									}
								}
							}

							if (cell < last)
							{
								for (int i=last-1; i>=cell; i--)
								{
									if (!this.selectedCells.Contains(i))
									{
										this.selectedCells.Add(i);
									}
								}
							}
						}
						else
						{
							if (this.selectedCells.Contains(cell))
							{
								this.selectedCells.Remove(cell);
							}
							else
							{
								this.selectedCells.Add(cell);
							}
						}

						this.UpdateSelectedCell();
						this.OnFinalCellSelectionChanged (new MessageEventArgs (message, this.MapClientToScreen (pos)));
					}
					else
					{
						if (this.SelectedCell != cell)
						{
							if (cell != -1)
							{
								this.SelectedCell = cell;
								this.OnDraggingCellSelectionChanged();
							}
						}
					}

					if (cell != -1)
					{
						this.isDragging = true;
						message.Captured = true;
						message.Consumer = this;
						return;
					}
				}

				if (message.MessageType == MessageType.MouseMove)
				{
					if (this.isDragging)
					{
						int cell = this.Detect(pos, false);
						if (!this.allowMultipleSelection && this.SelectedCell != cell)
						{
							this.SelectedCell = cell;
							this.OnDraggingCellSelectionChanged();
						}
						message.Captured = true;
						message.Consumer = this;
						return;
					}
				}

				if (message.MessageType == MessageType.MouseUp)
				{
					int cell = this.Detect(pos, false);

					if (!this.allowMultipleSelection)
					{
						this.SelectedCell = cell;
						System.Diagnostics.Debug.Assert (this.Window != null);
						this.OnFinalCellSelectionChanged (new MessageEventArgs (message, this.MapClientToScreen (pos)));
					}

					this.isDragging = false;
					message.Captured = true;
					message.Consumer = this;
					return;
				}

				if (message.MessageType == MessageType.MouseLeave)
				{
					message.Captured = true;
					message.Consumer = this;
					return;
				}
			}

			base.ProcessMessage(message, pos);
		}

		private int Detect(Point pos, bool margins)
		{
			//	D�tecte la cellule vis�e par la souris.
			Rectangle box = this.Client.Bounds;
			
			if (margins)
			{
				box.Left  += StringList.WidthDraggingDetectMargin;
				box.Right -= StringList.WidthDraggingDetectMargin;
			}

			if ( !box.Contains(pos) )  return -1;
			double py = box.Height-(pos.Y-box.Bottom);
			double h = box.Height/this.cells.Length;
			return (int) (py/h);
		}

		protected override void UpdateClientGeometry()
		{
			//	Met � jour la g�om�trie.
			base.UpdateClientGeometry();

			int length = (int) (this.Client.Bounds.Height/this.lineHeight);
			length = System.Math.Max(length, 1);
			if ( this.cells == null || this.cells.Length != length )
			{
				this.CreateCells(length);
				this.OnCellCountChanged();
			}
		}

		private void CreateCells(int length)
		{
			this.cells               = new Cell[length];
			this.forceLeftAlignment  = new bool[length];
			this.forceRightAlignment = new bool[length];

			for (int i=0; i<this.cells.Length; i++)
			{
				this.cells[i] = new Cell ();
				this.cells[i].TextLayout = new TextLayout ();
				this.cells[i].State = CellState.Normal;
				this.cells[i].Color = Color.Empty;
				this.cells[i].Selected = this.IsSelectedCell(i);
			}
		}

		public void UpdateSelectedCell()
		{
			if (this.cells != null)
			{
				for (int i=0; i<this.cells.Length; i++)
				{
					bool selected = this.IsSelectedCell(i);
					if (this.cells[i].Selected != selected)
					{
						this.cells[i].Selected = selected;
						this.Invalidate();
					}
				}
			}
		}

		private bool IsSelectedCell(int cell)
		{
			//	Indique si une cellule doit �tre s�lectionn�e.
			if (this.allowMultipleSelection)
			{
				return this.selectedCells != null && this.selectedCells.Contains(cell);
			}
			else
			{
				return cell == this.selectedCell;
			}
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			if ( this.cells == null )  return;

			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			Rectangle rect = this.Client.Bounds;

			//	Dessine le cadre et le fond du tableau.
			adorner.PaintArrayBackground(graphics, rect, this.PaintState);

			double h = rect.Height/this.cells.Length;
			rect.Bottom = rect.Top-h;

			for (int i=0; i<this.cells.Length; i++)
			{
				Rectangle cell = graphics.Align (rect);

#if false
				if (i == this.selectedLine)
				{
					graphics.AddFilledRectangle (cell);
					graphics.RenderSolid (Color.FromHexa ("ffa800"));  // TODO: provisoire
				}
				else if (i >= this.hilitedFirstLine && i < this.hilitedFirstLine+this.hilitedCountLine)
				{
					graphics.AddFilledRectangle (cell);
					graphics.RenderSolid (this.colorSelection);
				}
				else
				{
					if (this.cells[i].Selected)
					{
						graphics.AddFilledRectangle (cell);
						//?graphics.RenderSolid (this.colorSelection);
						graphics.RenderSolid (Color.FromHexa ("ff0000"));  // TODO: provisoire
					}
				}
#else
				if (this.cells[i].Selected)
				{
					graphics.AddFilledRectangle (cell);
					graphics.RenderSolid (this.colorSelection);
				}
				else if (i >= this.hilitedFirstLine && i < this.hilitedFirstLine+this.hilitedCountLine)
				{
					graphics.AddFilledRectangle (cell);
					graphics.RenderSolid (this.colorHilite);
				}
#endif

				if (this.cells[i].Color != Color.Empty && i != this.searchLocatorLine)
				{
					graphics.AddFilledRectangle(cell);
					graphics.RenderSolid(this.cells[i].Color);
				}

				if (this.cells[i].State == CellState.Warning)
				{
					graphics.AddFilledRectangle(cell);
					graphics.RenderSolid(Color.FromAlphaRgb(0.4, 1, 0, 0));  // rouge semi-transparent
				}

				if (this.cells[i].State == CellState.Modified)
				{
					graphics.AddFilledRectangle(cell);
					graphics.RenderSolid(Color.FromAlphaRgb(0.5, 1, 0.8, 0));  // jaune semi-transparent
				}

				if (this.cells[i].State == CellState.Disabled)
				{
					graphics.AddFilledRectangle(cell);
					//?graphics.RenderSolid(adorner.ColorText(WidgetPaintState.None));
					graphics.RenderSolid (Color.FromAlphaRgb (0.1, 0, 0, 0));  // gris tr�s clair
				}

				if (this.cells[i].State == CellState.Unused)
				{
					graphics.AddFilledRectangle(cell);
					graphics.RenderSolid(Color.FromAlphaRgb(0.1, 0, 0, 0));  // gris tr�s clair
				}

				if (i == this.searchLocatorLine)
				{
					graphics.AddFilledRectangle (cell);
					graphics.RenderSolid (UIBuilder.BackInsideSearchColor);  // jaune p�tant
				}

				var text = this.cells[i].TextLayout.Text;

				if (text != null)
				{
					if (text.StartsWith (StringArray.SpecialContentGraphicValue))
					{
						this.PaintGraphicValue (graphics, rect, text);
					}
					else
					{
						WidgetPaintState state = this.PaintState;
						//?state = WidgetPaintState.Enabled;
						if ((state & WidgetPaintState.Enabled) != 0 && this.cells[i].Selected)
						{
							state |= WidgetPaintState.Selected;
						}

						var alignment = this.alignment;

						if (this.forceLeftAlignment[i])
						{
							alignment = ContentAlignment.MiddleLeft;
						}

						if (this.forceRightAlignment[i])
						{
							alignment = ContentAlignment.MiddleRight;
						}

						double leftMargin = 0;
						if (alignment == ContentAlignment.MiddleLeft || alignment == ContentAlignment.TopLeft || alignment == ContentAlignment.BottomLeft)
						{
							leftMargin = rect.Left+5;
						}

						this.cells[i].TextLayout.Alignment = alignment;
						this.cells[i].TextLayout.BreakMode = this.breakMode;
						this.cells[i].TextLayout.LayoutSize = new Size (rect.Width-5, rect.Height);
						adorner.PaintButtonTextLayout (graphics, new Point (leftMargin, rect.Bottom+1), this.cells[i].TextLayout, state, ButtonStyle.ListItem);
					}
				}

				rect.Offset(0, -h);
			}

			//	Dessine les traits de s�parations horizontaux.
			rect = this.Client.Bounds;
			rect.Bottom = rect.Top-h;

			for (int i=0; i<this.cells.Length; i++)
			{
				Point p1 = graphics.Align (rect.BottomLeft) + new Point (0, 0.5);
				Point p2 = graphics.Align (rect.BottomRight) + new Point (0, 0.5);
				graphics.AddLine(p1, p2);

				Color separatorColor = adorner.ColorTextFieldBorder ((this.PaintState&WidgetPaintState.Enabled) != 0);
				if (!this.cells[i].BottomSeparator)
				{
					separatorColor = Color.FromAlphaColor (0.1, separatorColor);
				}
				graphics.RenderSolid (separatorColor);

				rect.Offset (0, -h);
			}

			//	Dessine le point d'insertion.
			if (this.insertionPointLine >= 0 && this.insertionPointLine < this.cells.Length)
			{
				rect = new Rectangle (this.Client.Bounds.X, this.Client.Bounds.Top-h*this.insertionPointLine-2, this.Client.Bounds.Width, 4);
				rect = graphics.Align (rect);
				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (adorner.ColorTextFieldBorder ((this.PaintState&WidgetPaintState.Enabled) != 0));

				rect.Deflate (0, 1);
				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (UIBuilder.SelectionColor);
			}
		}


		private void PaintGraphicValue(Graphics graphics, Rectangle rect, string text)
		{
			//	Dessine une valeur num�rique dans une cellule.
			//	Le format est "$${_graphic_}$$;row".
			this.graphEngine.PaintRow (this.cube, this.graphOptions, graphics, rect, text);
		}


		#region Helpers.IToolTipHost
		public object GetToolTipCaption(Point pos)
		{
			//	Donne l'objet (string ou widget) pour le tooltip en fonction de la position.
			if ( !this.isDynamicToolTips )  return null;
			return this.GetTooltipEditedText(pos);
		}

		private string GetTooltipEditedText(Point pos)
		{
			//	Donne le texte du tooltip en fonction de la position.
			int row = this.Detect(pos, false);
			if ( row != -1 )
			{
				string text = this.GetLineTooltip(row);
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}

				text = this.GetLineString(row);
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
			}

			return null;  // pas de tooltip
		}
		#endregion


		#region Events handler
		protected virtual void OnCellCountChanged()
		{
			//	G�n�re un �v�nement pour dire que le nombre de cellules a chang�.
			var handler = this.GetUserEventHandler("CellCountChanged");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event EventHandler CellCountChanged
		{
			add
			{
				this.AddUserEventHandler ("CellCountChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("CellCountChanged", value);
			}
		}


		protected virtual void OnDraggingCellSelectionChanged()
		{
			//	G�n�re un �v�nement pour dire qu'une cellule a �t� s�lectionn�e.
			var handler = this.GetUserEventHandler("DraggingCellSelectionChanged");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event EventHandler DraggingCellSelectionChanged
		{
			add
			{
				this.AddUserEventHandler("DraggingCellSelectionChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("DraggingCellSelectionChanged", value);
			}
		}


		protected virtual void OnFinalCellSelectionChanged(MessageEventArgs e)
		{
			//	G�n�re un �v�nement pour dire qu'une cellule a �t� s�lectionn�e.
			var handler = this.GetUserEventHandler<MessageEventArgs>("FinalCellSelectionChanged");
			if (handler != null)
			{
				handler(this, e);
			}
		}

		public event EventHandler<MessageEventArgs> FinalCellSelectionChanged
		{
			add
			{
				this.AddUserEventHandler("FinalCellSelectionChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("FinalCellSelectionChanged", value);
			}
		}
		#endregion


		#region Cell class
		private class Cell
		{
			public TextLayout				TextLayout;
			public CellState				State;
			public Color					Color;
			public bool						BottomSeparator;
			public bool						Selected;
			public string					Tooltip;
		}
		#endregion


		public static readonly double		WidthDraggingDetectMargin = 3;

		private readonly GraphEngine		graphEngine;

		private Cube						cube;
		private GraphOptions				graphOptions;
		private Color						colorSelection = UIBuilder.SelectionColor;
		private Color						colorHilite = UIBuilder.HiliteColor;
		private double						lineHeight = 14;
		private double						relativeWidth = 0;
		private ContentAlignment			alignment = ContentAlignment.MiddleLeft;
		private TextBreakMode				breakMode = TextBreakMode.None;
		private Cell[]						cells;
		private bool[]						forceLeftAlignment;
		private bool[]						forceRightAlignment;
		private bool						isDynamicToolTips = false;
		private bool						isDragging = false;
		private bool						allowMultipleSelection = false;
		private int							selectedCell = -1;
		private List<int>					selectedCells;
		private int							hilitedFirstLine;
		private int							hilitedCountLine;
		private int							insertionPointLine;
		private int							searchLocatorLine;
	}
}
