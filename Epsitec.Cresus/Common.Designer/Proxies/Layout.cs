using System.Collections.Generic;

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Proxies
{
	public class Layout : Abstract
	{
		public Layout(ProxyManager manager) : base (manager)
		{
		}

		public override int Rank
		{
			//	Retourne le rang de ce proxy parmi la liste de tous les proxies.
			//	Plus le num�ro est petit, plus le proxy appara�tra haut dans la
			//	liste.
			get
			{
				return 5;
			}
		}

		public override string IconName
		{
			get
			{
				return "PropertyLayout";
			}
		}

		public override double DataColumnWidth
		{
			get
			{
				return 22*5+1;
			}
		}

		public override double RowsSpacing
		{
			get
			{
				return 3;
			}
		}

		public ObjectModifier.ChildrenPlacement ChildrenPlacement
		{
			get
			{
				return (ObjectModifier.ChildrenPlacement) this.GetValue(Layout.ChildrenPlacementProperty);
			}
			set
			{
				this.SetValue(Layout.ChildrenPlacementProperty, value);
			}
		}

		public ObjectModifier.AnchoredHorizontalAttachment AnchoredHorizontalAttachment
		{
			get
			{
				return (ObjectModifier.AnchoredHorizontalAttachment) this.GetValue(Layout.AnchoredHorizontalAttachmentProperty);
			}
			set
			{
				this.SetValue(Layout.AnchoredHorizontalAttachmentProperty, value);
			}
		}

		public ObjectModifier.AnchoredVerticalAttachment AnchoredVerticalAttachment
		{
			get
			{
				return (ObjectModifier.AnchoredVerticalAttachment) this.GetValue(Layout.AnchoredVerticalAttachmentProperty);
			}
			set
			{
				this.SetValue(Layout.AnchoredVerticalAttachmentProperty, value);
			}
		}

		public ObjectModifier.StackedHorizontalAttachment StackedHorizontalAttachment
		{
			get
			{
				return (ObjectModifier.StackedHorizontalAttachment) this.GetValue(Layout.StackedHorizontalAttachmentProperty);
			}
			set
			{
				this.SetValue(Layout.StackedHorizontalAttachmentProperty, value);
			}
		}

		public ObjectModifier.StackedVerticalAttachment StackedVerticalAttachment
		{
			get
			{
				return (ObjectModifier.StackedVerticalAttachment) this.GetValue(Layout.StackedVerticalAttachmentProperty);
			}
			set
			{
				this.SetValue(Layout.StackedVerticalAttachmentProperty, value);
			}
		}

		public ObjectModifier.StackedHorizontalAlignment StackedHorizontalAlignment
		{
			get
			{
				return (ObjectModifier.StackedHorizontalAlignment) this.GetValue(Layout.StackedHorizontalAlignmentProperty);
			}
			set
			{
				this.SetValue(Layout.StackedHorizontalAlignmentProperty, value);
			}
		}

		public ObjectModifier.StackedVerticalAlignment StackedVerticalAlignment
		{
			get
			{
				return (ObjectModifier.StackedVerticalAlignment) this.GetValue(Layout.StackedVerticalAlignmentProperty);
			}
			set
			{
				this.SetValue(Layout.StackedVerticalAlignmentProperty, value);
			}
		}


		protected override void InitializePropertyValues()
		{
			//	Cette m�thode est appel�e par Proxies.Abstract quand on connecte
			//	le premier widget avec le proxy.
			
			//	Recopie localement les diverses propri�t�s du widget s�lectionn�
			//	pour pouvoir ensuite travailler dessus :
			if (this.ObjectModifier.HasChildrenPlacement(this.DefaultWidget))
			{
				ObjectModifier.ChildrenPlacement cp = this.ObjectModifier.GetChildrenPlacement(this.DefaultWidget);

				this.ChildrenPlacement = cp;
			}

			if (this.ObjectModifier.AreChildrenAnchored(this.DefaultWidget.Parent))
			{
				ObjectModifier.AnchoredHorizontalAttachment ha = this.ObjectModifier.GetAnchoredHorizontalAttachment(this.DefaultWidget);
				ObjectModifier.AnchoredVerticalAttachment va = this.ObjectModifier.GetAnchoredVerticalAttachment(this.DefaultWidget);

				this.AnchoredHorizontalAttachment = ha;
				this.AnchoredVerticalAttachment = va;
			}

			if (this.ObjectModifier.HasStackedHorizontalAttachment(this.DefaultWidget))
			{
				ObjectModifier.StackedHorizontalAttachment ha = this.ObjectModifier.GetStackedHorizontalAttachment(this.DefaultWidget);

				this.StackedHorizontalAttachment = ha;
			}

			if (this.ObjectModifier.HasStackedVerticalAttachment(this.DefaultWidget))
			{
				ObjectModifier.StackedVerticalAttachment va = this.ObjectModifier.GetStackedVerticalAttachment(this.DefaultWidget);

				this.StackedVerticalAttachment = va;
			}

			if (this.ObjectModifier.HasStackedHorizontalAlignment(this.DefaultWidget))
			{
				ObjectModifier.StackedHorizontalAlignment ha = this.ObjectModifier.GetStackedHorizontalAlignment(this.DefaultWidget);

				this.StackedHorizontalAlignment = ha;
			}

			if (this.ObjectModifier.HasStackedVerticalAlignment(this.DefaultWidget))
			{
				ObjectModifier.StackedVerticalAlignment va = this.ObjectModifier.GetStackedVerticalAlignment(this.DefaultWidget);
				ObjectModifier.ChildrenPlacement cp = this.ObjectModifier.GetChildrenPlacement(this.DefaultWidget.Parent);

#if false
				if (cp == ObjectModifier.ChildrenPlacement.HorizontalStacked || cp == ObjectModifier.ChildrenPlacement.Grid)
				{
					this.StackedVerticalBaseLineAlignment = va;
				}
				else
				{
					this.StackedVerticalAlignment = va;
				}
#else
				this.StackedVerticalAlignment = va;
#endif
			}
		}

		private static void NotifyChildrenPlacementChanged(DependencyObject o, object oldValue, object newValue)
		{
			//	Cette m�thode est appel�e � la suite de la modification d'une de
			//	nos propri�t�s de d�finition pour permettre de mettre � jour les
			//	widgets connect�s :
			Layout that = (Layout) o;
			ObjectModifier.ChildrenPlacement cp = (ObjectModifier.ChildrenPlacement)newValue;

			if (that.IsNotSuspended)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.Widgets)
					{
						GeometryCache.FixBounds(obj, that.ObjectModifier);
						that.ObjectModifier.SetChildrenPlacement(obj, cp);
						GeometryCache.AdaptBounds(obj, that.ObjectModifier, cp);
					}
				}
				finally
				{
					that.ResumeChanges();
					that.RegenerateProxies();  // (*)
				}
			}
		}

		private static void NotifyAnchoredHorizontalAttachmentChanged(DependencyObject o, object oldValue, object newValue)
		{
			//	Cette m�thode est appel�e � la suite de la modification d'une de
			//	nos propri�t�s de d�finition pour permettre de mettre � jour les
			//	widgets connect�s :
			Layout that = (Layout) o;
			ObjectModifier.AnchoredHorizontalAttachment ha = (ObjectModifier.AnchoredHorizontalAttachment) newValue;

			if (that.IsNotSuspended)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.Widgets)
					{
						Rectangle bounds = that.ObjectModifier.GetBounds(obj);
						that.ObjectModifier.SetAnchoredHorizontalAttachment(obj, ha);
						that.ObjectModifier.SetBounds(obj, bounds);
					}
				}
				finally
				{
					that.ResumeChanges();
					that.RegenerateProxies();  // (*)
				}
			}
		}

		private static void NotifyAnchoredVerticalAttachmentChanged(DependencyObject o, object oldValue, object newValue)
		{
			//	Cette m�thode est appel�e � la suite de la modification d'une de
			//	nos propri�t�s de d�finition pour permettre de mettre � jour les
			//	widgets connect�s :
			Layout that = (Layout) o;
			ObjectModifier.AnchoredVerticalAttachment va = (ObjectModifier.AnchoredVerticalAttachment) newValue;

			if (that.IsNotSuspended)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.Widgets)
					{
						Rectangle bounds = that.ObjectModifier.GetBounds(obj);
						that.ObjectModifier.SetAnchoredVerticalAttachment(obj, va);
						that.ObjectModifier.SetBounds(obj, bounds);
					}
				}
				finally
				{
					that.ResumeChanges();
					that.RegenerateProxies();  // (*)
				}
			}
		}

		private static void NotifyStackedHorizontalAttachmentChanged(DependencyObject o, object oldValue, object newValue)
		{
			//	Cette m�thode est appel�e � la suite de la modification d'une de
			//	nos propri�t�s de d�finition pour permettre de mettre � jour les
			//	widgets connect�s :
			Layout that = (Layout) o;
			ObjectModifier.StackedHorizontalAttachment ha = (ObjectModifier.StackedHorizontalAttachment) newValue;

			if (that.IsNotSuspended)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.Widgets)
					{
						that.ObjectModifier.SetStackedHorizontalAttachment(obj, ha);
					}
				}
				finally
				{
					that.ResumeChanges();
					that.RegenerateProxies();  // (*)
				}
			}
		}

		private static void NotifyStackedVerticalAttachmentChanged(DependencyObject o, object oldValue, object newValue)
		{
			//	Cette m�thode est appel�e � la suite de la modification d'une de
			//	nos propri�t�s de d�finition pour permettre de mettre � jour les
			//	widgets connect�s :
			Layout that = (Layout) o;
			ObjectModifier.StackedVerticalAttachment va = (ObjectModifier.StackedVerticalAttachment) newValue;

			if (that.IsNotSuspended)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.Widgets)
					{
						that.ObjectModifier.SetStackedVerticalAttachment(obj, va);
					}
				}
				finally
				{
					that.ResumeChanges();
					that.RegenerateProxies();  // (*)
				}
			}
		}

		private static void NotifyStackedHorizontalAlignmentChanged(DependencyObject o, object oldValue, object newValue)
		{
			//	Cette m�thode est appel�e � la suite de la modification d'une de
			//	nos propri�t�s de d�finition pour permettre de mettre � jour les
			//	widgets connect�s :
			Layout that = (Layout) o;
			ObjectModifier.StackedHorizontalAlignment ha = (ObjectModifier.StackedHorizontalAlignment) newValue;

			if (that.IsNotSuspended)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.Widgets)
					{
						that.ObjectModifier.SetStackedHorizontalAlignment(obj, ha);
					}
				}
				finally
				{
					that.ResumeChanges();
					that.RegenerateProxies();  // (*)
				}
			}
		}

		private static void NotifyStackedVerticalAlignmentChanged(DependencyObject o, object oldValue, object newValue)
		{
			//	Cette m�thode est appel�e � la suite de la modification d'une de
			//	nos propri�t�s de d�finition pour permettre de mettre � jour les
			//	widgets connect�s :
			Layout that = (Layout) o;
			ObjectModifier.StackedVerticalAlignment va = (ObjectModifier.StackedVerticalAlignment) newValue;

			if (that.IsNotSuspended)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.Widgets)
					{
						that.ObjectModifier.SetStackedVerticalAlignment(obj, va);
					}
				}
				finally
				{
					that.ResumeChanges();
					that.RegenerateProxies();  // (*)
				}
			}
		}

		//	(*)	Les propri�t�s Largeur ou Hauteur peuvent appara�tre ou dispara�tre !


		static Layout()
		{
			EnumType childrenPlacementEnumType = Res.Types.ObjectModifier.ChildrenPlacement;
			Layout.ChildrenPlacementProperty.DefaultMetadata.DefineNamedType(childrenPlacementEnumType);
			Layout.ChildrenPlacementProperty.DefaultMetadata.DefineCaptionId(Res.Captions.Layout.ChildrenPlacement.Id);

			EnumType anchoredHorizontalAttachmentEnumType = Res.Types.ObjectModifier.AnchoredHorizontalAttachment;
			Layout.AnchoredHorizontalAttachmentProperty.DefaultMetadata.DefineNamedType(anchoredHorizontalAttachmentEnumType);
			Layout.AnchoredHorizontalAttachmentProperty.DefaultMetadata.DefineCaptionId(Res.Captions.Layout.AnchoredHorizontalAttachment.Id);

			EnumType anchoredVerticalAttachmentEnumType = Res.Types.ObjectModifier.AnchoredVerticalAttachment;
			Layout.AnchoredVerticalAttachmentProperty.DefaultMetadata.DefineNamedType(anchoredVerticalAttachmentEnumType);
			Layout.AnchoredVerticalAttachmentProperty.DefaultMetadata.DefineCaptionId(Res.Captions.Layout.AnchoredVerticalAttachment.Id);

			EnumType stackedHorizontalAttachmentEnumType = Res.Types.ObjectModifier.StackedHorizontalAttachment;
			Layout.StackedHorizontalAttachmentProperty.DefaultMetadata.DefineNamedType(stackedHorizontalAttachmentEnumType);
			Layout.StackedHorizontalAttachmentProperty.DefaultMetadata.DefineCaptionId(Res.Captions.Layout.StackedHorizontalAttachment.Id);

			EnumType stackedVerticalAttachmentEnumType = Res.Types.ObjectModifier.StackedVerticalAttachment;
			Layout.StackedVerticalAttachmentProperty.DefaultMetadata.DefineNamedType(stackedVerticalAttachmentEnumType);
			Layout.StackedVerticalAttachmentProperty.DefaultMetadata.DefineCaptionId(Res.Captions.Layout.StackedVerticalAttachment.Id);

			EnumType stackedHorizontalAlignmentEnumType = Res.Types.ObjectModifier.StackedHorizontalAlignment;
			Layout.StackedHorizontalAlignmentProperty.DefaultMetadata.DefineNamedType(stackedHorizontalAlignmentEnumType);
			Layout.StackedHorizontalAlignmentProperty.DefaultMetadata.DefineCaptionId(Res.Captions.Layout.StackedHorizontalAlignment.Id);

			EnumType stackedVerticalAlignmentEnumType = Res.Types.ObjectModifier.StackedVerticalAlignment;
			Layout.StackedVerticalAlignmentProperty.DefaultMetadata.DefineNamedType(stackedVerticalAlignmentEnumType);
			Layout.StackedVerticalAlignmentProperty.DefaultMetadata.DefineCaptionId(Res.Captions.Layout.StackedVerticalAlignment.Id);
		}

		
		public static readonly DependencyProperty ChildrenPlacementProperty = DependencyProperty.Register("ChildrenPlacement", typeof(ObjectModifier.ChildrenPlacement), typeof(Layout), new DependencyPropertyMetadata(ObjectModifier.ChildrenPlacement.Anchored, Layout.NotifyChildrenPlacementChanged));
		public static readonly DependencyProperty AnchoredHorizontalAttachmentProperty = DependencyProperty.Register("AnchoredHorizontalAttachment", typeof(ObjectModifier.AnchoredHorizontalAttachment), typeof(Layout), new DependencyPropertyMetadata(ObjectModifier.AnchoredHorizontalAttachment.Left, Layout.NotifyAnchoredHorizontalAttachmentChanged));
		public static readonly DependencyProperty AnchoredVerticalAttachmentProperty = DependencyProperty.Register("AnchoredVerticalAttachment", typeof(ObjectModifier.AnchoredVerticalAttachment), typeof(Layout), new DependencyPropertyMetadata(ObjectModifier.AnchoredVerticalAttachment.Bottom, Layout.NotifyAnchoredVerticalAttachmentChanged));
		public static readonly DependencyProperty StackedHorizontalAttachmentProperty = DependencyProperty.Register("StackedHorizontalAttachment", typeof(ObjectModifier.StackedHorizontalAttachment), typeof(Layout), new DependencyPropertyMetadata(ObjectModifier.StackedHorizontalAttachment.Left, Layout.NotifyStackedHorizontalAttachmentChanged));
		public static readonly DependencyProperty StackedVerticalAttachmentProperty = DependencyProperty.Register("StackedVerticalAttachment", typeof(ObjectModifier.StackedVerticalAttachment), typeof(Layout), new DependencyPropertyMetadata(ObjectModifier.StackedVerticalAttachment.Bottom, Layout.NotifyStackedVerticalAttachmentChanged));
		public static readonly DependencyProperty StackedHorizontalAlignmentProperty = DependencyProperty.Register("StackedHorizontalAlignment", typeof(ObjectModifier.StackedHorizontalAlignment), typeof(Layout), new DependencyPropertyMetadata(ObjectModifier.StackedHorizontalAlignment.Stretch, Layout.NotifyStackedHorizontalAlignmentChanged));
		public static readonly DependencyProperty StackedVerticalAlignmentProperty = DependencyProperty.Register("StackedVerticalAlignment", typeof(ObjectModifier.StackedVerticalAlignment), typeof(Layout), new DependencyPropertyMetadata(ObjectModifier.StackedVerticalAlignment.Stretch, Layout.NotifyStackedVerticalAlignmentChanged));
	}
}
