using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe Arrow permet de choisir un type d'extrémité.
	/// </summary>
	[SuppressBundleSupport]
	public class Arrow : Abstract
	{
		public Arrow(Document document) : base(document)
		{
			this.label = new StaticText(this);
			this.label.Alignment = ContentAlignment.MiddleLeft;

			this.fieldType    = new TextFieldCombo[2];
			this.fieldLength  = new TextFieldReal[2];
			this.fieldEffect1 = new TextFieldReal[2];
			this.fieldEffect2 = new TextFieldReal[2];
			this.labelLength  = new StaticText[2];
			this.labelEffect1 = new StaticText[2];
			this.labelEffect2 = new StaticText[2];

			int index = 1;
			for ( int j=0 ; j<2 ; j++ )
			{
				this.fieldType[j] = new TextFieldCombo(this);
				this.fieldType[j].IsReadOnly = true;
				for ( int i=0 ; i<100 ; i++ )
				{
					Properties.ArrowType type = Properties.Arrow.ConvType(i);
					if ( type == Properties.ArrowType.None )  break;
					this.fieldType[j].Items.Add(Properties.Arrow.GetName(type));
				}
				//?this.fieldType[j].SelectedIndexChanged += new EventHandler(this.HandleTypeChanged);
				this.fieldType[j].TextChanged += new EventHandler(this.HandleTypeChanged);
				this.fieldType[j].TabIndex = index++;
				this.fieldType[j].TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				this.fieldLength[j] = new TextFieldReal(this);
				this.fieldLength[j].FactorMinRange = 0.0M;
				this.fieldLength[j].FactorMaxRange = 0.1M;
				this.fieldLength[j].FactorStep = 1.0M;
				this.document.Modifier.AdaptTextFieldRealDimension(this.fieldLength[j]);
				this.fieldLength[j].TextChanged += new EventHandler(this.HandleFieldChanged);
				this.fieldLength[j].TabIndex = index++;
				this.fieldLength[j].TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(this.fieldLength[j], "Longueur");

				this.fieldEffect1[j] = new TextFieldReal(this);
				this.document.Modifier.AdaptTextFieldRealScalar(this.fieldEffect1[j]);
				this.fieldEffect1[j].InternalMinValue = -100;
				this.fieldEffect1[j].InternalMaxValue = 200;
				this.fieldEffect1[j].TextChanged += new EventHandler(this.HandleFieldChanged);
				this.fieldEffect1[j].TabIndex = index++;
				this.fieldEffect1[j].TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(this.fieldEffect1[j], "Paramètre A");

				this.fieldEffect2[j] = new TextFieldReal(this);
				this.document.Modifier.AdaptTextFieldRealScalar(this.fieldEffect2[j]);
				this.fieldEffect2[j].InternalMinValue = -100;
				this.fieldEffect2[j].InternalMaxValue = 200;
				this.fieldEffect2[j].TextChanged += new EventHandler(this.HandleFieldChanged);
				this.fieldEffect2[j].TabIndex = index++;
				this.fieldEffect2[j].TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(this.fieldEffect2[j], "Paramètre B");

				this.labelLength[j] = new StaticText(this);
				this.labelLength[j].Text = "L";
				this.labelLength[j].Alignment = ContentAlignment.MiddleCenter;

				this.labelEffect1[j] = new StaticText(this);
				this.labelEffect1[j].Text = "A";
				this.labelEffect1[j].Alignment = ContentAlignment.MiddleRight;

				this.labelEffect2[j] = new StaticText(this);
				this.labelEffect2[j].Text = "B";
				this.labelEffect2[j].Alignment = ContentAlignment.MiddleRight;
			}

			this.separator = new Separator(this);

			this.swapArrow = new IconButton(this);
			this.swapArrow.IconName = @"file:images/swapdata.icon";
			this.swapArrow.Clicked += new MessageEventHandler(this.HandleSwapArrowClicked);
			ToolTip.Default.SetToolTip(this.swapArrow, "Permute les extrémités");

			this.isNormalAndExtended = true;
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				for ( int j=0 ; j<2 ; j++ )
				{
					//?this.fieldType[j].SelectedIndexChanged -= new EventHandler(this.HandleTypeChanged);
					this.fieldType[j].TextChanged -= new EventHandler(this.HandleTypeChanged);
					this.fieldLength[j].TextChanged -= new EventHandler(this.HandleFieldChanged);
					this.fieldEffect1[j].TextChanged -= new EventHandler(this.HandleFieldChanged);
					this.fieldEffect2[j].TextChanged -= new EventHandler(this.HandleFieldChanged);
					this.fieldType[j] = null;
					this.fieldLength[j] = null;
					this.fieldEffect1[j] = null;
					this.fieldEffect2[j] = null;
					this.labelLength[j] = null;
					this.labelEffect1[j] = null;
					this.labelEffect2[j] = null;
				}
				this.swapArrow.Clicked -= new MessageEventHandler(this.HandleSwapArrowClicked);

				this.label = null;
				this.separator = null;
				this.swapArrow = null;
			}
			
			base.Dispose(disposing);
		}

		
		// Retourne la hauteur standard.
		public override double DefaultHeight
		{
			get
			{
				return ( this.isExtendedSize ? 110 : 30 );
			}
		}

		// Propriété -> widgets.
		protected override void PropertyToWidgets()
		{
			base.PropertyToWidgets();

			Properties.Arrow p = this.property as Properties.Arrow;
			if ( p == null )  return;

			this.ignoreChanged = true;

			this.label.Text = p.TextStyle;

			for ( int j=0 ; j<2 ; j++ )
			{
				this.fieldType[j].SelectedIndex = Properties.Arrow.ConvType(p.GetArrowType(j));
				this.fieldLength[j].InternalValue  = (decimal) p.GetLength(j);
				this.fieldEffect1[j].InternalValue = (decimal) p.GetEffect1(j)*100;
				this.fieldEffect2[j].InternalValue = (decimal) p.GetEffect2(j)*100;
			}

			this.EnableWidgets();
			this.ignoreChanged = false;
		}

		// Widgets -> propriété.
		protected override void WidgetsToProperty()
		{
			Properties.Arrow p = this.property as Properties.Arrow;
			if ( p == null )  return;

			for ( int j=0 ; j<2 ; j++ )
			{
				p.SetArrowType(j, Properties.Arrow.ConvType(this.fieldType[j].SelectedIndex));
				p.SetLength(j,  (double) this.fieldLength[j].InternalValue);
				p.SetEffect1(j, (double) this.fieldEffect1[j].InternalValue/100);
				p.SetEffect2(j, (double) this.fieldEffect2[j].InternalValue/100);
			}
		}

		// Grise les widgets nécessaires.
		protected void EnableWidgets()
		{
			// Initialise les min/max en fonction du type choisi.
			for ( int j=0 ; j<2 ; j++ )
			{
				Properties.ArrowType type = Properties.Arrow.ConvType(this.fieldType[j].SelectedIndex);
				bool enableRadius, enable1, enable2;
				double effect1, min1, max1;
				double effect2, min2, max2;
				Properties.Arrow.GetFieldsParam(type, out enableRadius, out enable1, out effect1, out min1, out max1, out enable2, out effect2, out min2, out max2);

				this.fieldEffect1[j].InternalMinValue = (decimal) min1*100;
				this.fieldEffect1[j].InternalMaxValue = (decimal) max1*100;
				this.fieldEffect2[j].InternalMinValue = (decimal) min2*100;
				this.fieldEffect2[j].InternalMaxValue = (decimal) max2*100;

				this.fieldLength[j].SetEnabled(this.isExtendedSize && enableRadius);
				this.fieldEffect1[j].SetEnabled(this.isExtendedSize && enable1);
				this.fieldEffect2[j].SetEnabled(this.isExtendedSize && enable2);
			}
		}

		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.fieldType == null )  return;

			this.EnableWidgets();

			Rectangle rect = this.Client.Bounds;
			rect.Deflate(this.extendedZoneWidth, 0);
			rect.Deflate(5);

			Rectangle r = rect;
			r.Bottom = r.Top-20;
			r.Right = rect.Right-110;
			this.label.Bounds = r;

			for ( int j=0 ; j<2 ; j++ )
			{
				r.Left = rect.Right-110;
				r.Right = rect.Right;
				this.fieldType[j].Bounds = r;

				r.Offset(0, -25);
				r.Left = rect.Left;
				r.Width = 14;
				this.labelLength[j].Bounds = r;
				r.Left = r.Right;
				r.Width = 44;
				this.fieldLength[j].Bounds = r;
				r.Left = r.Right;
				r.Width = 15;
				this.labelEffect1[j].Bounds = r;
				r.Left = r.Right+2;
				r.Width = 44;
				this.fieldEffect1[j].Bounds = r;
				r.Left = r.Right;
				r.Width = 10;
				this.labelEffect2[j].Bounds = r;
				r.Left = r.Right+2;
				r.Width = 44;
				this.fieldEffect2[j].Bounds = r;

				r.Offset(0, -30);
			}

			r = rect;
			r.Bottom = r.Top-50;
			r.Height = 1;
			this.separator.Bounds = r;

			r.Left += 20;
			r.Width = 20;
			r.Bottom -= 11;
			r.Height = 12;
			this.swapArrow.Bounds = r;
		}
		
		// Le type a été changé.
		private void HandleTypeChanged(object sender)
		{
			if ( this.ignoreChanged )  return;

			if ( !this.isExtendedSize )
			{
				this.fieldType[1].SelectedIndex = this.fieldType[0].SelectedIndex;
			}

			for ( int j=0 ; j<2 ; j++ )
			{
				if ( this.isExtendedSize && sender != this.fieldType[j] )  continue;

				// Met les valeurs par défaut correspondant au type choisi.
				Properties.ArrowType type = Properties.Arrow.ConvType(this.fieldType[j].SelectedIndex);
				bool enableRadius, enable1, enable2;
				double effect1, min1, max1;
				double effect2, min2, max2;
				Properties.Arrow.GetFieldsParam(type, out enableRadius, out enable1, out effect1, out min1, out max1, out enable2, out effect2, out min2, out max2);
				this.fieldEffect1[j].InternalValue = (decimal) effect1*100;
				this.fieldEffect2[j].InternalValue = (decimal) effect2*100;
			}

			this.EnableWidgets();
			this.OnChanged();
		}

		// Un champ a été changé.
		private void HandleFieldChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}

		private void HandleSwapArrowClicked(object sender, MessageEventArgs e)
		{
			this.ignoreChanged = true;

			for ( int j=0 ; j<2 ; j++ )
			{
				this.fieldEffect1[j].InternalMinValue = -1000.0M;
				this.fieldEffect1[j].InternalMaxValue =  1000.0M;
				this.fieldEffect2[j].InternalMinValue = -1000.0M;
				this.fieldEffect2[j].InternalMaxValue =  1000.0M;
			}

			int type = this.fieldType[0].SelectedIndex;
			this.fieldType[0].SelectedIndex = this.fieldType[1].SelectedIndex;
			this.fieldType[1].SelectedIndex = type;

			decimal len = this.fieldLength[0].InternalValue;
			this.fieldLength[0].InternalValue = this.fieldLength[1].InternalValue;
			this.fieldLength[1].InternalValue = len;

			decimal ef1 = this.fieldEffect1[0].InternalValue;
			this.fieldEffect1[0].InternalValue = this.fieldEffect1[1].InternalValue;
			this.fieldEffect1[1].InternalValue = ef1;

			decimal ef2 = this.fieldEffect2[0].InternalValue;
			this.fieldEffect2[0].InternalValue = this.fieldEffect2[1].InternalValue;
			this.fieldEffect2[1].InternalValue = ef2;

			this.ignoreChanged = false;

			this.EnableWidgets();
			this.OnChanged();
		}


		protected StaticText				label;
		protected TextFieldCombo[]			fieldType;
		protected TextFieldReal[]			fieldLength;
		protected TextFieldReal[]			fieldEffect1;
		protected TextFieldReal[]			fieldEffect2;
		protected StaticText[]				labelLength;
		protected StaticText[]				labelEffect1;
		protected StaticText[]				labelEffect2;
		protected Separator					separator;
		protected IconButton				swapArrow;
	}
}
