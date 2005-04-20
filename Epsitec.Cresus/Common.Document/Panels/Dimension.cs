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
			this.label = new StaticText(this);
			this.label.Alignment = ContentAlignment.MiddleLeft;

			this.buttons = new IconButton[3+4];
			for ( int i=0 ; i<3+4 ; i++ )
			{
				this.buttons[i] = new IconButton(this);
				this.buttons[i].Clicked += new MessageEventHandler(this.HandleButtonClicked);
				this.buttons[i].TabIndex = i;
				this.buttons[i].TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			}

			this.buttons[0].IconName = "manifest:Epsitec.App.DocumentEditor.Images.DimensionAuto.icon";
			this.buttons[1].IconName = "manifest:Epsitec.App.DocumentEditor.Images.DimensionInside.icon";
			this.buttons[2].IconName = "manifest:Epsitec.App.DocumentEditor.Images.DimensionOutside.icon";
			ToolTip.Default.SetToolTip(this.buttons[0], Res.Strings.Panel.Dimension.Tooltip.Auto);
			ToolTip.Default.SetToolTip(this.buttons[1], Res.Strings.Panel.Dimension.Tooltip.Inside);
			ToolTip.Default.SetToolTip(this.buttons[2], Res.Strings.Panel.Dimension.Tooltip.Outside);

			this.buttons[3].IconName = "manifest:Epsitec.App.DocumentEditor.Images.DimensionCenterOrLeft.icon";
			this.buttons[4].IconName = "manifest:Epsitec.App.DocumentEditor.Images.DimensionCenterOrRight.icon";
			this.buttons[5].IconName = "manifest:Epsitec.App.DocumentEditor.Images.DimensionLeft.icon";
			this.buttons[6].IconName = "manifest:Epsitec.App.DocumentEditor.Images.DimensionRight.icon";
			ToolTip.Default.SetToolTip(this.buttons[3], Res.Strings.Panel.Dimension.Tooltip.CenterOrLeft);
			ToolTip.Default.SetToolTip(this.buttons[4], Res.Strings.Panel.Dimension.Tooltip.CenterOrRight);
			ToolTip.Default.SetToolTip(this.buttons[5], Res.Strings.Panel.Dimension.Tooltip.Left);
			ToolTip.Default.SetToolTip(this.buttons[6], Res.Strings.Panel.Dimension.Tooltip.Right);

			this.addLength = new TextFieldReal(this);
			this.addLength.FactorMinRange = 0.0M;
			this.addLength.FactorMaxRange = 0.1M;
			this.addLength.FactorStep = 1.0M;
			this.document.Modifier.AdaptTextFieldRealDimension(this.addLength);
			this.addLength.ValueChanged += new EventHandler(this.HandleFieldChanged);
			this.addLength.TabIndex = 10;
			this.addLength.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.addLength, Res.Strings.Panel.Dimension.Tooltip.AddLength);

			this.outLength = new TextFieldReal(this);
			this.outLength.FactorMinRange = 0.0M;
			this.outLength.FactorMaxRange = 0.1M;
			this.outLength.FactorStep = 1.0M;
			this.document.Modifier.AdaptTextFieldRealDimension(this.outLength);
			this.outLength.ValueChanged += new EventHandler(this.HandleFieldChanged);
			this.outLength.TabIndex = 11;
			this.outLength.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.outLength, Res.Strings.Panel.Dimension.Tooltip.OutLength);

			this.rotateText = new IconButton(this);
			this.rotateText.Clicked += new MessageEventHandler(this.HandleRotateTextClicked);
			this.rotateText.TabIndex = 12;
			this.rotateText.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.rotateText.IconName = "manifest:Epsitec.App.DocumentEditor.Images.DimensionRotateText.icon";
			ToolTip.Default.SetToolTip(this.rotateText, Res.Strings.Panel.Dimension.Tooltip.RotateText);

			this.fontOffset = new TextFieldReal(this);
			this.document.Modifier.AdaptTextFieldRealScalar(this.fontOffset);
			this.fontOffset.InternalMinValue = -100;
			this.fontOffset.InternalMaxValue =  100;
			this.fontOffset.Step = 5;
			this.fontOffset.TextSuffix = "%";
			this.fontOffset.ValueChanged += new EventHandler(this.HandleFieldChanged);
			this.fontOffset.TabIndex = 13;
			this.fontOffset.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fontOffset, Res.Strings.Panel.Dimension.Tooltip.FontOffset);

			this.prefix = new TextField(this);
			this.prefix.TextChanged += new EventHandler(this.HandleFieldChanged);
			this.prefix.TabIndex = 14;
			this.prefix.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.prefix, Res.Strings.Panel.Dimension.Tooltip.Prefix);

			this.postfix = new TextField(this);
			this.postfix.TextChanged += new EventHandler(this.HandleFieldChanged);
			this.postfix.TabIndex = 15;
			this.postfix.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.postfix, Res.Strings.Panel.Dimension.Tooltip.Postfix);

			this.labelAddLength = new StaticText(this);
			this.labelAddLength.Text = Res.Strings.Panel.Dimension.Label.AddLength;
			this.labelAddLength.Alignment = ContentAlignment.MiddleCenter;

			this.labelOutLength = new StaticText(this);
			this.labelOutLength.Text = Res.Strings.Panel.Dimension.Label.OutLength;
			this.labelOutLength.Alignment = ContentAlignment.MiddleCenter;

			this.labelFontOffset = new StaticText(this);
			this.labelFontOffset.Text = Res.Strings.Panel.Dimension.Label.FontOffset;
			this.labelFontOffset.Alignment = ContentAlignment.MiddleCenter;

			this.labelPrefix = new StaticText(this);
			this.labelPrefix.Text = Res.Strings.Panel.Dimension.Label.Prefix;
			this.labelPrefix.Alignment = ContentAlignment.MiddleCenter;

			this.labelPostfix = new StaticText(this);
			this.labelPostfix.Text = Res.Strings.Panel.Dimension.Label.Postfix;
			this.labelPostfix.Alignment = ContentAlignment.MiddleCenter;

			this.isNormalAndExtended = true;
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				for ( int i=0 ; i<3+4 ; i++ )
				{
					this.buttons[i].Clicked -= new MessageEventHandler(this.HandleButtonClicked);
					this.buttons[i] = null;
				}

				this.addLength.ValueChanged -= new EventHandler(this.HandleFieldChanged);
				this.outLength.ValueChanged -= new EventHandler(this.HandleFieldChanged);
				this.fontOffset.ValueChanged -= new EventHandler(this.HandleFieldChanged);
				this.prefix.TextChanged -= new EventHandler(this.HandleFieldChanged);
				this.postfix.TextChanged -= new EventHandler(this.HandleFieldChanged);
				this.rotateText.Clicked -= new MessageEventHandler(this.HandleRotateTextClicked);

				this.label = null;
				this.addLength = null;
				this.outLength = null;
				this.fontOffset = null;
				this.prefix = null;
				this.postfix = null;
				this.rotateText = null;
				this.labelAddLength = null;
				this.labelOutLength = null;
				this.labelFontOffset = null;
				this.labelPrefix = null;
				this.labelPostfix = null;
			}
			
			base.Dispose(disposing);
		}

		
		// Retourne la hauteur standard.
		public override double DefaultHeight
		{
			get
			{
				return ( this.isExtendedSize ? 80 : 30 );
			}
		}

		// Propriété -> widgets.
		protected override void PropertyToWidgets()
		{
			base.PropertyToWidgets();

			Properties.Dimension p = this.property as Properties.Dimension;
			if ( p == null )  return;

			this.ignoreChanged = true;

			this.label.Text = p.TextStyle;

			int sel = -1;
			if ( p.DimensionForm == Properties.DimensionForm.Auto    )  sel = 0;
			if ( p.DimensionForm == Properties.DimensionForm.Inside  )  sel = 1;
			if ( p.DimensionForm == Properties.DimensionForm.Outside )  sel = 2;
			this.SelectDimensionForm = sel;

			sel = -1;
			if ( p.DimensionJustif == Properties.DimensionJustif.CenterOrLeft  )  sel = 0;
			if ( p.DimensionJustif == Properties.DimensionJustif.CenterOrRight )  sel = 1;
			if ( p.DimensionJustif == Properties.DimensionJustif.Left          )  sel = 2;
			if ( p.DimensionJustif == Properties.DimensionJustif.Right         )  sel = 3;
			this.SelectDimensionJustif = sel;

			this.addLength.InternalValue = (decimal) p.AddLength;
			this.outLength.InternalValue = (decimal) p.OutLength;
			this.fontOffset.InternalValue = (decimal) p.FontOffset*100;
			this.prefix.Text = p.Prefix;
			this.postfix.Text = p.Postfix;

			this.rotateText.ActiveState = p.RotateText ? WidgetState.ActiveYes : WidgetState.ActiveNo;

			this.EnableWidgets();
			this.ignoreChanged = false;
		}

		// Widgets -> propriété.
		protected override void WidgetsToProperty()
		{
			Properties.Dimension p = this.property as Properties.Dimension;
			if ( p == null )  return;

			int sel = this.SelectDimensionForm;
			if ( sel == 0 )  p.DimensionForm = Properties.DimensionForm.Auto;
			if ( sel == 1 )  p.DimensionForm = Properties.DimensionForm.Inside;
			if ( sel == 2 )  p.DimensionForm = Properties.DimensionForm.Outside;

			sel = this.SelectDimensionJustif;
			if ( sel == 0 )  p.DimensionJustif = Properties.DimensionJustif.CenterOrLeft;
			if ( sel == 1 )  p.DimensionJustif = Properties.DimensionJustif.CenterOrRight;
			if ( sel == 2 )  p.DimensionJustif = Properties.DimensionJustif.Left;
			if ( sel == 3 )  p.DimensionJustif = Properties.DimensionJustif.Right;

			p.AddLength = (double) this.addLength.InternalValue;
			p.OutLength = (double) this.outLength.InternalValue;
			p.FontOffset = (double) this.fontOffset.InternalValue/100;
			p.Prefix = this.prefix.Text;
			p.Postfix = this.postfix.Text;

			p.RotateText = (this.rotateText.ActiveState == WidgetState.ActiveYes);
		}


		protected int SelectDimensionForm
		{
			get
			{
				for ( int i=0 ; i<3 ; i++ )
				{
					if ( this.buttons[i].ActiveState == WidgetState.ActiveYes )  return i;
				}
				return -1;
			}

			set
			{
				for ( int i=0 ; i<3 ; i++ )
				{
					this.buttons[i].ActiveState = (i==value) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
				}
			}
		}
		
		protected int SelectDimensionJustif
		{
			get
			{
				for ( int i=0 ; i<4 ; i++ )
				{
					if ( this.buttons[3+i].ActiveState == WidgetState.ActiveYes )  return i;
				}
				return -1;
			}

			set
			{
				for ( int i=0 ; i<4 ; i++ )
				{
					this.buttons[3+i].ActiveState = (i==value) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
				}
			}
		}

		
		// Grise les widgets nécessaires.
		protected void EnableWidgets()
		{
			this.addLength.SetVisible(this.isExtendedSize);
			this.outLength.SetVisible(this.isExtendedSize);
			this.fontOffset.SetVisible(this.isExtendedSize);
			this.prefix.SetVisible(this.isExtendedSize);
			this.postfix.SetVisible(this.isExtendedSize);
			this.labelAddLength.SetVisible(this.isExtendedSize);
			this.labelOutLength.SetVisible(this.isExtendedSize);
			this.labelFontOffset.SetVisible(this.isExtendedSize);
			this.labelPrefix.SetVisible(this.isExtendedSize);
			this.labelPostfix.SetVisible(this.isExtendedSize);
		}

		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.buttons == null )  return;

			this.EnableWidgets();

			Rectangle rect = this.Client.Bounds;
			rect.Deflate(this.extendedZoneWidth, 0);
			rect.Deflate(5);

			Rectangle r = rect;
			r.Bottom = r.Top-20;
			r.Right = rect.Right-40;
			this.label.Bounds = r;

			r.Left = rect.Right-(20*(3+4)+5);
			r.Width = 20;
			for ( int i=0 ; i<3+4 ; i++ )
			{
				this.buttons[i].Bounds = r;
				r.Offset(20+((i==2)?5:0), 0);
			}

			r.Offset(0, -25);
			r.Left = rect.Left;
			r.Width = 12;
			this.labelAddLength.Bounds = r;
			r.Left = r.Right;
			r.Width = 44;
			this.addLength.Bounds = r;
			r.Left = r.Right;
			r.Width = 12;
			this.labelOutLength.Bounds = r;
			r.Left = r.Right;
			r.Width = 45;
			this.outLength.Bounds = r;
			r.Left = r.Right+12;
			r.Width = 20;
			this.rotateText.Bounds = r;

			r.Offset(0, -25);
			r.Left = rect.Left;
			r.Width = 12;
			this.labelFontOffset.Bounds = r;
			r.Left = r.Right;
			r.Width = 44;
			this.fontOffset.Bounds = r;
			r.Left = r.Right;
			r.Width = 12;
			this.labelPrefix.Bounds = r;
			r.Left = r.Right;
			r.Width = 45;
			this.prefix.Bounds = r;
			r.Left = r.Right;
			r.Width = 12;
			this.labelPostfix.Bounds = r;
			r.Left = r.Right;
			r.Width = 50;
			this.postfix.Bounds = r;
		}
		
		// Un bouton a été cliqué.
		private void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			IconButton button = sender as IconButton;

			if ( button == this.buttons[0] )  this.SelectDimensionForm = 0;
			if ( button == this.buttons[1] )  this.SelectDimensionForm = 1;
			if ( button == this.buttons[2] )  this.SelectDimensionForm = 2;

			if ( button == this.buttons[3] )  this.SelectDimensionJustif = 0;
			if ( button == this.buttons[4] )  this.SelectDimensionJustif = 1;
			if ( button == this.buttons[5] )  this.SelectDimensionJustif = 2;
			if ( button == this.buttons[6] )  this.SelectDimensionJustif = 3;

			this.OnChanged();
		}

		// Un bouton a été cliqué.
		private void HandleRotateTextClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;

			IconButton button = sender as IconButton;
			button.ActiveState = (button.ActiveState==WidgetState.ActiveNo) ? WidgetState.ActiveYes : WidgetState.ActiveNo;

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


		protected StaticText				label;
		protected IconButton[]				buttons;
		protected TextFieldReal				addLength;
		protected TextFieldReal				outLength;
		protected TextFieldReal				fontOffset;
		protected TextField					prefix;
		protected TextField					postfix;
		protected IconButton				rotateText;
		protected StaticText				labelAddLength;
		protected StaticText				labelOutLength;
		protected StaticText				labelFontOffset;
		protected StaticText				labelPrefix;
		protected StaticText				labelPostfix;
	}
}
