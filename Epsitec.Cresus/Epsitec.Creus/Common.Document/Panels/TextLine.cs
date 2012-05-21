using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe TextLine permet de choisir un mode de justification.
	/// </summary>
	public class TextLine : Abstract
	{
		public TextLine(Document document) : base(document)
		{
			this.gridHorizontal = new RadioIconGrid(this);
			this.gridHorizontal.SelectionChanged += HandleTypeChanged;
			this.gridHorizontal.TabIndex = 0;
			this.gridHorizontal.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.AddRadioIcon(Properties.JustifHorizontal.Left);
			this.AddRadioIcon(Properties.JustifHorizontal.Center);
			this.AddRadioIcon(Properties.JustifHorizontal.Right);
			this.AddRadioIcon(Properties.JustifHorizontal.Stretch);

			this.fieldOffset = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldOffset.LabelShortText = Res.Strings.Panel.TextLine.Short.Offset;
			this.fieldOffset.LabelLongText  = Res.Strings.Panel.TextLine.Long.Offset;
			this.document.Modifier.AdaptTextFieldRealScalar(this.fieldOffset.TextFieldReal);
			this.fieldOffset.TextFieldReal.InternalMinValue =  0.0M;
			this.fieldOffset.TextFieldReal.InternalMaxValue = 70.0M;
			this.fieldOffset.TextFieldReal.Step = 5.0M;
			this.fieldOffset.TextFieldReal.TextSuffix = "%";
			this.fieldOffset.TextFieldReal.EditionAccepted += this.HandleFieldChanged;
			this.fieldOffset.TabIndex = 20;
			this.fieldOffset.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldOffset, Res.Strings.Panel.TextLine.Tooltip.Offset);

			this.fieldAdd = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldAdd.LabelShortText = Res.Strings.Panel.TextLine.Short.Add;
			this.fieldAdd.LabelLongText  = Res.Strings.Panel.TextLine.Long.Add;
			this.document.Modifier.AdaptTextFieldRealScalar(this.fieldAdd.TextFieldReal);
			this.fieldAdd.TextFieldReal.InternalMinValue = -20.0M;
			this.fieldAdd.TextFieldReal.InternalMaxValue = 100.0M;
			this.fieldAdd.TextFieldReal.Step = 1.0M;
			this.fieldAdd.TextFieldReal.TextSuffix = "%";
			this.fieldAdd.TextFieldReal.EditionAccepted += this.HandleFieldChanged;
			this.fieldAdd.TabIndex = 21;
			this.fieldAdd.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldAdd, Res.Strings.Panel.TextLine.Tooltip.Add);

			this.isNormalAndExtended = true;
		}
		
		protected void AddRadioIcon(Properties.JustifHorizontal type)
		{
			this.gridHorizontal.AddRadioIcon(Misc.Icon(Properties.Justif.GetIconText(type)), Properties.Justif.GetName(type), (int)type, false);
		}

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.gridHorizontal.SelectionChanged -= HandleTypeChanged;
				this.fieldOffset.TextFieldReal.EditionAccepted -= this.HandleFieldChanged;
				this.fieldAdd.TextFieldReal.EditionAccepted -= this.HandleFieldChanged;

				this.gridHorizontal = null;
				this.fieldOffset = null;
				this.fieldAdd = null;
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
						h += 80;
					}
					else	// étendu/compact ?
					{
						h += 55;
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

			Properties.TextLine p = this.property as Properties.TextLine;
			if ( p == null )  return;

			this.ignoreChanged = true;

			this.gridHorizontal.SelectedValue = (int) p.Horizontal;
			this.fieldOffset.TextFieldReal.InternalValue = (decimal) p.Offset*100;
			this.fieldAdd.TextFieldReal.InternalValue    = (decimal) p.Add*100;

			this.EnableWidgets();
			this.ignoreChanged = false;
		}

		protected override void WidgetsToProperty()
		{
			//	Widgets -> propriété.
			Properties.TextLine p = this.property as Properties.TextLine;
			if ( p == null )  return;

			p.Horizontal = (Properties.JustifHorizontal) this.gridHorizontal.SelectedValue;
			p.Offset     = (double) this.fieldOffset.TextFieldReal.InternalValue/100;
			p.Add        = (double) this.fieldAdd.TextFieldReal.InternalValue/100;
		}


		protected void EnableWidgets()
		{
			//	Grise les widgets nécessaires.
			this.fieldOffset.Visibility = (this.isExtendedSize);
			this.fieldAdd.Visibility = (this.isExtendedSize);
		}

		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.gridHorizontal == null )  return;

			this.EnableWidgets();

			Rectangle rect = this.UsefulZone;

			Rectangle r = rect;
			r.Bottom = r.Top-20;
			r.Width = 22*4;
			r.Inflate(1);
			this.gridHorizontal.SetManualBounds(r);

			if ( this.isExtendedSize && this.IsLabelProperties )
			{
				r.Offset(0, -25);
				r.Left = rect.Left;
				r.Right = rect.Right;
				this.fieldOffset.SetManualBounds(r);

				r.Offset(0, -25);
				this.fieldAdd.SetManualBounds(r);
			}
			else
			{
				r.Offset(0, -25);
				r.Left = rect.Right-Widgets.TextFieldLabel.ShortWidth-Widgets.TextFieldLabel.ShortWidth-10;
				r.Width = Widgets.TextFieldLabel.ShortWidth+10;
				this.fieldOffset.SetManualBounds(r);
				r.Left = r.Right;
				r.Width = Widgets.TextFieldLabel.ShortWidth;
				this.fieldAdd.SetManualBounds(r);
			}
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


		protected RadioIconGrid				gridHorizontal;
		protected Widgets.TextFieldLabel	fieldOffset;
		protected Widgets.TextFieldLabel	fieldAdd;
	}
}
