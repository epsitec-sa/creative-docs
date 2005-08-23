using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe Justif permet de choisir un mode de justification.
	/// </summary>
	[SuppressBundleSupport]
	public class Justif : Abstract
	{
		public Justif(Document document) : base(document)
		{
			this.gridHorizontal = new Widgets.RadioIconGrid(this);
			this.gridHorizontal.SelectionChanged += new EventHandler(HandleTypeChanged);
			this.gridHorizontal.TabIndex = 0;
			this.gridHorizontal.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.AddRadioIcon(Properties.JustifHorizontal.Left);
			this.AddRadioIcon(Properties.JustifHorizontal.Center);
			this.AddRadioIcon(Properties.JustifHorizontal.Right);
			this.AddRadioIcon(Properties.JustifHorizontal.Justif);
			this.AddRadioIcon(Properties.JustifHorizontal.All);

			this.gridVertical = new Widgets.RadioIconGrid(this);
			this.gridVertical.SelectionChanged += new EventHandler(HandleTypeChanged);
			this.gridVertical.TabIndex = 0;
			this.gridVertical.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.AddRadioIcon(Properties.JustifVertical.Top);
			this.AddRadioIcon(Properties.JustifVertical.Center);
			this.AddRadioIcon(Properties.JustifVertical.Bottom);

			this.gridOrientation = new Widgets.RadioIconGrid(this);
			this.gridOrientation.SelectionChanged += new EventHandler(HandleTypeChanged);
			this.gridOrientation.TabIndex = 0;
			this.gridOrientation.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.AddRadioIcon(Properties.JustifOrientation.LeftToRight);
			this.AddRadioIcon(Properties.JustifOrientation.BottomToTop);
			this.AddRadioIcon(Properties.JustifOrientation.RightToLeft);
			this.AddRadioIcon(Properties.JustifOrientation.TopToBottom);

			this.fieldMarginH = new Widgets.TextFieldLabel(this, false);
			this.fieldMarginH.LabelShortText = Res.Strings.Panel.Justif.Short.MarginH;
			this.fieldMarginH.LabelLongText  = Res.Strings.Panel.Justif.Long.MarginH;
			this.fieldMarginH.TextFieldReal.FactorMinRange = 0.0M;
			this.fieldMarginH.TextFieldReal.FactorMaxRange = 0.1M;
			this.fieldMarginH.TextFieldReal.FactorStep = 1.0M;
			this.document.Modifier.AdaptTextFieldRealDimension(this.fieldMarginH.TextFieldReal);
			this.fieldMarginH.TextFieldReal.ValueChanged += new EventHandler(this.HandleFieldChanged);
			this.fieldMarginH.TabIndex = 20;
			this.fieldMarginH.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldMarginH, Res.Strings.Panel.Justif.Tooltip.MarginH);

			this.fieldMarginV = new Widgets.TextFieldLabel(this, false);
			this.fieldMarginV.LabelShortText = Res.Strings.Panel.Justif.Short.MarginV;
			this.fieldMarginV.LabelLongText  = Res.Strings.Panel.Justif.Long.MarginV;
			this.fieldMarginV.TextFieldReal.FactorMinRange = 0.0M;
			this.fieldMarginV.TextFieldReal.FactorMaxRange = 0.1M;
			this.fieldMarginV.TextFieldReal.FactorStep = 1.0M;
			this.document.Modifier.AdaptTextFieldRealDimension(this.fieldMarginV.TextFieldReal);
			this.fieldMarginV.TextFieldReal.ValueChanged += new EventHandler(this.HandleFieldChanged);
			this.fieldMarginV.TabIndex = 21;
			this.fieldMarginV.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldMarginV, Res.Strings.Panel.Justif.Tooltip.MarginV);

			this.fieldOffsetV = new Widgets.TextFieldLabel(this, false);
			this.fieldOffsetV.LabelShortText = Res.Strings.Panel.Justif.Short.OffsetV;
			this.fieldOffsetV.LabelLongText  = Res.Strings.Panel.Justif.Long.OffsetV;
			this.document.Modifier.AdaptTextFieldRealScalar(this.fieldOffsetV.TextFieldReal);
			this.fieldOffsetV.TextFieldReal.InternalMinValue = -50.0M;
			this.fieldOffsetV.TextFieldReal.InternalMaxValue =  50.0M;
			this.fieldOffsetV.TextFieldReal.TextSuffix = "%";
			this.fieldOffsetV.TextFieldReal.ValueChanged += new EventHandler(this.HandleFieldChanged);
			this.fieldOffsetV.TabIndex = 22;
			this.fieldOffsetV.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldOffsetV, Res.Strings.Panel.Justif.Tooltip.OffsetV);

			this.isNormalAndExtended = true;
		}
		
		protected void AddRadioIcon(Properties.JustifHorizontal type)
		{
			this.gridHorizontal.AddRadioIcon(Properties.Justif.GetIconText(type), Properties.Justif.GetName(type), (int)type, false);
		}

		protected void AddRadioIcon(Properties.JustifVertical type)
		{
			this.gridVertical.AddRadioIcon(Properties.Justif.GetIconText(type), Properties.Justif.GetName(type), (int)type, false);
		}

		protected void AddRadioIcon(Properties.JustifOrientation type)
		{
			this.gridOrientation.AddRadioIcon(Properties.Justif.GetIconText(type), Properties.Justif.GetName(type), (int)type, false);
		}

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.gridHorizontal.SelectionChanged -= new EventHandler(HandleTypeChanged);
				this.gridVertical.SelectionChanged -= new EventHandler(HandleTypeChanged);
				this.gridOrientation.SelectionChanged -= new EventHandler(HandleTypeChanged);

				this.fieldMarginH.TextFieldReal.ValueChanged -= new EventHandler(this.HandleFieldChanged);
				this.fieldMarginV.TextFieldReal.ValueChanged -= new EventHandler(this.HandleFieldChanged);
				this.fieldOffsetV.TextFieldReal.ValueChanged -= new EventHandler(this.HandleFieldChanged);

				this.gridHorizontal = null;
				this.gridVertical = null;
				this.gridOrientation = null;
				this.fieldMarginH = null;
				this.fieldMarginV = null;
				this.fieldOffsetV = null;
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
						h += 105;
					}
					else	// étendu/compact ?
					{
						h += 80;
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

			Properties.Justif p = this.property as Properties.Justif;
			if ( p == null )  return;

			this.ignoreChanged = true;

			this.gridHorizontal.SelectedValue  = (int) p.Horizontal;
			this.gridVertical.SelectedValue    = (int) p.Vertical;
			this.gridOrientation.SelectedValue = (int) p.Orientation;
			this.fieldMarginH.TextFieldReal.InternalValue = (decimal) p.MarginH;
			this.fieldMarginV.TextFieldReal.InternalValue = (decimal) p.MarginV;
			this.fieldOffsetV.TextFieldReal.InternalValue = (decimal) p.OffsetV*100;

			this.EnableWidgets();
			this.ignoreChanged = false;
		}

		// Widgets -> propriété.
		protected override void WidgetsToProperty()
		{
			Properties.Justif p = this.property as Properties.Justif;
			if ( p == null )  return;

			p.Horizontal  = (Properties.JustifHorizontal)  this.gridHorizontal.SelectedValue;
			p.Vertical    = (Properties.JustifVertical)    this.gridVertical.SelectedValue;
			p.Orientation = (Properties.JustifOrientation) this.gridOrientation.SelectedValue;
			p.MarginH     = (double) this.fieldMarginH.TextFieldReal.InternalValue;
			p.MarginV     = (double) this.fieldMarginV.TextFieldReal.InternalValue;
			p.OffsetV     = (double) this.fieldOffsetV.TextFieldReal.InternalValue/100;
		}


		// Grise les widgets nécessaires.
		protected void EnableWidgets()
		{
			this.gridVertical.SetVisible(this.isExtendedSize);
			this.gridOrientation.SetVisible(this.isExtendedSize);

			this.fieldMarginH.SetVisible(this.isExtendedSize);

			if ( this.isExtendedSize )
			{
				Properties.JustifVertical v = (Properties.JustifVertical) this.gridVertical.SelectedValue;
				if ( v == Properties.JustifVertical.Center )
				{
					this.fieldMarginV.SetVisible(false);
					this.fieldOffsetV.SetVisible(true);
				}
				else
				{
					this.fieldMarginV.SetVisible(true);
					this.fieldOffsetV.SetVisible(false);
				}
			}
			else
			{
				this.fieldMarginV.SetVisible(false);
				this.fieldOffsetV.SetVisible(false);
			}
		}

		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.gridHorizontal == null )  return;

			this.EnableWidgets();

			Rectangle rect = this.UsefulZone;

			Rectangle r = rect;
			r.Bottom = r.Top-20;
			r.Width = 22*5;
			r.Inflate(1);
			this.gridHorizontal.Bounds = r;

			if ( this.isExtendedSize && this.IsLabelProperties )
			{
				r.Offset(0, -25);
				r.Left = rect.Left;
				r.Width = 22*3;
				r.Inflate(1);
				this.gridVertical.Bounds = r;
				r.Left = rect.Right-22*4;
				r.Width = 22*4;
				r.Inflate(1);
				this.gridOrientation.Bounds = r;

				r.Offset(0, -28);
				r.Bottom = r.Top-20;
				r.Left = rect.Left;
				r.Right = rect.Right;
				this.fieldMarginH.Bounds = r;

				r.Offset(0, -25);
				this.fieldMarginV.Bounds = r;
				this.fieldOffsetV.Bounds = r;
			}
			else
			{
				r.Offset(0, -25);
				r.Left = rect.Right-22*3;
				r.Width = 22*3;
				r.Inflate(1);
				this.gridVertical.Bounds = r;

				r.Offset(0, -25);
				r.Left = rect.Right-22*4;
				r.Width = 22*4;
				r.Inflate(1);
				this.gridOrientation.Bounds = r;

				r = rect;
				r.Bottom = rect.Top-45;
				r.Height = 20;
				r.Width = Widgets.TextFieldLabel.ShortWidth+10;
				this.fieldMarginH.Bounds = r;

				r = rect;
				r.Bottom = rect.Top-70;
				r.Height = 20;
				r.Width = Widgets.TextFieldLabel.ShortWidth+10;
				this.fieldMarginV.Bounds = r;
				this.fieldOffsetV.Bounds = r;
			}
		}
		
		// Le type a été changé.
		private void HandleTypeChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.EnableWidgets();
			this.OnChanged();
		}

		// Un champ a été changé.
		private void HandleFieldChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}


		protected Widgets.RadioIconGrid		gridHorizontal;
		protected Widgets.RadioIconGrid		gridVertical;
		protected Widgets.RadioIconGrid		gridOrientation;
		protected Widgets.TextFieldLabel	fieldMarginH;
		protected Widgets.TextFieldLabel	fieldMarginV;
		protected Widgets.TextFieldLabel	fieldOffsetV;
	}
}
