using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe Surface permet de choisir un objet surface 2d.
	/// </summary>
	public class Surface : Abstract
	{
		public Surface(Document document) : base(document)
		{
			this.grid = new RadioIconGrid(this);
			this.grid.SelectionChanged += HandleTypeChanged;
			this.grid.TabIndex = 0;
			this.grid.TabNavigationMode = TabNavigationMode.ActivateOnTab;

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
				this.fieldFactor[i] = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
				this.fieldFactor[i].LabelShortText = string.Format(Res.Strings.Panel.Surface.Short.Factor, i+1);
				this.document.Modifier.AdaptTextFieldRealPercent(this.fieldFactor[i].TextFieldReal);
				this.fieldFactor[i].TextFieldReal.EditionAccepted += this.HandleFieldChanged;
				this.fieldFactor[i].TabIndex = 2+i;
				this.fieldFactor[i].TabNavigationMode = TabNavigationMode.ActivateOnTab;
			}

			this.fieldScalar = new Widgets.TextFieldLabel[2];
			for ( int i=0 ; i<2 ; i++ )
			{
				this.fieldScalar[i] = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
				this.fieldScalar[i].LabelShortText = string.Format(Res.Strings.Panel.Surface.Short.Scalar, i+1);
				this.document.Modifier.AdaptTextFieldRealScalar(this.fieldScalar[i].TextFieldReal);
				this.fieldScalar[i].TextFieldReal.InternalMinValue = 1;
				this.fieldScalar[i].TextFieldReal.InternalMaxValue = 20;
				this.fieldScalar[i].TextFieldReal.Step = 1;
				this.fieldScalar[i].TextFieldReal.EditionAccepted += this.HandleFieldChanged;
				this.fieldScalar[i].TabIndex = 2+i;
				this.fieldScalar[i].TabNavigationMode = TabNavigationMode.ActivateOnTab;
			}

			this.resetButton = new Button(this);
			this.resetButton.Text = Res.Strings.Panel.Surface.Button.Reset;
			this.resetButton.Clicked += this.HandleResetButton;
			this.resetButton.TabIndex = 100;
			this.resetButton.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.resetButton, Res.Strings.Panel.Surface.Tooltip.Reset);

			this.isNormalAndExtended = true;
		}

		protected void AddRadioIcon(Properties.SurfaceType type, bool endOfLine)
		{
			this.grid.AddRadioIcon(Misc.Icon(Properties.Surface.GetIconText(type)), Properties.Surface.GetName(type), (int)type, endOfLine);
		}

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.grid.SelectionChanged -= this.HandleTypeChanged;
				for ( int i=0 ; i<4 ; i++ )
				{
					this.fieldFactor[i].TextFieldReal.EditionAccepted -= this.HandleFieldChanged;
					this.fieldFactor[i] = null;
				}
				for ( int i=0 ; i<2 ; i++ )
				{
					this.fieldScalar[i].TextFieldReal.EditionAccepted -= this.HandleFieldChanged;
					this.fieldScalar[i] = null;
				}
				this.resetButton.Clicked -= this.HandleResetButton;

				this.grid = null;
				this.resetButton = null;
			}
			
			base.Dispose(disposing);
		}

		
		public override double DefaultHeight
		{
			//	Retourne la hauteur standard.
			get
			{
				return ( this.isExtendedSize ? this.LabelHeight+124 : this.LabelHeight+30 );
			}
		}

		protected override void PropertyToWidgets()
		{
			//	Propriété -> widgets.
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

		protected override void WidgetsToProperty()
		{
			//	Widgets -> propriété.
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

		protected void EnableWidgets()
		{
			//	Grise les widgets nécessaires.
			Properties.Surface p = this.property as Properties.Surface;
			if ( p == null )  return;

			for ( int i=0 ; i<4 ; i++ )
			{
				bool visible = Properties.Surface.IsVisibleFactor(p.SurfaceType, i);
				this.fieldFactor[i].Visibility = (visible);

				bool enable = Properties.Surface.IsEnableFactor(p.SurfaceType, i);
				this.fieldFactor[i].Enable = (enable);
			}

			for ( int i=0 ; i<2 ; i++ )
			{
				bool visible = Properties.Surface.IsVisibleScalar(p.SurfaceType, i);
				this.fieldScalar[i].Visibility = (visible);

				bool enable = Properties.Surface.IsEnableScalar(p.SurfaceType, i);
				this.fieldScalar[i].Enable = (enable);
			}
		}


		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
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
			this.grid.SetManualBounds(r);

			double w = Widgets.TextFieldLabel.ShortWidth+8;

			r.Top = rect.Top-69;
			r.Bottom = r.Top-20;
			r.Left = rect.Right-w-w;
			r.Width = w;
			this.fieldFactor[0].SetManualBounds(r);
			this.fieldScalar[0].SetManualBounds(r);
			r.Left = r.Right;
			r.Width = w;
			this.fieldFactor[2].SetManualBounds(r);

			r.Top = r.Bottom-5;
			r.Bottom = r.Top-20;
			r.Left = rect.Right-w-w;
			r.Width = w;
			this.fieldFactor[1].SetManualBounds(r);
			this.fieldScalar[1].SetManualBounds(r);
			r.Left = r.Right;
			r.Width = w;
			this.fieldFactor[3].SetManualBounds(r);

			r.Left = rect.Left;
			r.Width = 24;
			this.resetButton.SetManualBounds(r);
		}


		private void HandleTypeChanged(object sender)
		{
			//	Le type a été changé.
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}

		private void HandleFieldChanged(object sender)
		{
			//	Un champ a été changé.
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}

		private void HandleResetButton(object sender, MessageEventArgs e)
		{
			//	Le bouton "reset" a été cliqué.
			this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.SurfaceReset);
			Properties.Surface p = this.property as Properties.Surface;
			p.Reset();
			this.document.Modifier.OpletQueueValidateAction();
			this.PropertyToWidgets();
		}


		protected RadioIconGrid				grid;
		protected Widgets.TextFieldLabel[]	fieldFactor;
		protected Widgets.TextFieldLabel[]	fieldScalar;
		protected Button					resetButton;
	}
}
