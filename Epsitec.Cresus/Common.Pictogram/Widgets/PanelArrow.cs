using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Pictogram.Data;

namespace Epsitec.Common.Pictogram.Widgets
{
	/// <summary>
	/// La classe PanelArrow permet de choisir un type d'extrémité.
	/// </summary>
	public class PanelArrow : AbstractPanel
	{
		public PanelArrow()
		{
			this.label = new StaticText(this);
			this.label.Alignment = Drawing.ContentAlignment.MiddleLeft;

			this.fieldType    = new TextFieldCombo[2];
			this.fieldLength  = new TextFieldSlider[2];
			this.fieldEffect1 = new TextFieldSlider[2];
			this.fieldEffect2 = new TextFieldSlider[2];
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
					ArrowType type = PropertyArrow.ConvType(i);
					if ( type == ArrowType.None )  break;
					this.fieldType[j].Items.Add(PropertyArrow.GetName(type));
				}
				this.fieldType[j].TextChanged += new EventHandler(this.HandleTypeChanged);
				this.fieldType[j].TabIndex = index++;
				this.fieldType[j].TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				this.fieldLength[j] = new TextFieldSlider(this);
				this.fieldLength[j].MinValue = 0;
				this.fieldLength[j].MaxValue = 10;
				this.fieldLength[j].Step = 0.1M;
				this.fieldLength[j].Resolution = 0.1M;
				this.fieldLength[j].TextChanged += new EventHandler(this.HandleFieldChanged);
				this.fieldLength[j].TabIndex = index++;
				this.fieldLength[j].TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				this.fieldEffect1[j] = new TextFieldSlider(this);
				this.fieldEffect1[j].MinValue = -100;
				this.fieldEffect1[j].MaxValue = 200;
				this.fieldEffect1[j].Step = 5;
				this.fieldEffect1[j].TextChanged += new EventHandler(this.HandleFieldChanged);
				this.fieldEffect1[j].TabIndex = index++;
				this.fieldEffect1[j].TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				this.fieldEffect2[j] = new TextFieldSlider(this);
				this.fieldEffect2[j].MinValue = -100;
				this.fieldEffect2[j].MaxValue = 200;
				this.fieldEffect2[j].Step = 5;
				this.fieldEffect2[j].TextChanged += new EventHandler(this.HandleFieldChanged);
				this.fieldEffect2[j].TabIndex = index++;
				this.fieldEffect2[j].TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				this.labelLength[j] = new StaticText(this);
				this.labelLength[j].Text = "L";
				this.labelLength[j].Alignment = Drawing.ContentAlignment.MiddleCenter;

				this.labelEffect1[j] = new StaticText(this);
				this.labelEffect1[j].Text = "A";
				this.labelEffect1[j].Alignment = Drawing.ContentAlignment.MiddleRight;

				this.labelEffect2[j] = new StaticText(this);
				this.labelEffect2[j].Text = "B";
				this.labelEffect2[j].Alignment = Drawing.ContentAlignment.MiddleRight;
			}

			this.separator = new Separator(this);

			this.swapArrow = new IconButton(this);
			this.swapArrow.IconName = @"file:images/swapdata.icon";
			this.swapArrow.Clicked += new MessageEventHandler(this.SwapArrowClicked);

			this.isNormalAndExtended = true;
		}
		
		public PanelArrow(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				for ( int j=0 ; j<2 ; j++ )
				{
					this.fieldType[j].TextChanged -= new EventHandler(this.HandleTypeChanged);
					this.fieldLength[j].TextChanged -= new EventHandler(this.HandleFieldChanged);
					this.fieldEffect1[j].TextChanged -= new EventHandler(this.HandleFieldChanged);
					this.fieldEffect2[j].TextChanged -= new EventHandler(this.HandleFieldChanged);
				}
				this.swapArrow.Clicked -= new MessageEventHandler(this.SwapArrowClicked);
			}
			
			base.Dispose(disposing);
		}

		
		// Retourne la hauteur standard.
		public override double DefaultHeight
		{
			get
			{
				return ( this.extendedSize ? 110 : 30 );
			}
		}

		// Propriété -> widget.
		public override void SetProperty(AbstractProperty property)
		{
			base.SetProperty(property);
			this.label.Text = this.textStyle;

			PropertyArrow p = property as PropertyArrow;
			if ( p == null )  return;

			for ( int j=0 ; j<2 ; j++ )
			{
				this.fieldType[j].SelectedIndex = PropertyArrow.ConvType(p.GetArrowType(j));
				this.fieldLength[j].Value  = (decimal) p.GetLength(j);
				this.fieldEffect1[j].Value = (decimal) p.GetEffect1(j)*100;
				this.fieldEffect2[j].Value = (decimal) p.GetEffect2(j)*100;
			}

			this.EnableWidgets();
		}

		// Widget -> propriété.
		public override AbstractProperty GetProperty()
		{
			PropertyArrow p = new PropertyArrow();
			base.GetProperty(p);

			for ( int j=0 ; j<2 ; j++ )
			{
				p.SetArrowType(j, PropertyArrow.ConvType(this.fieldType[j].SelectedIndex));
				p.SetLength(j,  (double) this.fieldLength[j].Value);
				p.SetEffect1(j, (double) this.fieldEffect1[j].Value/100);
				p.SetEffect2(j, (double) this.fieldEffect2[j].Value/100);
			}

			return p;
		}

		// Grise les widgets nécessaires.
		protected void EnableWidgets()
		{
			// Initialise les min/max en fonction du type choisi.
			for ( int j=0 ; j<2 ; j++ )
			{
				ArrowType type = PropertyArrow.ConvType(this.fieldType[j].SelectedIndex);
				bool enableRadius, enable1, enable2;
				double effect1, min1, max1;
				double effect2, min2, max2;
				PropertyArrow.GetFieldsParam(type, out enableRadius, out enable1, out effect1, out min1, out max1, out enable2, out effect2, out min2, out max2);

				this.fieldEffect1[j].MinValue = (decimal) min1*100;
				this.fieldEffect1[j].MaxValue = (decimal) max1*100;
				this.fieldEffect2[j].MinValue = (decimal) min2*100;
				this.fieldEffect2[j].MaxValue = (decimal) max2*100;

				this.fieldLength[j].SetEnabled(this.extendedSize && enableRadius);
				this.fieldEffect1[j].SetEnabled(this.extendedSize && enable1);
				this.fieldEffect2[j].SetEnabled(this.extendedSize && enable2);
			}
		}

		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.fieldType == null )  return;

			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Deflate(this.extendedZoneWidth, 0);
			rect.Deflate(5);

			Drawing.Rectangle r = rect;
			r.Bottom = r.Top-20;
			r.Right = rect.Right-100;
			this.label.Bounds = r;

			for ( int j=0 ; j<2 ; j++ )
			{
				r.Left = rect.Right-100;
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
			if ( this.ignoreChange )  return;

			if ( !this.extendedSize )
			{
				this.ignoreChange = true;
				this.fieldType[1].SelectedIndex = this.fieldType[0].SelectedIndex;
				this.ignoreChange = false;
			}

			for ( int j=0 ; j<2 ; j++ )
			{
				if ( this.extendedSize && sender != this.fieldType[j] )  continue;

				// Met les valeurs par défaut correspondant au type choisi.
				ArrowType type = PropertyArrow.ConvType(this.fieldType[j].SelectedIndex);
				bool enableRadius, enable1, enable2;
				double effect1, min1, max1;
				double effect2, min2, max2;
				PropertyArrow.GetFieldsParam(type, out enableRadius, out enable1, out effect1, out min1, out max1, out enable2, out effect2, out min2, out max2);
				this.fieldEffect1[j].Value = (decimal) effect1*100;
				this.fieldEffect2[j].Value = (decimal) effect2*100;
			}

			this.EnableWidgets();
			this.OnChanged();
		}

		// Un champ a été changé.
		private void HandleFieldChanged(object sender)
		{
			if ( this.ignoreChange )  return;
			this.OnChanged();
		}

		private void SwapArrowClicked(object sender, MessageEventArgs e)
		{
			this.ignoreChange = true;

			for ( int j=0 ; j<2 ; j++ )
			{
				this.fieldEffect1[j].MinValue = -1000.0M;
				this.fieldEffect1[j].MaxValue =  1000.0M;
				this.fieldEffect2[j].MinValue = -1000.0M;
				this.fieldEffect2[j].MaxValue =  1000.0M;
			}

			int type = this.fieldType[0].SelectedIndex;
			this.fieldType[0].SelectedIndex = this.fieldType[1].SelectedIndex;
			this.fieldType[1].SelectedIndex = type;

			decimal len = this.fieldLength[0].Value;
			this.fieldLength[0].Value = this.fieldLength[1].Value;
			this.fieldLength[1].Value = len;

			decimal ef1 = this.fieldEffect1[0].Value;
			this.fieldEffect1[0].Value = this.fieldEffect1[1].Value;
			this.fieldEffect1[1].Value = ef1;

			decimal ef2 = this.fieldEffect2[0].Value;
			this.fieldEffect2[0].Value = this.fieldEffect2[1].Value;
			this.fieldEffect2[1].Value = ef2;

			this.ignoreChange = false;

			this.EnableWidgets();
			this.OnChanged();
		}


		protected StaticText				label;
		protected TextFieldCombo[]			fieldType;
		protected TextFieldSlider[]			fieldLength;
		protected TextFieldSlider[]			fieldEffect1;
		protected TextFieldSlider[]			fieldEffect2;
		protected StaticText[]				labelLength;
		protected StaticText[]				labelEffect1;
		protected StaticText[]				labelEffect2;
		protected Separator					separator;
		protected IconButton				swapArrow;
		protected bool						ignoreChange = false;
	}
}
