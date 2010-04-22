using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe Dimension permet de choisir un type de cotation.
	/// </summary>
	public class Dimension : Abstract
	{
		public Dimension(Document document) : base(document)
		{
			this.gridForm = new RadioIconGrid(this);
			this.gridForm.SelectionChanged += HandleTypeChanged;
			this.gridForm.TabIndex = 0;
			this.gridForm.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.AddRadioIcon(Properties.DimensionForm.Auto);
			this.AddRadioIcon(Properties.DimensionForm.Inside);
			this.AddRadioIcon(Properties.DimensionForm.Outside);

			this.gridJustif = new RadioIconGrid(this);
			this.gridJustif.SelectionChanged += HandleTypeChanged;
			this.gridJustif.TabIndex = 1;
			this.gridJustif.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.AddRadioIcon(Properties.DimensionJustif.CenterOrLeft);
			this.AddRadioIcon(Properties.DimensionJustif.CenterOrRight);
			this.AddRadioIcon(Properties.DimensionJustif.Left);
			this.AddRadioIcon(Properties.DimensionJustif.Right);

			this.addLength = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.addLength.LabelShortText = Res.Strings.Panel.Dimension.Short.AddLength;
			this.addLength.LabelLongText  = Res.Strings.Panel.Dimension.Long.AddLength;
			this.addLength.TextFieldReal.FactorMinRange = 0.0M;
			this.addLength.TextFieldReal.FactorMaxRange = 0.1M;
			this.addLength.TextFieldReal.FactorStep = 1.0M;
			this.document.Modifier.AdaptTextFieldRealDimension(this.addLength.TextFieldReal);
			this.addLength.TextFieldReal.EditionAccepted += this.HandleFieldChanged;
			this.addLength.TabIndex = 10;
			this.addLength.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.addLength, Res.Strings.Panel.Dimension.Tooltip.AddLength);

			this.outLength = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.outLength.LabelShortText = Res.Strings.Panel.Dimension.Short.OutLength;
			this.outLength.LabelLongText  = Res.Strings.Panel.Dimension.Long.OutLength;
			this.outLength.TextFieldReal.FactorMinRange = 0.0M;
			this.outLength.TextFieldReal.FactorMaxRange = 0.1M;
			this.outLength.TextFieldReal.FactorStep = 1.0M;
			this.document.Modifier.AdaptTextFieldRealDimension(this.outLength.TextFieldReal);
			this.outLength.TextFieldReal.EditionAccepted += this.HandleFieldChanged;
			this.outLength.TabIndex = 11;
			this.outLength.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.outLength, Res.Strings.Panel.Dimension.Tooltip.OutLength);

			this.rotateText = new IconButton(this);
			this.rotateText.Clicked += this.HandleRotateTextClicked;
			this.rotateText.TabIndex = 12;
			this.rotateText.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.rotateText.IconUri = Misc.Icon("DimensionRotateText");
			this.rotateText.ButtonStyle = ButtonStyle.ActivableIcon;  // comme Statefull
			ToolTip.Default.SetToolTip(this.rotateText, Res.Strings.Panel.Dimension.Tooltip.RotateText);

			this.fontOffset = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fontOffset.LabelShortText = Res.Strings.Panel.Dimension.Short.FontOffset;
			this.fontOffset.LabelLongText  = Res.Strings.Panel.Dimension.Long.FontOffset;
			this.document.Modifier.AdaptTextFieldRealScalar(this.fontOffset.TextFieldReal);
			this.fontOffset.TextFieldReal.InternalMinValue = -100;
			this.fontOffset.TextFieldReal.InternalMaxValue =  100;
			this.fontOffset.TextFieldReal.Step = 5;
			this.fontOffset.TextFieldReal.TextSuffix = "%";
			this.fontOffset.TextFieldReal.EditionAccepted += this.HandleFieldChanged;
			this.fontOffset.TabIndex = 13;
			this.fontOffset.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fontOffset, Res.Strings.Panel.Dimension.Tooltip.FontOffset);

			this.dimensionText = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextField);
			this.dimensionText.LabelShortText = Res.Strings.Panel.Dimension.Short.Text;
			this.dimensionText.LabelLongText  = Res.Strings.Panel.Dimension.Long.Text;
			//?this.dimensionText.TextField.EditionAccepted += this.HandleFieldChanged;  // TODO: événement plus généré. Pourquoi ?
			this.dimensionText.TextField.TextChanged += this.HandleFieldChanged;
			this.dimensionText.TabIndex = 14;
			this.dimensionText.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.dimensionText, Res.Strings.Panel.Dimension.Tooltip.Text);

			this.isNormalAndExtended = true;
		}
		
		protected void AddRadioIcon(Properties.DimensionForm type)
		{
			this.gridForm.AddRadioIcon(Misc.Icon(Properties.Dimension.GetIconText(type)), Properties.Dimension.GetName(type), (int)type, false);
		}

		protected void AddRadioIcon(Properties.DimensionJustif type)
		{
			this.gridJustif.AddRadioIcon(Misc.Icon(Properties.Dimension.GetIconText(type)), Properties.Dimension.GetName(type), (int)type, false);
		}

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.gridForm.SelectionChanged -= HandleTypeChanged;
				this.gridJustif.SelectionChanged -= HandleTypeChanged;
				this.addLength.TextFieldReal.EditionAccepted -= this.HandleFieldChanged;
				this.outLength.TextFieldReal.EditionAccepted -= this.HandleFieldChanged;
				this.fontOffset.TextFieldReal.EditionAccepted -= this.HandleFieldChanged;
				this.dimensionText.TextField.EditionAccepted -= this.HandleFieldChanged;
				this.rotateText.Clicked -= this.HandleRotateTextClicked;

				this.gridForm = null;
				this.gridJustif = null;
				this.addLength = null;
				this.outLength = null;
				this.fontOffset = null;
				this.dimensionText = null;
				this.rotateText = null;
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
						h += 130;
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

			Properties.Dimension p = this.property as Properties.Dimension;
			if ( p == null )  return;

			this.ignoreChanged = true;

			this.gridForm.SelectedValue   = (int) p.DimensionForm;
			this.gridJustif.SelectedValue = (int) p.DimensionJustif;

			this.addLength.TextFieldReal.InternalValue = (decimal) p.AddLength;
			this.outLength.TextFieldReal.InternalValue = (decimal) p.OutLength;
			this.fontOffset.TextFieldReal.InternalValue = (decimal) p.FontOffset*100;
			this.dimensionText.TextField.Text = p.DimensionText;

			this.rotateText.ActiveState = p.RotateText ? ActiveState.Yes : ActiveState.No;

			this.EnableWidgets();
			this.ignoreChanged = false;
		}

		protected override void WidgetsToProperty()
		{
			//	Widgets -> propriété.
			Properties.Dimension p = this.property as Properties.Dimension;
			if ( p == null )  return;

			p.DimensionForm   = (Properties.DimensionForm)   this.gridForm.SelectedValue;
			p.DimensionJustif = (Properties.DimensionJustif) this.gridJustif.SelectedValue;

			p.AddLength = (double) this.addLength.TextFieldReal.InternalValue;
			p.OutLength = (double) this.outLength.TextFieldReal.InternalValue;
			p.FontOffset = (double) this.fontOffset.TextFieldReal.InternalValue/100;
			p.DimensionText = this.dimensionText.TextField.Text;

			p.RotateText = (this.rotateText.ActiveState == ActiveState.Yes);
		}


		protected void EnableWidgets()
		{
			//	Grise les widgets nécessaires.
			this.addLength.Visibility = (this.isExtendedSize);
			this.outLength.Visibility = (this.isExtendedSize);
			this.fontOffset.Visibility = (this.isExtendedSize);
			this.dimensionText.Visibility = (this.isExtendedSize);
		}

		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.gridForm == null )  return;

			this.EnableWidgets();

			Rectangle rect = this.UsefulZone;

			Rectangle r = rect;
			r.Width = 22*3;
			r.Inflate(1);
			this.gridForm.SetManualBounds(r);

			r = rect;
			r.Bottom = r.Top-20;
			r.Left = r.Right-22*4;
			r.Width = 22*4;
			r.Inflate(1);
			this.gridJustif.SetManualBounds(r);

			if ( this.isExtendedSize && this.IsLabelProperties )
			{
				r = rect;
				r.Bottom = r.Top-20;
				r.Offset(0, -25);
				r.Left = rect.Left;
				r.Right = rect.Right;
				this.addLength.SetManualBounds(r);

				r.Offset(0, -25);
				this.outLength.SetManualBounds(r);
				
				r.Offset(0, -25);
				this.fontOffset.SetManualBounds(r);
				
				r.Offset(0, -25);
				r.Left = rect.Left;
				r.Width = 20;
				this.rotateText.SetManualBounds(r);
				r.Left = r.Right;
				r.Right = rect.Right;
				this.dimensionText.SetManualBounds(r);
			}
			else
			{
				r = rect;
				r.Bottom = r.Top-20;
				r.Offset(0, -25);
				r.Left = rect.Left;
				r.Width = Widgets.TextFieldLabel.ShortWidth;
				this.addLength.SetManualBounds(r);
				r.Left = r.Right;
				r.Width = Widgets.TextFieldLabel.ShortWidth;
				this.outLength.SetManualBounds(r);
				r.Left = r.Right+Widgets.TextFieldLabel.DefaultLabelWidth+Widgets.TextFieldLabel.DefaultMarginWidth;
				r.Width = 20;
				this.rotateText.SetManualBounds(r);

				r.Offset(0, -25);
				r.Left = rect.Left;
				r.Width = Widgets.TextFieldLabel.ShortWidth;
				this.fontOffset.SetManualBounds(r);
				r.Left = r.Right;
				r.Width = Widgets.TextFieldLabel.ShortWidth;
				this.dimensionText.SetManualBounds(r);
			}
		}
		
		private void HandleTypeChanged(object sender)
		{
			//	Un bouton a été cliqué.
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}

		private void HandleRotateTextClicked(object sender, MessageEventArgs e)
		{
			//	Un bouton a été cliqué.
			if ( this.ignoreChanged )  return;

			IconButton button = sender as IconButton;
			button.ActiveState = (button.ActiveState==ActiveState.No) ? ActiveState.Yes : ActiveState.No;

			this.OnChanged();
		}

		private void HandleFieldChanged(object sender)
		{
			//	Un champ a été changé.
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}

		private void HandleCheckChanged(object sender)
		{
			//	Un champ a été changé.
			if ( this.ignoreChanged )  return;
			this.EnableWidgets();
			this.OnChanged();
		}


		protected RadioIconGrid				gridForm;
		protected RadioIconGrid				gridJustif;
		protected Widgets.TextFieldLabel	addLength;
		protected Widgets.TextFieldLabel	outLength;
		protected Widgets.TextFieldLabel	fontOffset;
		protected Widgets.TextFieldLabel	dimensionText;
		protected IconButton				rotateText;
	}
}
