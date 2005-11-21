using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe Dimension permet de choisir un type de cotation.
	/// </summary>
	[SuppressBundleSupport]
	public class Dimension : Abstract
	{
		public Dimension(Document document) : base(document)
		{
			this.gridForm = new Widgets.RadioIconGrid(this);
			this.gridForm.SelectionChanged += new EventHandler(HandleTypeChanged);
			this.gridForm.TabIndex = 0;
			this.gridForm.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.AddRadioIcon(Properties.DimensionForm.Auto);
			this.AddRadioIcon(Properties.DimensionForm.Inside);
			this.AddRadioIcon(Properties.DimensionForm.Outside);

			this.gridJustif = new Widgets.RadioIconGrid(this);
			this.gridJustif.SelectionChanged += new EventHandler(HandleTypeChanged);
			this.gridJustif.TabIndex = 1;
			this.gridJustif.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.AddRadioIcon(Properties.DimensionJustif.CenterOrLeft);
			this.AddRadioIcon(Properties.DimensionJustif.CenterOrRight);
			this.AddRadioIcon(Properties.DimensionJustif.Left);
			this.AddRadioIcon(Properties.DimensionJustif.Right);

			this.addLength = new Widgets.TextFieldLabel(this, false);
			this.addLength.LabelShortText = Res.Strings.Panel.Dimension.Short.AddLength;
			this.addLength.LabelLongText  = Res.Strings.Panel.Dimension.Long.AddLength;
			this.addLength.TextFieldReal.FactorMinRange = 0.0M;
			this.addLength.TextFieldReal.FactorMaxRange = 0.1M;
			this.addLength.TextFieldReal.FactorStep = 1.0M;
			this.document.Modifier.AdaptTextFieldRealDimension(this.addLength.TextFieldReal);
			this.addLength.TextFieldReal.ValueChanged += new EventHandler(this.HandleFieldChanged);
			this.addLength.TabIndex = 10;
			this.addLength.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.addLength, Res.Strings.Panel.Dimension.Tooltip.AddLength);

			this.outLength = new Widgets.TextFieldLabel(this, false);
			this.outLength.LabelShortText = Res.Strings.Panel.Dimension.Short.OutLength;
			this.outLength.LabelLongText  = Res.Strings.Panel.Dimension.Long.OutLength;
			this.outLength.TextFieldReal.FactorMinRange = 0.0M;
			this.outLength.TextFieldReal.FactorMaxRange = 0.1M;
			this.outLength.TextFieldReal.FactorStep = 1.0M;
			this.document.Modifier.AdaptTextFieldRealDimension(this.outLength.TextFieldReal);
			this.outLength.TextFieldReal.ValueChanged += new EventHandler(this.HandleFieldChanged);
			this.outLength.TabIndex = 11;
			this.outLength.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.outLength, Res.Strings.Panel.Dimension.Tooltip.OutLength);

			this.rotateText = new IconButton(this);
			this.rotateText.Clicked += new MessageEventHandler(this.HandleRotateTextClicked);
			this.rotateText.TabIndex = 12;
			this.rotateText.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.rotateText.IconName = Misc.Icon("DimensionRotateText");
			ToolTip.Default.SetToolTip(this.rotateText, Res.Strings.Panel.Dimension.Tooltip.RotateText);

			this.fontOffset = new Widgets.TextFieldLabel(this, false);
			this.fontOffset.LabelShortText = Res.Strings.Panel.Dimension.Short.FontOffset;
			this.fontOffset.LabelLongText  = Res.Strings.Panel.Dimension.Long.FontOffset;
			this.document.Modifier.AdaptTextFieldRealScalar(this.fontOffset.TextFieldReal);
			this.fontOffset.TextFieldReal.InternalMinValue = -100;
			this.fontOffset.TextFieldReal.InternalMaxValue =  100;
			this.fontOffset.TextFieldReal.Step = 5;
			this.fontOffset.TextFieldReal.TextSuffix = "%";
			this.fontOffset.TextFieldReal.ValueChanged += new EventHandler(this.HandleFieldChanged);
			this.fontOffset.TabIndex = 13;
			this.fontOffset.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fontOffset, Res.Strings.Panel.Dimension.Tooltip.FontOffset);

			this.dimensionText = new Widgets.TextFieldLabel(this, true);
			this.dimensionText.LabelShortText = Res.Strings.Panel.Dimension.Short.Text;
			this.dimensionText.LabelLongText  = Res.Strings.Panel.Dimension.Long.Text;
			this.dimensionText.TextField.TextChanged += new EventHandler(this.HandleFieldChanged);
			this.dimensionText.TabIndex = 14;
			this.dimensionText.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.dimensionText, Res.Strings.Panel.Dimension.Tooltip.Text);

			this.isNormalAndExtended = true;
		}
		
		protected void AddRadioIcon(Properties.DimensionForm type)
		{
			this.gridForm.AddRadioIcon(Properties.Dimension.GetIconText(type), Properties.Dimension.GetName(type), (int)type, false);
		}

		protected void AddRadioIcon(Properties.DimensionJustif type)
		{
			this.gridJustif.AddRadioIcon(Properties.Dimension.GetIconText(type), Properties.Dimension.GetName(type), (int)type, false);
		}

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.gridForm.SelectionChanged -= new EventHandler(HandleTypeChanged);
				this.gridJustif.SelectionChanged -= new EventHandler(HandleTypeChanged);
				this.addLength.TextFieldReal.ValueChanged -= new EventHandler(this.HandleFieldChanged);
				this.outLength.TextFieldReal.ValueChanged -= new EventHandler(this.HandleFieldChanged);
				this.fontOffset.TextFieldReal.ValueChanged -= new EventHandler(this.HandleFieldChanged);
				this.dimensionText.TextField.TextChanged -= new EventHandler(this.HandleFieldChanged);
				this.rotateText.Clicked -= new MessageEventHandler(this.HandleRotateTextClicked);

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

		// Propriété -> widgets.
		protected override void PropertyToWidgets()
		{
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

		// Widgets -> propriété.
		protected override void WidgetsToProperty()
		{
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


		// Grise les widgets nécessaires.
		protected void EnableWidgets()
		{
			this.addLength.SetVisible(this.isExtendedSize);
			this.outLength.SetVisible(this.isExtendedSize);
			this.fontOffset.SetVisible(this.isExtendedSize);
			this.dimensionText.SetVisible(this.isExtendedSize);
		}

		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.gridForm == null )  return;

			this.EnableWidgets();

			Rectangle rect = this.UsefulZone;

			Rectangle r = rect;
			r.Width = 22*3;
			r.Inflate(1);
			this.gridForm.Bounds = r;

			r = rect;
			r.Bottom = r.Top-20;
			r.Left = r.Right-22*4;
			r.Width = 22*4;
			r.Inflate(1);
			this.gridJustif.Bounds = r;

			if ( this.isExtendedSize && this.IsLabelProperties )
			{
				r = rect;
				r.Bottom = r.Top-20;
				r.Offset(0, -25);
				r.Left = rect.Left;
				r.Right = rect.Right;
				this.addLength.Bounds = r;

				r.Offset(0, -25);
				this.outLength.Bounds = r;
				
				r.Offset(0, -25);
				this.fontOffset.Bounds = r;
				
				r.Offset(0, -25);
				r.Left = rect.Left;
				r.Width = 20;
				this.rotateText.Bounds = r;
				r.Left = r.Right;
				r.Right = rect.Right;
				this.dimensionText.Bounds = r;
			}
			else
			{
				r = rect;
				r.Bottom = r.Top-20;
				r.Offset(0, -25);
				r.Left = rect.Left;
				r.Width = Widgets.TextFieldLabel.ShortWidth;
				this.addLength.Bounds = r;
				r.Left = r.Right;
				r.Width = Widgets.TextFieldLabel.ShortWidth;
				this.outLength.Bounds = r;
				r.Left = r.Right+Widgets.TextFieldLabel.DefaultLabelWidth+Widgets.TextFieldLabel.DefaultMarginWidth;
				r.Width = 20;
				this.rotateText.Bounds = r;

				r.Offset(0, -25);
				r.Left = rect.Left;
				r.Width = Widgets.TextFieldLabel.ShortWidth;
				this.fontOffset.Bounds = r;
				r.Left = r.Right;
				r.Width = Widgets.TextFieldLabel.ShortWidth;
				this.dimensionText.Bounds = r;
			}
		}
		
		// Un bouton a été cliqué.
		private void HandleTypeChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}

		// Un bouton a été cliqué.
		private void HandleRotateTextClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;

			IconButton button = sender as IconButton;
			button.ActiveState = (button.ActiveState==ActiveState.No) ? ActiveState.Yes : ActiveState.No;

			this.OnChanged();
		}

		// Un champ a été changé.
		private void HandleFieldChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}

		// Un champ a été changé.
		private void HandleCheckChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.EnableWidgets();
			this.OnChanged();
		}


		protected Widgets.RadioIconGrid		gridForm;
		protected Widgets.RadioIconGrid		gridJustif;
		protected Widgets.TextFieldLabel	addLength;
		protected Widgets.TextFieldLabel	outLength;
		protected Widgets.TextFieldLabel	fontOffset;
		protected Widgets.TextFieldLabel	dimensionText;
		protected IconButton				rotateText;
	}
}
