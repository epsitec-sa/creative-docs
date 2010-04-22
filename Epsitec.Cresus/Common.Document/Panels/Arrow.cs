using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe Arrow permet de choisir un type d'extrémité.
	/// </summary>
	public class Arrow : Abstract
	{
		public Arrow(Document document) : base(document)
		{
			this.grid         = new RadioIconGrid[2];
			this.fieldLength  = new Widgets.TextFieldLabel[2];
			this.fieldEffect1 = new Widgets.TextFieldLabel[2];
			this.fieldEffect2 = new Widgets.TextFieldLabel[2];

			int index = 1;
			for ( int j=0 ; j<2 ; j++ )
			{
				this.grid[j] = new RadioIconGrid(this);
				this.grid[j].SelectionChanged += HandleTypeChanged;
				this.grid[j].TabIndex = index++;
				this.grid[j].TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.AddRadioIcon(j, Properties.ArrowType.Right, false);
				this.AddRadioIcon(j, Properties.ArrowType.ArrowSimply, false);
				this.AddRadioIcon(j, Properties.ArrowType.ArrowOutline, true);

				this.AddRadioIcon(j, Properties.ArrowType.ArrowFantasy1, false);
				this.AddRadioIcon(j, Properties.ArrowType.ArrowCurve1, false);
				this.AddRadioIcon(j, Properties.ArrowType.ArrowCurve2, true);

				this.AddRadioIcon(j, Properties.ArrowType.Slash, false);
				this.AddRadioIcon(j, Properties.ArrowType.Dot, false);
				this.AddRadioIcon(j, Properties.ArrowType.Square, false);
				this.AddRadioIcon(j, Properties.ArrowType.Diamond, false);

				this.fieldLength[j] = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
				this.fieldLength[j].LabelShortText = Res.Strings.Panel.Arrow.Short.Length;
				this.fieldLength[j].LabelLongText  = Res.Strings.Panel.Arrow.Long.Length;
				this.fieldLength[j].TextFieldReal.FactorMinRange = 0.0M;
				this.fieldLength[j].TextFieldReal.FactorMaxRange = 0.1M;
				this.fieldLength[j].TextFieldReal.FactorStep = 1.0M;
				this.document.Modifier.AdaptTextFieldRealDimension(this.fieldLength[j].TextFieldReal);
				this.fieldLength[j].TextFieldReal.EditionAccepted += this.HandleFieldChanged;
				this.fieldLength[j].TabIndex = index++;
				this.fieldLength[j].TabNavigationMode = TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(this.fieldLength[j], Res.Strings.Panel.Arrow.Tooltip.Length);

				this.fieldEffect1[j] = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
				this.fieldEffect1[j].LabelShortText = Res.Strings.Panel.Arrow.Short.Effect1;
				this.fieldEffect1[j].LabelLongText  = Res.Strings.Panel.Arrow.Long.Effect1;
				this.document.Modifier.AdaptTextFieldRealScalar(this.fieldEffect1[j].TextFieldReal);
				this.fieldEffect1[j].TextFieldReal.InternalMinValue = -100;
				this.fieldEffect1[j].TextFieldReal.InternalMaxValue = 200;
				this.fieldEffect1[j].TextFieldReal.TextSuffix = "%";
				this.fieldEffect1[j].TextFieldReal.EditionAccepted += this.HandleFieldChanged;
				this.fieldEffect1[j].TabIndex = index++;
				this.fieldEffect1[j].TabNavigationMode = TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(this.fieldEffect1[j], Res.Strings.Panel.Arrow.Tooltip.Effect1);

				this.fieldEffect2[j] = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
				this.fieldEffect2[j].LabelShortText = Res.Strings.Panel.Arrow.Short.Effect2;
				this.fieldEffect2[j].LabelLongText  = Res.Strings.Panel.Arrow.Long.Effect2;
				this.document.Modifier.AdaptTextFieldRealScalar(this.fieldEffect2[j].TextFieldReal);
				this.fieldEffect2[j].TextFieldReal.InternalMinValue = -100;
				this.fieldEffect2[j].TextFieldReal.InternalMaxValue = 200;
				this.fieldEffect2[j].TextFieldReal.TextSuffix = "%";
				this.fieldEffect2[j].TextFieldReal.EditionAccepted += this.HandleFieldChanged;
				this.fieldEffect2[j].TabIndex = index++;
				this.fieldEffect2[j].TabNavigationMode = TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(this.fieldEffect2[j], Res.Strings.Panel.Arrow.Tooltip.Effect2);
			}

			this.separator1 = new Separator(this);
			this.separator2 = new Separator(this);

			this.swapArrow = new IconButton(this);
			this.swapArrow.IconUri = Misc.Icon("SwapData");
			this.swapArrow.Clicked += this.HandleSwapArrowClicked;
			ToolTip.Default.SetToolTip(this.swapArrow, Res.Strings.Panel.Arrow.Tooltip.Swap);

			this.isNormalAndExtended = true;
		}
		
		protected void AddRadioIcon(int j, Properties.ArrowType type, bool endOfLine)
		{
			this.grid[j].AddRadioIcon(Misc.Icon(Properties.Arrow.GetIconText(type)), Properties.Arrow.GetName(type), (int)type, endOfLine);
		}

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				for ( int j=0 ; j<2 ; j++ )
				{
					this.grid[j].SelectionChanged -= HandleTypeChanged;
					this.fieldLength[j].TextFieldReal.EditionAccepted -= this.HandleFieldChanged;
					this.fieldEffect1[j].TextFieldReal.EditionAccepted -= this.HandleFieldChanged;
					this.fieldEffect2[j].TextFieldReal.EditionAccepted -= this.HandleFieldChanged;
					this.grid[j] = null;
					this.fieldLength[j] = null;
					this.fieldEffect1[j] = null;
					this.fieldEffect2[j] = null;
				}
				this.swapArrow.Clicked -= this.HandleSwapArrowClicked;

				this.separator1 = null;
				this.separator2 = null;
				this.swapArrow = null;
			}
			
			base.Dispose(disposing);
		}

		
		public override double DefaultHeight
		{
			//	Retourne la hauteur standard.
			get
			{
				double h = this.LabelHeight;

				if ( this.isExtendedSize )  // panneau étendu ?
				{
					if ( this.IsLabelProperties )  // étendu/détails ?
					{
						h += 52*2;
						h += this.GetTotalTextField(0)*25;
						h += this.GetTotalTextField(1)*25;
					}
					else	// étendu/compact ?
					{
						h += 74*2;
					}
				}
				else	// panneau réduit ?
				{
					h += 30;
				}

				return h;
			}
		}

		protected override void PropertyToWidgets()
		{
			//	Propriété -> widgets.
			base.PropertyToWidgets();

			Properties.Arrow p = this.property as Properties.Arrow;
			if ( p == null )  return;

			this.ignoreChanged = true;

			for ( int j=0 ; j<2 ; j++ )
			{
				this.grid[j].SelectedValue = (int) p.GetArrowType(j^1);
				this.fieldLength[j].TextFieldReal.InternalValue  = (decimal) p.GetLength(j^1);
				this.fieldEffect1[j].TextFieldReal.InternalValue = (decimal) p.GetEffect1(j^1)*100;
				this.fieldEffect2[j].TextFieldReal.InternalValue = (decimal) p.GetEffect2(j^1)*100;
			}

			this.EnableWidgets();
			this.ignoreChanged = false;
		}

		protected override void WidgetsToProperty()
		{
			//	Widgets -> propriété.
			Properties.Arrow p = this.property as Properties.Arrow;
			if ( p == null )  return;

			for ( int j=0 ; j<2 ; j++ )
			{
				p.SetArrowType(j^1, (Properties.ArrowType) this.grid[j].SelectedValue);
				p.SetLength(j^1,  (double) this.fieldLength[j].TextFieldReal.InternalValue);
				p.SetEffect1(j^1, (double) this.fieldEffect1[j].TextFieldReal.InternalValue/100);
				p.SetEffect2(j^1, (double) this.fieldEffect2[j].TextFieldReal.InternalValue/100);
			}
		}

		protected void EnableWidgets()
		{
			//	Grise les widgets nécessaires.
			Properties.Arrow p = this.property as Properties.Arrow;

			//	Initialise les min/max en fonction du type choisi.
			for ( int j=0 ; j<2 ; j++ )
			{
				Properties.ArrowType type = (Properties.ArrowType) this.grid[j].SelectedValue;
				bool enableRadius, enable1, enable2;
				double effect1, min1, max1;
				double effect2, min2, max2;
				p.GetFieldsParam(type, out enableRadius, out enable1, out effect1, out min1, out max1, out enable2, out effect2, out min2, out max2);

				this.fieldEffect1[j].TextFieldReal.InternalMinValue = (decimal) min1*100;
				this.fieldEffect1[j].TextFieldReal.InternalMaxValue = (decimal) max1*100;
				this.fieldEffect2[j].TextFieldReal.InternalMinValue = (decimal) min2*100;
				this.fieldEffect2[j].TextFieldReal.InternalMaxValue = (decimal) max2*100;

				this.grid[j].Enable = (this.isExtendedSize || j== 0);
				this.fieldLength[j].Enable = ((this.isExtendedSize || j== 0) && enableRadius);
				this.fieldEffect1[j].Enable = (this.isExtendedSize && enable1);
				this.fieldEffect2[j].Enable = (this.isExtendedSize && enable2);

				this.grid[j].Visibility = (this.isExtendedSize || j== 0);

				if ( this.isExtendedSize && this.IsLabelProperties )
				{
					int n = this.GetTotalTextField(j);
					this.fieldLength[j].Visibility = (n >= 1);
					this.fieldEffect1[j].Visibility = (n >= 2);
					this.fieldEffect2[j].Visibility = (n >= 3);
				}
				else
				{
					this.fieldLength[j].Visibility = (this.isExtendedSize || j== 0);
					this.fieldEffect1[j].Visibility = (this.isExtendedSize);
					this.fieldEffect2[j].Visibility = (this.isExtendedSize);
				}
			}
		}

		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.grid == null )  return;

			this.EnableWidgets();

			Rectangle rect = this.UsefulZone;
			Rectangle r;
			rect.Bottom = rect.Top-64;

			double ps = 0;
			for ( int j=0 ; j<2 ; j++ )
			{
				if ( j == 1 )
				{
					ps = rect.Top;
				}

				if ( this.isExtendedSize && this.IsLabelProperties )
				{
					r = rect;
					r.Width = 22*6;
					r.Bottom = r.Top-22*2;
					r.Inflate(1);
					this.grid[j].EnableEndOfLine = false;
					this.grid[j].SetManualBounds(r);
					rect.Offset(0, -47);

					int n = this.GetTotalTextField(j);

					if ( n >= 1 )
					{
						r = rect;
						r.Bottom = r.Top-20;
						r.Left = rect.Left;
						r.Right = rect.Right;
						this.fieldLength[j].SetManualBounds(r);
						rect.Offset(0, -25);
					}

					if ( n >= 2 )
					{
						r = rect;
						r.Bottom = r.Top-20;
						r.Left = rect.Left;
						r.Right = rect.Right;
						this.fieldEffect1[j].SetManualBounds(r);
						rect.Offset(0, -25);
					}

					if ( n >= 3 )
					{
						r = rect;
						r.Bottom = r.Top-20;
						r.Left = rect.Left;
						r.Right = rect.Right;
						this.fieldEffect2[j].SetManualBounds(r);
						rect.Offset(0, -25);
					}

					rect.Offset(0, -5);
				}
				else
				{
					r = rect;
					r.Width = 22*4;
					if ( !this.isExtendedSize )
					{
						r.Bottom = r.Top-20;
					}
					r.Inflate(1);
					this.grid[j].EnableEndOfLine = !(this.isExtendedSize && this.IsLabelProperties);
					this.grid[j].SetManualBounds(r);

					r = rect;
					r.Bottom = r.Top-20;
					r.Left = r.Right-Widgets.TextFieldLabel.ShortWidth;
					r.Width = Widgets.TextFieldLabel.ShortWidth;
					this.fieldLength[j].SetManualBounds(r);

					r.Offset(0, -22);
					r.Left = r.Right-Widgets.TextFieldLabel.ShortWidth;
					r.Width = Widgets.TextFieldLabel.ShortWidth;
					this.fieldEffect1[j].SetManualBounds(r);

					r.Offset(0, -22);
					r.Left = r.Right-Widgets.TextFieldLabel.ShortWidth;
					r.Width = Widgets.TextFieldLabel.ShortWidth;
					this.fieldEffect2[j].SetManualBounds(r);

					rect.Offset(0, -74);
				}
			}

			r = this.UsefulZone;
			r.Left -= 5;
			r.Width = 5+90;
			r.Bottom = ps+4;
			r.Height = 1;
			this.separator1.SetManualBounds(r);
			r.Right = rect.Right+5;
			r.Left = rect.Left+90+20;
			this.separator2.SetManualBounds(r);

			r.Left = rect.Left+90;
			r.Width = 20;
			r.Bottom -= 6;
			r.Height = 12;
			this.swapArrow.SetManualBounds(r);
		}

		protected int GetTotalTextField(int j)
		{
			//	Retourne le nombre de lignes éditables selon le type.
			Properties.ArrowType type = (Properties.ArrowType) this.grid[j].SelectedValue;

			if ( type == Properties.ArrowType.Right        )  return 0;
			if ( type == Properties.ArrowType.ArrowOutline )  return 2;
			if ( type == Properties.ArrowType.Slash        )  return 2;
			if ( type == Properties.ArrowType.Dot          )  return 1;
			if ( type == Properties.ArrowType.Square       )  return 1;
			if ( type == Properties.ArrowType.Diamond      )  return 1;
			return 3;
		}
		
		private void HandleTypeChanged(object sender)
		{
			//	Le type a été changé.
			if ( this.ignoreChanged )  return;

			if ( this.IsLabelProperties )
			{
				this.HeightChanged();
			}

			Properties.Arrow p = this.property as Properties.Arrow;

			Properties.ArrowType[] iType = new Properties.ArrowType[2];
			for ( int j=0 ; j<2 ; j++ )
			{
				iType[j] = p.GetArrowType(j^1);
			}

			for ( int j=0 ; j<2 ; j++ )
			{
				if ( sender != this.grid[j] )  continue;

				Properties.ArrowType type = (Properties.ArrowType) this.grid[j].SelectedValue;

				double val = (double) this.fieldLength[j].TextFieldReal.InternalValue;
				val /= Arrow.TypeFactor(iType[j]);
				val *= Arrow.TypeFactor(type);
				this.fieldLength[j].TextFieldReal.InternalValue = (decimal) val;

				//	Met les valeurs par défaut correspondant au type choisi.
				bool enableRadius, enable1, enable2;
				double effect1, min1, max1;
				double effect2, min2, max2;
				p.GetFieldsParam(type, out enableRadius, out enable1, out effect1, out min1, out max1, out enable2, out effect2, out min2, out max2);
				this.fieldEffect1[j].TextFieldReal.InternalValue = (decimal) effect1*100;
				this.fieldEffect2[j].TextFieldReal.InternalValue = (decimal) effect2*100;
			}

			this.EnableWidgets();
			this.OnChanged();
		}

		protected static double TypeFactor(Properties.ArrowType type)
		{
			//	Retourne le facteur de réduction selon le type de flèche.
			//	L'idée est de réduire les points par rapport aux flèches.
			if ( type == Properties.ArrowType.Slash   )  return 0.5;
			if ( type == Properties.ArrowType.Dot     )  return 0.5;
			if ( type == Properties.ArrowType.Square  )  return 0.5;
			if ( type == Properties.ArrowType.Diamond )  return 0.5;
			return 1.0;
		}

		private void HandleFieldChanged(object sender)
		{
			//	Un champ a été changé.
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}

		private void HandleSwapArrowClicked(object sender, MessageEventArgs e)
		{
			this.ignoreChanged = true;

			for ( int j=0 ; j<2 ; j++ )
			{
				this.fieldEffect1[j].TextFieldReal.InternalMinValue = -1000.0M;
				this.fieldEffect1[j].TextFieldReal.InternalMaxValue =  1000.0M;
				this.fieldEffect2[j].TextFieldReal.InternalMinValue = -1000.0M;
				this.fieldEffect2[j].TextFieldReal.InternalMaxValue =  1000.0M;
			}

			int type = this.grid[0].SelectedValue;
			this.grid[0].SelectedValue = this.grid[1].SelectedValue;
			this.grid[1].SelectedValue = type;

			decimal len = this.fieldLength[0].TextFieldReal.InternalValue;
			this.fieldLength[0].TextFieldReal.InternalValue = this.fieldLength[1].TextFieldReal.InternalValue;
			this.fieldLength[1].TextFieldReal.InternalValue = len;

			decimal ef1 = this.fieldEffect1[0].TextFieldReal.InternalValue;
			this.fieldEffect1[0].TextFieldReal.InternalValue = this.fieldEffect1[1].TextFieldReal.InternalValue;
			this.fieldEffect1[1].TextFieldReal.InternalValue = ef1;

			decimal ef2 = this.fieldEffect2[0].TextFieldReal.InternalValue;
			this.fieldEffect2[0].TextFieldReal.InternalValue = this.fieldEffect2[1].TextFieldReal.InternalValue;
			this.fieldEffect2[1].TextFieldReal.InternalValue = ef2;

			this.ignoreChanged = false;

			if ( this.IsLabelProperties )
			{
				this.UpdateClientGeometry();
			}

			this.EnableWidgets();
			this.OnChanged();
		}


		protected RadioIconGrid[]			grid;
		protected Widgets.TextFieldLabel[]	fieldLength;
		protected Widgets.TextFieldLabel[]	fieldEffect1;
		protected Widgets.TextFieldLabel[]	fieldEffect2;
		protected Separator					separator1;
		protected Separator					separator2;
		protected IconButton				swapArrow;
	}
}
