using System.Collections.Generic;
using Epsitec.Common.Drawing;
using Epsitec.Common.UI;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Layouts;
using Epsitec.Common.Support;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.PanelEditor
{
	/// <summary>
	/// La classe ObjectModifier permet de gérer les 'widgets' de Designer.
	/// </summary>
	public class ObjectModifier
	{
		[DesignerVisible]
		public enum ChildrenPlacement
		{
			[Hidden] None,
			[Hidden] Anchored,
			VerticalStacked,
			HorizontalStacked,
			Grid,
		}

		[DesignerVisible]
		public enum AnchoredHorizontalAttachment
		{
			[Hidden] None,
			Left,
			Right,
			Fill,
		}

		[DesignerVisible]
		public enum AnchoredVerticalAttachment
		{
			[Hidden] None,
			Bottom,
			Top,
			Fill,
		}

		[DesignerVisible]
		public enum StackedHorizontalAttachment
		{
			[Hidden] None,
			Left,
			Right,
			Fill,
		}

		[DesignerVisible]
		public enum StackedVerticalAttachment
		{
			[Hidden] None,
			Bottom,
			Top,
			Fill,
		}

		[DesignerVisible]
		public enum StackedHorizontalAlignment
		{
			[Hidden] None,
			Stretch,
			Center,
			Left,
			Right,
		}

		[DesignerVisible]
		public enum StackedVerticalAlignment
		{
			[Hidden] None,
			Stretch,
			Center,
			Bottom,
			Top,
			BaseLine,
		}

		[DesignerVisible]
		public enum GridMode
		{
			[Hidden] None,
			Auto,
			Absolute,
			Proportional,
		}

		public enum BoundsMode
		{
			OriginX,
			OriginY,
			Width,
			Height,
		}

		public enum ObjectType
		{
			Unknow,
			HSeparator,			// séparateur horizontal
			VSeparator,			// séparateur vertical
			Button,				// bouton associé à une commande
			Placeholder,		// texte éditable lié à la base de données
			Table,				// table lié à la base de données
			StaticText,			// texte fixe
			Group,				// conteneur invisible
			GroupFrame,			// conteneur avec cadre
			GroupBox,			// conteneur avec cadre et titre
			MainPanel,			// panneau principal
			SubPanel,			// sous-panneau
		}


		public ObjectModifier(Editor panelEditor)
		{
			//	Constructeur unique.
			this.panelEditor = panelEditor;
		}

		public Editor PanelEditor
		{
			get
			{
				return this.panelEditor;
			}
		}

		protected UI.Panel Container
		{
			get
			{
				return this.panelEditor.Panel;
			}
		}


		public static bool IsAbstractGroup(Widget obj)
		{
			//	Retourne true si l'objet est un conteneur (invisible ou avec cadre).
			ObjectType type = ObjectModifier.GetObjectType(obj);
			return (type == ObjectType.Group      ||
					type == ObjectType.GroupFrame ||
					type == ObjectType.GroupBox   ||
					type == ObjectType.MainPanel  );
		}

		public static ObjectType GetObjectType(Widget obj)
		{
			//	Retourne le type d'un objet.
			if (obj is AbstractButton)
			{
				return ObjectType.Button;
			}

			if (obj is Separator)
			{
				if (obj.PreferredHeight == 1)  // séparateur horizontal ?
				{
					return ObjectType.HSeparator;
				}
				else  // séparateur vertical ?
				{
					return ObjectType.VSeparator;
				}
			}

			if (obj is UI.Placeholder)
			{
				return ObjectType.Placeholder;
			}

			if (obj is UI.TablePlaceholder)
			{
				return ObjectType.Table;
			}

			if (obj is StaticText)
			{
				return ObjectType.StaticText;
			}

			//	Ce test doit s'exécuter avant FrameBox, car un PanelPlaceholder est
			//	aussi un FrameBox !
			if (obj is UI.PanelPlaceholder)
			{
				return ObjectType.SubPanel;
			}

			if (obj is UI.Panel)
			{
				return ObjectType.MainPanel;
			}

			if (obj is FrameBox)
			{
				FrameBox frame = obj as FrameBox;
				if (frame.DrawFullFrame)
				{
					return ObjectType.GroupFrame;
				}
				else
				{
					return ObjectType.Group;
				}
			}
				
			if (obj is GroupBox)
			{
				return ObjectType.GroupBox;
			}
				
			return ObjectType.Unknow;
		}

		public static string GetObjectIcon(Widget obj)
		{
			//	Retourne l'icône d'un objet.
			ObjectModifier.ObjectType type = ObjectModifier.GetObjectType(obj);
			return ObjectModifier.GetObjectIcon(type);
		}

		protected static string GetObjectIcon(ObjectType type)
		{
			//	Retourne l'icône d'un objet.
			switch (type)
			{
				case ObjectType.HSeparator:   return "ObjectHLine";
				case ObjectType.VSeparator:   return "ObjectVLine";
				case ObjectType.Button:	      return "ObjectRectButton";
				case ObjectType.Table:        return "ObjectTable";
				case ObjectType.Placeholder:  return "ObjectText";
				case ObjectType.StaticText:   return "ObjectStatic";
				case ObjectType.Group:        return "ObjectGroup";
				case ObjectType.GroupFrame:   return "ObjectGroupFrame";
				case ObjectType.GroupBox:     return "ObjectGroupBox";
				case ObjectType.MainPanel:    return "ObjectMainPanel";
				case ObjectType.SubPanel:     return "ObjectPanel";
			}

			return null;
		}


		#region Druid
		public static bool HasDruid(Widget obj)
		{
			//	Indique si l'objet a un druid.
			ObjectType type = ObjectModifier.GetObjectType(obj);
			return (type == ObjectType.Button     ||
					type == ObjectType.StaticText ||
					type == ObjectType.GroupBox   ||
					type == ObjectType.SubPanel   );
		}

		public static void SetDruid(Widget obj, string druid)
		{
			//	Modifie le druid de l'objet.
			Druid d = Druid.Parse(druid);
			if (!d.IsValid)
			{
				return;
			}

			ObjectType type = ObjectModifier.GetObjectType(obj);
			if (type == ObjectType.Button)
			{
				obj.CommandId = d;
			}
			else if (type == ObjectType.StaticText || type == ObjectType.GroupBox)
			{
				obj.CaptionId = d;
			}
			else if (type == ObjectType.SubPanel)
			{
				UI.PanelPlaceholder panel = obj as UI.PanelPlaceholder;
				System.Diagnostics.Debug.Assert(panel.ResourceManager != null);
				panel.PanelId = d;
			}
		}

		public static string GetDruid(Widget obj)
		{
			//	Retourne le druid de l'objet.
			//	On peut obtenir le druid du panneau de base (MainPanel), bien que la
			//	méthode HasDruid retourne false !
			Druid druid = Druid.Empty;

			ObjectType type = ObjectModifier.GetObjectType(obj);
			if (type == ObjectType.Button)
			{
				druid = obj.CommandId;
			}
			else if (type == ObjectType.StaticText || type == ObjectType.GroupBox)
			{
				druid = obj.CaptionId;
			}
			else if (type == ObjectType.SubPanel)
			{
				UI.PanelPlaceholder panel = obj as UI.PanelPlaceholder;
				druid = panel.PanelId;
			}
			else if (type == ObjectType.MainPanel)
			{
				// Si nous venons de tomber sur le UI.Panel racine, celui-ci n'est pas
				// contenu dans un PanelPlaceholder: on ne peut pas obtenir son DRUID
				// de la même manière que pour les autres panels et il faut recourir à
				// une aide externe.
				// Quand ResourceAccess définit le lien entre bundle et panel au moyen
				// de UI.Panel.SetPanel(bundle, panel), UI.Panel prend aussi note du
				// lien inverse, à savoir panel-->bundle en conservant le DRUID du
				// bundle. Ce DRUID est accessible via la méthode UI.Panel.GetBundleId.
				druid = UI.Panel.GetBundleId(obj);
			}

			return druid.ToString();
		}
		#endregion


		#region TableColumns
		public static void TableDefineAllColumns(Widget obj)
		{
			//	Ajoute les colonnes avec des réglages par défaut si l'objet
			//	en cours d'édition est une table qui n'a aucune colonne (donc
			//	qui a été fraîchement créée).
			UI.TablePlaceholder table = obj as UI.TablePlaceholder;
			System.Diagnostics.Debug.Assert(table != null);

			if (table != null && table.Columns.Count == 0)
			{
				StructuredType sourceType = ObjectModifier.GetTableStructuredType(obj);
				foreach (string fieldId in sourceType.GetFieldIds())
				{
					table.Columns.Add(new UI.ItemTableColumn(fieldId));
				}
			}
		}

		public static StructuredType GetTableStructuredType(Widget obj)
		{
			//	Retourne la structure de données d'un objet 'Table'.
			UI.TablePlaceholder table = obj as UI.TablePlaceholder;
			System.Diagnostics.Debug.Assert(table != null);

			//	TODO: à supprimer un jour !!!
			Druid sourceTypeId = table.SourceTypeId;
			ResourceManager resourceManager = Widgets.Helpers.VisualTree.GetResourceManager(obj);
			Caption sourceTypeCaption = resourceManager.GetCaption(sourceTypeId);
			return TypeRosetta.GetTypeObject(sourceTypeCaption) as StructuredType;
		}
		#endregion


		#region Binding
		public static bool HasBinding(Widget obj)
		{
			//	Indique si l'objet a du binding.
			ObjectType type = ObjectModifier.GetObjectType(obj);
			return (type == ObjectType.Placeholder || type == ObjectType.SubPanel || type == ObjectType.Table);
		}

		public static void SetBinding(Widget obj, Binding binding, StructuredType structuredType)
		{
			//	Modifie le binding de l'objet.
			ObjectType type = ObjectModifier.GetObjectType(obj);

			if (type == ObjectType.Placeholder || type == ObjectType.SubPanel || type == ObjectType.Table)
			{
				UI.AbstractPlaceholder ph = obj as UI.AbstractPlaceholder;

				if (binding == null)
				{
					ph.ClearBinding(ph.GetValueProperty());
				}
				else
				{
					ph.SetBinding(ph.GetValueProperty(), binding);

					if (type == ObjectType.Table)
					{
						System.Diagnostics.Debug.Assert(binding != null);
						System.Diagnostics.Debug.Assert(structuredType != null);

						string path = binding.Path;
						path = path.StartsWith("*.") ? path.Substring(2) : path;

						StructuredTypeField field = StructuredTree.GetField(structuredType, path);
						UI.TablePlaceholder table = obj as UI.TablePlaceholder;

						System.Diagnostics.Debug.Assert(field != null);
						System.Diagnostics.Debug.Assert(field.Relation == FieldRelation.Collection);

						table.SourceTypeId = field.Type.CaptionId;
					}
				}
			}
		}

		public static Binding GetBinding(Widget obj)
		{
			//	Retourne le binding de l'objet.
			ObjectType type = ObjectModifier.GetObjectType(obj);

			if (type == ObjectType.Placeholder || type == ObjectType.SubPanel || type == ObjectType.Table)
			{
				UI.AbstractPlaceholder ph = obj as UI.AbstractPlaceholder;
				Types.Binding binding = ph.GetBinding(ph.GetValueProperty());
				return binding;
			}

			return null;
		}

		public static bool HasStructuredType(Widget obj)
		{
			//	Indique si l'objet a une structure de données associée.
			ObjectType type = ObjectModifier.GetObjectType(obj);
			return type == ObjectType.MainPanel;
		}

		public static void SetStructuredType(Widget obj, StructuredType st)
		{
			//	Modifie la structure de données associée à l'objet.
			ObjectType type = ObjectModifier.GetObjectType(obj);
			if (type == ObjectType.MainPanel)
			{
				UI.Panel panel = obj as UI.Panel;
				panel.DataSourceMetadata.DefaultDataType = st;
				panel.SetupSampleDataSource();
			}
		}

		public static StructuredType GetStructuredType(Widget obj)
		{
			//	Retourne la structure de données associée de l'objet.
			ObjectType type = ObjectModifier.GetObjectType(obj);
			if (type == ObjectType.MainPanel)
			{
				UI.Panel panel = obj as UI.Panel;
				return panel.DataSourceMetadata.DefaultDataType as StructuredType;
			}

			return null;
		}
		#endregion


		#region ChildrenPlacement
		public bool AreChildrenAnchored(Widget obj)
		{
			if (ObjectModifier.IsAbstractGroup(obj))
			{
				ChildrenPlacement p = this.GetChildrenPlacement(obj);
				return (p == ChildrenPlacement.Anchored);
			}
			return false;
		}

		public bool AreChildrenStacked(Widget obj)
		{
			if (ObjectModifier.IsAbstractGroup(obj))
			{
				ChildrenPlacement p = this.GetChildrenPlacement(obj);
				return (p == ChildrenPlacement.HorizontalStacked || p == ChildrenPlacement.VerticalStacked);
			}
			return false;
		}

		public bool AreChildrenGrid(Widget obj)
		{
			if (ObjectModifier.IsAbstractGroup(obj))
			{
				ChildrenPlacement p = this.GetChildrenPlacement(obj);
				return (p == ChildrenPlacement.Grid);
			}
			return false;
		}

		public bool AreChildrenHorizontal(Widget obj)
		{
			if (ObjectModifier.IsAbstractGroup(obj))
			{
				ChildrenPlacement p = this.GetChildrenPlacement(obj);
				return (p == ChildrenPlacement.HorizontalStacked);
			}
			return false;
		}

		public bool HasChildrenPlacement(Widget obj)
		{
			//	Indique s'il existe un mode de placement des enfants de l'objet.
			return ObjectModifier.IsAbstractGroup(obj);
		}

		public ChildrenPlacement GetChildrenPlacement(Widget obj)
		{
			//	Retourne le mode de placement des enfants de l'objet.
			//	Uniquement pour les objects AbstractGroup.
			if (ObjectModifier.IsAbstractGroup(obj))
			{
				AbstractGroup group = obj as AbstractGroup;

				if (group.ChildrenLayoutMode == LayoutMode.Anchored)
				{
					return ChildrenPlacement.Anchored;
				}

				if (group.ChildrenLayoutMode == LayoutMode.Stacked)
				{
					if (group.ContainerLayoutMode == ContainerLayoutMode.HorizontalFlow)
					{
						return ChildrenPlacement.HorizontalStacked;
					}
					else
					{
						return ChildrenPlacement.VerticalStacked;
					}
				}

				if (group.ChildrenLayoutMode == LayoutMode.Grid)
				{
					return ChildrenPlacement.Grid;
				}
			}

			return ChildrenPlacement.None;
		}

		public void SetChildrenPlacement(Widget obj, ChildrenPlacement mode)
		{
			//	Choix du mode de placement des enfants de l'objet.
			//	Uniquement pour les objects AbstractGroup.
			AbstractGroup group = obj as AbstractGroup;
			System.Diagnostics.Debug.Assert(group != null);

			switch (mode)
			{
				case ChildrenPlacement.Anchored:
					group.ChildrenLayoutMode = LayoutMode.Anchored;
					LayoutEngine.SetLayoutEngine(group, null);
					break;

				case ChildrenPlacement.HorizontalStacked:
					group.ChildrenLayoutMode = LayoutMode.Stacked;
					group.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
					LayoutEngine.SetLayoutEngine(group, null);
					break;

				case ChildrenPlacement.VerticalStacked:
					group.ChildrenLayoutMode = LayoutMode.Stacked;
					group.ContainerLayoutMode = ContainerLayoutMode.VerticalFlow;
					LayoutEngine.SetLayoutEngine(group, null);
					break;

				case ChildrenPlacement.Grid:
					group.ChildrenLayoutMode = LayoutMode.Grid;
					this.SetGridColumnsCount(obj, 2);
					this.SetGridRowsCount(obj, 2);
					break;
			}

			this.Invalidate();
		}
		#endregion


		#region Grid
		public void GridColumnAdd(Widget obj, int column, bool after)
		{
			//	Insère une colonne en poussant les suivantes.
			this.SetGridColumnsCount(obj, this.GetGridColumnsCount(obj)+1);
			this.GridColumnShift(obj, after ? column+1 : column, column+1, 1);
		}

		public void GridColumnRemove(Widget obj, int column)
		{
			//	Supprime une colonne en décalant les suivantes.
			//	Pour cela, la colonne ne doit contenir aucun objet.
			if (this.IsGridColumnEmpty(obj, column))
			{
				this.GridColumnShift(obj, column+1, column+1, -1);
				this.SetGridColumnsCount(obj, this.GetGridColumnsCount(obj)-1);
			}
		}

		public void GridRowAdd(Widget obj, int row, bool after)
		{
			//	Insère une ligne en poussant les suivantes.
			this.SetGridRowsCount(obj, this.GetGridRowsCount(obj)+1);
			this.GridRowShift(obj, after ? row+1 : row, row+1, 1);
		}

		public void GridRowRemove(Widget obj, int row)
		{
			//	Supprime une ligne en décalant les suivantes.
			//	Pour cela, la colonne ne doit contenir aucun objet.
			if (this.IsGridRowEmpty(obj, row))
			{
				this.GridRowShift(obj, row+1, row+1, -1);
				this.SetGridRowsCount(obj, this.GetGridRowsCount(obj)-1);
			}
		}

		public bool IsGridColumnEmpty(Widget obj, int column)
		{
			//	Vérifie si une colonne est entièrement vide.
			foreach (Widget children in obj.Children)
			{
				int c1 = this.GetGridColumn(children);
				int c2 = c1+this.GetGridColumnSpan(children)-1;

				if (column >= c1 && column <= c2)
				{
					return false;
				}
			}
			return true;
		}

		public bool IsGridRowEmpty(Widget obj, int row)
		{
			//	Vérifie si une ligne est entièrement vide.
			foreach (Widget children in obj.Children)
			{
				int r1 = this.GetGridRow(children);
				int r2 = r1+this.GetGridRowSpan(children)-1;

				if (row >= r1 && row <= r2)
				{
					return false;
				}
			}
			return true;
		}

		protected void GridColumnShift(Widget obj, int column, int columnGeometry, int direction)
		{
			//	Décale les colonnes.
			if (direction > 0)
			{
				for (int i=this.GetGridColumnsCount(obj)-1; i>=columnGeometry; i--)
				{
					this.GridColumnShiftGeometry(obj, i-1, i);
				}
			}
			else
			{
				for (int i=columnGeometry; i<this.GetGridColumnsCount(obj); i++)
				{
					this.GridColumnShiftGeometry(obj, i, i-1);
				}
			}

			foreach (Widget children in obj.Children)
			{
				int c = this.GetGridColumn(children);
				if (c >= column)
				{
					this.SetGridColumn(children, c+direction);
				}
			}
		}

		protected void GridRowShift(Widget obj, int row, int rowGeometry, int direction)
		{
			//	Décale les lignes.
			if (direction > 0)
			{
				for (int i=this.GetGridRowsCount(obj)-1; i>=rowGeometry; i--)
				{
					this.GridRowShiftGeometry(obj, i-1, i);
				}
			}
			else
			{
				for (int i=rowGeometry; i<this.GetGridRowsCount(obj); i++)
				{
					this.GridRowShiftGeometry(obj, i, i-1);
				}
			}

			foreach (Widget children in obj.Children)
			{
				int r = this.GetGridRow(children);
				if (r >= row)
				{
					this.SetGridRow(children, r+direction);
				}
			}
		}

		protected void GridColumnShiftGeometry(Widget obj, int src, int dst)
		{
			//	Copie les informations de géométrie d'une colonne.
			this.SetGridColumnMode(obj, dst, this.GetGridColumnMode(obj, src));  // (*)
			this.SetGridColumnWidth(obj, dst, this.GetGridColumnWidth(obj, src));
			this.SetGridColumnMinWidth(obj, dst, this.GetGridColumnMinWidth(obj, src));
			this.SetGridColumnMaxWidth(obj, dst, this.GetGridColumnMaxWidth(obj, src));
			this.SetGridColumnLeftBorder(obj, dst, this.GetGridColumnLeftBorder(obj, src));
			this.SetGridColumnRightBorder(obj, dst, this.GetGridColumnRightBorder(obj, src));
		}

		protected void GridRowShiftGeometry(Widget obj, int src, int dst)
		{
			//	Copie les informations de géométrie d'une ligne.
			this.SetGridRowMode(obj, dst, this.GetGridRowMode(obj, src));  // (*)
			this.SetGridRowHeight(obj, dst, this.GetGridRowHeight(obj, src));
			this.SetGridRowMinHeight(obj, dst, this.GetGridRowMinHeight(obj, src));
			this.SetGridRowMaxHeight(obj, dst, this.GetGridRowMaxHeight(obj, src));
			this.SetGridRowTopBorder(obj, dst, this.GetGridRowTopBorder(obj, src));
			this.SetGridRowBottomBorder(obj, dst, this.GetGridRowBottomBorder(obj, src));
		}

		// (*)	Il faut copier le mode en premier, car cela modifie la largeur/hauteur !


		public int GetGridColumnsCount(Widget obj)
		{
			//	Retourne le nombre total de colonnes de l'objet group.
			if (this.AreChildrenGrid(obj))
			{
				GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
				if (engine != null)
				{
					return engine.ColumnDefinitions.Count;
				}
			}

			return 0;
		}

		public void SetGridColumnsCount(Widget obj, int columns)
		{
			//	Détermine le nombre total de colonnes de l'objet group.
			System.Diagnostics.Debug.Assert(this.AreChildrenGrid(obj));

			GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
			if (engine == null)
			{
				engine = new GridLayoutEngine();
				LayoutEngine.SetLayoutEngine(obj, engine);
			}

			columns = System.Math.Max(columns, engine.MaxColumnIndex+1);
			GridSelection gs = GridSelection.Get(obj);

			while (columns != engine.ColumnDefinitions.Count)
			{
				int count = engine.ColumnDefinitions.Count;

				if (columns > count)
				{
					double value = 100;
					GridUnitType type = GridUnitType.Auto;
					double minWidth = 20;

					if (count > 0)
					{
						GridLength gl = engine.ColumnDefinitions[count-1].Width;
						value = gl.Value;
						type = gl.GridUnitType;
						minWidth = engine.ColumnDefinitions[count-1].MinWidth;
					}

					engine.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(value, type)));
					engine.ColumnDefinitions[count].MinWidth = minWidth;
				}
				else
				{
					engine.ColumnDefinitions.RemoveAt(count-1);

					if (gs != null)
					{
						int i = gs.Search(GridSelection.Unit.Column, count-1);
						if (i != -1)
						{
							gs.RemoveAt(i);  // supprime la colonne sélectionnée
						}
					}
				}
			}

			this.Invalidate();
		}

		public int GetGridRowsCount(Widget obj)
		{
			//	Retourne le nombre total de colonnes de l'objet group.
			if (this.AreChildrenGrid(obj))
			{
				GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
				if (engine != null)
				{
					return engine.RowDefinitions.Count;
				}
			}

			return 0;
		}

		public void SetGridRowsCount(Widget obj, int rows)
		{
			//	Détermine le nombre total de colonnes de l'objet group.
			System.Diagnostics.Debug.Assert(this.AreChildrenGrid(obj));

			GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
			if (engine == null)
			{
				engine = new GridLayoutEngine();
				LayoutEngine.SetLayoutEngine(obj, engine);
			}

			rows = System.Math.Max(rows, engine.MaxRowIndex+1);
			GridSelection gs = GridSelection.Get(obj);

			while (rows != engine.RowDefinitions.Count)
			{
				int count = engine.RowDefinitions.Count;

				if (rows > count)
				{
					double value = 100;
					GridUnitType type = GridUnitType.Auto;
					double minHeight = 20;

					if (count > 0)
					{
						GridLength gl = engine.RowDefinitions[count-1].Height;
						value = gl.Value;
						type = gl.GridUnitType;
						minHeight = engine.RowDefinitions[count-1].MinHeight;
					}

					engine.RowDefinitions.Add(new RowDefinition(new GridLength(value, type)));
					engine.RowDefinitions[count].MinHeight = minHeight;
				}
				else
				{
					engine.RowDefinitions.RemoveAt(count-1);

					if (gs != null)
					{
						int i = gs.Search(GridSelection.Unit.Row, count-1);
						if (i != -1)
						{
							gs.RemoveAt(i);  // supprime la ligne sélectionnée
						}
					}
				}
			}

			this.Invalidate();
		}


		public GridMode GetGridColumnMode(Widget obj, int column)
		{
			//	Retourne le mode d'une colonne.
			if (this.AreChildrenGrid(obj))
			{
				GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
				if (engine != null)
				{
					ColumnDefinition def = engine.ColumnDefinitions[column];
					GridLength gl = def.Width;
					return ObjectModifier.GridConvert(gl.GridUnitType);
				}
			}

			return GridMode.None;
		}

		public void SetGridColumnMode(Widget obj, int column, GridMode mode)
		{
			//	Modifie le mode d'une colonne.
			System.Diagnostics.Debug.Assert(this.AreChildrenGrid(obj));

			GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
			System.Diagnostics.Debug.Assert(engine != null);

			if (this.GetGridColumnMode(obj, column) != mode)
			{
				GridUnitType type = ObjectModifier.GridConvert(mode);
				double value = (type == GridUnitType.Absolute) ? 100 : 100;

				ColumnDefinition def = engine.ColumnDefinitions[column];
				def.Width = new GridLength(value, type);

				this.Invalidate();
			}
		}

		public GridMode GetGridRowMode(Widget obj, int row)
		{
			//	Retourne le mode d'une ligne.
			if (this.AreChildrenGrid(obj))
			{
				GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
				if (engine != null)
				{
					RowDefinition def = engine.RowDefinitions[row];
					GridLength gl = def.Height;
					return ObjectModifier.GridConvert(gl.GridUnitType);
				}
			}

			return GridMode.None;
		}

		public void SetGridRowMode(Widget obj, int row, GridMode mode)
		{
			//	Modifie le mode d'une ligne.
			System.Diagnostics.Debug.Assert(this.AreChildrenGrid(obj));

			GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
			System.Diagnostics.Debug.Assert(engine != null);

			if (this.GetGridRowMode(obj, row) != mode)
			{
				GridUnitType type = ObjectModifier.GridConvert(mode);
				double value = (type == GridUnitType.Absolute) ? 20 : 100;

				RowDefinition def = engine.RowDefinitions[row];
				def.Height = new GridLength(value, type);

				this.Invalidate();
			}
		}

		public double GetGridColumnWidth(Widget obj, int column)
		{
			//	Retourne la largeur d'une colonne.
			if (this.AreChildrenGrid(obj))
			{
				GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
				if (engine != null)
				{
					ColumnDefinition def = engine.ColumnDefinitions[column];
					GridLength gl = def.Width;
					return gl.Value;
				}
			}

			return 0;
		}

		public void SetGridColumnWidth(Widget obj, int column, double width)
		{
			//	Modifie la largeur d'une colonne.
			System.Diagnostics.Debug.Assert(this.AreChildrenGrid(obj));

			GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
			System.Diagnostics.Debug.Assert(engine != null);

			ColumnDefinition def = engine.ColumnDefinitions[column];
			def.Width = new GridLength(width, def.Width.GridUnitType);

			this.Invalidate();
		}

		public double GetGridRowHeight(Widget obj, int row)
		{
			//	Retourne la hauteur d'une ligne.
			if (this.AreChildrenGrid(obj))
			{
				GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
				if (engine != null)
				{
					RowDefinition def = engine.RowDefinitions[row];
					GridLength gl = def.Height;
					return gl.Value;
				}
			}

			return 0;
		}

		public void SetGridRowHeight(Widget obj, int row, double height)
		{
			//	Modifie la hauteur d'une ligne.
			System.Diagnostics.Debug.Assert(this.AreChildrenGrid(obj));

			GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
			System.Diagnostics.Debug.Assert(engine != null);

			RowDefinition def = engine.RowDefinitions[row];
			def.Height = new GridLength(height, def.Height.GridUnitType);

			this.Invalidate();
		}

		static protected GridUnitType GridConvert(GridMode mode)
		{
			switch (mode)
			{
				case GridMode.Proportional:  return GridUnitType.Proportional;
				case GridMode.Absolute:      return GridUnitType.Absolute;
				case GridMode.Auto:          return GridUnitType.Auto;
			}

			return GridUnitType.Proportional;
		}

		static protected GridMode GridConvert(GridUnitType type)
		{
			switch (type)
			{
				case GridUnitType.Proportional:  return GridMode.Proportional;
				case GridUnitType.Absolute:      return GridMode.Absolute;
				case GridUnitType.Auto:          return GridMode.Auto;
			}

			return GridMode.None;
		}


		public double GetGridColumnMinWidth(Widget obj, int column)
		{
			//	Retourne la largeur minimale d'une colonne.
			if (this.AreChildrenGrid(obj))
			{
				GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
				if (engine != null)
				{
					ColumnDefinition def = engine.ColumnDefinitions[column];
					return def.MinWidth;
				}
			}

			return 0;
		}

		public void SetGridColumnMinWidth(Widget obj, int column, double width)
		{
			//	Modifie la largeur minimale d'une colonne.
			System.Diagnostics.Debug.Assert(this.AreChildrenGrid(obj));

			GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
			System.Diagnostics.Debug.Assert(engine != null);

			ColumnDefinition def = engine.ColumnDefinitions[column];
			if (def.MinWidth != width)
			{
				def.MinWidth = width;
				this.Invalidate();
			}
		}

		public double GetGridColumnMaxWidth(Widget obj, int column)
		{
			//	Retourne la largeur maximale d'une colonne.
			if (this.AreChildrenGrid(obj))
			{
				GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
				if (engine != null)
				{
					ColumnDefinition def = engine.ColumnDefinitions[column];
					if (System.Double.IsInfinity(def.MaxWidth))  return 9999;  // TODO: à supprimer !
					return def.MaxWidth;
				}
			}

			return 0;
		}

		public void SetGridColumnMaxWidth(Widget obj, int column, double width)
		{
			//	Modifie la largeur maximale d'une colonne.
			System.Diagnostics.Debug.Assert(this.AreChildrenGrid(obj));

			GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
			System.Diagnostics.Debug.Assert(engine != null);

			ColumnDefinition def = engine.ColumnDefinitions[column];
			if (def.MaxWidth != width)
			{
				def.MaxWidth = width;
				this.Invalidate();
			}
		}

		public double GetGridRowMinHeight(Widget obj, int row)
		{
			//	Retourne la hauteur minimale d'une ligne.
			if (this.AreChildrenGrid(obj))
			{
				GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
				if (engine != null)
				{
					RowDefinition def = engine.RowDefinitions[row];
					return def.MinHeight;
				}
			}

			return 0;
		}

		public void SetGridRowMinHeight(Widget obj, int row, double height)
		{
			//	Modifie la hauteur minimale d'une ligne.
			System.Diagnostics.Debug.Assert(this.AreChildrenGrid(obj));

			GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
			System.Diagnostics.Debug.Assert(engine != null);

			RowDefinition def = engine.RowDefinitions[row];
			if (def.MinHeight != height)
			{
				def.MinHeight = height;
				this.Invalidate();
			}
		}

		public double GetGridRowMaxHeight(Widget obj, int row)
		{
			//	Retourne la hauteur maximale d'une ligne.
			if (this.AreChildrenGrid(obj))
			{
				GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
				if (engine != null)
				{
					RowDefinition def = engine.RowDefinitions[row];
					if (System.Double.IsInfinity(def.MaxHeight))  return 9999;  // TODO: à supprimer !
					return def.MaxHeight;
				}
			}

			return 0;
		}

		public void SetGridRowMaxHeight(Widget obj, int row, double height)
		{
			//	Modifie la hauteur maximale d'une ligne.
			System.Diagnostics.Debug.Assert(this.AreChildrenGrid(obj));

			GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
			System.Diagnostics.Debug.Assert(engine != null);

			RowDefinition def = engine.RowDefinitions[row];
			if (def.MaxHeight != height)
			{
				def.MaxHeight = height;
				this.Invalidate();
			}
		}


		public double GetGridColumnLeftBorder(Widget obj, int column)
		{
			//	Retourne le bord gauche d'une colonne.
			if (this.AreChildrenGrid(obj))
			{
				GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
				if (engine != null)
				{
					ColumnDefinition def = engine.ColumnDefinitions[column];
					return def.LeftBorder;
				}
			}

			return 0;
		}

		public void SetGridColumnLeftBorder(Widget obj, int column, double value)
		{
			//	Modifie le bord gauche d'une colonne.
			System.Diagnostics.Debug.Assert(this.AreChildrenGrid(obj));

			GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
			System.Diagnostics.Debug.Assert(engine != null);

			ColumnDefinition def = engine.ColumnDefinitions[column];
			if (def.LeftBorder != value)
			{
				def.LeftBorder = value;
				this.Invalidate();
			}
		}

		public double GetGridColumnRightBorder(Widget obj, int column)
		{
			//	Retourne le bord droite d'une colonne.
			if (this.AreChildrenGrid(obj))
			{
				GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
				if (engine != null)
				{
					ColumnDefinition def = engine.ColumnDefinitions[column];
					return def.RightBorder;
				}
			}

			return 0;
		}

		public void SetGridColumnRightBorder(Widget obj, int column, double value)
		{
			//	Modifie le bord droite d'une colonne.
			System.Diagnostics.Debug.Assert(this.AreChildrenGrid(obj));

			GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
			System.Diagnostics.Debug.Assert(engine != null);

			ColumnDefinition def = engine.ColumnDefinitions[column];
			if (def.RightBorder != value)
			{
				def.RightBorder = value;
				this.Invalidate();
			}
		}


		public double GetGridRowTopBorder(Widget obj, int row)
		{
			//	Retourne le bord supérieur d'une colonne.
			if (this.AreChildrenGrid(obj))
			{
				GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
				if (engine != null)
				{
					RowDefinition def = engine.RowDefinitions[row];
					return def.TopBorder;
				}
			}

			return 0;
		}

		public void SetGridRowTopBorder(Widget obj, int row, double value)
		{
			//	Modifie le bord supérieur d'une colonne.
			System.Diagnostics.Debug.Assert(this.AreChildrenGrid(obj));

			GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
			System.Diagnostics.Debug.Assert(engine != null);

			RowDefinition def = engine.RowDefinitions[row];
			if (def.TopBorder != value)
			{
				def.TopBorder = value;
				this.Invalidate();
			}
		}

		public double GetGridRowBottomBorder(Widget obj, int row)
		{
			//	Retourne le bord inférieur d'une colonne.
			if (this.AreChildrenGrid(obj))
			{
				GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
				if (engine != null)
				{
					RowDefinition def = engine.RowDefinitions[row];
					return def.BottomBorder;
				}
			}

			return 0;
		}

		public void SetGridRowBottomBorder(Widget obj, int row, double value)
		{
			//	Modifie le bord inférieur d'une colonne.
			System.Diagnostics.Debug.Assert(this.AreChildrenGrid(obj));

			GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
			System.Diagnostics.Debug.Assert(engine != null);

			RowDefinition def = engine.RowDefinitions[row];
			if (def.BottomBorder != value)
			{
				def.BottomBorder = value;
				this.Invalidate();
			}
		}


		public bool IsGridCellEmpty(Widget obj, Widget exclude, int column, int row, int columnCount, int rowCount)
		{
			if (column < 0 || column+columnCount > this.GetGridColumnsCount(obj))
			{
				return false;
			}

			if (row < 0 || row+rowCount > this.GetGridRowsCount(obj))
			{
				return false;
			}

			for (int c=column; c<column+columnCount; c++)
			{
				for (int r=row; r<row+rowCount; r++)
				{
					if (!this.IsGridCellEmpty(obj, exclude, c, r))
					{
						return false;
					}
				}
			}
			return true;
		}

		public bool IsGridCellEmpty(Widget obj, Widget exclude, int column, int row)
		{
			//	Indique si une cellule est libre, donc si elle ne contient aucun widget.
			if (column == GridSelection.Invalid || row == GridSelection.Invalid)
			{
				return false;
			}

			return (this.GetGridCellWidget(obj, exclude, column, row) == null);
		}

		public Widget GetGridCellWidget(Widget container, Widget exclude, int column, int row)
		{
			//	Retourne le premier widget occupant une cellule donnée.
			foreach (Widget widget in this.GetGridCellWidgets(container, exclude, column, row))
			{
				return widget;
			}
			
			return null;
		}

		protected IEnumerable<Widget> GetGridCellWidgets(Widget container, Widget exclude, int column, int row)
		{
			//	Retourne tous les widgets occupant une cellule donnée, en tenant
			//	compte de leur span.
			if (this.AreChildrenGrid(container) && container.HasChildren)
			{
				foreach (Widget child in container.Children)
				{
					if (child == exclude)
					{
						continue;
					}

					int c = GridLayoutEngine.GetColumn(child);
					int r = GridLayoutEngine.GetRow(child);

					if (c < 0 || c > column)
					{
						continue;
					}
					if (r < 0 || r > row)
					{
						continue;
					}

					int columnSpan = GridLayoutEngine.GetColumnSpan(child);
					int rowSpan    = GridLayoutEngine.GetRowSpan(child);

					//	Si le widget implémente IGridPermeable, cela implique qu'il a son
					//	mot à dire sur le nombre de lignes et/ou colonnes qu'il utilise :
					IGridPermeable permeable = child as IGridPermeable;
					
					if (permeable != null)
					{
						permeable.UpdateGridSpan(ref columnSpan, ref rowSpan);
					}
					
					if (c <= column && c+columnSpan-1 >= column &&
						r <= row    && r+rowSpan-1    >= row    )
					{
						yield return child;
					}
				}
			}
		}


		public void SetGridClear(Widget obj)
		{
			//	Annule les informations d'appartenance à une cellule, lorsque l'objet n'est
			//	plus dans un tableau.
			GridLayoutEngine.SetColumn(obj, -1);
			GridLayoutEngine.SetRow(obj, -1);

			GridLayoutEngine.SetColumnSpan(obj, 1);
			GridLayoutEngine.SetRowSpan(obj, 1);
		}

		public bool SetGridParentColumnRow(Widget obj, Widget parent, int column, int row)
		{
			//	Détermine la cellule dans un tableau à laquelle appartient l'objet.
			System.Diagnostics.Debug.Assert(this.AreChildrenGrid(parent));
			bool isChanging = false;

			obj.Anchor = AnchorStyles.None;
			obj.Dock = DockStyle.None;

			if (GridLayoutEngine.GetColumn(obj) != column)
			{
				GridLayoutEngine.SetColumn(obj, column);
				isChanging = true;
			}

			if (GridLayoutEngine.GetRow(obj) != row)
			{
				GridLayoutEngine.SetRow(obj, row);
				isChanging = true;
			}

			if (obj.Parent != parent)
			{
				obj.SetParent(parent);
				isChanging = true;
			}

			return isChanging;
		}

		public void SetGridColumn(Widget obj, int column)
		{
			//	Modifie la colonne à laquelle appartient l'objet.
			if (this.AreChildrenGrid(obj.Parent))
			{
				GridLayoutEngine.SetColumn(obj, column);
			}
		}

		public int GetGridColumn(Widget obj)
		{
			//	Retourne la colonne à laquelle appartient l'objet.
			if (this.AreChildrenGrid(obj.Parent))
			{
				return GridLayoutEngine.GetColumn(obj);
			}

			return 0;
		}

		public void SetGridRow(Widget obj, int row)
		{
			//	Modifie la ligne à laquelle appartient l'objet.
			if (this.AreChildrenGrid(obj.Parent))
			{
				GridLayoutEngine.SetRow(obj, row);
			}
		}

		public int GetGridRow(Widget obj)
		{
			//	Retourne la ligne à laquelle appartient l'objet.
			if (this.AreChildrenGrid(obj.Parent))
			{
				return GridLayoutEngine.GetRow(obj);
			}

			return 0;
		}


		public void SetGridColumnSpan(Widget obj, int span)
		{
			//	Détermine le nombre de colonnes occupées.
			//	Empêche le chevauchement de deux objets.
			System.Diagnostics.Debug.Assert(this.AreChildrenGrid(obj.Parent));

			int ic = GridLayoutEngine.GetColumn(obj);
			int ir = GridLayoutEngine.GetRow(obj);
			int irs = GridLayoutEngine.GetRowSpan(obj);
			GridLayoutEngine.SetColumnSpan(obj, 1);
			GridLayoutEngine.SetRowSpan(obj, 1);

			span = System.Math.Min(span, this.GetGridColumnsCount(obj.Parent)-ic);

			int maxSpan = span;
			for (int c=0; c<span; c++)
			{
				for (int r=0; r<irs; r++)
				{
					if (!this.IsGridCellEmpty(obj.Parent, obj, ic+c, ir+r))
					{
						maxSpan = System.Math.Min(maxSpan, c);
						break;
					}
				}
			}

			span = System.Math.Min(span, maxSpan);
			GridLayoutEngine.SetColumnSpan(obj, span);
			GridLayoutEngine.SetRowSpan(obj, irs);
			this.Invalidate();
		}

		public int GetGridColumnSpan(Widget obj)
		{
			//	Retourne le nombre de colonnes occupées.
			if (this.AreChildrenGrid(obj.Parent))
			{
				return GridLayoutEngine.GetColumnSpan(obj);
			}

			return 0;
		}

		public void SetGridRowSpan(Widget obj, int span)
		{
			//	Détermine le nombre de lignes occupées.
			//	Empêche le chevauchement de deux objets.
			System.Diagnostics.Debug.Assert(this.AreChildrenGrid(obj.Parent));

			int ic = GridLayoutEngine.GetColumn(obj);
			int ir = GridLayoutEngine.GetRow(obj);
			int ics = GridLayoutEngine.GetColumnSpan(obj);
			GridLayoutEngine.SetColumnSpan(obj, 1);
			GridLayoutEngine.SetRowSpan(obj, 1);

			span = System.Math.Min(span, this.GetGridRowsCount(obj.Parent)-ir);

			int maxSpan = span;
			for (int r=0; r<span; r++)
			{
				for (int c=0; c<ics; c++)
				{
					if (!this.IsGridCellEmpty(obj.Parent, obj, ic+c, ir+r))
					{
						maxSpan = System.Math.Min(maxSpan, r);
						break;
					}
				}
			}

			span = System.Math.Min(span, maxSpan);
			GridLayoutEngine.SetRowSpan(obj, span);
			GridLayoutEngine.SetColumnSpan(obj, ics);
			this.Invalidate();
		}

		public int GetGridRowSpan(Widget obj)
		{
			//	Retourne le nombre de lignes occupées.
			if (this.AreChildrenGrid(obj.Parent))
			{
				return GridLayoutEngine.GetRowSpan(obj);
			}

			return 0;
		}


		public Rectangle GetGridItemArea(Widget obj, GridSelection.OneItem item)
		{
			//	Retourne la zone rectangulaire correspondant à une sélection.
			if (this.AreChildrenGrid(obj))
			{
				GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
				if (engine != null && item.Index != GridSelection.Invalid)
				{
					if (item.Unit == GridSelection.Unit.Column)
					{
						int x = item.Index;
						double x1 = this.GetGridColumnPosition(obj, x);
						double x2 = this.GetGridColumnPosition(obj, x+1);
						double y1 = this.GetGridRowPosition(obj, engine.RowDefinitions.Count);
						double y2 = this.GetGridRowPosition(obj, 0);
						return new Rectangle(x1, y1, x2-x1, y2-y1);
					}

					if (item.Unit == GridSelection.Unit.Row)
					{
						int y = item.Index;
						double x1 = this.GetGridColumnPosition(obj, 0);
						double x2 = this.GetGridColumnPosition(obj, engine.ColumnDefinitions.Count);
						double y1 = this.GetGridRowPosition(obj, y+1);
						double y2 = this.GetGridRowPosition(obj, y);
						return new Rectangle(x1, y1, x2-x1, y2-y1);
					}
				}
			}

			return Rectangle.Empty;
		}

		public Rectangle GetGridCellArea(Widget obj, int column, int row, int columnCount, int rowCount)
		{
			//	Retourne le rectangle d'une cellule.
			if (this.AreChildrenGrid(obj))
			{
				GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
				if (engine != null)
				{
					double x1 = this.GetGridColumnPosition(obj, column);
					double x2 = this.GetGridColumnPosition(obj, column+columnCount);
					double y1 = this.GetGridRowPosition(obj, row+rowCount);
					double y2 = this.GetGridRowPosition(obj, row);
					return new Rectangle(x1, y1, x2-x1, y2-y1);
				}
			}

			return Rectangle.Empty;
		}

		public double GetGridColumnPosition(Widget obj, int index)
		{
			//	Retourne la position d'une colonne.
			//	Accepte des index < 0 ou > que le nombre de colonnes, en prenant comme
			//	valeur la première ou la dernière colonne.
			if (this.AreChildrenGrid(obj) && index != GridSelection.Invalid)
			{
				GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
				if (engine != null)
				{
					Rectangle rect = this.GetFinalPadding(obj);
					double position = rect.Left;

					while (index < 0)
					{
						ColumnDefinition def = engine.ColumnDefinitions[0];
						position -= def.LeftBorder;
						position -= def.ActualWidth;
						position -= def.RightBorder;
						index++;
					}

					for (int i=0; i<index; i++)
					{
						int ii = System.Math.Min(i, engine.ColumnCount-1);
						ColumnDefinition def = engine.ColumnDefinitions[ii];
						position += def.LeftBorder;
						position += def.ActualWidth;
						position += def.RightBorder;
					}

					return position;
				}
			}

			return 0;
		}

		public double GetGridRowPosition(Widget obj, int index)
		{
			//	Retourne la position d'une ligne.
			//	Accepte des index < 0 ou > que le nombre de lignes, en prenant comme
			//	valeur la première ou la dernière ligne.
			if (this.AreChildrenGrid(obj) && index != GridSelection.Invalid)
			{
				GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
				if (engine != null)
				{
					Rectangle rect = this.GetFinalPadding(obj);
					double position = rect.Top;

					while (index < 0)
					{
						RowDefinition def = engine.RowDefinitions[0];
						position += def.TopBorder;
						position += def.ActualHeight;
						position += def.BottomBorder;
						index++;
					}

					for (int i=0; i<index; i++)
					{
						int ii = System.Math.Min(i, engine.RowCount-1);
						RowDefinition def = engine.RowDefinitions[ii];
						position -= def.TopBorder;
						position -= def.ActualHeight;
						position -= def.BottomBorder;
					}

					return position;
				}
			}

			return 0;
		}
		#endregion


		#region ButtonClass
		public bool HasButtonClass(Widget obj)
		{
			//	Indique s'il existe un mode pour le bouton.
			MetaButton button = obj as MetaButton;
			return (button != null);
		}

		public ButtonClass GetButtonClass(Widget obj)
		{
			//	Retourne le mode mode pour le bouton de l'objet.
			//	Uniquement pour les objects MetaButton.
			MetaButton button = obj as MetaButton;
			if (button != null)
			{
				return button.ButtonClass;
			}

			return ButtonClass.None;
		}

		public void SetButtonClass(Widget obj, ButtonClass mode)
		{
			//	Choix du mode pour le bouton de l'objet.
			//	Uniquement pour les objects MetaButton.
			MetaButton button = obj as MetaButton;
			System.Diagnostics.Debug.Assert(button != null);

			button.ButtonClass = mode;
			this.Invalidate();
		}
		#endregion


		public void AdaptFromParent(Widget obj, ObjectModifier.StackedHorizontalAttachment ha, ObjectModifier.StackedVerticalAttachment va)
		{
			//	Adapte un objet pour son parent.
			if (this.AreChildrenAnchored(obj.Parent))
			{
				if (this.GetAnchoredVerticalAttachment(obj) == ObjectModifier.AnchoredVerticalAttachment.None)
				{
					this.SetAnchoredHorizontalAttachment(obj, ObjectModifier.AnchoredHorizontalAttachment.Left);
					this.SetAnchoredVerticalAttachment(obj, ObjectModifier.AnchoredVerticalAttachment.Bottom);
				}
			}

			if (this.AreChildrenStacked(obj.Parent))
			{
				if (this.GetStackedHorizontalAttachment(obj) == ObjectModifier.StackedHorizontalAttachment.None||
					this.GetStackedVerticalAttachment(obj) == ObjectModifier.StackedVerticalAttachment.None)
				{
					this.SetMargins(obj, new Margins(5, 5, 5, 5));

					if (this.AreChildrenHorizontal(obj.Parent))
					{
						this.SetStackedHorizontalAttachment(obj, ha);
					}
					else
					{
						this.SetStackedVerticalAttachment(obj, va);
					}
				}
			}

			if (this.AreChildrenGrid(obj.Parent))
			{
				this.SetMargins(obj, new Margins(0, 0, 0, 0));
			}

			ObjectType type = ObjectModifier.GetObjectType(obj);
			if (type == ObjectType.StaticText ||
				type == ObjectType.Button ||
				type == ObjectType.Placeholder)
			{
				obj.PreferredHeight = obj.MinHeight;
			}
		}


		public Rectangle GetActualBounds(Widget obj)
		{
			//	Retourne la position et les dimensions actuelles de l'objet.
			//	Le rectangle rendu est toujours valide, quel que soit le mode d'attachement.
			obj.Window.ForceLayout();
			Rectangle bounds = obj.Client.Bounds;

			while (obj != null && obj != this.Container)
			{
				bounds = obj.MapClientToParent(bounds);
				obj = obj.Parent;
			}

			return bounds;
		}

		public bool HasBounds(Widget obj)
		{
			//	Indique si l'objet a une position et des dimensions modifiables.
			ChildrenPlacement pp = this.GetParentPlacement(obj);
			return (pp == ChildrenPlacement.Anchored || pp == ChildrenPlacement.Grid);
		}

		public bool HasBounds(Widget obj, BoundsMode mode)
		{
			//	Indique si l'objet a une position ou des dimensions modifiables.
			ChildrenPlacement pp = this.GetParentPlacement(obj);

			if (pp == ChildrenPlacement.Anchored)
			{
				return true;
			}

			if (pp == ChildrenPlacement.Grid)
			{
				if (mode == BoundsMode.OriginX || mode == BoundsMode.OriginY)
				{
					return false;
				}

				if (mode == BoundsMode.Width)
				{
					return HandlesList.HasWidthHandles(obj);
				}

				if (mode == BoundsMode.Height)
				{
					return HandlesList.HasHeightHandles(obj);
				}
			}

			return false;
		}

		public Rectangle GetBounds(Widget obj)
		{
			//	Retourne la position et les dimensions de l'objet.
			//	Le rectangle rendu est toujours valide, quel que soit le mode d'attachement.
			Point center = this.GetActualBounds(obj).Center;
			Size size = obj.PreferredSize;
			Rectangle bounds = new Rectangle(center.X-size.Width/2, center.Y-size.Height/2, size.Width, size.Height);

			ChildrenPlacement pp = this.GetParentPlacement(obj);
			if (pp == ChildrenPlacement.Anchored)
			{
				AnchoredHorizontalAttachment ha = this.GetAnchoredHorizontalAttachment(obj);
				if (ha == AnchoredHorizontalAttachment.Fill)
				{
					Rectangle actual = this.GetActualBounds(obj);
					bounds.Left = actual.Left;
					bounds.Right = actual.Right;
				}

				AnchoredVerticalAttachment va = this.GetAnchoredVerticalAttachment(obj);
				if (va == AnchoredVerticalAttachment.Fill)
				{
					Rectangle actual = this.GetActualBounds(obj);
					bounds.Bottom = actual.Bottom;
					bounds.Top = actual.Top;
				}
			}

			return bounds;
		}

		public void SetBounds(Widget obj, Rectangle bounds)
		{
			//	Choix de la position et des dimensions de l'objet.
			//	Uniquement pour les objets Anchored ou Grid.
			System.Diagnostics.Debug.Assert(this.HasBounds(obj));

			bounds.Normalise();

			if (bounds.Width < obj.MinWidth)
			{
				bounds.Width = obj.MinWidth;
			}

			if (bounds.Height < obj.MinHeight)
			{
				bounds.Height = obj.MinHeight;
			}

			obj.Window.ForceLayout();
			Widget parent = obj.Parent;
			while (parent != this.Container)
			{
				bounds = parent.MapParentToClient(bounds);
				parent = parent.Parent;
			}

			parent = obj.Parent;
			Rectangle box = parent.ActualBounds;
			Margins margins = obj.Margins;
			Margins padding = parent.Padding + parent.GetInternalPadding();
			AnchoredHorizontalAttachment ha = this.GetAnchoredHorizontalAttachment(obj);
			AnchoredVerticalAttachment va = this.GetAnchoredVerticalAttachment(obj);

			if (ha == AnchoredHorizontalAttachment.Left || ha == AnchoredHorizontalAttachment.Fill)
			{
				double px = bounds.Left;
				px -= padding.Left;
				px = System.Math.Max(px, 0);
				margins.Left = px;
			}

			if (ha == AnchoredHorizontalAttachment.Right || ha == AnchoredHorizontalAttachment.Fill)
			{
				double px = box.Width - bounds.Right;
				px -= padding.Right;
				px = System.Math.Max(px, 0);
				margins.Right = px;
			}

			if (va == AnchoredVerticalAttachment.Bottom || va == AnchoredVerticalAttachment.Fill)
			{
				double py = bounds.Bottom;
				py -= padding.Bottom;
				py = System.Math.Max(py, 0);
				margins.Bottom = py;
			}

			if (va == AnchoredVerticalAttachment.Top || va == AnchoredVerticalAttachment.Fill)
			{
				double py = box.Height - bounds.Top;
				py -= padding.Top;
				py = System.Math.Max(py, 0);
				margins.Top = py;
			}

			obj.Margins = margins;
			obj.PreferredSize = bounds.Size;

			this.Invalidate();
		}


		public bool HasMargins(Widget obj)
		{
			//	Indique si l'objet a des marges.
			ChildrenPlacement pp = this.GetParentPlacement(obj);
			if (pp == ChildrenPlacement.HorizontalStacked || pp == ChildrenPlacement.VerticalStacked || pp == ChildrenPlacement.Grid)
			{
				return true;
			}

			return false;
		}

		public Margins GetMargins(Widget obj)
		{
			//	Retourne les marges de l'objet.
			//	Uniquement pour les objets Stacked et Grid.
			if (this.HasMargins(obj))
			{
				return obj.Margins;
			}

			return Margins.Zero;
		}

		public void SetMargins(Widget obj, Margins margins)
		{
			//	Choix des marges de l'objet.
			//	Uniquement pour les objets Stacked et Grid.
			System.Diagnostics.Debug.Assert(this.HasMargins(obj));

			if (obj.Margins != margins)
			{
				obj.Margins = margins;
				this.Invalidate();
			}
		}


		public bool HasPadding(Widget obj)
		{
			//	Indique si l'objet a des marges internes.
			ObjectType type = ObjectModifier.GetObjectType(obj);
			return (type == ObjectType.Group      ||
					type == ObjectType.GroupFrame ||
					type == ObjectType.GroupBox   ||
					type == ObjectType.MainPanel  );
		}

		public Margins GetPadding(Widget obj)
		{
			//	Retourne les marges internes de l'objet.
			//	Uniquement pour les objets AbstractGroup.
			if (this.HasPadding(obj))
			{
				return obj.Padding;
			}

			return Margins.Zero;
		}

		public void SetPadding(Widget obj, Margins padding)
		{
			//	Choix des marges internes de l'objet.
			//	Uniquement pour les objets AbstractGroup.
			System.Diagnostics.Debug.Assert(this.HasPadding(obj));

			if (obj.Padding != padding)
			{
				obj.Padding = padding;
				this.Invalidate();
			}
		}

		public Rectangle GetFinalPadding(Widget obj)
		{
			//	Retourne le rectangle intérieur d'un objet AbstractGroup.
			Rectangle bounds = this.GetActualBounds(obj);

			if (this.HasPadding(obj))
			{
				bounds.Deflate(obj.GetInternalPadding());
				bounds.Deflate(obj.Padding);
			}

			return bounds;
		}


		public bool HasWidth(Widget obj)
		{
			//	Indique s'il est possible de modifier la largeur d'un objet.
			//	A ne pas confondre avec SetBounds pour le mode ancré. Un objet ancré
			//	pour lequel on peut faire un SetBounds n'accepte pas le SetWidth !
			ObjectType type = ObjectModifier.GetObjectType(obj);
			if (type == ObjectType.MainPanel)
			{
				return true;
			}

			if (!HandlesList.HasWidthHandles(obj))
			{
				return false;
			}

			ChildrenPlacement pp = this.GetParentPlacement(obj);
			return (pp != ChildrenPlacement.Anchored);
		}

		public double GetWidth(Widget obj)
		{
			//	Retourne la largeur de l'objet.
			if (this.HasWidth(obj))
			{
				return obj.PreferredWidth;
			}

			return 0;
		}

		public void SetWidth(Widget obj, double width)
		{
			//	Choix de la largeur de l'objet.
			System.Diagnostics.Debug.Assert(this.HasWidth(obj));

			if (this.GetWidth(obj) != width)
			{
				obj.PreferredWidth = width;
				this.Invalidate();
			}
		}


		public bool HasHeight(Widget obj)
		{
			//	Indique s'il est possible de modifier la hauteur d'un objet.
			//	A ne pas confondre avec SetBounds pour le mode ancré. Un objet ancré
			//	pour lequel on peut faire un SetBounds n'accepte pas le HasHeight !
			ObjectType type = ObjectModifier.GetObjectType(obj);
			if (type == ObjectType.MainPanel)
			{
				return true;
			}

			if (!HandlesList.HasHeightHandles(obj))
			{
				return false;
			}

			ChildrenPlacement pp = this.GetParentPlacement(obj);
			return (pp != ChildrenPlacement.Anchored);
		}

		public double GetHeight(Widget obj)
		{
			//	Retourne la hauteur de l'objet.
			if (this.HasHeight(obj))
			{
				return obj.PreferredHeight;
			}

			return 0;
		}

		public void SetHeight(Widget obj, double height)
		{
			//	Choix de la hauteur de l'objet.
			System.Diagnostics.Debug.Assert(this.HasHeight(obj));

			if (this.GetHeight(obj) != height)
			{
				obj.PreferredHeight = height;
				this.Invalidate();
			}
		}


		public int GetZOrder(Widget obj)
		{
			//	Retourne l'ordre de l'objet.
			return obj.ZOrder;
		}

		public void SetZOrder(Widget obj, int order)
		{
			//	Choix de l'ordre de l'objet.
			if (obj.ZOrder != order)
			{
				obj.ZOrder = order;
				this.Invalidate();
			}
		}


		public bool HasAttachmentLeft(Widget obj)
		{
			ChildrenPlacement pp = this.GetParentPlacement(obj);

			if (pp == ChildrenPlacement.Anchored)
			{
				AnchoredHorizontalAttachment ha = this.GetAnchoredHorizontalAttachment(obj);
				return (ha != AnchoredHorizontalAttachment.Right);
			}

			if (pp == ChildrenPlacement.HorizontalStacked)
			{
				StackedHorizontalAttachment ha = this.GetStackedHorizontalAttachment(obj);
				return (ha == StackedHorizontalAttachment.Left);
			}

			return false;
		}

		public bool HasAttachmentRight(Widget obj)
		{
			ChildrenPlacement pp = this.GetParentPlacement(obj);

			if (pp == ChildrenPlacement.Anchored)
			{
				AnchoredHorizontalAttachment ha = this.GetAnchoredHorizontalAttachment(obj);
				return (ha != AnchoredHorizontalAttachment.Left);
			}

			if (pp == ChildrenPlacement.HorizontalStacked)
			{
				StackedHorizontalAttachment ha = this.GetStackedHorizontalAttachment(obj);
				return (ha == StackedHorizontalAttachment.Right);
			}

			return false;
		}

		public bool HasAttachmentBottom(Widget obj)
		{
			ChildrenPlacement pp = this.GetParentPlacement(obj);

			if (pp == ChildrenPlacement.Anchored)
			{
				AnchoredVerticalAttachment va = this.GetAnchoredVerticalAttachment(obj);
				return (va != AnchoredVerticalAttachment.Top);
			}

			if (pp == ChildrenPlacement.VerticalStacked)
			{
				StackedVerticalAttachment va = this.GetStackedVerticalAttachment(obj);
				return (va == StackedVerticalAttachment.Bottom);
			}

			return false;
		}

		public bool HasAttachmentTop(Widget obj)
		{
			ChildrenPlacement pp = this.GetParentPlacement(obj);

			if (pp == ChildrenPlacement.Anchored)
			{
				AnchoredVerticalAttachment va = this.GetAnchoredVerticalAttachment(obj);
				return (va != AnchoredVerticalAttachment.Bottom);
			}

			if (pp == ChildrenPlacement.VerticalStacked)
			{
				StackedVerticalAttachment va = this.GetStackedVerticalAttachment(obj);
				return (va == StackedVerticalAttachment.Top);
			}

			return false;
		}


		#region Anchored
		public AnchoredVerticalAttachment GetAnchoredVerticalAttachment(Widget obj)
		{
			//	Retourne l'attachement vertical de l'objet.
			//	Uniquement pour les objets Anchored.
			ChildrenPlacement pp = this.GetParentPlacement(obj);
			if (pp == ChildrenPlacement.Anchored)
			{
				AnchorStyles style = obj.Anchor;
				bool bottom = ((style & AnchorStyles.Bottom) != 0);
				bool top    = ((style & AnchorStyles.Top   ) != 0);

				if (bottom && top)  return AnchoredVerticalAttachment.Fill;
				if (bottom       )  return AnchoredVerticalAttachment.Bottom;
				if (top          )  return AnchoredVerticalAttachment.Top;
			}

			return AnchoredVerticalAttachment.None;
		}

		public void SetAnchoredVerticalAttachment(Widget obj, AnchoredVerticalAttachment attachment)
		{
			//	Choix de l'attachement vertical de l'objet.
			//	Uniquement pour les objets Anchored.
			ChildrenPlacement pp = this.GetParentPlacement(obj);
			System.Diagnostics.Debug.Assert(pp == ChildrenPlacement.Anchored);

			AnchorStyles style = obj.Anchor;

			switch (attachment)
			{
				case AnchoredVerticalAttachment.Bottom:
					style |=  AnchorStyles.Bottom;
					style &= ~AnchorStyles.Top;
					break;

				case AnchoredVerticalAttachment.Top:
					style |=  AnchorStyles.Top;
					style &= ~AnchorStyles.Bottom;
					break;

				case AnchoredVerticalAttachment.Fill:
					style |=  AnchorStyles.Bottom;
					style |=  AnchorStyles.Top;
					break;
			}

			if (obj.Anchor != style)
			{
				obj.Anchor = style;
				obj.Dock = DockStyle.None;
				this.Invalidate();
			}
		}


		public AnchoredHorizontalAttachment GetAnchoredHorizontalAttachment(Widget obj)
		{
			//	Retourne l'attachement horizontal de l'objet.
			//	Uniquement pour les objets Anchored.
			ChildrenPlacement pp = this.GetParentPlacement(obj);
			if (pp == ChildrenPlacement.Anchored)
			{
				AnchorStyles style = obj.Anchor;
				bool left  = ((style & AnchorStyles.Left ) != 0);
				bool right = ((style & AnchorStyles.Right) != 0);

				if (left && right)  return AnchoredHorizontalAttachment.Fill;
				if (left         )  return AnchoredHorizontalAttachment.Left;
				if (right        )  return AnchoredHorizontalAttachment.Right;
			}

			return AnchoredHorizontalAttachment.None;
		}

		public void SetAnchoredHorizontalAttachment(Widget obj, AnchoredHorizontalAttachment attachment)
		{
			//	Choix de l'attachement horizontal de l'objet.
			//	Uniquement pour les objets Anchored.
			ChildrenPlacement pp = this.GetParentPlacement(obj);
			System.Diagnostics.Debug.Assert(pp == ChildrenPlacement.Anchored);

			AnchorStyles style = obj.Anchor;

			switch (attachment)
			{
				case AnchoredHorizontalAttachment.Left:
					style |=  AnchorStyles.Left;
					style &= ~AnchorStyles.Right;
					break;

				case AnchoredHorizontalAttachment.Right:
					style |=  AnchorStyles.Right;
					style &= ~AnchorStyles.Left;
					break;

				case AnchoredHorizontalAttachment.Fill:
					style |=  AnchorStyles.Left;
					style |=  AnchorStyles.Right;
					break;
			}

			if (obj.Anchor != style)
			{
				obj.Anchor = style;
				obj.Dock = DockStyle.None;
				this.Invalidate();
			}
		}
		#endregion


		#region Stacked
		public bool HasStackedVerticalAttachment(Widget obj)
		{
			//	Retourne l'attachement vertical de l'objet.
			ChildrenPlacement pp = this.GetParentPlacement(obj);
			return (pp == ChildrenPlacement.VerticalStacked);
		}

		public StackedVerticalAttachment GetStackedVerticalAttachment(Widget obj)
		{
			//	Retourne l'attachement vertical de l'objet.
			//	Uniquement pour les objets VerticalStacked.
			if (this.HasStackedVerticalAttachment(obj))
			{
				DockStyle style = obj.Dock;
				if (style == DockStyle.StackFill )  return StackedVerticalAttachment.Fill;
				if (style == DockStyle.StackEnd  )  return StackedVerticalAttachment.Bottom;
				if (style == DockStyle.StackBegin)  return StackedVerticalAttachment.Top;
			}

			return StackedVerticalAttachment.None;
		}

		public void SetStackedVerticalAttachment(Widget obj, StackedVerticalAttachment attachment)
		{
			//	Choix de l'attachement vertical de l'objet.
			//	Uniquement pour les objets VerticalStacked.
			System.Diagnostics.Debug.Assert(this.HasStackedVerticalAttachment(obj));

			DockStyle style = obj.Dock;

			switch (attachment)
			{
				case StackedVerticalAttachment.Bottom:
					style = DockStyle.StackEnd;
					break;

				case StackedVerticalAttachment.Top:
					style = DockStyle.StackBegin;
					break;

				case StackedVerticalAttachment.Fill:
					style = DockStyle.StackFill;
					break;
			}

			if (obj.Dock != style)
			{
				obj.Dock = style;
				obj.Anchor = AnchorStyles.None;
				this.Invalidate();
			}
		}


		public bool HasStackedHorizontalAttachment(Widget obj)
		{
			//	Retourne l'attachement horizontal de l'objet.
			ChildrenPlacement pp = this.GetParentPlacement(obj);
			return (pp == ChildrenPlacement.HorizontalStacked);
		}

		public StackedHorizontalAttachment GetStackedHorizontalAttachment(Widget obj)
		{
			//	Retourne l'attachement horizontal de l'objet.
			//	Uniquement pour les objets HorizontalStacked.
			if (this.HasStackedHorizontalAttachment(obj))
			{
				DockStyle style = obj.Dock;
				if (style == DockStyle.StackFill )  return StackedHorizontalAttachment.Fill;
				if (style == DockStyle.StackBegin)  return StackedHorizontalAttachment.Left;
				if (style == DockStyle.StackEnd  )  return StackedHorizontalAttachment.Right;
			}

			return StackedHorizontalAttachment.None;
		}

		public void SetStackedHorizontalAttachment(Widget obj, StackedHorizontalAttachment attachment)
		{
			//	Choix de l'attachement horizontal de l'objet.
			//	Uniquement pour les objets HorizontalStacked.
			System.Diagnostics.Debug.Assert(this.HasStackedHorizontalAttachment(obj));

			DockStyle style = obj.Dock;

			switch (attachment)
			{
				case StackedHorizontalAttachment.Left:
					style = DockStyle.StackBegin;
					break;

				case StackedHorizontalAttachment.Right:
					style = DockStyle.StackEnd;
					break;

				case StackedHorizontalAttachment.Fill:
					style = DockStyle.StackFill;
					break;
			}

			if (obj.Dock != style)
			{
				obj.Dock = style;
				obj.Anchor = AnchorStyles.None;
				this.Invalidate();
			}
		}


		public bool HasStackedHorizontalAlignment(Widget obj)
		{
			//	Retourne l'alignement horizontal de l'objet.
			ChildrenPlacement pp = this.GetParentPlacement(obj);

			if (pp == ChildrenPlacement.HorizontalStacked)
			{
				StackedHorizontalAttachment ha = this.GetStackedHorizontalAttachment(obj);
				return (ha == StackedHorizontalAttachment.Fill);
			}

			if (pp == ChildrenPlacement.VerticalStacked)
			{
				return true;
			}

			if (pp == ChildrenPlacement.Grid)
			{
				return true;
			}

			return false;
		}

		public StackedHorizontalAlignment GetStackedHorizontalAlignment(Widget obj)
		{
			//	Retourne l'alignement horizontal de l'objet.
			if (this.HasStackedHorizontalAlignment(obj))
			{
				HorizontalAlignment ha = obj.HorizontalAlignment;
				if (ha == HorizontalAlignment.Stretch)  return StackedHorizontalAlignment.Stretch;
				if (ha == HorizontalAlignment.Center )  return StackedHorizontalAlignment.Center;
				if (ha == HorizontalAlignment.Left   )  return StackedHorizontalAlignment.Left;
				if (ha == HorizontalAlignment.Right  )  return StackedHorizontalAlignment.Right;
			}

			return StackedHorizontalAlignment.None;
		}

		public void SetStackedHorizontalAlignment(Widget obj, StackedHorizontalAlignment alignment)
		{
			//	Choix de l'alignement horizontal de l'objet.
			System.Diagnostics.Debug.Assert(this.HasStackedHorizontalAlignment(obj));

			HorizontalAlignment ha = obj.HorizontalAlignment;

			switch (alignment)
			{
				case StackedHorizontalAlignment.Stretch:
					ha = HorizontalAlignment.Stretch;
					break;

				case StackedHorizontalAlignment.Center:
					ha = HorizontalAlignment.Center;
					break;

				case StackedHorizontalAlignment.Left:
					ha = HorizontalAlignment.Left;
					break;

				case StackedHorizontalAlignment.Right:
					ha = HorizontalAlignment.Right;
					break;
			}

			if (obj.HorizontalAlignment != ha)
			{
				obj.HorizontalAlignment = ha;
				this.Invalidate();
			}
		}


		public bool HasStackedVerticalAlignment(Widget obj)
		{
			//	Retourne l'alignement vertical de l'objet.
			ChildrenPlacement pp = this.GetParentPlacement(obj);

			if (pp == ChildrenPlacement.HorizontalStacked)
			{
				return true;
			}

			if (pp == ChildrenPlacement.VerticalStacked)
			{
				StackedVerticalAttachment va = this.GetStackedVerticalAttachment(obj);
				return (va == StackedVerticalAttachment.Fill);
			}

			if (pp == ChildrenPlacement.Grid)
			{
				return true;
			}

			return false;
		}

		public StackedVerticalAlignment GetStackedVerticalAlignment(Widget obj)
		{
			//	Retourne l'alignement vertical de l'objet.
			if (this.HasStackedVerticalAlignment(obj))
			{
				VerticalAlignment va = obj.VerticalAlignment;
				if (va == VerticalAlignment.Stretch )  return StackedVerticalAlignment.Stretch;
				if (va == VerticalAlignment.Center  )  return StackedVerticalAlignment.Center;
				if (va == VerticalAlignment.Bottom  )  return StackedVerticalAlignment.Bottom;
				if (va == VerticalAlignment.Top     )  return StackedVerticalAlignment.Top;
				if (va == VerticalAlignment.BaseLine)  return StackedVerticalAlignment.BaseLine;
			}

			return StackedVerticalAlignment.None;
		}

		public void SetStackedVerticalAlignment(Widget obj, StackedVerticalAlignment alignment)
		{
			//	Choix de l'alignement vertical de l'objet.
			System.Diagnostics.Debug.Assert(this.HasStackedVerticalAlignment(obj));

			VerticalAlignment va = obj.VerticalAlignment;

			switch (alignment)
			{
				case StackedVerticalAlignment.Stretch:
					va = VerticalAlignment.Stretch;
					break;

				case StackedVerticalAlignment.Center:
					va = VerticalAlignment.Center;
					break;

				case StackedVerticalAlignment.Bottom:
					va = VerticalAlignment.Bottom;
					break;

				case StackedVerticalAlignment.Top:
					va = VerticalAlignment.Top;
					break;

				case StackedVerticalAlignment.BaseLine:
					va = VerticalAlignment.BaseLine;
					break;
			}

			if (obj.VerticalAlignment != va)
			{
				obj.VerticalAlignment = va;
				this.Invalidate();
			}
		}
		#endregion


		protected void UndoMemorize(string actionName)
		{
			//	Mémorise l'état actuel, avant d'effectuer une modification dans le masque.
			this.panelEditor.ViewersPanels.UndoMemorize(actionName, true);
		}


		protected ChildrenPlacement GetParentPlacement(Widget obj)
		{
			//	Retourne le mode de placement du parent d'un objet.
			if (obj == this.Container)
			{
				return ChildrenPlacement.None;
			}
			else
			{
				return this.GetChildrenPlacement(obj.Parent);
			}
		}

		public void Invalidate()
		{
			//	Invalide le PanelEditor.
			this.panelEditor.Invalidate();
			this.panelEditor.UpdateGeometry();
		}


		protected Editor				panelEditor;
	}
}
