using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe TextLine permet de choisir un mode de justification.
	/// </summary>
	[SuppressBundleSupport]
	public class TextLine : Abstract
	{
		public TextLine(Document document) : base(document)
		{
			this.label = new StaticText(this);
			this.label.Alignment = ContentAlignment.MiddleLeft;

			this.buttons = new IconButton[4];
			for ( int i=0 ; i<4 ; i++ )
			{
				this.buttons[i] = new IconButton(this);
				this.buttons[i].Clicked += new MessageEventHandler(this.HandlePanelTextLineClicked);
				this.buttons[i].TabIndex = 2+i;
				this.buttons[i].TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			}

			this.buttons[0].IconName = "manifest:Epsitec.App.DocumentEditor.Images.JustifHLeft.icon";
			this.buttons[1].IconName = "manifest:Epsitec.App.DocumentEditor.Images.JustifHCenter.icon";
			this.buttons[2].IconName = "manifest:Epsitec.App.DocumentEditor.Images.JustifHRight.icon";
			this.buttons[3].IconName = "manifest:Epsitec.App.DocumentEditor.Images.JustifHStretch.icon";
			ToolTip.Default.SetToolTip(this.buttons[0], Res.Strings.Panel.TextLine.Tooltip.JustifHLeft);
			ToolTip.Default.SetToolTip(this.buttons[1], Res.Strings.Panel.TextLine.Tooltip.JustifHCenter);
			ToolTip.Default.SetToolTip(this.buttons[2], Res.Strings.Panel.TextLine.Tooltip.JustifHRight);
			ToolTip.Default.SetToolTip(this.buttons[3], Res.Strings.Panel.TextLine.Tooltip.JustifHStretch);

			this.fieldOffset = new TextFieldReal(this);
			this.document.Modifier.AdaptTextFieldRealScalar(this.fieldOffset);
			this.fieldOffset.InternalMinValue =  0.0M;
			this.fieldOffset.InternalMaxValue = 70.0M;
			this.fieldOffset.Step = 5.0M;
			this.fieldOffset.TextSuffix = "%";
			this.fieldOffset.ValueChanged += new EventHandler(this.HandleFieldChanged);
			this.fieldOffset.TabIndex = 20;
			this.fieldOffset.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldOffset, Res.Strings.Panel.TextLine.Tooltip.Offset);

			this.fieldAdd = new TextFieldReal(this);
			this.document.Modifier.AdaptTextFieldRealScalar(this.fieldAdd);
			this.fieldAdd.InternalMinValue = -20.0M;
			this.fieldAdd.InternalMaxValue = 100.0M;
			this.fieldAdd.Step = 1.0M;
			this.fieldAdd.TextSuffix = "%";
			this.fieldAdd.ValueChanged += new EventHandler(this.HandleFieldChanged);
			this.fieldAdd.TabIndex = 21;
			this.fieldAdd.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldAdd, Res.Strings.Panel.TextLine.Tooltip.Add);

			this.labelOffset = new StaticText(this);
			this.labelOffset.Text = Res.Strings.Panel.TextLine.Label.Offset;
			this.labelOffset.Alignment = ContentAlignment.MiddleCenter;

			this.labelAdd = new StaticText(this);
			this.labelAdd.Text = Res.Strings.Panel.TextLine.Label.Add;
			this.labelAdd.Alignment = ContentAlignment.MiddleCenter;

			this.isNormalAndExtended = true;
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				for ( int i=0 ; i<4 ; i++ )
				{
					this.buttons[i].Clicked -= new MessageEventHandler(this.HandlePanelTextLineClicked);
					this.buttons[i] = null;
				}
				this.fieldOffset.ValueChanged -= new EventHandler(this.HandleFieldChanged);
				this.fieldAdd.ValueChanged -= new EventHandler(this.HandleFieldChanged);

				this.label = null;
				this.fieldOffset = null;
				this.fieldAdd = null;
				this.labelOffset = null;
				this.labelAdd = null;
			}
			
			base.Dispose(disposing);
		}

		
		// Retourne la hauteur standard.
		public override double DefaultHeight
		{
			get
			{
				return ( this.isExtendedSize ? 55 : 30 );
			}
		}

		// Propriété -> widgets.
		protected override void PropertyToWidgets()
		{
			base.PropertyToWidgets();

			Properties.TextLine p = this.property as Properties.TextLine;
			if ( p == null )  return;

			this.ignoreChanged = true;

			this.label.Text = p.TextStyle;

			this.SelectButtonHorizontal    = p.Horizontal;
			this.fieldOffset.InternalValue = (decimal) p.Offset*100;
			this.fieldAdd.InternalValue    = (decimal) p.Add*100;

			this.EnableWidgets();
			this.ignoreChanged = false;
		}

		// Widgets -> propriété.
		protected override void WidgetsToProperty()
		{
			Properties.TextLine p = this.property as Properties.TextLine;
			if ( p == null )  return;

			p.Horizontal = this.SelectButtonHorizontal;
			p.Offset     = (double) this.fieldOffset.InternalValue/100;
			p.Add        = (double) this.fieldAdd.InternalValue/100;
		}

		protected Properties.JustifHorizontal SelectButtonHorizontal
		{
			get
			{
				if ( this.ButtonActive(0) )  return Properties.JustifHorizontal.Left;
				if ( this.ButtonActive(1) )  return Properties.JustifHorizontal.Center;
				if ( this.ButtonActive(2) )  return Properties.JustifHorizontal.Right;
				if ( this.ButtonActive(3) )  return Properties.JustifHorizontal.Stretch;
				return Properties.JustifHorizontal.None;
			}

			set
			{
				this.ButtonActive(0, value == Properties.JustifHorizontal.Left);
				this.ButtonActive(1, value == Properties.JustifHorizontal.Center);
				this.ButtonActive(2, value == Properties.JustifHorizontal.Right);
				this.ButtonActive(3, value == Properties.JustifHorizontal.Stretch);
			}
		}

		protected bool ButtonActive(int i)
		{
			return ( this.buttons[i].ActiveState == WidgetState.ActiveYes );
		}

		protected void ButtonActive(int i, bool active)
		{
			this.buttons[i].ActiveState = active ? WidgetState.ActiveYes : WidgetState.ActiveNo;
		}


		// Grise les widgets nécessaires.
		protected void EnableWidgets()
		{
			this.fieldOffset.SetVisible(this.isExtendedSize);
			this.fieldAdd.SetVisible(this.isExtendedSize);
			this.labelOffset.SetVisible(this.isExtendedSize);
			this.labelAdd.SetVisible(this.isExtendedSize);
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
			r.Right = rect.Right-20*4;
			this.label.Bounds = r;

			r.Left = rect.Right-20*4;
			r.Width = 20;
			for ( int i=0 ; i<4 ; i++ )
			{
				this.buttons[i].Bounds = r;
				r.Offset(20, 0);
			}

			r.Offset(0, -25);
			r.Left = rect.Left+41;
			r.Width = 20;
			this.labelOffset.Bounds = r;
			r.Left = r.Right+2;
			r.Width = 44;
			this.fieldOffset.Bounds = r;
			r.Left = r.Right+2;
			r.Width = 20;
			this.labelAdd.Bounds = r;
			r.Left = r.Right+2;
			r.Width = 44;
			this.fieldAdd.Bounds = r;
		}
		
		// Un champ a été changé.
		private void HandleFieldChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}

		// Une valeur a été changée.
		private void HandlePanelTextLineClicked(object sender, MessageEventArgs e)
		{
			IconButton button = sender as IconButton;

			if ( button == this.buttons[0] )  this.SelectButtonHorizontal = Properties.JustifHorizontal.Left;
			if ( button == this.buttons[1] )  this.SelectButtonHorizontal = Properties.JustifHorizontal.Center;
			if ( button == this.buttons[2] )  this.SelectButtonHorizontal = Properties.JustifHorizontal.Right;
			if ( button == this.buttons[3] )  this.SelectButtonHorizontal = Properties.JustifHorizontal.Stretch;

			this.EnableWidgets();
			this.OnChanged();
		}


		protected StaticText				label;
		protected IconButton[]				buttons;
		protected TextFieldReal				fieldOffset;
		protected TextFieldReal				fieldAdd;
		protected StaticText				labelOffset;
		protected StaticText				labelAdd;
	}
}
