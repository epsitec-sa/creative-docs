using System.Collections.Generic;

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Proxies
{
	public class Layout : Abstract
	{
		public Layout(Widget widget, Viewers.Panels panel) : base(widget, panel)
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

		public ObjectModifier.StackedHorizontalAttachment DockedHorizontalAttachment
		{
			get
			{
				return (ObjectModifier.StackedHorizontalAttachment) this.GetValue(Layout.DockedHorizontalAttachmentProperty);
			}
			set
			{
				this.SetValue(Layout.DockedHorizontalAttachmentProperty, value);
			}
		}

		public ObjectModifier.StackedVerticalAttachment DockedVerticalAttachment
		{
			get
			{
				return (ObjectModifier.StackedVerticalAttachment) this.GetValue(Layout.DockedVerticalAttachmentProperty);
			}
			set
			{
				this.SetValue(Layout.DockedVerticalAttachmentProperty, value);
			}
		}

		public ObjectModifier.StackedHorizontalAlignment DockedHorizontalAlignment
		{
			get
			{
				return (ObjectModifier.StackedHorizontalAlignment) this.GetValue(Layout.DockedHorizontalAlignmentProperty);
			}
			set
			{
				this.SetValue(Layout.DockedHorizontalAlignmentProperty, value);
			}
		}

		public ObjectModifier.StackedVerticalAlignment DockedVerticalAlignment
		{
			get
			{
				return (ObjectModifier.StackedVerticalAlignment) this.GetValue(Layout.DockedVerticalAlignmentProperty);
			}
			set
			{
				this.SetValue(Layout.DockedVerticalAlignmentProperty, value);
			}
		}


		protected override void InitialisePropertyValues()
		{
			//	Cette méthode est appelée par Proxies.Abstract quand on connecte
			//	le premier widget avec le proxy.
			
			//	Recopie localement les diverses propriétés du widget sélectionné
			//	pour pouvoir ensuite travailler dessus :
			if (this.objectModifier.HasChildrenPlacement(this.widgets[0]))
			{
				ObjectModifier.ChildrenPlacement cp = this.objectModifier.GetChildrenPlacement(this.widgets[0]);

				this.ChildrenPlacement = cp;
			}

			if (this.objectModifier.AreChildrenAnchored(this.widgets[0].Parent))
			{
				ObjectModifier.AnchoredHorizontalAttachment ha = this.objectModifier.GetAnchoredHorizontalAttachment(this.widgets[0]);
				ObjectModifier.AnchoredVerticalAttachment va = this.objectModifier.GetAnchoredVerticalAttachment(this.widgets[0]);

				this.AnchoredHorizontalAttachment = ha;
				this.AnchoredVerticalAttachment = va;
			}

			if (this.objectModifier.HasStackedHorizontalAttachment(this.widgets[0]))
			{
				ObjectModifier.StackedHorizontalAttachment ha = this.objectModifier.GetStackedHorizontalAttachment(this.widgets[0]);

				this.DockedHorizontalAttachment = ha;
			}

			if (this.objectModifier.HasStackedVerticalAttachment(this.widgets[0]))
			{
				ObjectModifier.StackedVerticalAttachment va = this.objectModifier.GetStackedVerticalAttachment(this.widgets[0]);

				this.DockedVerticalAttachment = va;
			}

			if (this.objectModifier.HasStackedHorizontalAlignment(this.widgets[0]))
			{
				ObjectModifier.StackedHorizontalAlignment ha = this.objectModifier.GetStackedHorizontalAlignment(this.widgets[0]);

				this.DockedHorizontalAlignment = ha;
			}

			if (this.objectModifier.HasStackedVerticalAlignment(this.widgets[0]))
			{
				ObjectModifier.StackedVerticalAlignment va = this.objectModifier.GetStackedVerticalAlignment(this.widgets[0]);

				this.DockedVerticalAlignment = va;
			}
		}

		private static void NotifyChildrenPlacementChanged(DependencyObject o, object oldValue, object newValue)
		{
			//	Cette méthode est appelée à la suite de la modification d'une de
			//	nos propriétés de définition pour permettre de mettre à jour les
			//	widgets connectés :
			Layout that = (Layout) o;
			ObjectModifier.ChildrenPlacement cp = that.ChildrenPlacement;

			if (that.suspendChanges == 0)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.widgets)
					{
						GeometryCache.FixBounds(obj.Children.Widgets, that.panel.PanelEditor.ObjectModifier);
						that.objectModifier.SetChildrenPlacement(obj, cp);
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
			ObjectModifier.AnchoredHorizontalAttachment ha = that.AnchoredHorizontalAttachment;

			if (that.suspendChanges == 0)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.widgets)
					{
						Rectangle bounds = that.objectModifier.GetBounds(obj);
						that.objectModifier.SetAnchoredHorizontalAttachment(obj, ha);
						that.objectModifier.SetBounds(obj, bounds);
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
			ObjectModifier.AnchoredVerticalAttachment va = that.AnchoredVerticalAttachment;

			if (that.suspendChanges == 0)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.widgets)
					{
						Rectangle bounds = that.objectModifier.GetBounds(obj);
						that.objectModifier.SetAnchoredVerticalAttachment(obj, va);
						that.objectModifier.SetBounds(obj, bounds);
					}
				}
				finally
				{
					that.ResumeChanges();
					that.RegenerateProxies();  // (*)
				}
			}
		}

		private static void NotifyDockedHorizontalAttachmentChanged(DependencyObject o, object oldValue, object newValue)
		{
			//	Cette méthode est appelée à la suite de la modification d'une de
			//	nos propriétés de définition pour permettre de mettre à jour les
			//	widgets connectés :
			Layout that = (Layout) o;
			ObjectModifier.StackedHorizontalAttachment ha = that.DockedHorizontalAttachment;

			if (that.suspendChanges == 0)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.widgets)
					{
						that.objectModifier.SetStackedHorizontalAttachment(obj, ha);
					}
				}
				finally
				{
					that.ResumeChanges();
					that.RegenerateProxies();  // (*)
				}
			}
		}

		private static void NotifyDockedVerticalAttachmentChanged(DependencyObject o, object oldValue, object newValue)
		{
			//	Cette méthode est appelée à la suite de la modification d'une de
			//	nos propriétés de définition pour permettre de mettre à jour les
			//	widgets connectés :
			Layout that = (Layout) o;
			ObjectModifier.StackedVerticalAttachment va = that.DockedVerticalAttachment;

			if (that.suspendChanges == 0)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.widgets)
					{
						that.objectModifier.SetStackedVerticalAttachment(obj, va);
					}
				}
				finally
				{
					that.ResumeChanges();
					that.RegenerateProxies();  // (*)
				}
			}
		}

		private static void NotifyDockedHorizontalAlignmentChanged(DependencyObject o, object oldValue, object newValue)
		{
			//	Cette méthode est appelée à la suite de la modification d'une de
			//	nos propriétés de définition pour permettre de mettre à jour les
			//	widgets connectés :
			Layout that = (Layout) o;
			ObjectModifier.StackedHorizontalAlignment ha = that.DockedHorizontalAlignment;

			if (that.suspendChanges == 0)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.widgets)
					{
						that.objectModifier.SetStackedHorizontalAlignment(obj, ha);
					}
				}
				finally
				{
					that.ResumeChanges();
					that.RegenerateProxies();  // (*)
				}
			}
		}

		private static void NotifyDockedVerticalAlignmentChanged(DependencyObject o, object oldValue, object newValue)
		{
			//	Cette méthode est appelée à la suite de la modification d'une de
			//	nos propriétés de définition pour permettre de mettre à jour les
			//	widgets connectés :
			Layout that = (Layout) o;
			ObjectModifier.StackedVerticalAlignment va = that.DockedVerticalAlignment;

			if (that.suspendChanges == 0)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.widgets)
					{
						that.objectModifier.SetStackedVerticalAlignment(obj, va);
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
			childrenPlacementEnumType[ObjectModifier.ChildrenPlacement.Anchored].DefineCaptionId(new Support.Druid ("[10011]").ToLong());
			childrenPlacementEnumType[ObjectModifier.ChildrenPlacement.VerticalStacked].DefineCaptionId(new Support.Druid ("[10012]").ToLong());
			childrenPlacementEnumType[ObjectModifier.ChildrenPlacement.HorizontalStacked].DefineCaptionId(new Support.Druid ("[10013]").ToLong());
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

			EnumType dockedHorizontalAttachmentEnumType = new EnumType(typeof(ObjectModifier.StackedHorizontalAttachment));
			dockedHorizontalAttachmentEnumType.DefineDefaultController("Enum", "Icons");
			dockedHorizontalAttachmentEnumType[ObjectModifier.StackedHorizontalAttachment.Left].DefineCaptionId(new Support.Druid("[10021]").ToLong());
			dockedHorizontalAttachmentEnumType[ObjectModifier.StackedHorizontalAttachment.Right].DefineCaptionId(new Support.Druid("[10022]").ToLong());
			dockedHorizontalAttachmentEnumType[ObjectModifier.StackedHorizontalAttachment.Fill].DefineCaptionId(new Support.Druid("[10025]").ToLong());
			Layout.DockedHorizontalAttachmentProperty.DefaultMetadata.DefineNamedType(dockedHorizontalAttachmentEnumType);
			Layout.DockedHorizontalAttachmentProperty.DefaultMetadata.DefineCaptionId(new Support.Druid("[100F]").ToLong());

			EnumType dockedVerticalAttachmentEnumType = new EnumType(typeof(ObjectModifier.StackedVerticalAttachment));
			dockedVerticalAttachmentEnumType.DefineDefaultController("Enum", "Icons");
			dockedVerticalAttachmentEnumType[ObjectModifier.StackedVerticalAttachment.Bottom].DefineCaptionId(new Support.Druid("[10023]").ToLong());
			dockedVerticalAttachmentEnumType[ObjectModifier.StackedVerticalAttachment.Top].DefineCaptionId(new Support.Druid("[10024]").ToLong());
			dockedVerticalAttachmentEnumType[ObjectModifier.StackedVerticalAttachment.Fill].DefineCaptionId(new Support.Druid("[10026]").ToLong());
			Layout.DockedVerticalAttachmentProperty.DefaultMetadata.DefineNamedType(dockedVerticalAttachmentEnumType);
			Layout.DockedVerticalAttachmentProperty.DefaultMetadata.DefineCaptionId(new Support.Druid("[100G]").ToLong());

			EnumType dockedHorizontalAlignmentEnumType = new EnumType(typeof(ObjectModifier.StackedHorizontalAlignment));
			dockedHorizontalAlignmentEnumType.DefineDefaultController("Enum", "Icons");
			dockedHorizontalAlignmentEnumType[ObjectModifier.StackedHorizontalAlignment.Stretch].DefineCaptionId(new Support.Druid("[10037]").ToLong());
			dockedHorizontalAlignmentEnumType[ObjectModifier.StackedHorizontalAlignment.Center].DefineCaptionId(new Support.Druid("[10035]").ToLong());
			dockedHorizontalAlignmentEnumType[ObjectModifier.StackedHorizontalAlignment.Left].DefineCaptionId(new Support.Druid("[10031]").ToLong());
			dockedHorizontalAlignmentEnumType[ObjectModifier.StackedHorizontalAlignment.Right].DefineCaptionId(new Support.Druid("[10032]").ToLong());
			Layout.DockedHorizontalAlignmentProperty.DefaultMetadata.DefineNamedType(dockedHorizontalAlignmentEnumType);
			Layout.DockedHorizontalAlignmentProperty.DefaultMetadata.DefineCaptionId(new Support.Druid("[100H]").ToLong());

			EnumType dockedVerticalAlignmentEnumType = new EnumType(typeof(ObjectModifier.StackedVerticalAlignment));
			dockedVerticalAlignmentEnumType.DefineDefaultController("Enum", "Icons");
			dockedVerticalAlignmentEnumType[ObjectModifier.StackedVerticalAlignment.Stretch].DefineCaptionId(new Support.Druid("[10038]").ToLong());
			dockedVerticalAlignmentEnumType[ObjectModifier.StackedVerticalAlignment.Center].DefineCaptionId(new Support.Druid("[10036]").ToLong());
			dockedVerticalAlignmentEnumType[ObjectModifier.StackedVerticalAlignment.Bottom].DefineCaptionId(new Support.Druid("[10033]").ToLong());
			dockedVerticalAlignmentEnumType[ObjectModifier.StackedVerticalAlignment.Top].DefineCaptionId(new Support.Druid("[10034]").ToLong());
			dockedVerticalAlignmentEnumType[ObjectModifier.StackedVerticalAlignment.BaseLine].DefineCaptionId(new Support.Druid("[10039]").ToLong());
			Layout.DockedVerticalAlignmentProperty.DefaultMetadata.DefineNamedType(dockedVerticalAlignmentEnumType);
			Layout.DockedVerticalAlignmentProperty.DefaultMetadata.DefineCaptionId(new Support.Druid("[100I]").ToLong());
		}

		
		public static readonly DependencyProperty ChildrenPlacementProperty = DependencyProperty.Register("ChildrenPlacement", typeof(ObjectModifier.ChildrenPlacement), typeof(Layout), new DependencyPropertyMetadata(ObjectModifier.ChildrenPlacement.Anchored, Layout.NotifyChildrenPlacementChanged));
		public static readonly DependencyProperty AnchoredHorizontalAttachmentProperty = DependencyProperty.Register("AnchoredHorizontalAttachment", typeof(ObjectModifier.AnchoredHorizontalAttachment), typeof(Layout), new DependencyPropertyMetadata(ObjectModifier.AnchoredHorizontalAttachment.Left, Layout.NotifyAnchoredHorizontalAttachmentChanged));
		public static readonly DependencyProperty AnchoredVerticalAttachmentProperty = DependencyProperty.Register("AnchoredVerticalAttachment", typeof(ObjectModifier.AnchoredVerticalAttachment), typeof(Layout), new DependencyPropertyMetadata(ObjectModifier.AnchoredVerticalAttachment.Bottom, Layout.NotifyAnchoredVerticalAttachmentChanged));
		public static readonly DependencyProperty DockedHorizontalAttachmentProperty = DependencyProperty.Register("DockedHorizontalAttachment", typeof(ObjectModifier.StackedHorizontalAttachment), typeof(Layout), new DependencyPropertyMetadata(ObjectModifier.StackedHorizontalAttachment.Left, Layout.NotifyDockedHorizontalAttachmentChanged));
		public static readonly DependencyProperty DockedVerticalAttachmentProperty = DependencyProperty.Register("DockedVerticalAttachment", typeof(ObjectModifier.StackedVerticalAttachment), typeof(Layout), new DependencyPropertyMetadata(ObjectModifier.StackedVerticalAttachment.Bottom, Layout.NotifyDockedVerticalAttachmentChanged));
		public static readonly DependencyProperty DockedHorizontalAlignmentProperty = DependencyProperty.Register("DockedHorizontalAlignment", typeof(ObjectModifier.StackedHorizontalAlignment), typeof(Layout), new DependencyPropertyMetadata(ObjectModifier.StackedHorizontalAlignment.Stretch, Layout.NotifyDockedHorizontalAlignmentChanged));
		public static readonly DependencyProperty DockedVerticalAlignmentProperty = DependencyProperty.Register("DockedVerticalAlignment", typeof(ObjectModifier.StackedVerticalAlignment), typeof(Layout), new DependencyPropertyMetadata(ObjectModifier.StackedVerticalAlignment.Stretch, Layout.NotifyDockedVerticalAlignmentChanged));
	}
}
