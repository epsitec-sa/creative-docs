//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Common.Support;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.App.Settings;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// TreeTable de base, constituée de lignes AbstractTreeTableColumn créées avec SetColumns.
	/// La première colonne de gauche est spéciale; elle contient les informations sur l'arborescence
	/// et elle ne fait pas partie des colonnes scrollables horizontalement.
	/// On ne gère ici aucun déplacement vertical.
	/// On se contente d'afficher les AbstractTreeTableColumn passées avec SetColumns.
	/// Un seul événement RowClicked permet de connaître la colonne et la ligne cliquée.
	/// </summary>
	public class TreeTable : Widget
	{
		public TreeTable(DataAccessor accessor, int rowHeight, int headerHeight, int footerHeight)
		{
			this.accessor = accessor;

			//	Permet de mettre le focus sur le TreeTable. Cela est nécessaire pour que les
			//	touches flèches fonctionnent. Sinon, ProcessMessage n'est pas appelé !
			this.AutoFocus = true;
			this.InternalState |= WidgetInternalState.Focusable;

			this.rowHeight    = rowHeight;
			this.headerHeight = headerHeight;
			this.footerHeight = footerHeight;

			this.hoverMode = TreeTableHoverMode.VerticalGradient;

			this.treeTableColumns = new List<AbstractTreeTableColumn> ();

			//	Crée le conteneur de gauche, qui contiendra toutes les colonnes
			//	en mode DockToLeft (habituellement la seule TreeTableColumnTree).
			//	Ce conteneur n'est pas scrollable horizontalement; sa largeur
			//	s'adapte en fonctions du total des colonnes contenues.
			this.leftContainer = new FrameBox
			{
				Parent         = this,
				Dock           = DockStyle.Left,
				PreferredWidth = 0,
				Margins        = new Margins (0, 0, 0, AbstractScroller.DefaultBreadth),
			};

			//	Crée le conteneur de droite, qui contiendra toutes les colonnes
			//	qui n'ont pas le mode DockToLeft. Ce conteneur est scrollable
			//	horizontalement.
			this.columnsContainer = new Scrollable
			{
				Parent                 = this,
				HorizontalScrollerMode = ScrollableScrollerMode.ShowAlways,
				VerticalScrollerMode   = ScrollableScrollerMode.HideAlways,
				Dock                   = DockStyle.Fill,
			};

			this.columnsContainer.Viewport.IsAutoFitting = true;

			//	Crée les surcouches interactives. La permier surcouche sera dessinée
			//	sous les autres. La dernière surcouche est celle qui réagit prioritairement
			//	aux autres.
			this.interactiveLayers = new List<AbstractInteractiveLayer> ();

			this.interactiveLayers.Add (new InteractiveLayerColumnOrder     (this));
			this.interactiveLayers.Add (new InteractiveLayerColumnSeparator (this));
			this.interactiveLayers.Add (new InteractiveLayerColumnResurrect (this));

			foreach (var layer in this.interactiveLayers)
			{
				layer.CreateUI ();
			}

			this.IsEnabledChanged += delegate
			{
				foreach (var column in this.treeTableColumns)
				{
					column.Enable = this.Enable;
				}
			};

			this.AllowsMovement = true;
			this.AllowsSorting  = true;
		}


		public bool								AllowsMovement;

		public bool								AllowsSorting
		{
			get
			{
				return this.allowsSorting;
			}
			set
			{
				if (this.allowsSorting != value)
				{
					this.allowsSorting = value;
					this.UpdateSortedColumns ();
				}
			}
		}

		public int								VScrollerTopMargin
		{
			get
			{
				return this.headerHeight;
			}
		}

		public int								VScrollerBottomMargin
		{
			get
			{
				return this.footerHeight + (int) AbstractScroller.DefaultBreadth;
			}
		}

		public int								TotalRowsCount;

		public int								VisibleRowsCount
		{
			get
			{
				return (int) ((this.ActualHeight - this.headerHeight - this.footerHeight - AbstractScroller.DefaultBreadth) / this.rowHeight);
			}
		}

		public int								HeaderHeight
		{
			get
			{
				return this.headerHeight;
			}
		}

		public int								FooterHeight
		{
			get
			{
				return this.footerHeight;
			}
		}

		public Scrollable						ColumnsContainer
		{
			get
			{
				return this.columnsContainer;
			}
		}

		public int								DockToLeftCount
		{
			get
			{
				return this.columnsState.DockToLeftCount;
			}
		}

		public List<AbstractTreeTableColumn>	TreeTableColumns
		{
			get
			{
				return this.treeTableColumns;
			}
		}


		public ColumnsState						ColumnsState
		{
			get
			{
				return this.columnsState;
			}
		}

		public string GetTooltip(Point screenPos)
		{
			//	Fouille les colonnes à la recherche du bon tooltip dynamique.
			foreach (var column in this.leftContainer.Children)
			{
				var x = column as AbstractTreeTableColumn;
				if (x != null)
				{
					var pos = x.MapScreenToClient (screenPos);

					if (column.Client.Bounds.Contains (pos))
					{
						var s = x.GetToolTipCaption (pos) as string;
						if (!string.IsNullOrEmpty (s))
						{
							return s;
						}
					}
				}
			}

			foreach (var column in this.columnsContainer.Viewport.Children)
			{
				var x = column as AbstractTreeTableColumn;
				if (x != null)
				{
					var pos = x.MapScreenToClient (screenPos);

					if (column.Client.Bounds.Contains (pos))
					{
						var s = x.GetToolTipCaption (pos) as string;
						if (!string.IsNullOrEmpty (s))
						{
							return s;
						}
					}
				}
			}

			return null;
		}

		public void AddSortedColumn(ObjectField field)
		{
			var list = this.columnsState.Sorted.ToList ();

			if (list.Count == 0)
			{
				list.Add (new SortedColumn (field, SortedType.Ascending));
			}
			else
			{
				if (field == list[0].Field)
				{
					if (list[0].Type == SortedType.Ascending)
					{
						list[0] = new SortedColumn (field, SortedType.Descending);
					}
					else
					{
						list[0] = new SortedColumn (field, SortedType.Ascending);
					}
				}
				else
				{
					list.Insert (0, new SortedColumn (field, SortedType.Ascending));

					//	Jamais plus de 2 (un tri primaire et un tri secondaire).
					while (list.Count > 2)
					{
						list.RemoveAt (2);
					}
				}
			}

			this.columnsState = new ColumnsState (this.columnsState.Mapper, this.columnsState.Columns, list.ToArray (), this.columnsState.DockToLeftCount);
			this.SaveSettings ();

			this.UpdateSortedColumns ();
			this.OnSortingChanged ();
		}

		private void UpdateSortedColumns()
		{
			if (this.columnsState.Sorted == null)  // garde-fou
			{
				return;
			}

			var sorted = this.columnsState.Sorted.ToList();

			for (int i=0; i<this.treeTableColumns.Count; i++)
			{
				var column = this.treeTableColumns[i];
				int j = sorted.FindIndex (x => x.Field == column.Field);

				if (j == -1 || !this.AllowsSorting)
				{
					column.SetSortedColumn (SortedType.None, false);
				}
				else
				{
					column.SetSortedColumn (sorted[j].Type, j == 0);
				}
			}
		}


		public void SetColumns(TreeTableColumnDescription[] descriptions, SortingInstructions defaultSorting, int defaultDockToLeftCount, string treeTableName)
		{
			//	Spécifie les colonnes affichables, puis restaure les réglages enregistrés. S'ils
			//	n'existent pas, on initialise des réglages par défaut (toutes les colonnes visibles,
			//	sans mapping).
			this.columnDescriptions = descriptions;
			this.treeTableName = treeTableName;

			this.columnsState = ColumnsState.Empty;

			//	Si le TreeTable a un nom, on essaie de restaurer ses réglages de colonnes.
			if (!string.IsNullOrEmpty (this.treeTableName))
			{
				this.RestoreSettings ();
			}

			//	Si le TreeTable n'a pas de réglages de colonnes spécifiques, on crée les réglages par défaut.
			if (this.columnsState.IsEmpty)
			{
				var mapper = new int[this.columnDescriptions.Length];
				var columnState = new ColumnState[this.columnDescriptions.Length];

				for (int i=0; i<this.columnDescriptions.Length; i++)
				{
					var columnDescription = this.columnDescriptions[i];
					mapper[i] = i;
					columnState[i] = new ColumnState (columnDescription.Field, columnDescription.Width, false);
				}

				var sorted = new List<SortedColumn> ();
				if (defaultSorting.PrimaryField != ObjectField.Unknown)
				{
					sorted.Add (new SortedColumn (defaultSorting.PrimaryField, defaultSorting.PrimaryType));
				}
				if (defaultSorting.SecondaryField != ObjectField.Unknown)
				{
					sorted.Add (new SortedColumn (defaultSorting.SecondaryField, defaultSorting.SecondaryType));
				}

				this.columnsState = new ColumnsState (mapper, columnState, sorted.ToArray (), defaultDockToLeftCount);
				//	Il n'est pas nécessaire de sauvegarder ces réglages (avec SaveSettings),
				//	puisqu'ils peuvent être générés à nouveau exactement à l'identique.
			}

			this.CreateColumns ();
		}

		public void SetColumnWidth(int rank, int? width)
		{
			//	Modifie la largeur d'une colonne.
			//	Si width == 0		->	cache la colonne
			//	Si width == null	->	restitue la largeur originale
			rank = this.columnsState.MappedToAbsolute (rank);

			var field = this.columnsState.Columns[rank].Field;
			int newWidth;
			bool hide;

			if (width.HasValue)
			{
				if (width == 0)  // cache la colonne ?
				{
					newWidth = this.columnsState.Columns[rank].OriginalWidth;
					hide = true;
				}
				else  // modifie la largeur ?
				{
					newWidth = width.Value;
					hide = false;
				}
			}
			else  // restitue la largeur originale ?
			{
				newWidth = this.columnsState.Columns[rank].OriginalWidth;
				hide = false;
			}

			this.columnsState.Columns[rank] = new ColumnState (field, newWidth, hide);
			this.SaveSettings ();

			this.GetColumn (rank).PreferredWidth = this.columnsState.Columns[rank].FinalWidth;
		}

		public void ChangeColumnOrder(int columnSrc, TreeTableColumnSeparator separatorDst)
		{
			//	Change l'ordre des colonnes en déplaçant une colonne vers la gauche
			//	ou vers la droite. On modifie simplement le 'mapping', puis on
			//	reconstruit le tableau.
			var dockToLeftCount = this.columnsState.DockToLeftCount;

			bool srcDockToLeft = (columnSrc < dockToLeftCount);
			bool dstDockToLeft = (separatorDst.Rank < dockToLeftCount);

			//	Si on est sur la frontière des conteneurs left|scrollable, on force
			//	la destination dans la bonne direction.
			if (separatorDst.Left)
			{
				dstDockToLeft = true;
			}

			if (separatorDst.Right)
			{
				dstDockToLeft = false;
			}

			var mapper = this.columnsState.Mapper.ToList ();

			if (separatorDst.Rank <= columnSrc)  // déplacement vers la gauche ?
			{
				int x = mapper[columnSrc];
				mapper.RemoveAt (columnSrc);
				mapper.Insert (separatorDst.Rank, x);

				if (!srcDockToLeft && dstDockToLeft)
				{
					dockToLeftCount++;
				}
			}
			else  // déplacement vers la droite ?
			{
				int x = mapper[columnSrc];
				mapper.RemoveAt (columnSrc);
				mapper.Insert (separatorDst.Rank-1, x);

				if (srcDockToLeft && !dstDockToLeft)
				{
					System.Diagnostics.Debug.Assert (dockToLeftCount > 0);
					dockToLeftCount--;
				}
			}

			this.columnsState = new ColumnsState (mapper.ToArray (), this.columnsState.Columns, this.columnsState.Sorted, dockToLeftCount);
			this.SaveSettings ();

			this.CreateColumns ();
			this.OnContentChanged (true);  // on demande de mettre à jour le contenu
		}

		private void CreateColumns()
		{
			//	Crée les widgets des différentes colonnes, selon le mapping.
			this.treeTableColumns.Clear ();
			this.leftContainer.Children.Clear ();
			this.columnsContainer.Viewport.Children.Clear ();

			int index = 0;

			foreach (var columnState in this.columnsState.MappedColumns)
			{
				var description = this.columnDescriptions.Where (x => x.Field == columnState.Field).FirstOrDefault ();

				var column = TreeTableColumnHelper.Create (this.accessor, description);

				column.PreferredWidth = columnState.FinalWidth;
				column.Index          = index;
				column.AllowsMovement = this.AllowsMovement;
				column.AllowsSorting  = this.AllowsSorting;

				this.treeTableColumns.Add (column);

				if (index < this.columnsState.DockToLeftCount)  // dans le conteneur fixe de gauche ?
				{
					column.DockToLeft = true;
					column.Dock       = DockStyle.Left;
					this.leftContainer.Children.Add (column);
				}
				else  // dans le conteneur scrollable de droite ?
				{
					column.DockToLeft = false;
					column.Dock       = DockStyle.Left;
					this.columnsContainer.Viewport.Children.Add (column);
				}

				column.ChildrenMouseMove += delegate (object sender, int row)
				{
					this.SetHilitedHoverRow (row);
				};

				if (column is TreeTableColumnTree)
				{
					var tree = column as TreeTableColumnTree;

					tree.TreeButtonClicked += delegate (object sender, int row, NodeType type)
					{
						this.OnTreeButtonClicked (row, type);
					};
				}

				index++;
			}

			this.UpdateHoverMode ();
			this.UpdateChildrensGeometry ();
			this.UpdateSortedColumns ();
		}

		public void SetColumnCells(int rank, TreeTableColumnItem columnItem)
		{
			var columnWidget = this.GetColumn (rank);
			columnWidget.SetCells (columnItem);
		}


		protected override void OnExited(MessageEventArgs e)
		{
			this.ProcessMouseLeave (e.Point);

			base.OnExited (e);
		}

		protected override void ProcessMessage(Message message, Point pos)
		{
			switch (message.MessageType)
			{
				case MessageType.MouseDown:
					this.ProcessMouseDown (pos);
					this.ProcessMouseClick (pos);
					break;

				case MessageType.MouseMove:
					this.ProcessMouseMove (pos);
					break;

				case MessageType.MouseUp:
					this.ProcessMouseUp (pos);
					this.ProcessMouseMove (pos);
					if ((message.Button & MouseButtons.Right) != 0)
					{
						this.ProcessMouseRightClick (pos);
					}
					break;

				case MessageType.MouseWheel:
					if (message.Wheel > 0)
					{
						this.OnMouseWheel (-1);
					}
					else if (message.Wheel < 0)
					{
						this.OnMouseWheel (1);
					}
					break;

				case MessageType.MouseLeave:
					this.ProcessMouseLeave (pos);
					break;

				case MessageType.KeyDown:
					if (this.ProcessKey (message.KeyCode))
					{
						message.Captured = true;
						message.Consumer = this;
					}
					break;
			}
		
			base.ProcessMessage (message, pos);
		}

		protected override void OnDoubleClicked(MessageEventArgs e)
		{
			var rowResult    = this.DetectRow    (e.Point);
			var columnResult = this.DetectColumn (e.Point);

			if (rowResult.IsValid && columnResult.IsValid)
			{
				this.OnRowDoubleClicked (rowResult.Rank);
				this.Focus ();  // pour que les touches flèches fonctionnent
			}

			base.OnDoubleClicked (e);
		}

		private void ProcessMouseDown(Point pos)
		{
			AbstractInteractiveLayer draggingLayer = null;

			//	Essaie de démarrer le drag d'une surcouche.
			if (this.AllowsMovement)
			{
				for (int i=this.interactiveLayers.Count-1; i>=0; i--)
				{
					var layer = this.interactiveLayers[i];

					layer.MouseDown (pos);

					if (layer.IsDragging)
					{
						draggingLayer = layer;
						break;
					}
				}

				//	Si une surcouche a démarré un drag, on clear toutes les autres.
				if (draggingLayer != null)
				{
					foreach (var layer in this.interactiveLayers.Where (x => x != draggingLayer))
					{
						layer.ClearActiveHover ();
					}
				}
			}
		}

		private void ProcessMouseMove(Point pos)
		{
			if (this.AllowsMovement)
			{
				var draggingLayer = this.DraggingLayer;

				if (draggingLayer == null)  // pas de drag en cours ?
				{
					//	Fait bouger le hover d'une surcouche.
					for (int i=this.interactiveLayers.Count-1; i>=0; i--)
					{
						var layer = this.interactiveLayers[i];

						layer.MouseMove (pos);

						if (layer.HasActiveHover)
						{
							//	Dès qu'une surcouche a réagi, on clear celles qui sont en dessous.
							for (int j=i-1; j>=0; j--)
							{
								this.interactiveLayers[j].ClearActiveHover ();
							}

							break;
						}
					}
				}
				else  // drag en cours ?
				{
					//	Fait réagir uniquement la surcouche en cours de drag.
					draggingLayer.MouseMove (pos);
				}

				this.UpdateMouseCursor ();
			}

			var rowResult    = this.DetectRow    (pos);
			var columnResult = this.DetectColumn (pos);

			if (columnResult.IsValid == false)  // à droite de la dernière colonne ?
			{
				rowResult = DetectResult.After;  // pas de lighe hilitée
			}

			this.SetHilitedHoverRow (rowResult.Rank);
		}

		private void ProcessMouseUp(Point pos)
		{
			if (this.AllowsMovement)
			{
				for (int i=this.interactiveLayers.Count-1; i>=0; i--)
				{
					var layer = this.interactiveLayers[i];
					layer.MouseUp (pos);
				}
			}
		}

		private void ProcessMouseClick(Point pos)
		{
			//	Si on clique hors des cellules existantes (soit plus bas que la dernière
			//	ligne, soit à droite de la dernière colonne), row vaut -1 et on effectue
			//	une sélection normalement, ce qui revient à désélectionner.
			var rowResult = this.DetectRow (pos);

			if (rowResult.IsBefore == false)
			{
				var columnResult = this.DetectColumn (pos);

				if (columnResult.IsValid == false)  // clic à droite de la dernière colonne ?
				{
					rowResult = DetectResult.After;  // on désélectionne
				}

				this.OnRowClicked (rowResult.Rank, columnResult.Rank);
				this.Focus ();  // pour que les touches flèches fonctionnent
			}
		}

		private void ProcessMouseRightClick(Point pos)
		{
			var rowResult = this.DetectRow (pos);

			if (rowResult.IsValid)
			{
				var columnResult = this.DetectColumn (pos);

				if (columnResult.IsValid)
				{
					var p = this.MapClientToScreen (pos);
					this.OnRowRightClicked (rowResult.Rank, columnResult.Rank, p);
					this.Focus ();  // pour que les touches flèches fonctionnent
				}
			}
		}

		private void ProcessMouseLeave(Point pos)
		{
			if (this.AllowsMovement)
			{
				for (int i=this.interactiveLayers.Count-1; i>=0; i--)
				{
					var layer = this.interactiveLayers[i];
					layer.MouseLeave (pos);
				}
			}

			this.SetHilitedHoverRow (-1);

			MouseCursorManager.Clear ();
		}

		private bool ProcessKey(KeyCode keyCode)
		{
			switch (keyCode)
			{
				case KeyCode.ArrowUp:
				case KeyCode.ArrowDown:
				case KeyCode.PageUp:
				case KeyCode.PageDown:
				case KeyCode.Home:
				case KeyCode.End:
					this.OnDokeySelect (keyCode);
					return true;

				default:
					return false;
			}
		}


		private DetectResult DetectRow(Point pos)
		{
			int max = this.VisibleRowsCount;

			double h = this.ActualHeight - this.headerHeight - this.footerHeight - AbstractScroller.DefaultBreadth;
			double dy = h / max;

			double y = this.ActualHeight - this.headerHeight - pos.Y;

			if (y < 0)
			{
				return DetectResult.Before;  // on est dans l'en-tête
			}

			int row = (int) (y / dy);

			if (this.TotalRowsCount > 0)  // est-ce que TotalRowsCount est défini ?
			{
				max = System.Math.Min (max, this.TotalRowsCount);
			}

			if (row >= 0 && row < max)
			{
				return new DetectResult (row);
			}

			return DetectResult.After;  // on est en dessous de la dernière ligne
		}

		private DetectResult DetectColumn(Point pos)
		{
			var layer = this.interactiveLayers.Where (x => x is InteractiveLayerColumnOrder).FirstOrDefault () as InteractiveLayerColumnOrder;
			int rank = layer.DetectColumn (pos);

			if (rank == -1)
			{
				return DetectResult.After;
			}
			else
			{
				int column = this.columnsState.MappedToAbsolute (rank);
				return new DetectResult (column);
			}
		}


		private struct DetectResult
		{
			public DetectResult(int rank, bool isBefore = false, bool isAfter = false)
			{
				this.Rank     = rank;
				this.IsBefore = isBefore;
				this.IsAfter  = isAfter;
			}

			public bool							IsValid
			{
				get
				{
					return this.Rank >= 0 && !this.IsBefore && !this.IsAfter;
				}
			}

			public static DetectResult Before = new DetectResult (-1, isBefore: true,  isAfter: false);
			public static DetectResult After  = new DetectResult (-1, isBefore: false, isAfter: true);

			public readonly int					Rank;
			public readonly bool				IsBefore;
			public readonly bool				IsAfter;
		}


		private void UpdateMouseCursor()
		{
			var cursor = MouseCursorType.Default;

			for (int i=this.interactiveLayers.Count-1; i>=0; i--)
			{
				var layer = this.interactiveLayers[i];
				var c = layer.MouseCursorType;

				if (c != MouseCursorType.Default)
				{
					cursor = c;
					break;
				}
			}

			MouseCursorManager.SetMouseCursor (cursor);
		}


		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry ();
			this.UpdateChildrensGeometry ();
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			graphics.AddFilledRectangle (new Rectangle (0, AbstractScroller.DefaultBreadth, this.ActualWidth, this.ActualHeight-AbstractScroller.DefaultBreadth));
			graphics.RenderSolid (ColorManager.TreeTableBackgroundColor);
		}


		public void SetHoverMode(TreeTableHoverMode mode)
		{
			this.hoverMode = mode;
			this.UpdateHoverMode ();
		}

		private void UpdateHoverMode()
		{
			foreach (var column in this.treeTableColumns)
			{
				column.HoverMode = this.hoverMode;
			}
		}

		private void SetHilitedHoverRow(int row)
		{
			foreach (var column in this.treeTableColumns)
			{
				column.HilitedHoverRow = row;
			}
		}

		private void UpdateChildrensGeometry()
		{
			foreach (var column in this.treeTableColumns)
			{
				column.HeaderHeight = this.headerHeight;
				column.FooterHeight = this.footerHeight;
				column.RowHeight    = this.rowHeight;
			}
		}

		private AbstractTreeTableColumn GetColumn(int absRank)
		{
			System.Diagnostics.Debug.Assert (absRank >= 0 && absRank < this.treeTableColumns.Count);
			int rank = this.columnsState.AbsoluteToMapped (absRank);
			return this.treeTableColumns[rank];
		}


		private AbstractInteractiveLayer DraggingLayer
		{
			get
			{
				return this.interactiveLayers.Where (x => x.IsDragging).FirstOrDefault ();
			}
		}


		private void SaveSettings()
		{
			if (string.IsNullOrEmpty (this.treeTableName))
			{
				return;
			}

			LocalSettings.SetColumnsState (this.treeTableName, this.columnsState);
		}

		private void RestoreSettings()
		{
			if (string.IsNullOrEmpty (this.treeTableName))
			{
				return;
			}

			var cs = LocalSettings.GetColumnsState (this.treeTableName);
			if (!cs.IsEmpty)
			{
				this.columnsState = cs;
			}
		}


		#region Events handler
		private void OnRowClicked(int row, int column)
		{
			this.RowClicked.Raise (this, row, column);
		}

		public event EventHandler<int, int> RowClicked;


		private void OnRowDoubleClicked(int row)
		{
			this.RowDoubleClicked.Raise (this, row);
		}

		public event EventHandler<int> RowDoubleClicked;


		private void OnRowRightClicked(int row, int column, Point pos)
		{
			this.RowRightClicked.Raise (this, row, column, pos);
		}

		public event EventHandler<int, int, Point> RowRightClicked;


		private void OnContentChanged(bool crop)
		{
			this.ContentChanged.Raise (this, crop);
		}

		public event EventHandler<bool> ContentChanged;


		private void OnSortingChanged()
		{
			this.SortingChanged.Raise (this);
		}

		public event EventHandler SortingChanged;


		private void OnTreeButtonClicked(int row, NodeType type)
		{
			this.TreeButtonClicked.Raise (this, row, type);
		}
		
		public event EventHandler<int, NodeType> TreeButtonClicked;


		private void OnDokeySelect(KeyCode key)
		{
			if (!PopupStack.HasPopup)
			{
				this.DokeySelect.Raise (this, key);
			}
		}

		public event EventHandler<KeyCode> DokeySelect;


		private void OnMouseWheel(int direction)
		{
			this.MouseWheel.Raise (this, direction);
		}

		public event EventHandler<int> MouseWheel;
		#endregion


		private readonly DataAccessor					accessor;
		private readonly List<AbstractTreeTableColumn>	treeTableColumns;
		private readonly FrameBox						leftContainer;
		private readonly Scrollable						columnsContainer;
		private readonly List<AbstractInteractiveLayer>	interactiveLayers;

		private TreeTableColumnDescription[]			columnDescriptions;
		private ColumnsState							columnsState;
		private int										headerHeight;
		private int										footerHeight;
		private int										rowHeight;
		private TreeTableHoverMode						hoverMode;
		private bool									allowsSorting;
		private string									treeTableName;
	}
}