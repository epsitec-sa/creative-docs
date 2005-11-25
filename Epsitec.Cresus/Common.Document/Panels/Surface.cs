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
			this.grid = new Widgets.RadioIconGrid(this);
			this.grid.SelectionChanged += new EventHandler(HandleTypeChanged);
			this.grid.TabIndex = 0;
			this.grid.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.AddRadioIcon(Properties.SurfaceType.ParallelT, false);
			this.AddRadioIcon(Properties.SurfaceType.ParallelB, false);
			this.AddRadioIcon(Properties.SurfaceType.ParallelL, false);
			this.AddRadioIcon(Properties.SurfaceType.ParallelR, false);
			this.AddRadioIcon(Properties.SurfaceType.TrapezeT, false);
			this.AddRadioIcon(Properties.SurfaceType.TrapezeB, false);
			this.AddRadioIcon(Properties.SurfaceType.TrapezeL, false);
			this.AddRadioIcon(Properties.SurfaceType.TrapezeR, false);

			this.AddRadioIcon(Properties.SurfaceType.QuadriL, false);
			this.AddRadioIcon(Properties.SurfaceType.QuadriP, false);
			this.AddRadioIcon(Properties.SurfaceType.QuadriC, false);
			this.AddRadioIcon(Properties.SurfaceType.QuadriX, true);
			
			this.AddRadioIcon(Properties.SurfaceType.Grid, false);
			this.AddRadioIcon(Properties.SurfaceType.Pattern, false);
			this.AddRadioIcon(Properties.SurfaceType.Ring, false);
			this.AddRadioIcon(Properties.SurfaceType.SpiralCW, false);
			this.AddRadioIcon(Properties.SurfaceType.SpiralCCW, false);

			this.fieldFactor = new Widgets.TextFieldLabel[4];
			for ( int i=0 ; i<4 ; i++ )
			{
				this.fieldFactor[i] = new Widgets.TextFieldLabel(this, false);
				this.fieldFactor[i].LabelShortText = string.Format(Res.Strings.Panel.Surface.Short.Factor, i+1);
				this.document.Modifier.AdaptTextFieldRealPercent(this.fieldFactor[i].TextFieldReal);
				this.fieldFactor[i].TextFieldReal.ValueChanged += new EventHandler(this.HandleFieldChanged);
				this.fieldFactor[i].TabIndex = 2+i;
				this.fieldFactor[i].TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			}

			this.fieldScalar = new Widgets.TextFieldLabel[2];
			for ( int i=0 ; i<2 ; i++ )
			{
				this.fieldScalar[i] = new Widgets.TextFieldLabel(this, false);
				this.fieldScalar[i].LabelShortText = string.Format(Res.Strings.Panel.Surface.Short.Scalar, i+1);
				this.document.Modifier.AdaptTextFieldRealScalar(this.fieldScalar[i].TextFieldReal);
				this.fieldScalar[i].TextFieldReal.InternalMinValue = 1;
				this.fieldScalar[i].TextFieldReal.InternalMaxValue = 20;
				this.fieldScalar[i].TextFieldReal.Step = 1;
				this.fieldScalar[i].TextFieldReal.ValueChanged += new EventHandler(this.HandleFieldChanged);
				this.fieldScalar[i].TabIndex = 2+i;
				this.fieldScalar[i].TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			}

			this.resetButton = new Button(this);
			this.resetButton.Text = Res.Strings.Panel.Surface.Button.Reset;
			this.resetButton.Clicked += new MessageEventHandler(this.HandleResetButton);
			this.resetButton.TabIndex = 100;
			this.resetButton.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.resetButton, Res.Strings.Panel.Surface.Tooltip.Reset);

			this.isNormalAndExtended = true;
		}

		protected void AddRadioIcon(Properties.SurfaceType type, bool endOfLine)
		{
			this.grid.AddRadioIcon(Properties.Surface.GetIconText(type), Properties.Surface.GetName(type), (int)type, endOfLine);
		}

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.grid.SelectionChanged -= new EventHandler(this.HandleTypeChanged);
				for ( int i=0 ; i<4 ; i++ )
				{
					this.fieldFactor[i].TextFieldReal.ValueChanged -= new EventHandler(this.HandleFieldChanged);
					this.fieldFactor[i] = null;
				}
				for ( int i=0 ; i<2 ; i++ )
				{
					this.fieldScalar[i].TextFieldReal.ValueChanged -= new EventHandler(this.HandleFieldChanged);
					this.fieldScalar[i] = null;
				}
				this.resetButton.Clicked -= new MessageEventHandler(this.HandleResetButton);

				this.grid = null;
				this.resetButton = null;
			}
			
			base.Dispose(disposing);
		}

		
		// Retourne la hauteur standard.
		public override double DefaultHeight
		{
			get
			{
				return ( this.isExtendedSize ? this.LabelHeight+124 : this.LabelHeight+30 );
			}
		}

		// Propriété -> widgets.
		protected override void PropertyToWidgets()
		{
			base.PropertyToWidgets();

			Properties.Surface p = this.property as Properties.Surface;
			if ( p == null )  return;

			this.ignoreChanged = true;

			this.grid.SelectedValue = (int) p.SurfaceType;
			for ( int i=0 ; i<4 ; i++ )
			{
				this.fieldFactor[i].TextFieldReal.InternalValue = (decimal) p.GetFactor(i);
			}
			for ( int i=0 ; i<2 ; i++ )
			{
				this.fieldScalar[i].TextFieldReal.InternalValue = (decimal) p.GetScalar(i);
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
					p.SetFactor(i, (double) this.fieldFactor[i].TextFieldReal.InternalValue);
				}
			}
			for ( int i=0 ; i<2 ; i++ )
			{
				if ( Properties.Surface.IsEnableScalar(p.SurfaceType, i) )
				{
					p.SetScalar(i, (int) this.fieldScalar[i].TextFieldReal.InternalValue);
				}
			}
			p.SurfaceType = (Properties.SurfaceType) this.grid.SelectedValue;
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
				this.fieldFactor[i].SetVisible(visible);

				bool enable = Properties.Surface.IsEnableFactor(p.SurfaceType, i);
				this.fieldFactor[i].Enable = (enable);
			}

			for ( int i=0 ; i<2 ; i++ )
			{
				bool visible = Properties.Surface.IsVisibleScalar(p.SurfaceType, i);
				this.fieldScalar[i].SetVisible(visible);

				bool enable = Properties.Surface.IsEnableScalar(p.SurfaceType, i);
				this.fieldScalar[i].Enable = (enable);
			}
		}


		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.grid == null )  return;

			this.EnableWidgets();

			Rectangle rect = this.UsefulZone;

			Rectangle r = rect;
			r.Bottom = r.Top-22*3;
			if ( !this.isExtendedSize )
			{
				r.Bottom = r.Top-20;
			}
			r.Inflate(1);
			this.grid.Bounds = r;

			double w = Widgets.TextFieldLabel.ShortWidth+8;

			r.Top = rect.Top-69;
			r.Bottom = r.Top-20;
			r.Left = rect.Right-w-w;
			r.Width = w;
			this.fieldFactor[0].Bounds = r;
			this.fieldScalar[0].Bounds = r;
			r.Left = r.Right;
			r.Width = w;
			this.fieldFactor[2].Bounds = r;

			r.Top = r.Bottom-5;
			r.Bottom = r.Top-20;
			r.Left = rect.Right-w-w;
			r.Width = w;
			this.fieldFactor[1].Bounds = r;
			this.fieldScalar[1].Bounds = r;
			r.Left = r.Right;
			r.Width = w;
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


		protected Widgets.RadioIconGrid		grid;
		protected Widgets.TextFieldLabel[]	fieldFactor;
		protected Widgets.TextFieldLabel[]	fieldScalar;
		protected Button					resetButton;
	}
}
