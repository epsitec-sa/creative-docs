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
			// Déplacement.
			this.boxMove = new GroupBox(this);
			this.boxMove.Height = 45;
			this.boxMove.Text = "Déplacements";
			this.boxMove.Dock = DockStyle.Top;
			this.boxMove.DockMargins = new Margins(0, 0, 0, 10);

			this.toolBarMove = new HToolBar(this.boxMove);
			this.toolBarMove.Dock = DockStyle.Top;
			this.toolBarMove.DockMargins = new Margins(0, 0, 0, 10);

			this.fieldMoveH = new TextFieldReal();
			this.document.Modifier.AdaptTextFieldRealDimension(this.fieldMoveH);
			this.fieldMoveH.Width = 50;
			if ( this.document.Type == DocumentType.Pictogram )
			{
				this.fieldMoveH.InternalValue = 1.0M;
			}
			else
			{
				this.fieldMoveH.InternalValue = 100.0M;  // 10mm
			}
			ToolTip.Default.SetToolTip(this.fieldMoveH, "Valeur du déplacement horizontal");
			this.toolBarMove.Items.Add(this.fieldMoveH);

			this.buttonMoveHi = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.OperMoveHi.icon");
			this.buttonMoveHi.Clicked += new MessageEventHandler(this.HandleButtonMoveHi);
			ToolTip.Default.SetToolTip(this.buttonMoveHi, "Déplacement à gauche");
			this.toolBarMove.Items.Add(this.buttonMoveHi);

			this.buttonMoveH = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.OperMoveH.icon");
			this.buttonMoveH.Clicked += new MessageEventHandler(this.HandleButtonMoveH);
			ToolTip.Default.SetToolTip(this.buttonMoveH, "Déplacement à droite");
			this.toolBarMove.Items.Add(this.buttonMoveH);

			this.toolBarMove.Items.Add(new IconSeparator());

			this.fieldMoveV = new TextFieldReal();
			this.document.Modifier.AdaptTextFieldRealDimension(this.fieldMoveV);
			this.fieldMoveV.Width = 50;
			if ( this.document.Type == DocumentType.Pictogram )
			{
				this.fieldMoveV.InternalValue = 1.0M;
			}
			else
			{
				this.fieldMoveV.InternalValue = 100.0M;  // 10mm
			}
			ToolTip.Default.SetToolTip(this.fieldMoveV, "Valeur du déplacement vertical");
			this.toolBarMove.Items.Add(this.fieldMoveV);

			this.buttonMoveVi = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.OperMoveVi.icon");
			this.buttonMoveVi.Clicked += new MessageEventHandler(this.HandleButtonMoveVi);
			ToolTip.Default.SetToolTip(this.buttonMoveVi, "Déplacement en bas");
			this.toolBarMove.Items.Add(this.buttonMoveVi);

			this.buttonMoveV = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.OperMoveV.icon");
			this.buttonMoveV.Clicked += new MessageEventHandler(this.HandleButtonMoveV);
			ToolTip.Default.SetToolTip(this.buttonMoveV, "Déplacement en haut");
			this.toolBarMove.Items.Add(this.buttonMoveV);

			// Rotation.
			this.boxRotate = new GroupBox(this);
			this.boxRotate.Height = 45;
			this.boxRotate.Text = "Rotations";
			this.boxRotate.Dock = DockStyle.Top;
			this.boxRotate.DockMargins = new Margins(0, 0, 0, 10);

			this.toolBarRot = new HToolBar(this.boxRotate);
			this.toolBarRot.Dock = DockStyle.Top;
			this.toolBarRot.DockMargins = new Margins(0, 0, 0, 10);

			this.buttonRotate90 = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.OperRot90.icon");
			this.buttonRotate90.Clicked += new MessageEventHandler(this.HandleButtonRotate90);
			ToolTip.Default.SetToolTip(this.buttonRotate90, "Quart de tour à gauche");
			this.toolBarRot.Items.Add(this.buttonRotate90);

			this.buttonRotate180 = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.OperRot180.icon");
			this.buttonRotate180.Clicked += new MessageEventHandler(this.HandleButtonRotate180);
			ToolTip.Default.SetToolTip(this.buttonRotate180, "Demi-tour");
			this.toolBarRot.Items.Add(this.buttonRotate180);

			this.buttonRotate270 = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.OperRot270.icon");
			this.buttonRotate270.Clicked += new MessageEventHandler(this.HandleButtonRotate270);
			ToolTip.Default.SetToolTip(this.buttonRotate270, "Quart de tour à droite");
			this.toolBarRot.Items.Add(this.buttonRotate270);

			this.toolBarRot.Items.Add(new IconSeparator());

			this.fieldRotate = new TextFieldReal();
			this.document.Modifier.AdaptTextFieldRealAngle(this.fieldRotate);
			this.fieldRotate.Width = 50;
			this.fieldRotate.InternalValue = 10.0M;
			ToolTip.Default.SetToolTip(this.fieldRotate, "Angle de rotation en degrés");
			this.toolBarRot.Items.Add(this.fieldRotate);

			this.buttonRotate = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.OperRot.icon");
			this.buttonRotate.Clicked += new MessageEventHandler(this.HandleButtonRotate);
			ToolTip.Default.SetToolTip(this.buttonRotate, "Rotation anti-horaire");
			this.toolBarRot.Items.Add(this.buttonRotate);

			this.buttonRotatei = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.OperRoti.icon");
			this.buttonRotatei.Clicked += new MessageEventHandler(this.HandleButtonRotatei);
			ToolTip.Default.SetToolTip(this.buttonRotatei, "Rotation horaire");
			this.toolBarRot.Items.Add(this.buttonRotatei);

			// Miroir.
			this.boxMirror = new GroupBox(this);
			this.boxMirror.Height = 45;
			this.boxMirror.Text = "Miroirs";
			this.boxMirror.Dock = DockStyle.Top;
			this.boxMirror.DockMargins = new Margins(0, 0, 0, 10);

			this.toolBarMirror = new HToolBar(this.boxMirror);
			this.toolBarMirror.Dock = DockStyle.Top;
			this.toolBarMirror.DockMargins = new Margins(0, 0, 0, 10);

			this.buttonMirrorH = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.OperMirrorH.icon");
			this.buttonMirrorH.Clicked += new MessageEventHandler(this.HandleButtonMirrorH);
			ToolTip.Default.SetToolTip(this.buttonMirrorH, "Miroir horizontal");
			this.toolBarMirror.Items.Add(this.buttonMirrorH);

			this.buttonMirrorV = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.OperMirrorV.icon");
			this.buttonMirrorV.Clicked += new MessageEventHandler(this.HandleButtonMirrorV);
			ToolTip.Default.SetToolTip(this.buttonMirrorV, "Miroir vertical");
			this.toolBarMirror.Items.Add(this.buttonMirrorV);

			// Zoom.
			this.boxZoom = new GroupBox(this);
			this.boxZoom.Height = 45;
			this.boxZoom.Text = "Réductions et agrandissements";
			this.boxZoom.Dock = DockStyle.Top;
			this.boxZoom.DockMargins = new Margins(0, 0, 0, 10);

			this.toolBarZoom = new HToolBar(this.boxZoom);
			this.toolBarZoom.Dock = DockStyle.Top;
			this.toolBarZoom.DockMargins = new Margins(0, 0, 0, 10);

			this.buttonZoomDiv2 = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.OperZoomDiv2.icon");
			this.buttonZoomDiv2.Clicked += new MessageEventHandler(this.HandleButtonZoomDiv2);
			ToolTip.Default.SetToolTip(this.buttonZoomDiv2, "Réduction /2");
			this.toolBarZoom.Items.Add(this.buttonZoomDiv2);

			this.buttonZoomMul2 = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.OperZoomMul2.icon");
			this.buttonZoomMul2.Clicked += new MessageEventHandler(this.HandleButtonZoomMul2);
			ToolTip.Default.SetToolTip(this.buttonZoomMul2, "Agrandissement x2");
			this.toolBarZoom.Items.Add(this.buttonZoomMul2);

			this.toolBarZoom.Items.Add(new IconSeparator());

			this.fieldZoom = new TextFieldReal();
			this.document.Modifier.AdaptTextFieldRealScalar(this.fieldZoom);
			this.fieldZoom.Width = 50;
			this.fieldZoom.InternalMinValue = 1.0M;
			this.fieldZoom.InternalMaxValue = 2.0M;
			this.fieldZoom.Step = 0.1M;
			this.fieldZoom.Resolution = 0.01M;
			this.fieldZoom.InternalValue = 1.2M;
			ToolTip.Default.SetToolTip(this.fieldZoom, "Facteur d'agrandissement/réduction");
			this.toolBarZoom.Items.Add(this.fieldZoom);

			this.buttonZoomi = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.OperZoomi.icon");
			this.buttonZoomi.Clicked += new MessageEventHandler(this.HandleButtonZoomi);
			ToolTip.Default.SetToolTip(this.buttonZoomi, "Réduction");
			this.toolBarZoom.Items.Add(this.buttonZoomi);

			this.buttonZoom = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.OperZoom.icon");
			this.buttonZoom.Clicked += new MessageEventHandler(this.HandleButtonZoom);
			ToolTip.Default.SetToolTip(this.buttonZoom, "Agrandissement");
			this.toolBarZoom.Items.Add(this.buttonZoom);

			// Alignement.
			this.boxAlign = new GroupBox(this);
			this.boxAlign.Height = 45;
			this.boxAlign.Text = "Alignements";
			this.boxAlign.Dock = DockStyle.Top;
			this.boxAlign.DockMargins = new Margins(0, 0, 0, 10);

			this.toolBarAlign = new HToolBar(this.boxAlign);
			this.toolBarAlign.Dock = DockStyle.Top;
			this.toolBarAlign.DockMargins = new Margins(0, 0, 0, 10);

			this.buttonAlignGrid = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.AlignGrid.icon");
			this.buttonAlignGrid.Clicked += new MessageEventHandler(this.HandleButtonAlignGrid);
			ToolTip.Default.SetToolTip(this.buttonAlignGrid, "Alignement sur la grille magnétique");
			this.toolBarAlign.Items.Add(this.buttonAlignGrid);

			this.toolBarAlign.Items.Add(new IconSeparator());

			this.buttonAlignLeft = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.AlignLeft.icon");
			this.buttonAlignLeft.Clicked += new MessageEventHandler(this.HandleButtonAlignLeft);
			ToolTip.Default.SetToolTip(this.buttonAlignLeft, "Alignement à gauche");
			this.toolBarAlign.Items.Add(this.buttonAlignLeft);

			this.buttonAlignCenterX = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.AlignCenterX.icon");
			this.buttonAlignCenterX.Clicked += new MessageEventHandler(this.HandleButtonAlignCenterX);
			ToolTip.Default.SetToolTip(this.buttonAlignCenterX, "Alignement centré horizontalement");
			this.toolBarAlign.Items.Add(this.buttonAlignCenterX);

			this.buttonAlignRight = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.AlignRight.icon");
			this.buttonAlignRight.Clicked += new MessageEventHandler(this.HandleButtonAlignRight);
			ToolTip.Default.SetToolTip(this.buttonAlignRight, "Alignement à droite");
			this.toolBarAlign.Items.Add(this.buttonAlignRight);

			this.toolBarAlign.Items.Add(new IconSeparator());

			this.buttonAlignTop = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.AlignTop.icon");
			this.buttonAlignTop.Clicked += new MessageEventHandler(this.HandleButtonAlignTop);
			ToolTip.Default.SetToolTip(this.buttonAlignTop, "Alignement en haut");
			this.toolBarAlign.Items.Add(this.buttonAlignTop);

			this.buttonAlignCenterY = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.AlignCenterY.icon");
			this.buttonAlignCenterY.Clicked += new MessageEventHandler(this.HandleButtonAlignCenterY);
			ToolTip.Default.SetToolTip(this.buttonAlignCenterY, "Alignement centré verticalement");
			this.toolBarAlign.Items.Add(this.buttonAlignCenterY);

			this.buttonAlignBottom = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.AlignBottom.icon");
			this.buttonAlignBottom.Clicked += new MessageEventHandler(this.HandleButtonAlignBottom);
			ToolTip.Default.SetToolTip(this.buttonAlignBottom, "Alignement en bas");
			this.toolBarAlign.Items.Add(this.buttonAlignBottom);

			// Distribution.
			this.boxShare = new GroupBox(this);
			this.boxShare.Height = 45;
			this.boxShare.Text = "Distributions";
			this.boxShare.Dock = DockStyle.Top;
			this.boxShare.DockMargins = new Margins(0, 0, 0, 10);

			this.toolBarShare = new HToolBar(this.boxShare);
			this.toolBarShare.Dock = DockStyle.Top;
			this.toolBarShare.DockMargins = new Margins(0, 0, 0, 10);

			this.buttonShareSpaceX = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.ShareSpaceX.icon");
			this.buttonShareSpaceX.Clicked += new MessageEventHandler(this.HandleButtonShareSpaceX);
			ToolTip.Default.SetToolTip(this.buttonShareSpaceX, "Distribution espacée horizontalement");
			this.toolBarShare.Items.Add(this.buttonShareSpaceX);

			this.buttonShareLeft = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.ShareLeft.icon");
			this.buttonShareLeft.Clicked += new MessageEventHandler(this.HandleButtonShareLeft);
			ToolTip.Default.SetToolTip(this.buttonShareLeft, "Distribution sur la gauche");
			this.toolBarShare.Items.Add(this.buttonShareLeft);

			this.buttonShareCenterX = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.ShareCenterX.icon");
			this.buttonShareCenterX.Clicked += new MessageEventHandler(this.HandleButtonShareCenterX);
			ToolTip.Default.SetToolTip(this.buttonShareCenterX, "Distribution centrée horizontalement");
			this.toolBarShare.Items.Add(this.buttonShareCenterX);

			this.buttonShareRight = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.ShareRight.icon");
			this.buttonShareRight.Clicked += new MessageEventHandler(this.HandleButtonShareRight);
			ToolTip.Default.SetToolTip(this.buttonShareRight, "Distribution sur la droite");
			this.toolBarShare.Items.Add(this.buttonShareRight);

			this.toolBarShare.Items.Add(new IconSeparator());

			this.buttonShareSpaceY = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.ShareSpaceY.icon");
			this.buttonShareSpaceY.Clicked += new MessageEventHandler(this.HandleButtonShareSpaceY);
			ToolTip.Default.SetToolTip(this.buttonShareSpaceY, "Distribution espacée verticalement");
			this.toolBarShare.Items.Add(this.buttonShareSpaceY);

			this.buttonShareTop = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.ShareTop.icon");
			this.buttonShareTop.Clicked += new MessageEventHandler(this.HandleButtonShareTop);
			ToolTip.Default.SetToolTip(this.buttonShareTop, "Distribution sur le haut");
			this.toolBarShare.Items.Add(this.buttonShareTop);

			this.buttonShareCenterY = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.ShareCenterY.icon");
			this.buttonShareCenterY.Clicked += new MessageEventHandler(this.HandleButtonShareCenterY);
			ToolTip.Default.SetToolTip(this.buttonShareCenterY, "Distribution centrée verticalement");
			this.toolBarShare.Items.Add(this.buttonShareCenterY);

			this.buttonShareBottom = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.ShareBottom.icon");
			this.buttonShareBottom.Clicked += new MessageEventHandler(this.HandleButtonShareBottom);
			ToolTip.Default.SetToolTip(this.buttonShareBottom, "Distribution sur le bas");
			this.toolBarShare.Items.Add(this.buttonShareBottom);
		}
		

		// Effectue la mise à jour du contenu.
		protected override void DoUpdateContent()
		{
			bool enabled = (this.document.Modifier.TotalSelected > 0);
			bool enabled2 = (this.document.Modifier.TotalSelected > 1);
			bool enabled3 = (this.document.Modifier.TotalSelected > 2);

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

		protected GroupBox				boxRotate;
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
	}
}
