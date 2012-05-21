using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe Corner permet de choisir un type de coin.
	/// </summary>
	public class Corner : Abstract
	{
		public Corner(Document document) : base(document)
		{
			this.grid = new RadioIconGrid(this);
			this.grid.SelectionChanged += this.HandleTypeChanged;
			this.grid.TabIndex = 1;
			this.grid.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.AddRadioIcon(Properties.CornerType.Right, false);
			this.AddRadioIcon(Properties.CornerType.Round, false);
			this.AddRadioIcon(Properties.CornerType.Bevel, false);
			this.AddRadioIcon(Properties.CornerType.Line31, false);
			this.AddRadioIcon(Properties.CornerType.Line41, false);

			this.AddRadioIcon(Properties.CornerType.Line42, false);
			this.AddRadioIcon(Properties.CornerType.Line51, false);
			this.AddRadioIcon(Properties.CornerType.Line61, false);
			this.AddRadioIcon(Properties.CornerType.Line62, false);
			this.AddRadioIcon(Properties.CornerType.Curve21, false);
			
			this.AddRadioIcon(Properties.CornerType.Curve22, false);
			this.AddRadioIcon(Properties.CornerType.Curve31, false);
			this.AddRadioIcon(Properties.CornerType.Fantasy51, false);
			this.AddRadioIcon(Properties.CornerType.Fantasy61, false);
			this.AddRadioIcon(Properties.CornerType.Fantasy62, false);

			this.fieldRadius = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldRadius.LabelShortText = Res.Strings.Panel.Corner.Short.Radius;
			this.fieldRadius.LabelLongText  = Res.Strings.Panel.Corner.Long.Radius;
			this.fieldRadius.TextFieldReal.FactorMinRange = 0.0M;
			this.fieldRadius.TextFieldReal.FactorMaxRange = 0.1M;
			this.fieldRadius.TextFieldReal.FactorStep = 1.0M;
			this.document.Modifier.AdaptTextFieldRealDimension(this.fieldRadius.TextFieldReal);
			this.fieldRadius.TextFieldReal.EditionAccepted += this.HandleFieldChanged;
			this.fieldRadius.TabIndex = 2;
			this.fieldRadius.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldRadius, Res.Strings.Panel.Corner.Tooltip.Radius);

			this.fieldEffect1 = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldEffect1.LabelShortText = Res.Strings.Panel.Corner.Short.Effect1;
			this.fieldEffect1.LabelLongText  = Res.Strings.Panel.Corner.Long.Effect1;
			this.document.Modifier.AdaptTextFieldRealScalar(this.fieldEffect1.TextFieldReal);
			this.fieldEffect1.TextFieldReal.InternalMinValue = -100;
			this.fieldEffect1.TextFieldReal.InternalMaxValue = 200;
			this.fieldEffect1.TextFieldReal.Step = 5;
			this.fieldEffect1.TextFieldReal.TextSuffix = "%";
			this.fieldEffect1.TextFieldReal.EditionAccepted += this.HandleFieldChanged;
			this.fieldEffect1.TabIndex = 3;
			this.fieldEffect1.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldEffect1, Res.Strings.Panel.Corner.Tooltip.Effect1);

			this.fieldEffect2 = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldEffect2.LabelShortText = Res.Strings.Panel.Corner.Short.Effect2;
			this.fieldEffect2.LabelLongText  = Res.Strings.Panel.Corner.Long.Effect2;
			this.document.Modifier.AdaptTextFieldRealScalar(this.fieldEffect2.TextFieldReal);
			this.fieldEffect2.TextFieldReal.InternalMinValue = -100;
			this.fieldEffect2.TextFieldReal.InternalMaxValue = 200;
			this.fieldEffect2.TextFieldReal.Step = 5;
			this.fieldEffect2.TextFieldReal.TextSuffix = "%";
			this.fieldEffect2.TextFieldReal.EditionAccepted += this.HandleFieldChanged;
			this.fieldEffect2.TabIndex = 4;
			this.fieldEffect2.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldEffect2, Res.Strings.Panel.Corner.Tooltip.Effect2);

			this.isNormalAndExtended = true;
		}
		
		protected void AddRadioIcon(Properties.CornerType type, bool endOfLine)
		{
			this.grid.AddRadioIcon(Misc.Icon(Properties.Corner.GetIconText(type)), Properties.Corner.GetName(type), (int)type, endOfLine);
		}

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.grid.SelectionChanged -= HandleTypeChanged;
				this.fieldRadius.TextFieldReal.EditionAccepted -= this.HandleFieldChanged;
				this.fieldEffect1.TextFieldReal.EditionAccepted -= this.HandleFieldChanged;
				this.fieldEffect2.TextFieldReal.EditionAccepted -= this.HandleFieldChanged;

				this.grid = null;
				this.fieldRadius = null;
				this.fieldEffect1 = null;
				this.fieldEffect2 = null;
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
						Properties.CornerType type = (Properties.CornerType) this.grid.SelectedValue;
						if ( type == Properties.CornerType.Right )
						{
							h += 52;
						}
						else if ( type == Properties.CornerType.Round ||
								  type == Properties.CornerType.Bevel )
						{
							h += 102;
						}
						else
						{
							h += 127;
						}
					}
					else	// étendu/compact ?
					{
						h += 74;
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

			Properties.Corner p = this.property as Properties.Corner;
			if ( p == null )  return;

			this.ignoreChanged = true;

			this.grid.SelectedValue = (int) p.CornerType;
			this.fieldRadius.TextFieldReal.InternalValue  = (decimal) p.Radius;
			this.fieldEffect1.TextFieldReal.InternalValue = (decimal) p.Effect1*100;
			this.fieldEffect2.TextFieldReal.InternalValue = (decimal) p.Effect2*100;

			this.EnableWidgets();
			this.ignoreChanged = false;
		}

		protected override void WidgetsToProperty()
		{
			//	Widgets -> propriété.
			Properties.Corner p = this.property as Properties.Corner;
			if ( p == null )  return;

			p.CornerType = (Properties.CornerType) this.grid.SelectedValue;
			p.Radius  = (double) this.fieldRadius.TextFieldReal.InternalValue;
			p.Effect1 = (double) this.fieldEffect1.TextFieldReal.InternalValue/100;
			p.Effect2 = (double) this.fieldEffect2.TextFieldReal.InternalValue/100;
		}

		protected void EnableWidgets()
		{
			//	Grise les widgets nécessaires.
			//	Initialise les min/max en fonction du type choisi.
			Properties.CornerType type = (Properties.CornerType) this.grid.SelectedValue;
			bool enableRadius, enable1, enable2;
			double effect1, min1, max1;
			double effect2, min2, max2;
			Properties.Corner.GetFieldsParam(type, out enableRadius, out enable1, out effect1, out min1, out max1, out enable2, out effect2, out min2, out max2);

			this.fieldEffect1.TextFieldReal.InternalMinValue = (decimal) min1*100;
			this.fieldEffect1.TextFieldReal.InternalMaxValue = (decimal) max1*100;
			this.fieldEffect2.TextFieldReal.InternalMinValue = (decimal) min2*100;
			this.fieldEffect2.TextFieldReal.InternalMaxValue = (decimal) max2*100;

			this.fieldRadius.Enable = (enableRadius);
			this.fieldEffect1.Enable = (this.isExtendedSize && enable1);
			this.fieldEffect2.Enable = (this.isExtendedSize && enable2);

			this.fieldRadius.Visibility = true;
			this.fieldEffect1.Visibility = (this.isExtendedSize);
			this.fieldEffect2.Visibility = (this.isExtendedSize);
		}

		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.grid == null )  return;

			this.EnableWidgets();

			Rectangle rect = this.UsefulZone;
			Rectangle r = rect;

			if ( this.isExtendedSize && this.IsLabelProperties )
			{
				r.Bottom = r.Top-22*2;
				r.Width = 22*8;
				r.Inflate(1);
				this.grid.SetManualBounds(r);

				r.Top = rect.Top-47;
				r.Bottom = r.Top-20;
				r.Left = rect.Left;
				r.Right = rect.Right;
				this.fieldRadius.SetManualBounds(r);

				r.Offset(0, -25);
				this.fieldEffect1.SetManualBounds(r);

				r.Offset(0, -25);
				this.fieldEffect2.SetManualBounds(r);
			}
			else
			{
				r.Width = 22*5;
				r.Inflate(1);
				this.grid.SetManualBounds(r);

				r = rect;
				r.Bottom = r.Top-20;
				r.Left = rect.Right-Widgets.TextFieldLabel.ShortWidth;
				r.Width = Widgets.TextFieldLabel.ShortWidth;
				this.fieldRadius.SetManualBounds(r);

				r.Offset(0, -22);
				r.Left = rect.Right-Widgets.TextFieldLabel.ShortWidth;
				r.Width = Widgets.TextFieldLabel.ShortWidth;
				this.fieldEffect1.SetManualBounds(r);

				r.Offset(0, -22);
				r.Left = rect.Right-Widgets.TextFieldLabel.ShortWidth;
				r.Width = Widgets.TextFieldLabel.ShortWidth;
				this.fieldEffect2.SetManualBounds(r);
			}
		}
		
		private void HandleTypeChanged(object sender)
		{
			//	Le type a été changé.
			if ( this.ignoreChanged )  return;

			if ( this.IsLabelProperties )
			{
				this.HeightChanged();
			}

			//	Met les valeurs par défaut correspondant au type choisi.
			this.EnableWidgets();

			Properties.CornerType type = (Properties.CornerType) this.grid.SelectedValue;
			bool enableRadius, enable1, enable2;
			double effect1, min1, max1;
			double effect2, min2, max2;
			Properties.Corner.GetFieldsParam(type, out enableRadius, out enable1, out effect1, out min1, out max1, out enable2, out effect2, out min2, out max2);
			this.fieldEffect1.TextFieldReal.InternalValue = (decimal) effect1*100;
			this.fieldEffect2.TextFieldReal.InternalValue = (decimal) effect2*100;

			this.OnChanged();
		}

		private void HandleFieldChanged(object sender)
		{
			//	Un champ a été changé.
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}


		protected RadioIconGrid				grid;
		protected Widgets.TextFieldLabel	fieldRadius;
		protected Widgets.TextFieldLabel	fieldEffect1;
		protected Widgets.TextFieldLabel	fieldEffect2;
	}
}
