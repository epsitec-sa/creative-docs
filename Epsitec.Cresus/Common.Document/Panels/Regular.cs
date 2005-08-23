using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe Regular permet de choisir un type de polygone régulier.
	/// </summary>
	[SuppressBundleSupport]
	public class Regular : Abstract
	{
		public Regular(Document document) : base(document)
		{
			this.grid = new Widgets.RadioIconGrid(this);
			this.grid.SelectionChanged += new EventHandler(HandleTypeChanged);
			this.grid.TabIndex = 0;
			this.grid.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.AddRadioIcon(false);
			this.AddRadioIcon(true);

			this.fieldNbFaces = new Widgets.TextFieldLabel(this, false);
			this.fieldNbFaces.LabelShortText = Res.Strings.Panel.Regular.Short.Faces;
			this.fieldNbFaces.LabelLongText  = Res.Strings.Panel.Regular.Long.Faces;
			this.document.Modifier.AdaptTextFieldRealScalar(this.fieldNbFaces.TextFieldReal);
			this.fieldNbFaces.TextFieldReal.InternalMinValue = 3;
			this.fieldNbFaces.TextFieldReal.InternalMaxValue = 24;
			this.fieldNbFaces.TextFieldReal.Step = 1;
			this.fieldNbFaces.TextFieldReal.ValueChanged += new EventHandler(this.HandleFieldChanged);
			this.fieldNbFaces.TabIndex = 1;
			this.fieldNbFaces.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldNbFaces, Res.Strings.Panel.Regular.Tooltip.Faces);

			this.fieldDeep = new Widgets.TextFieldLabel(this, false);
			this.fieldDeep.LabelShortText = Res.Strings.Panel.Regular.Short.Deep;
			this.fieldDeep.LabelLongText  = Res.Strings.Panel.Regular.Long.Deep;
			this.document.Modifier.AdaptTextFieldRealScalar(this.fieldDeep.TextFieldReal);
			this.fieldDeep.TextFieldReal.InternalMinValue = 0;
			this.fieldDeep.TextFieldReal.InternalMaxValue = 100;
			this.fieldDeep.TextFieldReal.Step = 5;
			this.fieldDeep.TextFieldReal.TextSuffix = "%";
			this.fieldDeep.TextFieldReal.ValueChanged += new EventHandler(this.HandleFieldChanged);
			this.fieldDeep.TabIndex = 3;
			this.fieldDeep.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldDeep, Res.Strings.Panel.Regular.Tooltip.Deep);

			this.isNormalAndExtended = true;
		}
		
		protected void AddRadioIcon(bool type)
		{
			this.grid.AddRadioIcon(Properties.Regular.GetIconText(type), Properties.Regular.GetName(type), type?1:0, false);
		}

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.grid.SelectionChanged -= new EventHandler(HandleTypeChanged);
				this.fieldNbFaces.TextFieldReal.ValueChanged -= new EventHandler(this.HandleFieldChanged);
				this.fieldDeep.TextFieldReal.ValueChanged -= new EventHandler(this.HandleFieldChanged);

				this.grid = null;
				this.fieldNbFaces = null;
				this.fieldDeep = null;
			}
			
			base.Dispose(disposing);
		}

		
		// Retourne la hauteur standard.
		public override double DefaultHeight
		{
			get
			{
				return ( this.isExtendedSize ? this.LabelHeight+55 : this.LabelHeight+30 );
			}
		}

		// Propriété -> widgets.
		protected override void PropertyToWidgets()
		{
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

		// Widgets -> propriété.
		protected override void WidgetsToProperty()
		{
			Properties.Regular p = this.property as Properties.Regular;
			if ( p == null )  return;

			p.Star = (this.grid.SelectedValue == 1);
			p.NbFaces = (int)this.fieldNbFaces.TextFieldReal.InternalValue;
			p.Deep = (double) this.fieldDeep.TextFieldReal.InternalValue/100;
		}

		// Grise les widgets nécessaires.
		protected void EnableWidgets()
		{
			bool star = (this.grid.SelectedValue == 1);
			this.fieldDeep.SetEnabled(this.isExtendedSize && star);
		}

		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.grid == null )  return;

			this.EnableWidgets();

			Rectangle rect = this.UsefulZone;

			Rectangle r = rect;
			r.Bottom = r.Top-20;
			r.Width = 22*2;
			r.Inflate(1);
			this.grid.Bounds = r;

			if ( this.isExtendedSize && this.IsLabelProperties )
			{
				r = rect;
				r.Bottom = r.Top-20;
				r.Left = rect.Left+22*2;
				r.Right = rect.Right;
				this.fieldNbFaces.Bounds = r;

				rect.Top = r.Bottom-5;
				rect.Bottom = rect.Top-20;
				r = rect;
				r.Left = rect.Left;
				r.Right = rect.Right;
				this.fieldDeep.Bounds = r;
			}
			else
			{
				r = rect;
				r.Bottom = r.Top-20;
				r.Left = rect.Right-Widgets.TextFieldLabel.ShortWidth;
				r.Right = rect.Right;
				this.fieldNbFaces.Bounds = r;

				rect.Top = r.Bottom-5;
				rect.Bottom = rect.Top-20;
				r = rect;
				r.Left = rect.Right-Widgets.TextFieldLabel.ShortWidth;
				r.Right = rect.Right;
				this.fieldDeep.Bounds = r;
			}
		}
		
		// Un champ a été changé.
		private void HandleFieldChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}

		// Un champ a été changé.
		private void HandleTypeChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.EnableWidgets();
			this.OnChanged();
		}


		protected Widgets.RadioIconGrid		grid;
		protected Widgets.TextFieldLabel	fieldNbFaces;
		protected Widgets.TextFieldLabel	fieldDeep;
	}
}
