using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Containers
{
	/// <summary>
	/// La classe Containers.Operations contient toutes les opérations.
	/// </summary>
	[SuppressBundleSupport]
	public class Operations : Abstract
	{
		public Operations(Document document) : base(document)
		{
			this.tabIndex = 0;

			// Déplacement.
			this.CreateHeader(ref this.boxMove, ref this.toolBarMove, "Déplacements");
			this.CreateFieldMove(this.toolBarMove, ref this.fieldMoveH, "Valeur du déplacement horizontal");
			this.CreateButton(this.toolBarMove, ref this.buttonMoveHi, "OperMoveHi", "Déplacement à gauche", new MessageEventHandler(this.HandleButtonMoveHi));
			this.CreateButton(this.toolBarMove, ref this.buttonMoveH,  "OperMoveH",  "Déplacement à droite", new MessageEventHandler(this.HandleButtonMoveH));
			this.CreateSeparator(this.toolBarMove);
			this.CreateFieldMove(this.toolBarMove, ref this.fieldMoveV, "Valeur du déplacement vertical");
			this.CreateButton(this.toolBarMove, ref this.buttonMoveVi, "OperMoveVi", "Déplacement en bas",  new MessageEventHandler(this.HandleButtonMoveVi));
			this.CreateButton(this.toolBarMove, ref this.buttonMoveV,  "OperMoveV",  "Déplacement en haut", new MessageEventHandler(this.HandleButtonMoveV));

			// Rotation.
			this.CreateHeader(ref this.boxRot, ref this.toolBarRot, "Rotations");
			this.CreateButton(this.toolBarRot, ref this.buttonRotate90,  "OperRot90",  "Quart de tour à gauche", new MessageEventHandler(this.HandleButtonRotate90));
			this.CreateButton(this.toolBarRot, ref this.buttonRotate180, "OperRot180", "Demi-tour",              new MessageEventHandler(this.HandleButtonRotate180));
			this.CreateButton(this.toolBarRot, ref this.buttonRotate270, "OperRot270", "Quart de tour à droite", new MessageEventHandler(this.HandleButtonRotate270));
			this.CreateSeparator(this.toolBarRot);
			this.CreateFieldRot(this.toolBarRot, ref this.fieldRotate, "Angle de rotation en degrés");
			this.CreateButton(this.toolBarRot, ref this.buttonRotate,  "OperRot",  "Rotation anti-horaire", new MessageEventHandler(this.HandleButtonRotate));
			this.CreateButton(this.toolBarRot, ref this.buttonRotatei, "OperRoti", "Rotation horaire",      new MessageEventHandler(this.HandleButtonRotatei));

			// Miroir.
			this.CreateHeader(ref this.boxMirror, ref this.toolBarMirror, "Miroirs");
			this.CreateButton(this.toolBarMirror, ref this.buttonMirrorH, "OperMirrorH", "Miroir horizontal", new MessageEventHandler(this.HandleButtonMirrorH));
			this.CreateButton(this.toolBarMirror, ref this.buttonMirrorV, "OperMirrorV", "Miroir vertical",   new MessageEventHandler(this.HandleButtonMirrorV));

			// Zoom.
			this.CreateHeader(ref this.boxZoom, ref this.toolBarZoom, "Réductions et agrandissements");
			this.CreateButton(this.toolBarZoom, ref this.buttonZoomDiv2, "OperZoomDiv2", "Réduction /2",      new MessageEventHandler(this.HandleButtonZoomDiv2));
			this.CreateButton(this.toolBarZoom, ref this.buttonZoomMul2, "OperZoomMul2", "Agrandissement x2", new MessageEventHandler(this.HandleButtonZoomMul2));
			this.CreateSeparator(this.toolBarZoom);
			this.CreateFieldZoom(this.toolBarZoom, ref this.fieldZoom, "Facteur d'agrandissement/réduction");
			this.CreateButton(this.toolBarZoom, ref this.buttonZoomi, "OperZoomi", "Réduction",     new MessageEventHandler(this.HandleButtonZoomi));
			this.CreateButton(this.toolBarZoom, ref this.buttonZoom,  "OperZoom", "Agrandissement", new MessageEventHandler(this.HandleButtonZoom));

			// Alignement.
			this.CreateHeader(ref this.boxAlign, ref this.toolBarAlign, "Alignements");
			this.CreateButton(this.toolBarAlign, ref this.buttonAlignGrid,    "AlignGrid",    "Alignement sur la grille magnétique", new MessageEventHandler(this.HandleButtonAlignGrid));
			this.CreateSeparator(this.toolBarAlign);
			this.CreateButton(this.toolBarAlign, ref this.buttonAlignLeft,    "AlignLeft",    "Alignement à gauche",                 new MessageEventHandler(this.HandleButtonAlignLeft));
			this.CreateButton(this.toolBarAlign, ref this.buttonAlignCenterX, "AlignCenterX", "Alignement centré horizontalement",   new MessageEventHandler(this.HandleButtonAlignCenterX));
			this.CreateButton(this.toolBarAlign, ref this.buttonAlignRight,   "AlignRight",   "Alignement à droite",                 new MessageEventHandler(this.HandleButtonAlignRight));
			this.CreateSeparator(this.toolBarAlign);
			this.CreateButton(this.toolBarAlign, ref this.buttonAlignTop,     "AlignTop",     "Alignement en haut",                  new MessageEventHandler(this.HandleButtonAlignTop));
			this.CreateButton(this.toolBarAlign, ref this.buttonAlignCenterY, "AlignCenterY", "Alignement centré verticalement",     new MessageEventHandler(this.HandleButtonAlignCenterY));
			this.CreateButton(this.toolBarAlign, ref this.buttonAlignBottom,  "AlignBottom",  "Alignement en bas",                   new MessageEventHandler(this.HandleButtonAlignBottom));

			// Distribution.
			this.CreateHeader(ref this.boxShare, ref this.toolBarShare, "Distributions");
			this.CreateButton(this.toolBarShare, ref this.buttonShareSpaceX,  "ShareSpaceX",  "Distribution espacée horizontalement", new MessageEventHandler(this.HandleButtonShareSpaceX));
			this.CreateButton(this.toolBarShare, ref this.buttonShareLeft,    "ShareLeft",    "Distribution sur la gauche",           new MessageEventHandler(this.HandleButtonShareLeft));
			this.CreateButton(this.toolBarShare, ref this.buttonShareCenterX, "ShareCenterX", "Distribution centrée horizontalement", new MessageEventHandler(this.HandleButtonShareCenterX));
			this.CreateButton(this.toolBarShare, ref this.buttonShareRight,   "ShareRight",   "Distribution sur la droite",           new MessageEventHandler(this.HandleButtonShareRight));
			this.CreateSeparator(this.toolBarShare);
			this.CreateButton(this.toolBarShare, ref this.buttonShareSpaceY,  "ShareSpaceX",  "Distribution espacée verticalement",   new MessageEventHandler(this.HandleButtonShareSpaceY));
			this.CreateButton(this.toolBarShare, ref this.buttonShareTop,     "ShareTop",     "Distribution sur le haut",             new MessageEventHandler(this.HandleButtonShareTop));
			this.CreateButton(this.toolBarShare, ref this.buttonShareCenterY, "ShareCenterX", "Distribution centrée verticalement",   new MessageEventHandler(this.HandleButtonShareCenterY));
			this.CreateButton(this.toolBarShare, ref this.buttonShareBottom,  "ShareBottom",  "Distribution sur le bas",              new MessageEventHandler(this.HandleButtonShareBottom));
		}

		// Crée l'en-tête d'un groupe.
		protected void CreateHeader(ref GroupBox group, ref HToolBar bar, string title)
		{
			group = new GroupBox(this);
			group.Height = 45;
			group.Text = title;
			group.Dock = DockStyle.Top;
			group.DockMargins = new Margins(0, 0, 0, 10);
			group.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;

			bar = new HToolBar(group);
			bar.Dock = DockStyle.Top;
			bar.DockMargins = new Margins(0, 0, 0, 10);
			bar.TabIndex = this.tabIndex++;
			bar.TabNavigation = Widget.TabNavigationMode.ForwardTabActive;
		}

		// Crée un IconButton. 
		protected void CreateButton(HToolBar bar, ref IconButton button, string icon, string tooltip, MessageEventHandler handler)
		{
			button = new IconButton(string.Format("manifest:Epsitec.App.DocumentEditor.Images.{0}.icon", icon));
			button.Clicked += handler;
			button.TabIndex = this.tabIndex++;
			button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(button, tooltip);
			bar.Items.Add(button);
		}

		// Crée un champ éditable pour un déplacement.
		protected void CreateFieldMove(HToolBar bar, ref TextFieldReal field, string tooltip)
		{
			field = new TextFieldReal();
			this.document.Modifier.AdaptTextFieldRealDimension(field);
			field.Width = 50;
			if ( this.document.Type == DocumentType.Pictogram )
			{
				field.InternalValue = 1.0M;
			}
			else
			{
				field.InternalValue = 100.0M;  // 10mm
			}
			field.TabIndex = this.tabIndex++;
			field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(field, tooltip);
			bar.Items.Add(field);
		}

		// Crée un champ éditable pour une rotation.
		protected void CreateFieldRot(HToolBar bar, ref TextFieldReal field, string tooltip)
		{
			field = new TextFieldReal();
			this.document.Modifier.AdaptTextFieldRealAngle(field);
			field.Width = 50;
			field.InternalValue = 10.0M;
			field.TabIndex = tabIndex++;
			field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(field, tooltip);
			bar.Items.Add(this.fieldRotate);
		}

		// Crée un champ éditable pour un zoom.
		protected void CreateFieldZoom(HToolBar bar, ref TextFieldReal field, string tooltip)
		{
			field = new TextFieldReal();
			this.document.Modifier.AdaptTextFieldRealScalar(field);
			field.Width = 50;
			field.InternalMinValue = 1.0M;
			field.InternalMaxValue = 2.0M;
			field.Step = 0.1M;
			field.Resolution = 0.01M;
			field.InternalValue = 1.2M;
			field.TabIndex = tabIndex++;
			field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(field, tooltip);
			bar.Items.Add(this.fieldZoom);
		}

		// Crée un séparateur.
		protected void CreateSeparator(HToolBar bar)
		{
			bar.Items.Add(new IconSeparator());
		}
		

		// Effectue la mise à jour du contenu.
		protected override void DoUpdateContent()
		{
			bool enabled  = (this.document.Modifier.TotalSelected > 0);
			bool enabled2 = (this.document.Modifier.TotalSelected > 1);
			bool enabled3 = (this.document.Modifier.TotalSelected > 2);

			if ( this.document.Modifier.Tool == "Edit" )
			{
				enabled  = false;
				enabled2 = false;
				enabled3 = false;
			}

			this.buttonMoveH.SetEnabled(enabled);
			this.buttonMoveHi.SetEnabled(enabled);
			this.buttonMoveV.SetEnabled(enabled);
			this.buttonMoveVi.SetEnabled(enabled);

			this.buttonRotate90.SetEnabled(enabled);
			this.buttonRotate180.SetEnabled(enabled);
			this.buttonRotate270.SetEnabled(enabled);
			this.buttonRotate.SetEnabled(enabled);
			this.buttonRotatei.SetEnabled(enabled);
			
			this.buttonMirrorH.SetEnabled(enabled);
			this.buttonMirrorV.SetEnabled(enabled);
			
			this.buttonZoomMul2.SetEnabled(enabled);
			this.buttonZoomDiv2.SetEnabled(enabled);
			this.buttonZoom.SetEnabled(enabled);
			this.buttonZoomi.SetEnabled(enabled);

			this.buttonAlignGrid.SetEnabled(enabled);
			this.buttonAlignLeft.SetEnabled(enabled2);
			this.buttonAlignCenterX.SetEnabled(enabled2);
			this.buttonAlignRight.SetEnabled(enabled2);
			this.buttonAlignTop.SetEnabled(enabled2);
			this.buttonAlignCenterY.SetEnabled(enabled2);
			this.buttonAlignBottom.SetEnabled(enabled2);

			this.buttonShareLeft.SetEnabled(enabled3);
			this.buttonShareCenterX.SetEnabled(enabled3);
			this.buttonShareSpaceX.SetEnabled(enabled3);
			this.buttonShareRight.SetEnabled(enabled3);
			this.buttonShareTop.SetEnabled(enabled3);
			this.buttonShareCenterY.SetEnabled(enabled3);
			this.buttonShareSpaceY.SetEnabled(enabled3);
			this.buttonShareBottom.SetEnabled(enabled3);
		}


		private void HandleButtonMoveH(object sender, MessageEventArgs e)
		{
			double dx = (double) this.fieldMoveH.InternalValue;
			this.document.Modifier.MoveSelection(new Point(dx,0));
		}

		private void HandleButtonMoveHi(object sender, MessageEventArgs e)
		{
			double dx = (double) this.fieldMoveH.InternalValue;
			this.document.Modifier.MoveSelection(new Point(-dx,0));
		}

		private void HandleButtonMoveV(object sender, MessageEventArgs e)
		{
			double dy = (double) this.fieldMoveV.InternalValue;
			this.document.Modifier.MoveSelection(new Point(0,dy));
		}

		private void HandleButtonMoveVi(object sender, MessageEventArgs e)
		{
			double dy = (double) this.fieldMoveV.InternalValue;
			this.document.Modifier.MoveSelection(new Point(0,-dy));
		}

		private void HandleButtonRotate90(object sender, MessageEventArgs e)
		{
			this.document.Modifier.RotateSelection(90.0);
		}

		private void HandleButtonRotate180(object sender, MessageEventArgs e)
		{
			this.document.Modifier.RotateSelection(180.0);
		}

		private void HandleButtonRotate270(object sender, MessageEventArgs e)
		{
			this.document.Modifier.RotateSelection(270.0);
		}

		private void HandleButtonRotate(object sender, MessageEventArgs e)
		{
			double angle = (double) this.fieldRotate.InternalValue;
			this.document.Modifier.RotateSelection(angle);
		}

		private void HandleButtonRotatei(object sender, MessageEventArgs e)
		{
			double angle = (double) this.fieldRotate.InternalValue;
			this.document.Modifier.RotateSelection(-angle);
		}

		private void HandleButtonMirrorH(object sender, MessageEventArgs e)
		{
			this.document.Modifier.MirrorSelection(true);
		}

		private void HandleButtonMirrorV(object sender, MessageEventArgs e)
		{
			this.document.Modifier.MirrorSelection(false);
		}

		private void HandleButtonZoomMul2(object sender, MessageEventArgs e)
		{
			this.document.Modifier.ZoomSelection(2.0);
		}

		private void HandleButtonZoomDiv2(object sender, MessageEventArgs e)
		{
			this.document.Modifier.ZoomSelection(0.5);
		}

		private void HandleButtonZoom(object sender, MessageEventArgs e)
		{
			double scale = (double) this.fieldZoom.InternalValue;
			this.document.Modifier.ZoomSelection(scale);
		}

		private void HandleButtonZoomi(object sender, MessageEventArgs e)
		{
			double scale = (double) this.fieldZoom.InternalValue;
			this.document.Modifier.ZoomSelection(1.0/scale);
		}

		private void HandleButtonAlignGrid(object sender, MessageEventArgs e)
		{
			this.document.Modifier.AlignGridSelection();
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


		protected GroupBox				boxMove;
		protected HToolBar				toolBarMove;
		protected TextFieldReal			fieldMoveH;
		protected IconButton			buttonMoveH;
		protected IconButton			buttonMoveHi;
		protected TextFieldReal			fieldMoveV;
		protected IconButton			buttonMoveV;
		protected IconButton			buttonMoveVi;

		protected GroupBox				boxRot;
		protected HToolBar				toolBarRot;
		protected IconButton			buttonRotate90;
		protected IconButton			buttonRotate180;
		protected IconButton			buttonRotate270;
		protected TextFieldReal			fieldRotate;
		protected IconButton			buttonRotate;
		protected IconButton			buttonRotatei;
		
		protected GroupBox				boxMirror;
		protected HToolBar				toolBarMirror;
		protected IconButton			buttonMirrorH;
		protected IconButton			buttonMirrorV;
		
		protected GroupBox				boxZoom;
		protected HToolBar				toolBarZoom;
		protected IconButton			buttonZoomDiv2;
		protected IconButton			buttonZoomMul2;
		protected TextFieldReal			fieldZoom;
		protected IconButton			buttonZoomi;
		protected IconButton			buttonZoom;
		
		protected GroupBox				boxAlign;
		protected HToolBar				toolBarAlign;
		protected IconButton			buttonAlignGrid;
		protected IconButton			buttonAlignLeft;
		protected IconButton			buttonAlignCenterX;
		protected IconButton			buttonAlignRight;
		protected IconButton			buttonAlignTop;
		protected IconButton			buttonAlignCenterY;
		protected IconButton			buttonAlignBottom;
		
		protected GroupBox				boxShare;
		protected HToolBar				toolBarShare;
		protected IconButton			buttonShareLeft;
		protected IconButton			buttonShareCenterX;
		protected IconButton			buttonShareSpaceX;
		protected IconButton			buttonShareRight;
		protected IconButton			buttonShareTop;
		protected IconButton			buttonShareCenterY;
		protected IconButton			buttonShareSpaceY;
		protected IconButton			buttonShareBottom;

		protected int					tabIndex;
	}
}
