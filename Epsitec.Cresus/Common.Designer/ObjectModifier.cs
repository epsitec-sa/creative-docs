using System.Collections.Generic;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Layouts;

namespace Epsitec.Common.Designer
{
	/// <summary>
	/// La classe ObjectModifier permet de gérer les 'widgets' de Designer.
	/// </summary>
	public class ObjectModifier
	{
		public enum ChildrenPlacement
		{
			[Types.Hidden] None,
			Anchored,
			VerticalStacked,
			HorizontalStacked,
			Grid,
		}

		public enum AnchoredHorizontalAttachment
		{
			[Types.Hidden] None,
			Left,
			Right,
			Fill,
		}

		public enum AnchoredVerticalAttachment
		{
			[Types.Hidden] None,
			Bottom,
			Top,
			Fill,
		}

		public enum StackedHorizontalAttachment
		{
			[Types.Hidden] None,
			Left,
			Right,
			Fill,
		}

		public enum StackedVerticalAttachment
		{
			[Types.Hidden] None,
			Bottom,
			Top,
			Fill,
		}

		public enum StackedHorizontalAlignment
		{
			[Types.Hidden] None,
			Stretch,
			Center,
			Left,
			Right,
		}

		public enum StackedVerticalAlignment
		{
			[Types.Hidden] None,
			Stretch,
			Center,
			Bottom,
			Top,
			BaseLine,
		}


		public ObjectModifier(MyWidgets.PanelEditor panelEditor)
		{
			//	Constructeur unique.
			this.panelEditor = panelEditor;
		}

		protected UI.Panel Container
		{
			get
			{
				return this.panelEditor.Panel;
			}
		}

		#region ChildrenPlacement
		public bool AreChildrenAnchored(Widget obj)
		{
			AbstractGroup group = obj as AbstractGroup;
			if (group != null)
			{
				ChildrenPlacement p = this.GetChildrenPlacement(obj);
				return (p == ChildrenPlacement.Anchored);
			}
			return false;
		}

		public bool AreChildrenStacked(Widget obj)
		{
			AbstractGroup group = obj as AbstractGroup;
			if (group != null)
			{
				ChildrenPlacement p = this.GetChildrenPlacement(obj);
				return (p == ChildrenPlacement.HorizontalStacked || p == ChildrenPlacement.VerticalStacked);
			}
			return false;
		}

		public bool AreChildrenGrid(Widget obj)
		{
			AbstractGroup group = obj as AbstractGroup;
			if (group != null)
			{
				ChildrenPlacement p = this.GetChildrenPlacement(obj);
				return (p == ChildrenPlacement.Grid);
			}
			return false;
		}

		public bool AreChildrenHorizontal(Widget obj)
		{
			AbstractGroup group = obj as AbstractGroup;
			if (group != null)
			{
				ChildrenPlacement p = this.GetChildrenPlacement(obj);
				return (p == ChildrenPlacement.HorizontalStacked);
			}
			return false;
		}

		public bool HasChildrenPlacement(Widget obj)
		{
			//	Indique s'il existe un mode de placement des enfants de l'objet.
			AbstractGroup group = obj as AbstractGroup;
			return (group != null);
		}

		public ChildrenPlacement GetChildrenPlacement(Widget obj)
		{
			//	Retourne le mode de placement des enfants de l'objet.
			//	Uniquement pour les objects AbstractGroup.
			AbstractGroup group = obj as AbstractGroup;
			if (group != null)
			{
				if (group.ChildrenLayoutMode == Widgets.Layouts.LayoutMode.Anchored)
				{
					return ChildrenPlacement.Anchored;
				}

				if (group.ChildrenLayoutMode == Widgets.Layouts.LayoutMode.Stacked)
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

				if (group.ChildrenLayoutMode == Widgets.Layouts.LayoutMode.Grid)
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
					group.ChildrenLayoutMode = Widgets.Layouts.LayoutMode.Anchored;
					LayoutEngine.SetLayoutEngine(group, null);
					break;

				case ChildrenPlacement.HorizontalStacked:
					group.ChildrenLayoutMode = Widgets.Layouts.LayoutMode.Stacked;
					group.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
					LayoutEngine.SetLayoutEngine(group, null);
					break;

				case ChildrenPlacement.VerticalStacked:
					group.ChildrenLayoutMode = Widgets.Layouts.LayoutMode.Stacked;
					group.ContainerLayoutMode = ContainerLayoutMode.VerticalFlow;
					LayoutEngine.SetLayoutEngine(group, null);
					break;

				case ChildrenPlacement.Grid:
					group.ChildrenLayoutMode = Widgets.Layouts.LayoutMode.Grid;

					GridLayoutEngine engine = new GridLayoutEngine();
					engine.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Proportional)));
					engine.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Proportional)));
					engine.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Proportional)));
					engine.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Proportional)));
					engine.ColumnDefinitions[0].MinWidth = 20;
					engine.ColumnDefinitions[1].MinWidth = 20;
					engine.RowDefinitions[0].MinHeight = 20;
					engine.RowDefinitions[1].MinHeight = 20;
					LayoutEngine.SetLayoutEngine(group, engine);
					LayoutContext.AddToMeasureQueue(group);
					break;
			}
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
			}

			if (obj is StaticText)
			{
				obj.PreferredHeight = obj.MinHeight;
			}

			if (obj is Button)
			{
				obj.PreferredHeight = obj.MinHeight;
			}

			if (obj is TextField)
			{
				obj.PreferredHeight = obj.MinHeight;
			}
		}


		public bool HasBounds(Widget obj)
		{
			//	Indique si l'objet a une position et des dimensions modifiables.
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			return (placement == ChildrenPlacement.Anchored);
		}

		public Rectangle GetBounds(Widget obj)
		{
			//	Retourne la position et les dimensions de l'objet.
			obj.Window.ForceLayout();
			Rectangle bounds = obj.Client.Bounds;

			while (obj != this.Container)
			{
				bounds = obj.MapClientToParent(bounds);
				obj = obj.Parent;
			}

			return bounds;
		}

		public void SetBounds(Widget obj, Rectangle bounds)
		{
			//	Choix de la position et des dimensions de l'objet.
			//	Uniquement pour les objets Anchored.
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
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			if (placement == ChildrenPlacement.HorizontalStacked || placement == ChildrenPlacement.VerticalStacked)
			{
				return true;
			}

			return false;
		}

		public Margins GetMargins(Widget obj)
		{
			//	Retourne les marges de l'objet.
			//	Uniquement pour les objets Stacked.
			if (this.HasMargins(obj))
			{
				return obj.Margins;
			}

			return Margins.Zero;
		}

		public void SetMargins(Widget obj, Margins margins)
		{
			//	Choix des marges de l'objet.
			//	Uniquement pour les objets Stacked.
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
			return (obj is AbstractGroup);
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
			Rectangle bounds = this.GetBounds(obj);

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
			if (!HandlesList.HasWidthHandles(obj))
			{
				return false;
			}

			ChildrenPlacement placement = this.GetParentPlacement(obj);

			if (placement == ChildrenPlacement.HorizontalStacked)
			{
				return true;
			}

			if (placement == ChildrenPlacement.VerticalStacked)
			{
				StackedHorizontalAlignment ha = this.GetStackedHorizontalAlignment(obj);
				return (ha != StackedHorizontalAlignment.Stretch && ha != StackedHorizontalAlignment.None);
			}

			return false;
		}

		public double GetWidth(Widget obj)
		{
			//	Retourne la largeur de l'objet.
			//	Uniquement pour les objets HorizontalStacked.
			if (this.HasWidth(obj))
			{
				return obj.PreferredWidth;
			}

			return 0;
		}

		public void SetWidth(Widget obj, double width)
		{
			//	Choix de la largeur de l'objet.
			//	Uniquement pour les objets VerticalStacked.
			System.Diagnostics.Debug.Assert(this.HasWidth(obj));

			if (obj.PreferredWidth != width)
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
			if (!HandlesList.HasHeightHandles(obj))
			{
				return false;
			}

			ChildrenPlacement placement = this.GetParentPlacement(obj);

			if (placement == ChildrenPlacement.VerticalStacked)
			{
				return true;
			}

			if (placement == ChildrenPlacement.HorizontalStacked)
			{
				StackedVerticalAlignment ha = this.GetStackedVerticalAlignment(obj);
				return (ha != StackedVerticalAlignment.Stretch && ha != StackedVerticalAlignment.None);
			}

			return false;
		}

		public double GetHeight(Widget obj)
		{
			//	Retourne la hauteur de l'objet.
			//	Uniquement pour les objets VerticalStacked.
			if (this.HasHeight(obj))
			{
				return obj.PreferredHeight;
			}

			return 0;
		}

		public void SetHeight(Widget obj, double height)
		{
			//	Choix de la hauteur de l'objet.
			//	Uniquement pour les objets HorizontalStacked.
			System.Diagnostics.Debug.Assert(this.HasHeight(obj));

			if (obj.PreferredHeight != height)
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
			ChildrenPlacement placement = this.GetParentPlacement(obj);

			if (placement == ChildrenPlacement.Anchored)
			{
				AnchoredHorizontalAttachment ha = this.GetAnchoredHorizontalAttachment(obj);
				return (ha != AnchoredHorizontalAttachment.Right);
			}

			if (placement == ChildrenPlacement.HorizontalStacked)
			{
				StackedHorizontalAttachment ha = this.GetStackedHorizontalAttachment(obj);
				return (ha == StackedHorizontalAttachment.Left);
			}

			return false;
		}

		public bool HasAttachmentRight(Widget obj)
		{
			ChildrenPlacement placement = this.GetParentPlacement(obj);

			if (placement == ChildrenPlacement.Anchored)
			{
				AnchoredHorizontalAttachment ha = this.GetAnchoredHorizontalAttachment(obj);
				return (ha != AnchoredHorizontalAttachment.Left);
			}

			if (placement == ChildrenPlacement.HorizontalStacked)
			{
				StackedHorizontalAttachment ha = this.GetStackedHorizontalAttachment(obj);
				return (ha == StackedHorizontalAttachment.Right);
			}

			return false;
		}

		public bool HasAttachmentBottom(Widget obj)
		{
			ChildrenPlacement placement = this.GetParentPlacement(obj);

			if (placement == ChildrenPlacement.Anchored)
			{
				AnchoredVerticalAttachment va = this.GetAnchoredVerticalAttachment(obj);
				return (va != AnchoredVerticalAttachment.Top);
			}

			if (placement == ChildrenPlacement.VerticalStacked)
			{
				StackedVerticalAttachment va = this.GetStackedVerticalAttachment(obj);
				return (va == StackedVerticalAttachment.Bottom);
			}

			return false;
		}

		public bool HasAttachmentTop(Widget obj)
		{
			ChildrenPlacement placement = this.GetParentPlacement(obj);

			if (placement == ChildrenPlacement.Anchored)
			{
				AnchoredVerticalAttachment va = this.GetAnchoredVerticalAttachment(obj);
				return (va != AnchoredVerticalAttachment.Bottom);
			}

			if (placement == ChildrenPlacement.VerticalStacked)
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
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			if (placement == ChildrenPlacement.Anchored)
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
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			System.Diagnostics.Debug.Assert(placement == ChildrenPlacement.Anchored);

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
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			if (placement == ChildrenPlacement.Anchored)
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
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			System.Diagnostics.Debug.Assert(placement == ChildrenPlacement.Anchored);

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
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			return (placement == ChildrenPlacement.VerticalStacked);
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
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			return (placement == ChildrenPlacement.HorizontalStacked);
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
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			return (placement == ChildrenPlacement.HorizontalStacked || placement == ChildrenPlacement.VerticalStacked);
		}

		public StackedHorizontalAlignment GetStackedHorizontalAlignment(Widget obj)
		{
			//	Retourne l'alignement horizontal de l'objet.
			//	Uniquement pour les objets VerticalStacked.
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
			//	Uniquement pour les objets VerticalStacked.
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
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			return (placement == ChildrenPlacement.HorizontalStacked || placement == ChildrenPlacement.VerticalStacked);
		}

		public StackedVerticalAlignment GetStackedVerticalAlignment(Widget obj)
		{
			//	Retourne l'alignement vertical de l'objet.
			//	Uniquement pour les objets HorizontalStacked.
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
			//	Uniquement pour les objets HorizontalStacked.
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

		protected void Invalidate()
		{
			//	Invalide le PanelEditor.
			this.panelEditor.Invalidate();
			this.panelEditor.UpdateGeometry();
		}


		protected MyWidgets.PanelEditor				panelEditor;
	}
}
