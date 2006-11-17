using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe Regular permet de choisir un type de polygone régulier.
	/// </summary>
	public class Regular : Abstract
	{
		public Regular(Document document) : base(document)
		{
			this.grid = new RadioIconGrid(this);
			this.grid.SelectionChanged += new EventHandler(HandleTypeChanged);
			this.grid.TabIndex = 0;
			this.grid.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.AddRadioIcon(false);
			this.AddRadioIcon(true);

			this.fieldNbFaces = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldNbFaces.LabelShortText = Res.Strings.Panel.Regular.Short.Faces;
			this.fieldNbFaces.LabelLongText  = Res.Strings.Panel.Regular.Long.Faces;
			this.document.Modifier.AdaptTextFieldRealScalar(this.fieldNbFaces.TextFieldReal);
			this.fieldNbFaces.TextFieldReal.InternalMinValue = 3;
			this.fieldNbFaces.TextFieldReal.InternalMaxValue = 24;
			this.fieldNbFaces.TextFieldReal.Step = 1;
			this.fieldNbFaces.TextFieldReal.EditionAccepted += new EventHandler(this.HandleFieldChanged);
			this.fieldNbFaces.TabIndex = 1;
			this.fieldNbFaces.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldNbFaces, Res.Strings.Panel.Regular.Tooltip.Faces);

			this.fieldDeep = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldDeep.LabelShortText = Res.Strings.Panel.Regular.Short.Deep;
			this.fieldDeep.LabelLongText  = Res.Strings.Panel.Regular.Long.Deep;
			this.document.Modifier.AdaptTextFieldRealScalar(this.fieldDeep.TextFieldReal);
			this.fieldDeep.TextFieldReal.InternalMinValue = 0;
			this.fieldDeep.TextFieldReal.InternalMaxValue = 100;
			this.fieldDeep.TextFieldReal.Step = 5;
			this.fieldDeep.TextFieldReal.TextSuffix = "%";
			this.fieldDeep.TextFieldReal.EditionAccepted += new EventHandler(this.HandleFieldChanged);
			this.fieldDeep.TabIndex = 3;
			this.fieldDeep.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldDeep, Res.Strings.Panel.Regular.Tooltip.Deep);

			this.isNormalAndExtended = true;
		}
		
		protected void AddRadioIcon(bool type)
		{
			this.grid.AddRadioIcon(Misc.Icon(Properties.Regular.GetIconText(type)), Properties.Regular.GetName(type), type?1:0, false);
		}

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.grid.SelectionChanged -= new EventHandler(HandleTypeChanged);
				this.fieldNbFaces.TextFieldReal.EditionAccepted -= new EventHandler(this.HandleFieldChanged);
				this.fieldDeep.TextFieldReal.EditionAccepted -= new EventHandler(this.HandleFieldChanged);

				this.grid = null;
				this.fieldNbFaces = null;
				this.fieldDeep = null;
			}
			
			base.Dispose(disposing);
		}

		
		public override double DefaultHeight
		{
			//	Retourne la hauteur standard.
			get
			{
				return ( this.isExtendedSize ? this.LabelHeight+55 : this.LabelHeight+30 );
			}
		}

		protected override void PropertyToWidgets()
		{
			//	Propriété -> widgets.
			base.PropertyToWidgets();

			Properties.Regular p = this.property as Properties.Regular;
			if ( p == null )  return;

			this.ignoreChanged = true;

			this.grid.SelectedValue = p.Star ? 1 : 0;
			this.fieldNbFaces.TextFieldReal.InternalValue = p.NbFaces;
			this.fieldDeep.TextFieldReal.InternalValue = (decimal) p.Deep*100;

			this.EnableWidgets();
			this.ignoreChanged = false;
		}

		protected override void WidgetsToProperty()
		{
			//	Widgets -> propriété.
			Properties.Regular p = this.property as Properties.Regular;
			if ( p == null )  return;

			p.Star = (this.grid.SelectedValue == 1);
			p.NbFaces = (int)this.fieldNbFaces.TextFieldReal.InternalValue;
			p.Deep = (double) this.fieldDeep.TextFieldReal.InternalValue/100;
		}

		protected void EnableWidgets()
		{
			//	Grise les widgets nécessaires.
			bool star = (this.grid.SelectedValue == 1);
			this.fieldDeep.Enable = (this.isExtendedSize && star);
		}

		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.grid == null )  return;

			this.EnableWidgets();

			Rectangle rect = this.UsefulZone;

			Rectangle r = rect;
			r.Bottom = r.Top-20;
			r.Width = 22*2;
			r.Inflate(1);
			this.grid.SetManualBounds(r);

			if ( this.isExtendedSize && this.IsLabelProperties )
			{
				r = rect;
				r.Bottom = r.Top-20;
				r.Left = rect.Left+22*2;
				r.Right = rect.Right;
				this.fieldNbFaces.SetManualBounds(r);

				rect.Top = r.Bottom-5;
				rect.Bottom = rect.Top-20;
				r = rect;
				r.Left = rect.Left;
				r.Right = rect.Right;
				this.fieldDeep.SetManualBounds(r);
			}
			else
			{
				r = rect;
				r.Bottom = r.Top-20;
				r.Left = rect.Right-Widgets.TextFieldLabel.ShortWidth;
				r.Right = rect.Right;
				this.fieldNbFaces.SetManualBounds(r);

				rect.Top = r.Bottom-5;
				rect.Bottom = rect.Top-20;
				r = rect;
				r.Left = rect.Right-Widgets.TextFieldLabel.ShortWidth;
				r.Right = rect.Right;
				this.fieldDeep.SetManualBounds(r);
			}
		}
		
		private void HandleFieldChanged(object sender)
		{
			//	Un champ a été changé.
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}

		private void HandleTypeChanged(object sender)
		{
			//	Un champ a été changé.
			if ( this.ignoreChanged )  return;
			this.EnableWidgets();
			this.OnChanged();
		}


		protected RadioIconGrid				grid;
		protected Widgets.TextFieldLabel	fieldNbFaces;
		protected Widgets.TextFieldLabel	fieldDeep;
	}
}
