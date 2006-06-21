using System.Collections.Generic;

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Proxies
{
	public class Layout : Abstract
	{
		public Layout(Widget widget, ObjectModifier objectModifier) : base(widget, objectModifier)
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
				return 80;
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

		public ObjectModifier.DockedHorizontalAttachment DockedHorizontalAttachment
		{
			get
			{
				return (ObjectModifier.DockedHorizontalAttachment) this.GetValue(Layout.DockedHorizontalAttachmentProperty);
			}
			set
			{
				this.SetValue(Layout.DockedHorizontalAttachmentProperty, value);
			}
		}

		public ObjectModifier.DockedVerticalAttachment DockedVerticalAttachment
		{
			get
			{
				return (ObjectModifier.DockedVerticalAttachment) this.GetValue(Layout.DockedVerticalAttachmentProperty);
			}
			set
			{
				this.SetValue(Layout.DockedVerticalAttachmentProperty, value);
			}
		}

		public ObjectModifier.DockedHorizontalAlignment DockedHorizontalAlignment
		{
			get
			{
				return (ObjectModifier.DockedHorizontalAlignment) this.GetValue(Layout.DockedHorizontalAlignmentProperty);
			}
			set
			{
				this.SetValue(Layout.DockedHorizontalAlignmentProperty, value);
			}
		}

		public ObjectModifier.DockedVerticalAlignment DockedVerticalAlignment
		{
			get
			{
				return (ObjectModifier.DockedVerticalAlignment) this.GetValue(Layout.DockedVerticalAlignmentProperty);
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

			if (this.objectModifier.HasDockedHorizontalAttachment(this.widgets[0]))
			{
				ObjectModifier.DockedHorizontalAttachment ha = this.objectModifier.GetDockedHorizontalAttachment(this.widgets[0]);

				this.DockedHorizontalAttachment = ha;
			}

			if (this.objectModifier.HasDockedVerticalAttachment(this.widgets[0]))
			{
				ObjectModifier.DockedVerticalAttachment va = this.objectModifier.GetDockedVerticalAttachment(this.widgets[0]);

				this.DockedVerticalAttachment = va;
			}

			if (this.objectModifier.HasDockedHorizontalAlignment(this.widgets[0]))
			{
				ObjectModifier.DockedHorizontalAlignment ha = this.objectModifier.GetDockedHorizontalAlignment(this.widgets[0]);

				this.DockedHorizontalAlignment = ha;
			}

			if (this.objectModifier.HasDockedVerticalAlignment(this.widgets[0]))
			{
				ObjectModifier.DockedVerticalAlignment va = this.objectModifier.GetDockedVerticalAlignment(this.widgets[0]);

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
						that.objectModifier.SetChildrenPlacement(obj, cp);
					}
				}
				finally
				{
					that.ResumeChanges();
				}
			}
		}

		private static void NotifyAnchoredHorizontalAttachmentChanged(DependencyObject o, object oldValue, object newValue)
		{
			//	Cette méthode est appelée à la suite de la modification d'une de
			//	nos propriétés de définition pour permettre de mettre à jour les
			//	widgets connectés :
			Layout that = (Layout) o;
			ObjectModifier.AnchoredHorizontalAttachment cp = that.AnchoredHorizontalAttachment;

			if (that.suspendChanges == 0)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.widgets)
					{
						that.objectModifier.SetAnchoredHorizontalAttachment(obj, cp);
					}
				}
				finally
				{
					that.ResumeChanges();
				}
			}
		}

		private static void NotifyAnchoredVerticalAttachmentChanged(DependencyObject o, object oldValue, object newValue)
		{
			//	Cette méthode est appelée à la suite de la modification d'une de
			//	nos propriétés de définition pour permettre de mettre à jour les
			//	widgets connectés :
			Layout that = (Layout) o;
			ObjectModifier.AnchoredVerticalAttachment cp = that.AnchoredVerticalAttachment;

			if (that.suspendChanges == 0)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.widgets)
					{
						that.objectModifier.SetAnchoredVerticalAttachment(obj, cp);
					}
				}
				finally
				{
					that.ResumeChanges();
				}
			}
		}

		private static void NotifyDockedHorizontalAttachmentChanged(DependencyObject o, object oldValue, object newValue)
		{
			//	Cette méthode est appelée à la suite de la modification d'une de
			//	nos propriétés de définition pour permettre de mettre à jour les
			//	widgets connectés :
			Layout that = (Layout) o;
			ObjectModifier.DockedHorizontalAttachment cp = that.DockedHorizontalAttachment;

			if (that.suspendChanges == 0)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.widgets)
					{
						that.objectModifier.SetDockedHorizontalAttachment(obj, cp);
					}
				}
				finally
				{
					that.ResumeChanges();
				}
			}
		}

		private static void NotifyDockedVerticalAttachmentChanged(DependencyObject o, object oldValue, object newValue)
		{
			//	Cette méthode est appelée à la suite de la modification d'une de
			//	nos propriétés de définition pour permettre de mettre à jour les
			//	widgets connectés :
			Layout that = (Layout) o;
			ObjectModifier.DockedVerticalAttachment cp = that.DockedVerticalAttachment;

			if (that.suspendChanges == 0)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.widgets)
					{
						that.objectModifier.SetDockedVerticalAttachment(obj, cp);
					}
				}
				finally
				{
					that.ResumeChanges();
				}
			}
		}

		private static void NotifyDockedHorizontalAlignmentChanged(DependencyObject o, object oldValue, object newValue)
		{
			//	Cette méthode est appelée à la suite de la modification d'une de
			//	nos propriétés de définition pour permettre de mettre à jour les
			//	widgets connectés :
			Layout that = (Layout) o;
			ObjectModifier.DockedHorizontalAlignment cp = that.DockedHorizontalAlignment;

			if (that.suspendChanges == 0)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.widgets)
					{
						that.objectModifier.SetDockedHorizontalAlignment(obj, cp);
					}
				}
				finally
				{
					that.ResumeChanges();
				}
			}
		}

		private static void NotifyDockedVerticalAlignmentChanged(DependencyObject o, object oldValue, object newValue)
		{
			//	Cette méthode est appelée à la suite de la modification d'une de
			//	nos propriétés de définition pour permettre de mettre à jour les
			//	widgets connectés :
			Layout that = (Layout) o;
			ObjectModifier.DockedVerticalAlignment cp = that.DockedVerticalAlignment;

			if (that.suspendChanges == 0)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.widgets)
					{
						that.objectModifier.SetDockedVerticalAlignment(obj, cp);
					}
				}
				finally
				{
					that.ResumeChanges();
				}
			}
		}


		static Layout()
		{
			EnumType childrenPlacementEnumType = new EnumType (typeof (ObjectModifier.ChildrenPlacement));

			childrenPlacementEnumType.DefineDefaultController ("Enum", "Icons");
			childrenPlacementEnumType[ObjectModifier.ChildrenPlacement.Anchored].DefineCaptionId (new Support.Druid ("[10001]").ToLong ());
			childrenPlacementEnumType[ObjectModifier.ChildrenPlacement.VerticalDocked].DefineCaptionId (new Support.Druid ("[10011]").ToLong ());
			childrenPlacementEnumType[ObjectModifier.ChildrenPlacement.HorizontalDocked].DefineCaptionId (new Support.Druid ("[10021]").ToLong ());
			
			Layout.ChildrenPlacementProperty.DefaultMetadata.DefineNamedType (childrenPlacementEnumType);
			Layout.ChildrenPlacementProperty.DefaultMetadata.DefineCaptionId(new Support.Druid("[100C]").ToLong());
			Layout.AnchoredHorizontalAttachmentProperty.DefaultMetadata.DefineCaptionId(new Support.Druid("[100D]").ToLong());
			Layout.AnchoredVerticalAttachmentProperty.DefaultMetadata.DefineCaptionId(new Support.Druid("[100E]").ToLong());
			Layout.DockedHorizontalAttachmentProperty.DefaultMetadata.DefineCaptionId(new Support.Druid("[100F]").ToLong());
			Layout.DockedVerticalAttachmentProperty.DefaultMetadata.DefineCaptionId(new Support.Druid("[100G]").ToLong());
			Layout.DockedHorizontalAlignmentProperty.DefaultMetadata.DefineCaptionId(new Support.Druid("[100H]").ToLong());
			Layout.DockedVerticalAlignmentProperty.DefaultMetadata.DefineCaptionId(new Support.Druid("[100I]").ToLong());
		}

		
		public static readonly DependencyProperty ChildrenPlacementProperty = DependencyProperty.Register("ChildrenPlacement", typeof(ObjectModifier.ChildrenPlacement), typeof(Layout), new DependencyPropertyMetadata(ObjectModifier.ChildrenPlacement.Anchored, Layout.NotifyChildrenPlacementChanged));
		public static readonly DependencyProperty AnchoredHorizontalAttachmentProperty = DependencyProperty.Register("AnchoredHorizontalAttachment", typeof(ObjectModifier.AnchoredHorizontalAttachment), typeof(Layout), new DependencyPropertyMetadata(ObjectModifier.AnchoredHorizontalAttachment.Left, Layout.NotifyAnchoredHorizontalAttachmentChanged));
		public static readonly DependencyProperty AnchoredVerticalAttachmentProperty = DependencyProperty.Register("AnchoredVerticalAttachment", typeof(ObjectModifier.AnchoredVerticalAttachment), typeof(Layout), new DependencyPropertyMetadata(ObjectModifier.AnchoredVerticalAttachment.Bottom, Layout.NotifyAnchoredVerticalAttachmentChanged));
		public static readonly DependencyProperty DockedHorizontalAttachmentProperty = DependencyProperty.Register("DockedHorizontalAttachment", typeof(ObjectModifier.DockedHorizontalAttachment), typeof(Layout), new DependencyPropertyMetadata(ObjectModifier.DockedHorizontalAttachment.Left, Layout.NotifyDockedHorizontalAttachmentChanged));
		public static readonly DependencyProperty DockedVerticalAttachmentProperty = DependencyProperty.Register("DockedVerticalAttachment", typeof(ObjectModifier.DockedVerticalAttachment), typeof(Layout), new DependencyPropertyMetadata(ObjectModifier.DockedVerticalAttachment.Bottom, Layout.NotifyDockedVerticalAttachmentChanged));
		public static readonly DependencyProperty DockedHorizontalAlignmentProperty = DependencyProperty.Register("DockedHorizontalAlignment", typeof(ObjectModifier.DockedHorizontalAlignment), typeof(Layout), new DependencyPropertyMetadata(ObjectModifier.DockedHorizontalAlignment.Stretch, Layout.NotifyDockedHorizontalAlignmentChanged));
		public static readonly DependencyProperty DockedVerticalAlignmentProperty = DependencyProperty.Register("DockedVerticalAlignment", typeof(ObjectModifier.DockedVerticalAlignment), typeof(Layout), new DependencyPropertyMetadata(ObjectModifier.DockedVerticalAlignment.Stretch, Layout.NotifyDockedVerticalAlignmentChanged));
	}
}
