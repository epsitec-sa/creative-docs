using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Ribbons
{
	/// <summary>
	/// La classe Locator correspond aux commandes de navigation précédent/suivant.
	/// </summary>
	public class LocatorRibbon : AbstractRibbon
	{
		public LocatorRibbon(DesignerApplication designerApplication) : base(designerApplication)
		{
			this.Title = Res.Strings.Ribbon.Section.Locator;
			this.PreferredWidth = 8 + 22*1.5*2;

			double dx = 22;
			double dy = 22;

			this.group = new FrameBox(this);

			this.buttonList = this.CreateMenuButton("LocatorList", Res.Strings.Action.LocatorList, this.HandleListPressed);
			this.buttonList.ContentAlignment = ContentAlignment.BottomCenter;
			this.buttonList.GlyphSize = new Size(dx*3.0, dy*0.5);
			this.buttonList.Anchor = AnchorStyles.All;
			this.buttonList.SetParent(this.group);

			this.buttonPrev = this.CreateIconButton("LocatorPrev", "Large");
			this.buttonPrev.ButtonStyle = ButtonStyle.ComboItem;
			this.buttonPrev.PreferredSize = new Size(dx*1.5, dy*1.5);
			this.buttonPrev.Dock = DockStyle.Left;
			this.buttonPrev.VerticalAlignment = VerticalAlignment.Top;
			this.buttonPrev.SetParent(this.group);

			this.buttonNext = this.CreateIconButton("LocatorNext", "Large");
			this.buttonNext.ButtonStyle = ButtonStyle.ComboItem;
			this.buttonNext.PreferredSize = new Size(dx*1.5, dy*1.5);
			this.buttonNext.Dock = DockStyle.Left;
			this.buttonNext.VerticalAlignment = VerticalAlignment.Top;
			this.buttonNext.SetParent(this.group);
			
			this.UpdateClientGeometry();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}


		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.buttonPrev == null )  return;

			double dx = 22;
			double dy = 22;

			Rectangle rect = this.UsefulZone;
			rect.Width  = dx*3.0;
			rect.Height = dy*2.0;
			this.group.SetManualBounds(rect);
		}


		private void HandleListPressed(object sender, MessageEventArgs e)
		{
			//	Bouton pour ouvrir la liste cliqué.
			if (!this.designerApplication.IsCurrentModule)
			{
				return;
			}

			GlyphButton button = sender as GlyphButton;
			if (button == null)
			{
				return;
			}

			VMenu menu = this.designerApplication.LocatorCreateMenu(null);
			menu.Host = this;
			menu.MinWidth = button.ActualWidth;
			TextFieldCombo.AdjustComboSize(button, menu, false);
			menu.ShowAsComboList(button, Point.Zero, button);
		}


		protected Widget					group;
		protected IconButton				buttonPrev;
		protected IconButton				buttonNext;
		protected GlyphButton				buttonList;
	}
}
