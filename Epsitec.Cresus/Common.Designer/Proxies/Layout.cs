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
			//	Plus le numéro est petit, plus le proxy apparaîtra haut dans la
			//	liste.
			get
			{
				return 3;
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

		public ObjectModifier.StackedVerticalAlignment StackedVerticalBaseLineAlignment
		{
			get
			{
				return (ObjectModifier.StackedVerticalAlignment) this.GetValue(Layout.StackedVerticalBaseLineAlignmentProperty);
			}
			set
			{
				this.SetValue(Layout.StackedVerticalBaseLineAlignmentProperty, value);
			}
		}


		protected override void InitialisePropertyValues()
		{
			//	Cette méthode est appelée par Proxies.Abstract quand on connecte
			//	le premier widget avec le proxy.
			
			//	Recopie localement les diverses propriétés du widget sélectionné
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

				if (cp == ObjectModifier.ChildrenPlacement.HorizontalStacked)
				{
					this.StackedVerticalBaseLineAlignment = va;
				}
				else
				{
					this.StackedVerticalAlignment = va;
				}
			}
		}

		private static void NotifyChildrenPlacementChanged(DependencyObject o, object oldValue, object newValue)
		{
			//	Cette méthode est appelée à la suite de la modification d'une de
			//	nos propriétés de définition pour permettre de mettre à jour les
			//	widgets connectés :
			Layout that = (Layout) o;
			ObjectModifier.ChildrenPlacement cp = (ObjectModifier.ChildrenPlacement)newValue;

			if (that.IsNotSuspended)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.Widgets)
					{
						GeometryCache.FixBounds(obj.Children.Widgets, that.ObjectModifier);
						that.ObjectModifier.SetChildrenPlacement(obj, cp);
						GeometryCache.AdaptBounds(obj.Children.Widgets, cp);
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
			//	Cette méthode est appelée à la suite de la modification d'une de
			//	nos propriétés de définition pour permettre de mettre à jour les
			//	widgets connectés :
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
			//	Cette méthode est appelée à la suite de la modification d'une de
			//	nos propriétés de définition pour permettre de mettre à jour les
			//	widgets connectés :
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
			//	Cette méthode est appelée à la suite de la modification d'une de
			//	nos propriétés de définition pour permettre de mettre à jour les
			//	widgets connectés :
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
			//	Cette méthode est appelée à la suite de la modification d'une de
			//	nos propriétés de définition pour permettre de mettre à jour les
			//	widgets connectés :
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
			//	Cette méthode est appelée à la suite de la modification d'une de
			//	nos propriétés de définition pour permettre de mettre à jour les
			//	widgets connectés :
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
			//	Cette méthode est appelée à la suite de la modification d'une de
			//	nos propriétés de définition pour permettre de mettre à jour les
			//	widgets connectés :
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

		//	(*)	Les propriétés Largeur ou Hauteur peuvent apparaître ou disparaître !


		static Layout()
		{
			EnumType childrenPlacementEnumType = new EnumType(typeof(ObjectModifier.ChildrenPlacement));
			childrenPlacementEnumType.DefineDefaultController("Enum", "Icons");
			childrenPlacementEnumType[ObjectModifier.ChildrenPlacement.Anchored].DefineCaptionId(new Support.Druid("[10011]").ToLong());
			childrenPlacementEnumType[ObjectModifier.ChildrenPlacement.VerticalStacked].DefineCaptionId(new Support.Druid("[10012]").ToLong());
			childrenPlacementEnumType[ObjectModifier.ChildrenPlacement.HorizontalStacked].DefineCaptionId(new Support.Druid("[10013]").ToLong());
			childrenPlacementEnumType[ObjectModifier.ChildrenPlacement.Grid].DefineCaptionId(new Support.Druid("[10014]").ToLong());
			Layout.ChildrenPlacementProperty.DefaultMetadata.DefineNamedType(childrenPlacementEnumType);
			Layout.ChildrenPlacementProperty.DefaultMetadata.DefineCaptionId(new Support.Druid("[100C]").ToLong());

			EnumType anchoredHorizontalAttachmentEnumType = new EnumType(typeof(ObjectModifier.AnchoredHorizontalAttachment));
			anchoredHorizontalAttachmentEnumType.DefineDefaultController("Enum", "Icons");
			anchoredHorizontalAttachmentEnumType[ObjectModifier.AnchoredHorizontalAttachment.Left].DefineCaptionId(new Support.Druid("[10021]").ToLong());
			anchoredHorizontalAttachmentEnumType[ObjectModifier.AnchoredHorizontalAttachment.Right].DefineCaptionId(new Support.Druid("[10022]").ToLong());
			anchoredHorizontalAttachmentEnumType[ObjectModifier.AnchoredHorizontalAttachment.Fill].DefineCaptionId(new Support.Druid("[10025]").ToLong());
			Layout.AnchoredHorizontalAttachmentProperty.DefaultMetadata.DefineNamedType(anchoredHorizontalAttachmentEnumType);
			Layout.AnchoredHorizontalAttachmentProperty.DefaultMetadata.DefineCaptionId(new Support.Druid("[100D]").ToLong());

			EnumType anchoredVerticalAttachmentEnumType = new EnumType(typeof(ObjectModifier.AnchoredVerticalAttachment));
			anchoredVerticalAttachmentEnumType.DefineDefaultController("Enum", "Icons");
			anchoredVerticalAttachmentEnumType[ObjectModifier.AnchoredVerticalAttachment.Bottom].DefineCaptionId(new Support.Druid("[10023]").ToLong());
			anchoredVerticalAttachmentEnumType[ObjectModifier.AnchoredVerticalAttachment.Top].DefineCaptionId(new Support.Druid("[10024]").ToLong());
			anchoredVerticalAttachmentEnumType[ObjectModifier.AnchoredVerticalAttachment.Fill].DefineCaptionId(new Support.Druid("[10026]").ToLong());
			Layout.AnchoredVerticalAttachmentProperty.DefaultMetadata.DefineNamedType(anchoredVerticalAttachmentEnumType);
			Layout.AnchoredVerticalAttachmentProperty.DefaultMetadata.DefineCaptionId(new Support.Druid("[100E]").ToLong());

			EnumType stackedHorizontalAttachmentEnumType = new EnumType(typeof(ObjectModifier.StackedHorizontalAttachment));
			stackedHorizontalAttachmentEnumType.DefineDefaultController("Enum", "Icons");
			stackedHorizontalAttachmentEnumType[ObjectModifier.StackedHorizontalAttachment.Left].DefineCaptionId(new Support.Druid("[10021]").ToLong());
			stackedHorizontalAttachmentEnumType[ObjectModifier.StackedHorizontalAttachment.Right].DefineCaptionId(new Support.Druid("[10022]").ToLong());
			stackedHorizontalAttachmentEnumType[ObjectModifier.StackedHorizontalAttachment.Fill].DefineCaptionId(new Support.Druid("[10025]").ToLong());
			Layout.StackedHorizontalAttachmentProperty.DefaultMetadata.DefineNamedType(stackedHorizontalAttachmentEnumType);
			Layout.StackedHorizontalAttachmentProperty.DefaultMetadata.DefineCaptionId(new Support.Druid("[100F]").ToLong());

			EnumType stackedVerticalAttachmentEnumType = new EnumType(typeof(ObjectModifier.StackedVerticalAttachment));
			stackedVerticalAttachmentEnumType.DefineDefaultController("Enum", "Icons");
			stackedVerticalAttachmentEnumType[ObjectModifier.StackedVerticalAttachment.Bottom].DefineCaptionId(new Support.Druid("[10023]").ToLong());
			stackedVerticalAttachmentEnumType[ObjectModifier.StackedVerticalAttachment.Top].DefineCaptionId(new Support.Druid("[10024]").ToLong());
			stackedVerticalAttachmentEnumType[ObjectModifier.StackedVerticalAttachment.Fill].DefineCaptionId(new Support.Druid("[10026]").ToLong());
			Layout.StackedVerticalAttachmentProperty.DefaultMetadata.DefineNamedType(stackedVerticalAttachmentEnumType);
			Layout.StackedVerticalAttachmentProperty.DefaultMetadata.DefineCaptionId(new Support.Druid("[100G]").ToLong());

			EnumType stackedHorizontalAlignmentEnumType = new EnumType(typeof(ObjectModifier.StackedHorizontalAlignment));
			stackedHorizontalAlignmentEnumType.DefineDefaultController("Enum", "Icons");
			stackedHorizontalAlignmentEnumType[ObjectModifier.StackedHorizontalAlignment.Stretch].DefineCaptionId(new Support.Druid("[10037]").ToLong());
			stackedHorizontalAlignmentEnumType[ObjectModifier.StackedHorizontalAlignment.Center].DefineCaptionId(new Support.Druid("[10035]").ToLong());
			stackedHorizontalAlignmentEnumType[ObjectModifier.StackedHorizontalAlignment.Left].DefineCaptionId(new Support.Druid("[10031]").ToLong());
			stackedHorizontalAlignmentEnumType[ObjectModifier.StackedHorizontalAlignment.Right].DefineCaptionId(new Support.Druid("[10032]").ToLong());
			Layout.StackedHorizontalAlignmentProperty.DefaultMetadata.DefineNamedType(stackedHorizontalAlignmentEnumType);
			Layout.StackedHorizontalAlignmentProperty.DefaultMetadata.DefineCaptionId(new Support.Druid("[100H]").ToLong());

			EnumType stackedVerticalAlignmentEnumType = new EnumType(typeof(ObjectModifier.StackedVerticalAlignment));
			stackedVerticalAlignmentEnumType.DefineDefaultController("Enum", "Icons");
			stackedVerticalAlignmentEnumType[ObjectModifier.StackedVerticalAlignment.Stretch].DefineCaptionId(new Support.Druid("[10038]").ToLong());
			stackedVerticalAlignmentEnumType[ObjectModifier.StackedVerticalAlignment.Center].DefineCaptionId(new Support.Druid("[10036]").ToLong());
			stackedVerticalAlignmentEnumType[ObjectModifier.StackedVerticalAlignment.Bottom].DefineCaptionId(new Support.Druid("[10033]").ToLong());
			stackedVerticalAlignmentEnumType[ObjectModifier.StackedVerticalAlignment.Top].DefineCaptionId(new Support.Druid("[10034]").ToLong());
			stackedVerticalAlignmentEnumType[ObjectModifier.StackedVerticalAlignment.BaseLine].DefineHidden(true);
			Layout.StackedVerticalAlignmentProperty.DefaultMetadata.DefineNamedType(stackedVerticalAlignmentEnumType);
			Layout.StackedVerticalAlignmentProperty.DefaultMetadata.DefineCaptionId(new Support.Druid("[100I]").ToLong());

			EnumType stackedVerticalBaseLineAlignmentEnumType = new EnumType(typeof(ObjectModifier.StackedVerticalAlignment));
			stackedVerticalBaseLineAlignmentEnumType.DefineDefaultController("Enum", "Icons");
			stackedVerticalBaseLineAlignmentEnumType[ObjectModifier.StackedVerticalAlignment.Stretch].DefineCaptionId(new Support.Druid("[10038]").ToLong());
			stackedVerticalBaseLineAlignmentEnumType[ObjectModifier.StackedVerticalAlignment.Center].DefineCaptionId(new Support.Druid("[10036]").ToLong());
			stackedVerticalBaseLineAlignmentEnumType[ObjectModifier.StackedVerticalAlignment.Bottom].DefineCaptionId(new Support.Druid("[10033]").ToLong());
			stackedVerticalBaseLineAlignmentEnumType[ObjectModifier.StackedVerticalAlignment.Top].DefineCaptionId(new Support.Druid("[10034]").ToLong());
			stackedVerticalBaseLineAlignmentEnumType[ObjectModifier.StackedVerticalAlignment.BaseLine].DefineCaptionId(new Support.Druid("[10039]").ToLong());
			Layout.StackedVerticalBaseLineAlignmentProperty.DefaultMetadata.DefineNamedType(stackedVerticalBaseLineAlignmentEnumType);
			Layout.StackedVerticalBaseLineAlignmentProperty.DefaultMetadata.DefineCaptionId(new Support.Druid("[100I]").ToLong());
		}

		
		public static readonly DependencyProperty ChildrenPlacementProperty = DependencyProperty.Register("ChildrenPlacement", typeof(ObjectModifier.ChildrenPlacement), typeof(Layout), new DependencyPropertyMetadata(ObjectModifier.ChildrenPlacement.Anchored, Layout.NotifyChildrenPlacementChanged));
		public static readonly DependencyProperty AnchoredHorizontalAttachmentProperty = DependencyProperty.Register("AnchoredHorizontalAttachment", typeof(ObjectModifier.AnchoredHorizontalAttachment), typeof(Layout), new DependencyPropertyMetadata(ObjectModifier.AnchoredHorizontalAttachment.Left, Layout.NotifyAnchoredHorizontalAttachmentChanged));
		public static readonly DependencyProperty AnchoredVerticalAttachmentProperty = DependencyProperty.Register("AnchoredVerticalAttachment", typeof(ObjectModifier.AnchoredVerticalAttachment), typeof(Layout), new DependencyPropertyMetadata(ObjectModifier.AnchoredVerticalAttachment.Bottom, Layout.NotifyAnchoredVerticalAttachmentChanged));
		public static readonly DependencyProperty StackedHorizontalAttachmentProperty = DependencyProperty.Register("StackedHorizontalAttachment", typeof(ObjectModifier.StackedHorizontalAttachment), typeof(Layout), new DependencyPropertyMetadata(ObjectModifier.StackedHorizontalAttachment.Left, Layout.NotifyStackedHorizontalAttachmentChanged));
		public static readonly DependencyProperty StackedVerticalAttachmentProperty = DependencyProperty.Register("StackedVerticalAttachment", typeof(ObjectModifier.StackedVerticalAttachment), typeof(Layout), new DependencyPropertyMetadata(ObjectModifier.StackedVerticalAttachment.Bottom, Layout.NotifyStackedVerticalAttachmentChanged));
		public static readonly DependencyProperty StackedHorizontalAlignmentProperty = DependencyProperty.Register("StackedHorizontalAlignment", typeof(ObjectModifier.StackedHorizontalAlignment), typeof(Layout), new DependencyPropertyMetadata(ObjectModifier.StackedHorizontalAlignment.Stretch, Layout.NotifyStackedHorizontalAlignmentChanged));
		public static readonly DependencyProperty StackedVerticalAlignmentProperty = DependencyProperty.Register("StackedVerticalAlignment", typeof(ObjectModifier.StackedVerticalAlignment), typeof(Layout), new DependencyPropertyMetadata(ObjectModifier.StackedVerticalAlignment.Stretch, Layout.NotifyStackedVerticalAlignmentChanged));
		public static readonly DependencyProperty StackedVerticalBaseLineAlignmentProperty = DependencyProperty.Register("StackedVerticalBaseLineAlignment", typeof(ObjectModifier.StackedVerticalAlignment), typeof(Layout), new DependencyPropertyMetadata(ObjectModifier.StackedVerticalAlignment.Stretch, Layout.NotifyStackedVerticalAlignmentChanged));
	}
}
