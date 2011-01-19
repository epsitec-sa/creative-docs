using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe Frame permet de choisir un cadre.
	/// </summary>
	public class Frame : Abstract
	{
		public Frame(Document document)
			: base (document)
		{
			this.grid = new RadioIconGrid(this);
			this.grid.SelectionChanged += HandleTypeChanged;
			this.grid.TabIndex = 0;
			this.grid.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.AddRadioIcon (Properties.FrameType.None);
			this.AddRadioIcon (Properties.FrameType.Simple);
			this.AddRadioIcon (Properties.FrameType.White);
			this.AddRadioIcon (Properties.FrameType.Shadow);
			this.AddRadioIcon (Properties.FrameType.WhiteAndSnadow);

			this.fieldFrameWidth = new Widgets.TextFieldLabel (this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldFrameWidth.LabelShortText = "T"; //Res.Strings.Panel.Frame.Short.FrameWidth;
			this.fieldFrameWidth.LabelLongText  = "Epaisseur du trait"; //Res.Strings.Panel.Frame.Long.FrameWidth;
			this.document.Modifier.AdaptTextFieldRealPercent (this.fieldFrameWidth.TextFieldReal);
			this.fieldFrameWidth.TextFieldReal.EditionAccepted += this.HandleFieldChanged;
			this.fieldFrameWidth.TabIndex = 2;
			this.fieldFrameWidth.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			//?ToolTip.Default.SetToolTip (this.fieldFrameWidth, Res.Strings.Panel.Frame.Tooltip.FrameWidth);

			this.fieldMarginWidth = new Widgets.TextFieldLabel (this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldMarginWidth.LabelShortText = "C"; //Res.Strings.Panel.Frame.Short.MarginWidth;
			this.fieldMarginWidth.LabelLongText  = "Epaisseur de l'ombre"; //Res.Strings.Panel.Frame.Long.MarginWidth;
			this.document.Modifier.AdaptTextFieldRealAngle (this.fieldMarginWidth.TextFieldReal);
			this.fieldMarginWidth.TextFieldReal.InternalMaxValue = 90.0M;
			this.fieldMarginWidth.TextFieldReal.EditionAccepted += this.HandleFieldChanged;
			this.fieldMarginWidth.TabIndex = 3;
			this.fieldMarginWidth.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			//?ToolTip.Default.SetToolTip (this.fieldMarginWidth, Res.Strings.Panel.Frame.Tooltip.MarginWidth);

			this.fieldShadowSize = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldShadowSize.LabelShortText = "O"; //Res.Strings.Panel.Frame.Short.ShadowSize;
			this.fieldShadowSize.LabelLongText  = "Dimension de l'ombre"; //Res.Strings.Panel.Frame.Long.ShadowSize;
			this.document.Modifier.AdaptTextFieldRealAngle (this.fieldShadowSize.TextFieldReal);
			this.fieldShadowSize.TextFieldReal.InternalMaxValue = 90.0M;
			this.fieldShadowSize.TextFieldReal.EditionAccepted += this.HandleFieldChanged;
			this.fieldShadowSize.TabIndex = 4;
			this.fieldShadowSize.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			//?ToolTip.Default.SetToolTip (this.fieldShadowSize, Res.Strings.Panel.Frame.Tooltip.ShadowSize);

			this.isNormalAndExtended = true;
		}

		protected void AddRadioIcon(Properties.FrameType type)
		{
			this.grid.AddRadioIcon (Misc.Icon (Properties.Frame.GetIconText (type)), Properties.Frame.GetName (type), (int) type, false);
		}

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.grid.SelectionChanged -= HandleTypeChanged;
				this.fieldFrameWidth.TextFieldReal.EditionAccepted -= this.HandleFieldChanged;
				this.fieldMarginWidth.TextFieldReal.EditionAccepted -= this.HandleFieldChanged;
				this.fieldShadowSize.TextFieldReal.EditionAccepted -= this.HandleFieldChanged;

				this.grid = null;
				this.fieldFrameWidth = null;
				this.fieldMarginWidth = null;
				this.fieldShadowSize = null;
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

		protected override void PropertyToWidgets()
		{
			//	Propriété -> widgets.
			base.PropertyToWidgets();

			Properties.Frame p = this.property as Properties.Frame;
			if ( p == null )  return;

			this.ignoreChanged = true;

			this.grid.SelectedValue = (int) p.FrameType;
			this.fieldFrameWidth.TextFieldReal.InternalValue = (decimal) p.FrameWidth;
			this.fieldMarginWidth.TextFieldReal.InternalValue = (decimal) p.FrameWidth;
			this.fieldShadowSize.TextFieldReal.InternalValue = (decimal) p.ShadowSize;

			this.EnableWidgets();
			this.ignoreChanged = false;
		}

		protected override void WidgetsToProperty()
		{
			//	Widgets -> propriété.
			Properties.Frame p = this.property as Properties.Frame;
			if ( p == null )  return;

			p.FrameType = (Properties.FrameType) this.grid.SelectedValue;
			p.FrameWidth = (double) this.fieldFrameWidth.TextFieldReal.InternalValue;
			p.FrameWidth = (double) this.fieldMarginWidth.TextFieldReal.InternalValue;
			p.ShadowSize = (double) this.fieldShadowSize.TextFieldReal.InternalValue;
		}

		protected void EnableWidgets()
		{
			//	Grise les widgets nécessaires.
		}


		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.grid == null )  return;

			this.EnableWidgets();

			Rectangle rect = this.UsefulZone;

			Rectangle r = rect;
			r.Bottom = r.Top-20;
			r.Inflate(1);
			this.grid.SetManualBounds(r);

			if ( this.isExtendedSize && this.IsLabelProperties )
			{
				r.Top = rect.Top-25;
				r.Bottom = r.Top-20;
				r.Left = rect.Left;
				r.Right = rect.Right;
				this.fieldFrameWidth.SetManualBounds (r);

				r.Top = r.Bottom-5;
				r.Bottom = r.Top-20;
				r.Left = rect.Left;
				r.Right = rect.Right;
				this.fieldMarginWidth.SetManualBounds (r);

				r.Top = r.Bottom-5;
				r.Bottom = r.Top-20;
				r.Left = rect.Left;
				r.Right = rect.Right;
				this.fieldShadowSize.SetManualBounds (r);
			}
			else
			{
				r.Top = rect.Top-25;
				r.Bottom = r.Top-20;
				r.Left = rect.Left;
				r.Width = Widgets.TextFieldLabel.ShortWidth;
				this.fieldFrameWidth.SetManualBounds (r);

				r.Left = r.Right;
				r.Width = Widgets.TextFieldLabel.ShortWidth;
				this.fieldMarginWidth.SetManualBounds (r);

				r.Left = r.Right;
				r.Width = Widgets.TextFieldLabel.ShortWidth;
				this.fieldShadowSize.SetManualBounds (r);
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


		protected RadioIconGrid				grid;
		protected Widgets.TextFieldLabel	fieldFrameWidth;
		protected Widgets.TextFieldLabel	fieldMarginWidth;
		protected Widgets.TextFieldLabel	fieldShadowSize;
	}
}
