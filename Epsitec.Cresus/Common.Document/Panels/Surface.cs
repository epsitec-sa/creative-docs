using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe Surface permet de choisir un objet surface 2d.
	/// </summary>
	[SuppressBundleSupport]
	public class Surface : Abstract
	{
		public Surface(Document document) : base(document)
		{
			this.label = new StaticText(this);
			this.label.Alignment = ContentAlignment.MiddleLeft;

			this.surfaceType = new TextFieldCombo(this);
			this.surfaceType.IsReadOnly = true;
			for ( int i=0 ; i<100 ; i++ )
			{
				Properties.SurfaceType type = Properties.Surface.ConvType(i);
				if ( type == Properties.SurfaceType.None )  break;
				this.surfaceType.Items.Add(Properties.Surface.GetName(type));
			}
			//?this.surfaceType.SelectedIndexChanged += new EventHandler(this.HandleTypeChanged);
			this.surfaceType.TextChanged += new EventHandler(this.HandleTypeChanged);
			this.surfaceType.TabIndex = 1;
			this.surfaceType.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.surfaceType, Res.Strings.Panel.Surface.Tooltip.Type);

			this.labelFactor = new StaticText[4];
			this.fieldFactor = new TextFieldReal[4];
			for ( int i=0 ; i<4 ; i++ )
			{
				this.labelFactor[i] = new StaticText(this);
				this.labelFactor[i].Text = string.Format(Res.Strings.Panel.Surface.Label.Factor, i+1);

				this.fieldFactor[i] = new TextFieldReal(this);
				this.document.Modifier.AdaptTextFieldRealPercent(this.fieldFactor[i]);
				this.fieldFactor[i].ValueChanged += new EventHandler(this.HandleFieldChanged);
				this.fieldFactor[i].TabIndex = 2+i;
				this.fieldFactor[i].TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			}

			this.labelScalar = new StaticText[2];
			this.fieldScalar = new TextFieldReal[2];
			for ( int i=0 ; i<2 ; i++ )
			{
				this.labelScalar[i] = new StaticText(this);
				this.labelScalar[i].Text = string.Format(Res.Strings.Panel.Surface.Label.Scalar, i+1);

				this.fieldScalar[i] = new TextFieldReal(this);
				this.document.Modifier.AdaptTextFieldRealScalar(this.fieldScalar[i]);
				this.fieldScalar[i].InternalMinValue = 1;
				this.fieldScalar[i].InternalMaxValue = 20;
				this.fieldScalar[i].Step = 1;
				this.fieldScalar[i].ValueChanged += new EventHandler(this.HandleFieldChanged);
				this.fieldScalar[i].TabIndex = 2+i;
				this.fieldScalar[i].TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			}

			this.resetButton = new Button(this);
			this.resetButton.Text = Res.Strings.Panel.Surface.Label.Reset;
			this.resetButton.Clicked += new MessageEventHandler(this.HandleResetButton);
			this.resetButton.TabIndex = 100;
			this.resetButton.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.resetButton, Res.Strings.Panel.Surface.Tooltip.Reset);

			this.isNormalAndExtended = true;
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				//?this.surfaceType.SelectedIndexChanged -= new EventHandler(this.HandleTypeChanged);
				this.surfaceType.TextChanged -= new EventHandler(this.HandleTypeChanged);
				for ( int i=0 ; i<4 ; i++ )
				{
					this.fieldFactor[i].ValueChanged -= new EventHandler(this.HandleFieldChanged);
					this.fieldFactor[i] = null;
				}
				for ( int i=0 ; i<2 ; i++ )
				{
					this.fieldScalar[i].ValueChanged -= new EventHandler(this.HandleFieldChanged);
					this.fieldScalar[i] = null;
				}
				this.resetButton.Clicked -= new MessageEventHandler(this.HandleResetButton);

				this.label = null;
				this.surfaceType = null;
				this.resetButton = null;
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

			Properties.Surface p = this.property as Properties.Surface;
			if ( p == null )  return;

			this.ignoreChanged = true;

			this.label.Text = p.TextStyle;

			this.surfaceType.SelectedIndex = Properties.Surface.ConvType(p.SurfaceType);
			for ( int i=0 ; i<4 ; i++ )
			{
				this.fieldFactor[i].InternalValue = (decimal) p.GetFactor(i);
			}
			for ( int i=0 ; i<2 ; i++ )
			{
				this.fieldScalar[i].InternalValue = (decimal) p.GetScalar(i);
			}

			this.EnableWidgets();
			this.ignoreChanged = false;
		}

		// Widgets -> propriété.
		protected override void WidgetsToProperty()
		{
			Properties.Surface p = this.property as Properties.Surface;
			if ( p == null )  return;

			this.ignoreChanged = true;
			for ( int i=0 ; i<4 ; i++ )
			{
				if ( Properties.Surface.IsEnableFactor(p.SurfaceType, i) )
				{
					p.SetFactor(i, (double) this.fieldFactor[i].InternalValue);
				}
			}
			for ( int i=0 ; i<2 ; i++ )
			{
				if ( Properties.Surface.IsEnableScalar(p.SurfaceType, i) )
				{
					p.SetScalar(i, (int) this.fieldScalar[i].InternalValue);
				}
			}
			p.SurfaceType = Properties.Surface.ConvType(this.surfaceType.SelectedIndex);
			this.ignoreChanged = false;
			this.PropertyToWidgets();
		}

		// Grise les widgets nécessaires.
		protected void EnableWidgets()
		{
			Properties.Surface p = this.property as Properties.Surface;
			if ( p == null )  return;

			for ( int i=0 ; i<4 ; i++ )
			{
				bool visible = Properties.Surface.IsVisibleFactor(p.SurfaceType, i);
				this.labelFactor[i].SetVisible(visible);
				this.fieldFactor[i].SetVisible(visible);

				bool enable = Properties.Surface.IsEnableFactor(p.SurfaceType, i);
				this.labelFactor[i].SetEnabled(enable);
				this.fieldFactor[i].SetEnabled(enable);
			}

			for ( int i=0 ; i<2 ; i++ )
			{
				bool visible = Properties.Surface.IsVisibleScalar(p.SurfaceType, i);
				this.labelScalar[i].SetVisible(visible);
				this.fieldScalar[i].SetVisible(visible);

				bool enable = Properties.Surface.IsEnableScalar(p.SurfaceType, i);
				this.labelScalar[i].SetEnabled(enable);
				this.fieldScalar[i].SetEnabled(enable);
			}
		}


		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.surfaceType == null )  return;

			this.EnableWidgets();

			Rectangle rect = this.Client.Bounds;
			rect.Deflate(this.extendedZoneWidth, 0);
			rect.Deflate(5);

			Rectangle r = rect;
			r.Bottom = r.Top-20;
			r.Left = rect.Left;
			r.Right = rect.Right-110;
			this.label.Bounds = r;
			r.Left = rect.Right-110;
			r.Right = rect.Right;
			this.surfaceType.Bounds = r;

			r.Top = r.Bottom-5;
			r.Bottom = r.Top-20;
			r.Left = rect.Right-50-16-8-50-16;
			r.Right = rect.Right-50-16-8-50;
			this.labelFactor[0].Bounds = r;
			this.labelScalar[0].Bounds = r;
			r.Left = rect.Right-50-16-8-50;
			r.Right = rect.Right-50-16-8;
			this.fieldFactor[0].Bounds = r;
			this.fieldScalar[0].Bounds = r;
			r.Left = rect.Right-50-16;
			r.Right = rect.Right-50;
			this.labelFactor[2].Bounds = r;
			r.Left = rect.Right-50;
			r.Right = rect.Right;
			this.fieldFactor[2].Bounds = r;

			r.Top = r.Bottom-5;
			r.Bottom = r.Top-20;
			r.Left = rect.Right-50-16-8-50-16;
			r.Right = rect.Right-50-16-8-50;
			this.labelFactor[1].Bounds = r;
			this.labelScalar[1].Bounds = r;
			r.Left = rect.Right-50-16-8-50;
			r.Right = rect.Right-50-16-8;
			this.fieldFactor[1].Bounds = r;
			this.fieldScalar[1].Bounds = r;
			r.Left = rect.Right-50-16;
			r.Right = rect.Right-50;
			this.labelFactor[3].Bounds = r;
			r.Left = rect.Right-50;
			r.Right = rect.Right;
			this.fieldFactor[3].Bounds = r;

			r.Left = rect.Left;
			r.Width = 24;
			this.resetButton.Bounds = r;
		}


		// Le type a été changé.
		private void HandleTypeChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}

		// Un champ a été changé.
		private void HandleFieldChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}

		// Le bouton "reset" a été cliqué.
		private void HandleResetButton(object sender, MessageEventArgs e)
		{
			this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.SurfaceReset);
			Properties.Surface p = this.property as Properties.Surface;
			p.Reset();
			this.document.Modifier.OpletQueueValidateAction();
			this.PropertyToWidgets();
		}


		protected StaticText				label;
		protected TextFieldCombo			surfaceType;
		protected StaticText[]				labelFactor;
		protected TextFieldReal[]			fieldFactor;
		protected StaticText[]				labelScalar;
		protected TextFieldReal[]			fieldScalar;
		protected Button					resetButton;
	}
}
