//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

using Epsitec.Cresus.Compta.Accessors;

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


		public Color ColorSelection
		{
			//	Couleur utilisée pour les lignes sélectionnées..
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
			//	Largeur relative (dans n'importe quelle unité) de la colonne.
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
			//	Césure des textes pour la colonne.
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
			//	Spécifie le texte contenu dans une ligne.
			if ( this.cells == null )  return;
			if ( index < 0 || index >= this.cells.Length )  return;

			if ( this.cells[index].TextLayout.Text != text )
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
			//	Spécifie le tooltip d'une ligne.
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
			//	Spécifie l'état d'une ligne.
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
			//	Retourne l'état d'une ligne.
			if ( this.cells == null )  return CellState.Normal;
			if ( index < 0 || index >= this.cells.Length )  return CellState.Normal;
			return this.cells[index].State;
		}

		public void SetLineColor(int index, Color color)
		{
			//	Spécifie la couleur de fond d'une ligne.
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
			//	Indique si les sélections multiples sont possibles.
			//	En mode 'true' la sélection multiple est forcée, c'est-à-dire qu'il
			//	n'est pas nécessaire d'utiliser la touche Ctrl.
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
			//	Cellule sélectionnée.
			//	Attention, les cellules sont comptés ici à partir de zéro, contraitement à
			//	StringArray qui compte des lignes en tenant compte de la première ligne
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
			//	Cellules sélectionnées.
			//	Attention, les cellules sont comptés ici à partir de zéro, contraitement à
			//	StringArray qui compte des lignes en tenant compte de la première ligne
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
			//	Met en évidence un groupe de lignes.
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
			//	Faut-il générer les tooltips dynamiques ?
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
						this.OnFinalCellSelectionChanged();
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
						this.OnFinalCellSelectionChanged();
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
			//	Détecte la cellule visée par la souris.
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
			//	Met à jour la géométrie.
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
			this.cells = new Cell[length];
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
			//	Indique si une cellule doit être sélectionnée.
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
				Rectangle cell = rect;
				graphics.Align(ref cell);

				if (i >= this.hilitedFirstLine && i < this.hilitedFirstLine+this.hilitedCountLine)
				{
					graphics.AddFilledRectangle (cell);
					graphics.RenderSolid (this.colorSelection);
				}
				else
				{
					if (this.cells[i].Selected)
					{
						graphics.AddFilledRectangle (cell);
						graphics.RenderSolid (this.colorSelection);
					}
				}

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
					graphics.RenderSolid (Color.FromAlphaRgb (0.1, 0, 0, 0));  // gris très clair
				}

				if (this.cells[i].State == CellState.Unused)
				{
					graphics.AddFilledRectangle(cell);
					graphics.RenderSolid(Color.FromAlphaRgb(0.1, 0, 0, 0));  // gris très clair
				}

				if (i == this.searchLocatorLine)
				{
					graphics.AddFilledRectangle (cell);
					graphics.RenderSolid (SearchResult.BackInsideSearch);  // jaune pétant
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

						double leftMargin = 0;
						if (this.alignment == ContentAlignment.MiddleLeft || this.alignment == ContentAlignment.TopLeft || this.alignment == ContentAlignment.BottomLeft)
						{
							leftMargin = rect.Left+5;
						}

						this.cells[i].TextLayout.Alignment = this.alignment;
						this.cells[i].TextLayout.BreakMode = this.breakMode;
						this.cells[i].TextLayout.LayoutSize = new Size (rect.Width-5, rect.Height);
						adorner.PaintButtonTextLayout (graphics, new Point (leftMargin, rect.Bottom), this.cells[i].TextLayout, state, ButtonStyle.ListItem);
					}
				}

				rect.Offset(0, -h);
			}

			//	Dessine les traits de séparations horizontaux.
			rect = this.Client.Bounds;
			rect.Bottom = rect.Top-h;

			for (int i=0; i<this.cells.Length; i++)
			{
				Point p1 = new Point(rect.Left, rect.Bottom);
				Point p2 = new Point(rect.Right, rect.Bottom);
				graphics.Align(ref p1);
				graphics.Align(ref p2);
				p1.Y += 0.5;
				p2.Y += 0.5;
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
				graphics.Align (ref rect);
				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (adorner.ColorTextFieldBorder ((this.PaintState&WidgetPaintState.Enabled) != 0));

				rect.Deflate (0, 1);
				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (Color.FromName ("Gold"));
			}
		}


		private void PaintGraphicValue(Graphics graphics, Rectangle rect, string text)
		{
			//	Dessine une valeur numérique dans une cellule, en rouge si elle est négative et en vert
			//	si elle est positive.
			//	Le format est "$${_graphic_}$$/-10/100/55.2" ou "$${_graphic_}$$/-10/100/55.2/60".
			var words = text.Split ('/');

			if (words.Length < 4 || words.Length > 5)
			{
				return;
			}

			decimal min, max, value1, value2 = 0;

			if (!decimal.TryParse (words[1], out min))
			{
				return;
			}

			if (!decimal.TryParse (words[2], out max))
			{
				return;
			}

			if (!decimal.TryParse (words[3], out value1))
			{
				return;
			}

			if (words.Length >= 5 && !decimal.TryParse (words[4], out value2))
			{
				return;
			}

			min = System.Math.Min (min, 0);
			max = System.Math.Max (max, 0);

			if (words.Length == 5)
			{
				this.PaintGraphicValue (graphics, rect, min, max, value1, value2);
			}
			else
			{
				this.PaintGraphicValue (graphics, rect, min, max, value1);
			}
		}

		private void PaintGraphicValue(Graphics graphics, Rectangle rect, decimal min, decimal max, decimal value)
		{
			if (max-min == 0)
			{
				return;
			}

			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (Color.FromAlphaColor (0.5, adorner.ColorTextFieldBorder ((this.PaintState&WidgetPaintState.Enabled) != 0)));

			rect.Deflate (2);
			rect = graphics.Align (rect);
			rect.Deflate (0.5);

			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (Color.FromBrightness (1));

			double sep = rect.Width * (double) -min  / (double) (max-min);
			double val = rect.Width * (double) value / (double) (max-min);

			if (val < 0)
			{
				var r = new Rectangle (rect.Left+sep+val, rect.Bottom, -val, rect.Height);
				graphics.AddFilledRectangle (r);
				graphics.RenderSolid (StringList.colorRed);
			}

			if (val > 0)
			{
				var r = new Rectangle (rect.Left+sep, rect.Bottom, val, rect.Height);
				graphics.AddFilledRectangle (r);
				graphics.RenderSolid (StringList.colorGreen);
			}

			sep = System.Math.Floor (sep);

			graphics.AddLine (rect.Left+sep, rect.Bottom, rect.Left+sep, rect.Top);
			graphics.RenderSolid (Color.FromBrightness (0));

			graphics.AddRectangle (rect);
			graphics.RenderSolid (adorner.ColorTextFieldBorder ((this.PaintState&WidgetPaintState.Enabled) != 0));
		}

		private void PaintGraphicValue(Graphics graphics, Rectangle rect, decimal min, decimal max, decimal value, decimal solde)
		{
			if (max-min == 0)
			{
				return;
			}

			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (Color.FromAlphaColor (0.5, adorner.ColorTextFieldBorder ((this.PaintState&WidgetPaintState.Enabled) != 0)));

			rect.Deflate (2);
			rect = graphics.Align (rect);
			rect.Deflate (0.5);

			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (Color.FromBrightness (1));

			double sep = rect.Width * (double) -min  / (double) (max-min);
			double val = rect.Width * (double) value / (double) (max-min);
			double sol = rect.Width * (double) solde / (double) (max-min);

			var color = value >= solde ? StringList.colorGreen : StringList.colorRed;

			if (val < 0)
			{
				var r = new Rectangle (rect.Left+sep+val, rect.Bottom, -val, rect.Height);
				graphics.AddFilledRectangle (r);
				graphics.RenderSolid (color);
			}

			if (val > 0)
			{
				var r = new Rectangle (rect.Left+sep, rect.Bottom, val, rect.Height);
				graphics.AddFilledRectangle (r);
				graphics.RenderSolid (color);
			}

			sep = System.Math.Floor (sep);
			sol = System.Math.Floor (sol);

			graphics.AddLine (rect.Left+sep, rect.Bottom, rect.Left+sep, rect.Top);
			graphics.AddLine (rect.Left+sol, rect.Bottom, rect.Left+sol, rect.Top);
			graphics.RenderSolid (Color.FromBrightness (0));

			graphics.AddRectangle (rect);
			graphics.RenderSolid (adorner.ColorTextFieldBorder ((this.PaintState&WidgetPaintState.Enabled) != 0));
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
			//	Génère un événement pour dire que le nombre de cellules a changé.
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
			//	Génère un événement pour dire qu'une cellule a été sélectionnée.
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


		protected virtual void OnFinalCellSelectionChanged()
		{
			//	Génère un événement pour dire qu'une cellule a été sélectionnée.
			var handler = this.GetUserEventHandler("FinalCellSelectionChanged");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event EventHandler FinalCellSelectionChanged
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

		private static readonly Color		colorRed   = Color.FromHexa ("ff0000");
		private static readonly Color		colorGreen = Color.FromHexa ("00bb00");

		private Color						colorSelection = Color.FromName ("Gold");
		private double						lineHeight = 20;
		private double						relativeWidth = 0;
		private ContentAlignment			alignment = ContentAlignment.MiddleLeft;
		private TextBreakMode				breakMode = TextBreakMode.None;
		private Cell[]						cells;
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
