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
			this.label = new StaticText(this);
			this.label.Alignment = ContentAlignment.MiddleLeft;

			this.buttons = new IconButton[5+3+4];
			for ( int i=0 ; i<5+3+4 ; i++ )
			{
				this.buttons[i] = new IconButton(this);
				this.buttons[i].Clicked += new MessageEventHandler(this.HandlePanelJustifClicked);
				this.buttons[i].TabIndex = 2+i;
				this.buttons[i].TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			}

			this.buttons[0].IconName = @"file:images/justifhleft.icon";
			this.buttons[1].IconName = @"file:images/justifhcenter.icon";
			this.buttons[2].IconName = @"file:images/justifhright.icon";
			this.buttons[3].IconName = @"file:images/justifhjustif.icon";
			this.buttons[4].IconName = @"file:images/justifhall.icon";
			ToolTip.Default.SetToolTip(this.buttons[0], "Justification à gauche");
			ToolTip.Default.SetToolTip(this.buttons[1], "Justification centrée");
			ToolTip.Default.SetToolTip(this.buttons[2], "Justification à droite");
			ToolTip.Default.SetToolTip(this.buttons[3], "Justification alignée, sauf dernière ligne");
			ToolTip.Default.SetToolTip(this.buttons[4], "Justification alignée, y compris dernière ligne");

			this.buttons[5].IconName = @"file:images/justifvtop.icon";
			this.buttons[6].IconName = @"file:images/justifvcenter.icon";
			this.buttons[7].IconName = @"file:images/justifvbottom.icon";
			ToolTip.Default.SetToolTip(this.buttons[5], "En haut");
			ToolTip.Default.SetToolTip(this.buttons[6], "Centré verticalement");
			ToolTip.Default.SetToolTip(this.buttons[7], "En bas");

			this.buttons[8].IconName = @"file:images/justifolr.icon";
			this.buttons[9].IconName = @"file:images/justifobt.icon";
			this.buttons[10].IconName = @"file:images/justiforl.icon";
			this.buttons[11].IconName = @"file:images/justifotb.icon";
			ToolTip.Default.SetToolTip(this.buttons[8], "Orientation normale");
			ToolTip.Default.SetToolTip(this.buttons[9], "Orientation de bas en haut");
			ToolTip.Default.SetToolTip(this.buttons[10], "Orientation de droite à gauche");
			ToolTip.Default.SetToolTip(this.buttons[11], "Orientation de haut en bas");

			this.fieldMarginH = new TextFieldSlider(this);
			this.fieldMarginH.MinValue = 0;
			this.fieldMarginH.MaxValue = 10;
			this.fieldMarginH.Step = 0.1M;
			this.fieldMarginH.Resolution = 0.1M;
			this.fieldMarginH.TextChanged += new EventHandler(this.HandleFieldChanged);
			this.fieldMarginH.TabIndex = 20;
			this.fieldMarginH.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldMarginH, "Marges gauche et droite");

			this.fieldMarginV = new TextFieldSlider(this);
			this.fieldMarginV.MinValue = 0;
			this.fieldMarginV.MaxValue = 10;
			this.fieldMarginV.Step = 0.1M;
			this.fieldMarginV.Resolution = 0.1M;
			this.fieldMarginV.TextChanged += new EventHandler(this.HandleFieldChanged);
			this.fieldMarginV.TabIndex = 21;
			this.fieldMarginV.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldMarginV, "Marges sup/inf");

			this.fieldOffsetV = new TextFieldSlider(this);
			this.fieldOffsetV.MinValue = -50;
			this.fieldOffsetV.MaxValue =  50;
			this.fieldOffsetV.Step = 5;
			this.fieldOffsetV.Resolution = 1;
			this.fieldOffsetV.TextChanged += new EventHandler(this.HandleFieldChanged);
			this.fieldOffsetV.TabIndex = 22;
			this.fieldOffsetV.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldOffsetV, "Décalage vertical");

			this.labelMarginH = new StaticText(this);
			this.labelMarginH.Text = "Mx";
			this.labelMarginH.Alignment = ContentAlignment.MiddleCenter;

			this.labelMarginV = new StaticText(this);
			this.labelMarginV.Text = "My";
			this.labelMarginV.Alignment = ContentAlignment.MiddleCenter;

			this.labelOffsetV = new StaticText(this);
			this.labelOffsetV.Text = "Oy";
			this.labelOffsetV.Alignment = ContentAlignment.MiddleCenter;

			this.isNormalAndExtended = true;
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				for ( int i=0 ; i<5+3+4 ; i++ )
				{
					this.buttons[i].Clicked -= new MessageEventHandler(this.HandlePanelJustifClicked);
					this.buttons[i] = null;
				}
				this.fieldMarginH.TextChanged -= new EventHandler(this.HandleFieldChanged);
				this.fieldMarginV.TextChanged -= new EventHandler(this.HandleFieldChanged);
				this.fieldOffsetV.TextChanged -= new EventHandler(this.HandleFieldChanged);

				this.label = null;
				this.fieldMarginH = null;
				this.fieldMarginV = null;
				this.fieldOffsetV = null;
				this.labelMarginH = null;
				this.labelMarginV = null;
				this.labelOffsetV = null;
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

			Properties.Justif p = this.property as Properties.Justif;
			if ( p == null )  return;

			this.ignoreChanged = true;

			this.label.Text = p.TextStyle;

			this.SelectButtonHorizontal  = p.Horizontal;
			this.SelectButtonVertical    = p.Vertical;
			this.SelectButtonOrientation = p.Orientation;
			this.fieldMarginH.Value      = (decimal) p.MarginH;
			this.fieldMarginV.Value      = (decimal) p.MarginV;
			this.fieldOffsetV.Value      = (decimal) p.OffsetV*100;

			this.EnableWidgets();
			this.ignoreChanged = false;
		}

		// Widgets -> propriété.
		protected override void WidgetsToProperty()
		{
			Properties.Justif p = this.property as Properties.Justif;
			if ( p == null )  return;

			p.Horizontal  = this.SelectButtonHorizontal;
			p.Vertical    = this.SelectButtonVertical;
			p.Orientation = this.SelectButtonOrientation;
			p.MarginH     = (double) this.fieldMarginH.Value;
			p.MarginV     = (double) this.fieldMarginV.Value;
			p.OffsetV     = (double) this.fieldOffsetV.Value/100;
		}

		protected Properties.JustifHorizontal SelectButtonHorizontal
		{
			get
			{
				if ( this.ButtonActive(0) )  return Properties.JustifHorizontal.Left;
				if ( this.ButtonActive(1) )  return Properties.JustifHorizontal.Center;
				if ( this.ButtonActive(2) )  return Properties.JustifHorizontal.Right;
				if ( this.ButtonActive(3) )  return Properties.JustifHorizontal.Justif;
				if ( this.ButtonActive(4) )  return Properties.JustifHorizontal.All;
				return Properties.JustifHorizontal.None;
			}

			set
			{
				this.ButtonActive(0, value == Properties.JustifHorizontal.Left);
				this.ButtonActive(1, value == Properties.JustifHorizontal.Center);
				this.ButtonActive(2, value == Properties.JustifHorizontal.Right);
				this.ButtonActive(3, value == Properties.JustifHorizontal.Justif);
				this.ButtonActive(4, value == Properties.JustifHorizontal.All);
			}
		}

		protected Properties.JustifVertical SelectButtonVertical
		{
			get
			{
				if ( this.ButtonActive(5) )  return Properties.JustifVertical.Top;
				if ( this.ButtonActive(6) )  return Properties.JustifVertical.Center;
				if ( this.ButtonActive(7) )  return Properties.JustifVertical.Bottom;
				return Properties.JustifVertical.None;
			}

			set
			{
				this.ButtonActive(5, value == Properties.JustifVertical.Top);
				this.ButtonActive(6, value == Properties.JustifVertical.Center);
				this.ButtonActive(7, value == Properties.JustifVertical.Bottom);
			}
		}

		protected Properties.JustifOrientation SelectButtonOrientation
		{
			get
			{
				if ( this.ButtonActive( 8) )  return Properties.JustifOrientation.LeftToRight;
				if ( this.ButtonActive( 9) )  return Properties.JustifOrientation.BottomToTop;
				if ( this.ButtonActive(10) )  return Properties.JustifOrientation.RightToLeft;
				if ( this.ButtonActive(11) )  return Properties.JustifOrientation.TopToBottom;
				return Properties.JustifOrientation.None;
			}

			set
			{
				this.ButtonActive( 8, value == Properties.JustifOrientation.LeftToRight);
				this.ButtonActive( 9, value == Properties.JustifOrientation.BottomToTop);
				this.ButtonActive(10, value == Properties.JustifOrientation.RightToLeft);
				this.ButtonActive(11, value == Properties.JustifOrientation.TopToBottom);
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
			for ( int i=5 ; i<5+3+4 ; i++ )
			{
				this.buttons[i].SetVisible(this.isExtendedSize);
			}

			this.fieldMarginH.SetVisible(this.isExtendedSize);

			if ( this.isExtendedSize )
			{
				if ( this.ButtonActive(6) )  // JustifVertical.Center ?
				{
					this.fieldMarginV.SetVisible(false);
					this.fieldOffsetV.SetVisible(true);
					this.labelMarginV.SetVisible(false);
					this.labelOffsetV.SetVisible(true);
				}
				else
				{
					this.fieldMarginV.SetVisible(true);
					this.fieldOffsetV.SetVisible(false);
					this.labelMarginV.SetVisible(true);
					this.labelOffsetV.SetVisible(false);
				}
			}
			else
			{
				this.fieldMarginV.SetVisible(false);
				this.fieldOffsetV.SetVisible(false);
				this.labelMarginV.SetVisible(false);
				this.labelOffsetV.SetVisible(false);
			}
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
			r.Right = rect.Right-20*5;
			this.label.Bounds = r;

			r.Left = rect.Right-20*5;
			r.Width = 20;
			for ( int i=0 ; i<5 ; i++ )
			{
				this.buttons[i].Bounds = r;
				r.Offset(20, 0);
			}

			r.Offset(0, -25);
			r.Left = rect.Right-20*3;
			r.Width = 20;
			for ( int i=5 ; i<5+3 ; i++ )
			{
				this.buttons[i].Bounds = r;
				r.Offset(20, 0);
			}

			r.Offset(0, -25);
			r.Left = rect.Right-20*4;
			r.Width = 20;
			for ( int i=5+3 ; i<5+3+4 ; i++ )
			{
				this.buttons[i].Bounds = r;
				r.Offset(20, 0);
			}

			r = rect;
			r.Bottom = rect.Top-45;
			r.Height = 20;
			r.Width = 20;
			this.labelMarginH.Bounds = r;
			r.Left = r.Right+2;
			r.Width = 44;
			this.fieldMarginH.Bounds = r;

			r = rect;
			r.Bottom = rect.Top-70;
			r.Height = 20;
			r.Width = 20;
			this.labelMarginV.Bounds = r;
			this.labelOffsetV.Bounds = r;
			r.Left = r.Right+2;
			r.Width = 44;
			this.fieldMarginV.Bounds = r;
			this.fieldOffsetV.Bounds = r;
		}
		
		// Un champ a été changé.
		private void HandleFieldChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}

		// Une valeur a été changée.
		private void HandlePanelJustifClicked(object sender, MessageEventArgs e)
		{
			IconButton button = sender as IconButton;

			if ( button == this.buttons[0] )  this.SelectButtonHorizontal = Properties.JustifHorizontal.Left;
			if ( button == this.buttons[1] )  this.SelectButtonHorizontal = Properties.JustifHorizontal.Center;
			if ( button == this.buttons[2] )  this.SelectButtonHorizontal = Properties.JustifHorizontal.Right;
			if ( button == this.buttons[3] )  this.SelectButtonHorizontal = Properties.JustifHorizontal.Justif;
			if ( button == this.buttons[4] )  this.SelectButtonHorizontal = Properties.JustifHorizontal.All;

			if ( button == this.buttons[5] )  this.SelectButtonVertical = Properties.JustifVertical.Top;
			if ( button == this.buttons[6] )  this.SelectButtonVertical = Properties.JustifVertical.Center;
			if ( button == this.buttons[7] )  this.SelectButtonVertical = Properties.JustifVertical.Bottom;

			if ( button == this.buttons[8] )  this.SelectButtonOrientation = Properties.JustifOrientation.LeftToRight;
			if ( button == this.buttons[9] )  this.SelectButtonOrientation = Properties.JustifOrientation.BottomToTop;
			if ( button == this.buttons[10])  this.SelectButtonOrientation = Properties.JustifOrientation.RightToLeft;
			if ( button == this.buttons[11])  this.SelectButtonOrientation = Properties.JustifOrientation.TopToBottom;

			this.EnableWidgets();
			this.OnChanged();
		}


		protected StaticText				label;
		protected IconButton[]				buttons;
		protected TextFieldSlider			fieldMarginH;
		protected TextFieldSlider			fieldMarginV;
		protected TextFieldSlider			fieldOffsetV;
		protected StaticText				labelMarginH;
		protected StaticText				labelMarginV;
		protected StaticText				labelOffsetV;
	}
}
