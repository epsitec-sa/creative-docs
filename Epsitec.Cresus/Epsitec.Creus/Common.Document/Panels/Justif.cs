using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe Justif permet de choisir un mode de justification.
	/// </summary>
	public class Justif : Abstract
	{
		public Justif(Document document) : base(document)
		{
			this.gridHorizontal = new RadioIconGrid(this);
			this.gridHorizontal.SelectionChanged += HandleTypeChanged;
			this.gridHorizontal.TabIndex = 0;
			this.gridHorizontal.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.AddRadioIcon(Properties.JustifHorizontal.Left);
			this.AddRadioIcon(Properties.JustifHorizontal.Center);
			this.AddRadioIcon(Properties.JustifHorizontal.Right);
			this.AddRadioIcon(Properties.JustifHorizontal.Justif);
			this.AddRadioIcon(Properties.JustifHorizontal.All);

			this.gridVertical = new RadioIconGrid(this);
			this.gridVertical.SelectionChanged += HandleTypeChanged;
			this.gridVertical.TabIndex = 0;
			this.gridVertical.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.AddRadioIcon(Properties.JustifVertical.Top);
			this.AddRadioIcon(Properties.JustifVertical.Center);
			this.AddRadioIcon(Properties.JustifVertical.Bottom);

			this.gridOrientation = new RadioIconGrid(this);
			this.gridOrientation.SelectionChanged += HandleTypeChanged;
			this.gridOrientation.TabIndex = 0;
			this.gridOrientation.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.AddRadioIcon(Properties.JustifOrientation.LeftToRight);
			this.AddRadioIcon(Properties.JustifOrientation.BottomToTop);
			this.AddRadioIcon(Properties.JustifOrientation.RightToLeft);
			this.AddRadioIcon(Properties.JustifOrientation.TopToBottom);

			this.fieldMarginH = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldMarginH.LabelShortText = Res.Strings.Panel.Justif.Short.MarginH;
			this.fieldMarginH.LabelLongText  = Res.Strings.Panel.Justif.Long.MarginH;
			this.fieldMarginH.TextFieldReal.FactorMinRange = 0.0M;
			this.fieldMarginH.TextFieldReal.FactorMaxRange = 0.1M;
			this.fieldMarginH.TextFieldReal.FactorStep = 1.0M;
			this.document.Modifier.AdaptTextFieldRealDimension(this.fieldMarginH.TextFieldReal);
			this.fieldMarginH.TextFieldReal.EditionAccepted += this.HandleFieldChanged;
			this.fieldMarginH.TabIndex = 20;
			this.fieldMarginH.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldMarginH, Res.Strings.Panel.Justif.Tooltip.MarginH);

			this.fieldMarginV = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldMarginV.LabelShortText = Res.Strings.Panel.Justif.Short.MarginV;
			this.fieldMarginV.LabelLongText  = Res.Strings.Panel.Justif.Long.MarginV;
			this.fieldMarginV.TextFieldReal.FactorMinRange = 0.0M;
			this.fieldMarginV.TextFieldReal.FactorMaxRange = 0.1M;
			this.fieldMarginV.TextFieldReal.FactorStep = 1.0M;
			this.document.Modifier.AdaptTextFieldRealDimension(this.fieldMarginV.TextFieldReal);
			this.fieldMarginV.TextFieldReal.EditionAccepted += this.HandleFieldChanged;
			this.fieldMarginV.TabIndex = 21;
			this.fieldMarginV.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldMarginV, Res.Strings.Panel.Justif.Tooltip.MarginV);

			this.fieldOffsetV = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldOffsetV.LabelShortText = Res.Strings.Panel.Justif.Short.OffsetV;
			this.fieldOffsetV.LabelLongText  = Res.Strings.Panel.Justif.Long.OffsetV;
			this.document.Modifier.AdaptTextFieldRealScalar(this.fieldOffsetV.TextFieldReal);
			this.fieldOffsetV.TextFieldReal.InternalMinValue = -50.0M;
			this.fieldOffsetV.TextFieldReal.InternalMaxValue =  50.0M;
			this.fieldOffsetV.TextFieldReal.TextSuffix = "%";
			this.fieldOffsetV.TextFieldReal.EditionAccepted += this.HandleFieldChanged;
			this.fieldOffsetV.TabIndex = 22;
			this.fieldOffsetV.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldOffsetV, Res.Strings.Panel.Justif.Tooltip.OffsetV);

			this.isNormalAndExtended = true;
		}
		
		protected void AddRadioIcon(Properties.JustifHorizontal type)
		{
			this.gridHorizontal.AddRadioIcon(Misc.Icon(Properties.Justif.GetIconText(type)), Properties.Justif.GetName(type), (int)type, false);
		}

		protected void AddRadioIcon(Properties.JustifVertical type)
		{
			this.gridVertical.AddRadioIcon(Misc.Icon(Properties.Justif.GetIconText(type)), Properties.Justif.GetName(type), (int)type, false);
		}

		protected void AddRadioIcon(Properties.JustifOrientation type)
		{
			this.gridOrientation.AddRadioIcon(Misc.Icon(Properties.Justif.GetIconText(type)), Properties.Justif.GetName(type), (int)type, false);
		}

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.gridHorizontal.SelectionChanged -= HandleTypeChanged;
				this.gridVertical.SelectionChanged -= HandleTypeChanged;
				this.gridOrientation.SelectionChanged -= HandleTypeChanged;

				this.fieldMarginH.TextFieldReal.EditionAccepted -= this.HandleFieldChanged;
				this.fieldMarginV.TextFieldReal.EditionAccepted -= this.HandleFieldChanged;
				this.fieldOffsetV.TextFieldReal.EditionAccepted -= this.HandleFieldChanged;

				this.gridHorizontal = null;
				this.gridVertical = null;
				this.gridOrientation = null;
				this.fieldMarginH = null;
				this.fieldMarginV = null;
				this.fieldOffsetV = null;
			}
			
			base.Dispose(disposing);
		}

		
		public override double DefaultHeight
		{
			//	Retourne la hauteur standard.
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

		protected override void PropertyToWidgets()
		{
			//	Propriété -> widgets.
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

		protected override void WidgetsToProperty()
		{
			//	Widgets -> propriété.
			Properties.Justif p = this.property as Properties.Justif;
			if ( p == null )  return;

			p.Horizontal  = (Properties.JustifHorizontal)  this.gridHorizontal.SelectedValue;
			p.Vertical    = (Properties.JustifVertical)    this.gridVertical.SelectedValue;
			p.Orientation = (Properties.JustifOrientation) this.gridOrientation.SelectedValue;
			p.MarginH     = (double) this.fieldMarginH.TextFieldReal.InternalValue;
			p.MarginV     = (double) this.fieldMarginV.TextFieldReal.InternalValue;
			p.OffsetV     = (double) this.fieldOffsetV.TextFieldReal.InternalValue/100;
		}


		protected void EnableWidgets()
		{
			//	Grise les widgets nécessaires.
			this.gridVertical.Visibility = (this.isExtendedSize);
			this.gridOrientation.Visibility = (this.isExtendedSize);

			this.fieldMarginH.Visibility = (this.isExtendedSize);

			if ( this.isExtendedSize )
			{
				Properties.JustifVertical v = (Properties.JustifVertical) this.gridVertical.SelectedValue;
				if ( v == Properties.JustifVertical.Center )
				{
					this.fieldMarginV.Visibility = false;
					this.fieldOffsetV.Visibility = true;
				}
				else
				{
					this.fieldMarginV.Visibility = true;
					this.fieldOffsetV.Visibility = false;
				}
			}
			else
			{
				this.fieldMarginV.Visibility = false;
				this.fieldOffsetV.Visibility = false;
			}
		}

		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.gridHorizontal == null )  return;

			this.EnableWidgets();

			Rectangle rect = this.UsefulZone;

			Rectangle r = rect;
			r.Bottom = r.Top-20;
			r.Width = 22*5;
			r.Inflate(1);
			this.gridHorizontal.SetManualBounds(r);

			if ( this.isExtendedSize && this.IsLabelProperties )
			{
				r.Offset(0, -25);
				r.Left = rect.Left;
				r.Width = 22*3;
				r.Inflate(1);
				this.gridVertical.SetManualBounds(r);
				r.Left = rect.Right-22*4;
				r.Width = 22*4;
				r.Inflate(1);
				this.gridOrientation.SetManualBounds(r);

				r.Offset(0, -28);
				r.Bottom = r.Top-20;
				r.Left = rect.Left;
				r.Right = rect.Right;
				this.fieldMarginH.SetManualBounds(r);

				r.Offset(0, -25);
				this.fieldMarginV.SetManualBounds(r);
				this.fieldOffsetV.SetManualBounds(r);
			}
			else
			{
				r.Offset(0, -25);
				r.Left = rect.Right-22*3;
				r.Width = 22*3;
				r.Inflate(1);
				this.gridVertical.SetManualBounds(r);

				r.Offset(0, -25);
				r.Left = rect.Right-22*4;
				r.Width = 22*4;
				r.Inflate(1);
				this.gridOrientation.SetManualBounds(r);

				r = rect;
				r.Bottom = rect.Top-45;
				r.Height = 20;
				r.Width = Widgets.TextFieldLabel.ShortWidth+10;
				this.fieldMarginH.SetManualBounds(r);

				r = rect;
				r.Bottom = rect.Top-70;
				r.Height = 20;
				r.Width = Widgets.TextFieldLabel.ShortWidth+10;
				this.fieldMarginV.SetManualBounds(r);
				this.fieldOffsetV.SetManualBounds(r);
			}
		}
		
		private void HandleTypeChanged(object sender)
		{
			//	Le type a été changé.
			if ( this.ignoreChanged )  return;
			this.EnableWidgets();
			this.OnChanged();
		}

		private void HandleFieldChanged(object sender)
		{
			//	Un champ a été changé.
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}


		protected RadioIconGrid				gridHorizontal;
		protected RadioIconGrid				gridVertical;
		protected RadioIconGrid				gridOrientation;
		protected Widgets.TextFieldLabel	fieldMarginH;
		protected Widgets.TextFieldLabel	fieldMarginV;
		protected Widgets.TextFieldLabel	fieldOffsetV;
	}
}
