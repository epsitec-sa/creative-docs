using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe Arc permet de choisir un mode d'arc de cercle ou d'ellipse.
	/// </summary>
	[SuppressBundleSupport]
	public class Arc : Abstract
	{
		public Arc(Document document) : base(document)
		{
			this.grid = new Widgets.RadioIconGrid(this);
			this.grid.SelectionChanged += new EventHandler(HandleTypeChanged);
			this.grid.TabIndex = 1;
			this.grid.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.AddRadioIcon(Properties.ArcType.Full);
			this.AddRadioIcon(Properties.ArcType.Open);
			this.AddRadioIcon(Properties.ArcType.Close);
			this.AddRadioIcon(Properties.ArcType.Pie);
			
			this.fieldStarting = new Widgets.TextFieldLabel(this, false);
			this.fieldStarting.LabelShortText = Res.Strings.Panel.Arc.Short.Initial;
			this.fieldStarting.LabelLongText  = Res.Strings.Panel.Arc.Long.Initial;
			this.document.Modifier.AdaptTextFieldRealAngle(this.fieldStarting.TextFieldReal);
			this.fieldStarting.TextFieldReal.EditionAccepted += new EventHandler(this.HandleFieldChanged);
			this.fieldStarting.TabIndex = 2;
			this.fieldStarting.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldStarting, Res.Strings.Panel.Arc.Tooltip.Initial);

			this.fieldEnding = new Widgets.TextFieldLabel(this, false);
			this.fieldEnding.LabelShortText = Res.Strings.Panel.Arc.Short.Final;
			this.fieldEnding.LabelLongText  = Res.Strings.Panel.Arc.Long.Final;
			this.document.Modifier.AdaptTextFieldRealAngle(this.fieldEnding.TextFieldReal);
			this.fieldEnding.TextFieldReal.EditionAccepted += new EventHandler(this.HandleFieldChanged);
			this.fieldEnding.TabIndex = 3;
			this.fieldEnding.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldEnding, Res.Strings.Panel.Arc.Tooltip.Final);

			this.isNormalAndExtended = true;
		}
		
		protected void AddRadioIcon(Properties.ArcType type)
		{
			this.grid.AddRadioIcon(Properties.Arc.GetIconText(type), Properties.Arc.GetName(type), (int)type, false);
		}

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.grid.SelectionChanged -= new EventHandler(HandleTypeChanged);
				this.fieldStarting.TextFieldReal.EditionAccepted -= new EventHandler(this.HandleFieldChanged);
				this.fieldEnding.TextFieldReal.EditionAccepted -= new EventHandler(this.HandleFieldChanged);

				this.grid = null;
				this.fieldStarting = null;
				this.fieldEnding = null;
			}
			
			base.Dispose(disposing);
		}

		
		// Retourne la hauteur standard.
		public override double DefaultHeight
		{
			get
			{
				double h = this.LabelHeight;

				if ( this.isExtendedSize )  // panneau étendu ?
				{
					if ( this.IsLabelProperties )  // étendu/détails ?
					{
						h += 80;
					}
					else	// étendu/compact ?
					{
						h += 55;
					}
				}
				else	// panneau réduit ?
				{
					h += 30;
				}

				return h;
			}
		}

		// Propriété -> widgets.
		protected override void PropertyToWidgets()
		{
			base.PropertyToWidgets();

			Properties.Arc p = this.property as Properties.Arc;
			if ( p == null )  return;

			this.ignoreChanged = true;

			this.SelectButtonType = p.ArcType;
			this.fieldStarting.TextFieldReal.InternalValue = (decimal) p.StartingAngle;
			this.fieldEnding.TextFieldReal.InternalValue   = (decimal) p.EndingAngle;

			this.EnableWidgets();
			this.ignoreChanged = false;
		}

		// Widgets -> propriété.
		protected override void WidgetsToProperty()
		{
			Properties.Arc p = this.property as Properties.Arc;
			if ( p == null )  return;

			p.ArcType       = this.SelectButtonType;
			p.StartingAngle = (double) this.fieldStarting.TextFieldReal.InternalValue;
			p.EndingAngle   = (double) this.fieldEnding.TextFieldReal.InternalValue;
		}

		protected Properties.ArcType SelectButtonType
		{
			get
			{
				return (Properties.ArcType) this.grid.SelectedValue;
			}

			set
			{
				this.grid.SelectedValue = (int) value;
			}
		}


		// Grise les widgets nécessaires.
		protected void EnableWidgets()
		{
			bool enable = (this.SelectButtonType != Properties.ArcType.Full);

			this.fieldStarting.Enable = (this.isExtendedSize && enable);
			this.fieldEnding.Enable = (this.isExtendedSize && enable);

			this.fieldStarting.Visibility = (this.isExtendedSize);
			this.fieldEnding.Visibility = (this.isExtendedSize);
		}

		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.grid == null )  return;

			this.EnableWidgets();

			Rectangle rect = this.UsefulZone;

			Rectangle r = rect;
			r.Bottom = r.Top-20;
			r.Inflate(1);
			this.grid.Bounds = r;

			if ( this.isExtendedSize && this.IsLabelProperties )
			{
				r.Offset(0, -25);
				r.Left = rect.Left;
				r.Right = rect.Right;
				this.fieldStarting.Bounds = r;
				r.Offset(0, -25);
				this.fieldEnding.Bounds = r;
			}
			else
			{
				double w = Widgets.TextFieldLabel.ShortWidth+10;

				r.Offset(0, -25);
				r.Left = rect.Right-w-w;
				r.Width = w;
				this.fieldStarting.Bounds = r;
				r.Left = r.Right;
				r.Width = w;
				this.fieldEnding.Bounds = r;
			}
		}
		
		// Un champ a été changé.
		private void HandleFieldChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}

		// Une valeur a été changée.
		private void HandleTypeChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.SelectButtonType = (Properties.ArcType) this.grid.SelectedValue;
			this.EnableWidgets();
			this.OnChanged();
		}


		protected Widgets.RadioIconGrid		grid;
		protected Widgets.TextFieldLabel	fieldStarting;
		protected Widgets.TextFieldLabel	fieldEnding;
	}
}
