using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Align permet de choisir l'ordre.
	/// </summary>
	[SuppressBundleSupport]
	public class Align : Abstract
	{
		public Align() : base()
		{
			this.title.Text = "Align";

			this.CreateButton(ref this.buttonAlignLeft,    "AlignLeft",    Res.Strings.Action.AlignLeft,    new MessageEventHandler(this.HandleButtonAlignLeft));
			this.CreateButton(ref this.buttonAlignCenterX, "AlignCenterX", Res.Strings.Action.AlignCenterX, new MessageEventHandler(this.HandleButtonAlignCenterX));
			this.CreateButton(ref this.buttonAlignRight,   "AlignRight",   Res.Strings.Action.AlignRight,   new MessageEventHandler(this.HandleButtonAlignRight));
			this.CreateButton(ref this.buttonAlignTop,     "AlignTop",     Res.Strings.Action.AlignTop,     new MessageEventHandler(this.HandleButtonAlignTop));
			this.CreateButton(ref this.buttonAlignCenterY, "AlignCenterY", Res.Strings.Action.AlignCenterY, new MessageEventHandler(this.HandleButtonAlignCenterY));
			this.CreateButton(ref this.buttonAlignBottom,  "AlignBottom",  Res.Strings.Action.AlignBottom,  new MessageEventHandler(this.HandleButtonAlignBottom));

			this.CreateSeparator(ref this.separator1);

			this.CreateButton(ref this.buttonAlignGrid,    "AlignGrid",    Res.Strings.Action.AlignGrid,    new MessageEventHandler(this.HandleButtonAlignGrid));

			this.CreateSeparator(ref this.separator2);

			this.CreateButton(ref this.buttonShareSpaceX,  "ShareSpaceX",  Res.Strings.Action.ShareSpaceX,  new MessageEventHandler(this.HandleButtonShareSpaceX));
			this.CreateButton(ref this.buttonShareLeft,    "ShareLeft",    Res.Strings.Action.ShareLeft,    new MessageEventHandler(this.HandleButtonShareLeft));
			this.CreateButton(ref this.buttonShareCenterX, "ShareCenterX", Res.Strings.Action.ShareCenterX, new MessageEventHandler(this.HandleButtonShareCenterX));
			this.CreateButton(ref this.buttonShareRight,   "ShareRight",   Res.Strings.Action.ShareRight,   new MessageEventHandler(this.HandleButtonShareRight));
			this.CreateButton(ref this.buttonShareSpaceY,  "ShareSpaceY",  Res.Strings.Action.ShareSpaceY,  new MessageEventHandler(this.HandleButtonShareSpaceY));
			this.CreateButton(ref this.buttonShareTop,     "ShareTop",     Res.Strings.Action.ShareTop,     new MessageEventHandler(this.HandleButtonShareTop));
			this.CreateButton(ref this.buttonShareCenterY, "ShareCenterY", Res.Strings.Action.ShareCenterY, new MessageEventHandler(this.HandleButtonShareCenterY));
			this.CreateButton(ref this.buttonShareBottom,  "ShareBottom",  Res.Strings.Action.ShareBottom,  new MessageEventHandler(this.HandleButtonShareBottom));

			this.CreateSeparator(ref this.separator3);

			this.CreateButton(ref this.buttonAdjustWidth,  "AdjustWidth",  Res.Strings.Action.AdjustWidth,  new MessageEventHandler(this.HandleButtonAdjustWidth));
			this.CreateButton(ref this.buttonAdjustHeight, "AdjustHeight", Res.Strings.Action.AdjustHeight, new MessageEventHandler(this.HandleButtonAdjustHeight));
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}

		// Retourne la largeur standard.
		public override double DefaultWidth
		{
			get
			{
				return 8 + 22*3 + this.separatorWidth + 22 + this.separatorWidth + 22*4 + this.separatorWidth + 22;
			}
		}


		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.buttonAlignLeft == null )  return;

			double dx = this.buttonAlignLeft.DefaultWidth;
			double dy = this.buttonAlignLeft.DefaultHeight;

			Rectangle rect = this.UsefulZone;
			rect.Left += dx*3;
			rect.Width = this.separatorWidth;
			this.separator1.Bounds = rect;

			rect = this.UsefulZone;
			rect.Left += dx*3 + this.separatorWidth + dx;
			rect.Width = this.separatorWidth;
			this.separator2.Bounds = rect;

			rect = this.UsefulZone;
			rect.Left += dx*3 + this.separatorWidth + dx + this.separatorWidth + dx*4;
			rect.Width = this.separatorWidth;
			this.separator3.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy);
			this.buttonAlignLeft.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonAlignCenterX.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonAlignRight.Bounds = rect;
			rect.Offset(dx+this.separatorWidth, 0);
			this.buttonAlignGrid.Bounds = rect;
			rect.Offset(dx+this.separatorWidth, 0);
			this.buttonShareLeft.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonShareCenterX.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonShareSpaceX.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonShareRight.Bounds = rect;
			rect.Offset(dx+this.separatorWidth, 0);
			this.buttonAdjustWidth.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			this.buttonAlignTop.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonAlignCenterY.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonAlignBottom.Bounds = rect;
			rect.Offset(dx+this.separatorWidth+dx+this.separatorWidth, 0);
			this.buttonShareTop.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonShareCenterY.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonShareSpaceY.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonShareBottom.Bounds = rect;
			rect.Offset(dx+this.separatorWidth, 0);
			this.buttonAdjustHeight.Bounds = rect;
		}

		// Effectue la mise à jour du contenu.
		protected override void DoUpdateContent()
		{
			bool enabled  = false;
			bool enabled2 = false;
			bool enabled3 = false;

			if ( this.document != null )
			{
				enabled  = (this.document.Modifier.TotalSelected > 0);
				enabled2 = (this.document.Modifier.TotalSelected > 1);
				enabled3 = (this.document.Modifier.TotalSelected > 2);

				if ( this.document.Modifier.Tool == "Edit" )
				{
					enabled  = false;
					enabled2 = false;
					enabled3 = false;
				}
			}

			this.buttonAlignLeft.SetEnabled(enabled2);
			this.buttonAlignCenterX.SetEnabled(enabled2);
			this.buttonAlignRight.SetEnabled(enabled2);
			this.buttonAlignTop.SetEnabled(enabled2);
			this.buttonAlignCenterY.SetEnabled(enabled2);
			this.buttonAlignBottom.SetEnabled(enabled2);
			this.buttonAlignGrid.SetEnabled(enabled);

			this.buttonShareLeft.SetEnabled(enabled3);
			this.buttonShareCenterX.SetEnabled(enabled3);
			this.buttonShareSpaceX.SetEnabled(enabled3);
			this.buttonShareRight.SetEnabled(enabled3);
			this.buttonShareTop.SetEnabled(enabled3);
			this.buttonShareCenterY.SetEnabled(enabled3);
			this.buttonShareSpaceY.SetEnabled(enabled3);
			this.buttonShareBottom.SetEnabled(enabled3);

			this.buttonAdjustWidth.SetEnabled(enabled2);
			this.buttonAdjustHeight.SetEnabled(enabled2);
		}

		
		private void HandleButtonAlignLeft(object sender, MessageEventArgs e)
		{
			this.document.Modifier.AlignSelection(-1, true);
		}

		private void HandleButtonAlignCenterX(object sender, MessageEventArgs e)
		{
			this.document.Modifier.AlignSelection(0, true);
		}

		private void HandleButtonAlignRight(object sender, MessageEventArgs e)
		{
			this.document.Modifier.AlignSelection(1, true);
		}

		private void HandleButtonAlignTop(object sender, MessageEventArgs e)
		{
			this.document.Modifier.AlignSelection(1, false);
		}

		private void HandleButtonAlignCenterY(object sender, MessageEventArgs e)
		{
			this.document.Modifier.AlignSelection(0, false);
		}

		private void HandleButtonAlignBottom(object sender, MessageEventArgs e)
		{
			this.document.Modifier.AlignSelection(-1, false);
		}

		private void HandleButtonAlignGrid(object sender, MessageEventArgs e)
		{
			this.document.Modifier.AlignGridSelection();
		}

		private void HandleButtonShareLeft(object sender, MessageEventArgs e)
		{
			this.document.Modifier.ShareSelection(-1, true);
		}

		private void HandleButtonShareCenterX(object sender, MessageEventArgs e)
		{
			this.document.Modifier.ShareSelection(0, true);
		}

		private void HandleButtonShareSpaceX(object sender, MessageEventArgs e)
		{
			this.document.Modifier.SpaceSelection(true);
		}

		private void HandleButtonShareRight(object sender, MessageEventArgs e)
		{
			this.document.Modifier.ShareSelection(1, true);
		}

		private void HandleButtonShareTop(object sender, MessageEventArgs e)
		{
			this.document.Modifier.ShareSelection(1, false);
		}

		private void HandleButtonShareCenterY(object sender, MessageEventArgs e)
		{
			this.document.Modifier.ShareSelection(0, false);
		}

		private void HandleButtonShareSpaceY(object sender, MessageEventArgs e)
		{
			this.document.Modifier.SpaceSelection(false);
		}

		private void HandleButtonShareBottom(object sender, MessageEventArgs e)
		{
			this.document.Modifier.ShareSelection(-1, false);
		}

		private void HandleButtonAdjustWidth(object sender, MessageEventArgs e)
		{
			this.document.Modifier.AdjustSelection(true);
		}

		private void HandleButtonAdjustHeight(object sender, MessageEventArgs e)
		{
			this.document.Modifier.AdjustSelection(false);
		}

		
		protected IconButton				buttonAlignLeft;
		protected IconButton				buttonAlignCenterX;
		protected IconButton				buttonAlignRight;
		protected IconButton				buttonAlignTop;
		protected IconButton				buttonAlignCenterY;
		protected IconButton				buttonAlignBottom;
		protected IconSeparator				separator1;
		protected IconButton				buttonAlignGrid;
		protected IconSeparator				separator2;
		protected IconButton				buttonShareLeft;
		protected IconButton				buttonShareCenterX;
		protected IconButton				buttonShareSpaceX;
		protected IconButton				buttonShareRight;
		protected IconButton				buttonShareTop;
		protected IconButton				buttonShareCenterY;
		protected IconButton				buttonShareSpaceY;
		protected IconButton				buttonShareBottom;
		protected IconSeparator				separator3;
		protected IconButton				buttonAdjustWidth;
		protected IconButton				buttonAdjustHeight;
	}
}
