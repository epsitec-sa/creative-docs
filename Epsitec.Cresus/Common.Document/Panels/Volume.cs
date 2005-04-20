using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe Volume permet de choisir un objet volume 3d.
	/// </summary>
	[SuppressBundleSupport]
	public class Volume : Abstract
	{
		public Volume(Document document) : base(document)
		{
			this.label = new StaticText(this);
			this.label.Alignment = ContentAlignment.MiddleLeft;

			this.volumeType = new TextFieldCombo(this);
			this.volumeType.IsReadOnly = true;
			for ( int i=0 ; i<100 ; i++ )
			{
				Properties.VolumeType type = Properties.Volume.ConvType(i);
				if ( type == Properties.VolumeType.None )  break;
				this.volumeType.Items.Add(Properties.Volume.GetName(type));
			}
			//?this.volumeType.SelectedIndexChanged += new EventHandler(this.HandleTypeChanged);
			this.volumeType.TextChanged += new EventHandler(this.HandleTypeChanged);
			this.volumeType.TabIndex = 1;
			this.volumeType.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.volumeType, Res.Strings.Panel.Volume.ToolTip.Type);

			this.labelRapport = new StaticText(this);
			this.labelRapport.Text = Res.Strings.Panel.Volume.Label.Rapport;

			this.fieldRapport = new TextFieldReal(this);
			this.document.Modifier.AdaptTextFieldRealPercent(this.fieldRapport);
			this.fieldRapport.ValueChanged += new EventHandler(this.HandleFieldChanged);
			this.fieldRapport.TabIndex = 2;
			this.fieldRapport.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.labelLeft = new StaticText(this);
			this.labelLeft.Text = Res.Strings.Panel.Volume.Label.Left;

			this.fieldLeft = new TextFieldReal(this);
			this.document.Modifier.AdaptTextFieldRealAngle(this.fieldLeft);
			this.fieldLeft.InternalMaxValue = 90.0M;
			this.fieldLeft.ValueChanged += new EventHandler(this.HandleFieldChanged);
			this.fieldLeft.TabIndex = 3;
			this.fieldLeft.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.labelRight = new StaticText(this);
			this.labelRight.Text = Res.Strings.Panel.Volume.Label.Right;

			this.fieldRight = new TextFieldReal(this);
			this.document.Modifier.AdaptTextFieldRealAngle(this.fieldRight);
			this.fieldRight.InternalMaxValue = 90.0M;
			this.fieldRight.ValueChanged += new EventHandler(this.HandleFieldChanged);
			this.fieldRight.TabIndex = 4;
			this.fieldRight.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.isNormalAndExtended = true;
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				//?this.volumeType.SelectedIndexChanged -= new EventHandler(this.HandleTypeChanged);
				this.volumeType.TextChanged -= new EventHandler(this.HandleTypeChanged);
				this.fieldRapport.ValueChanged -= new EventHandler(this.HandleFieldChanged);
				this.fieldLeft.ValueChanged -= new EventHandler(this.HandleFieldChanged);
				this.fieldRight.ValueChanged -= new EventHandler(this.HandleFieldChanged);

				this.label = null;
				this.volumeType = null;
				this.fieldRapport = null;
				this.fieldLeft = null;
				this.fieldRight = null;
			}
			
			base.Dispose(disposing);
		}

		
		// Retourne la hauteur standard.
		public override double DefaultHeight
		{
			get
			{
				return ( this.isExtendedSize ? 105 : 30 );
			}
		}

		// Propriété -> widgets.
		protected override void PropertyToWidgets()
		{
			base.PropertyToWidgets();

			Properties.Volume p = this.property as Properties.Volume;
			if ( p == null )  return;

			this.ignoreChanged = true;

			this.label.Text = p.TextStyle;

			this.volumeType.SelectedIndex = Properties.Volume.ConvType(p.VolumeType);
			this.fieldRapport.InternalValue = (decimal) p.Rapport;
			this.fieldLeft.InternalValue = (decimal) p.AngleLeft;
			this.fieldRight.InternalValue = (decimal) p.AngleRight;

			this.EnableWidgets();
			this.ignoreChanged = false;
		}

		// Widgets -> propriété.
		protected override void WidgetsToProperty()
		{
			Properties.Volume p = this.property as Properties.Volume;
			if ( p == null )  return;

			p.VolumeType = Properties.Volume.ConvType(this.volumeType.SelectedIndex);
			p.Rapport = (double) this.fieldRapport.InternalValue;
			p.AngleLeft = (double) this.fieldLeft.InternalValue;
			p.AngleRight = (double) this.fieldRight.InternalValue;
		}

		// Grise les widgets nécessaires.
		protected void EnableWidgets()
		{
		}


		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.volumeType == null )  return;

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
			this.volumeType.Bounds = r;

			r.Top = r.Bottom-5;
			r.Bottom = r.Top-20;
			r.Left = rect.Left;
			r.Right = rect.Right-50;
			this.labelRapport.Bounds = r;
			r.Left = rect.Right-50;
			r.Right = rect.Right;
			this.fieldRapport.Bounds = r;

			r.Top = r.Bottom-5;
			r.Bottom = r.Top-20;
			r.Left = rect.Left;
			r.Right = rect.Right-50;
			this.labelLeft.Bounds = r;
			r.Left = rect.Right-50;
			r.Right = rect.Right;
			this.fieldLeft.Bounds = r;

			r.Top = r.Bottom-5;
			r.Bottom = r.Top-20;
			r.Left = rect.Left;
			r.Right = rect.Right-50;
			this.labelRight.Bounds = r;
			r.Left = rect.Right-50;
			r.Right = rect.Right;
			this.fieldRight.Bounds = r;
		}


		// Le type a été changé.
		private void HandleTypeChanged(object sender)
		{
			if ( this.ignoreChanged )  return;

			// Met les valeurs par défaut correspondant au type choisi.
			this.EnableWidgets();

			Properties.VolumeType type = Properties.Volume.ConvType(this.volumeType.SelectedIndex);

			this.OnChanged();
		}

		// Un champ a été changé.
		private void HandleFieldChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}


		protected StaticText				label;
		protected TextFieldCombo			volumeType;
		protected StaticText				labelRapport;
		protected TextFieldReal				fieldRapport;
		protected StaticText				labelLeft;
		protected TextFieldReal				fieldLeft;
		protected StaticText				labelRight;
		protected TextFieldReal				fieldRight;
	}
}
