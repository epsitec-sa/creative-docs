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

			this.fieldMoveH = new TextFieldSlider();
			this.fieldMoveH.Width = 50;
			this.fieldMoveH.MinValue = 0.0M;
			this.fieldMoveH.MaxValue = 100.0M;
			this.fieldMoveH.Step = 1.0M;
			this.fieldMoveH.Resolution = 0.1M;
			this.fieldMoveH.Value = 1.0M;
			ToolTip.Default.SetToolTip(this.fieldMoveH, "Valeur du déplacement horizontal");
			this.toolBarMove.Items.Add(this.fieldMoveH);

			this.buttonMoveHi = new IconButton(@"file:images/opermovehi.icon");
			this.buttonMoveHi.Clicked += new MessageEventHandler(this.HandleButtonMoveHi);
			ToolTip.Default.SetToolTip(this.buttonMoveHi, "Déplacement à gauche");
			this.toolBarMove.Items.Add(this.buttonMoveHi);

			this.buttonMoveH = new IconButton(@"file:images/opermoveh.icon");
			this.buttonMoveH.Clicked += new MessageEventHandler(this.HandleButtonMoveH);
			ToolTip.Default.SetToolTip(this.buttonMoveH, "Déplacement à droite");
			this.toolBarMove.Items.Add(this.buttonMoveH);

			this.toolBarMove.Items.Add(new IconSeparator());

			this.fieldMoveV = new TextFieldSlider();
			this.fieldMoveV.Width = 50;
			this.fieldMoveV.MinValue = 0.0M;
			this.fieldMoveV.MaxValue = 100.0M;
			this.fieldMoveV.Step = 1.0M;
			this.fieldMoveV.Resolution = 0.1M;
			this.fieldMoveV.Value = 1.0M;
			ToolTip.Default.SetToolTip(this.fieldMoveV, "Valeur du déplacement vertical");
			this.toolBarMove.Items.Add(this.fieldMoveV);

			this.buttonMoveVi = new IconButton(@"file:images/opermovevi.icon");
			this.buttonMoveVi.Clicked += new MessageEventHandler(this.HandleButtonMoveVi);
			ToolTip.Default.SetToolTip(this.buttonMoveVi, "Déplacement en bas");
			this.toolBarMove.Items.Add(this.buttonMoveVi);

			this.buttonMoveV = new IconButton(@"file:images/opermovev.icon");
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

			this.buttonRotate90 = new IconButton(@"file:images/operrot90.icon");
			this.buttonRotate90.Clicked += new MessageEventHandler(this.HandleButtonRotate90);
			ToolTip.Default.SetToolTip(this.buttonRotate90, "Quart de tour à gauche");
			this.toolBarRot.Items.Add(this.buttonRotate90);

			this.buttonRotate180 = new IconButton(@"file:images/operrot180.icon");
			this.buttonRotate180.Clicked += new MessageEventHandler(this.HandleButtonRotate180);
			ToolTip.Default.SetToolTip(this.buttonRotate180, "Demi-tour");
			this.toolBarRot.Items.Add(this.buttonRotate180);

			this.buttonRotate270 = new IconButton(@"file:images/operrot270.icon");
			this.buttonRotate270.Clicked += new MessageEventHandler(this.HandleButtonRotate270);
			ToolTip.Default.SetToolTip(this.buttonRotate270, "Quart de tour à droite");
			this.toolBarRot.Items.Add(this.buttonRotate270);

			this.toolBarRot.Items.Add(new IconSeparator());

			this.fieldRotate = new TextFieldSlider();
			this.fieldRotate.Width = 50;
			this.fieldRotate.MinValue = 0.0M;
			this.fieldRotate.MaxValue = 360.0M;
			this.fieldRotate.Step = 2.5M;
			this.fieldRotate.Resolution = 0.1M;
			this.fieldRotate.Value = 10.0M;
			ToolTip.Default.SetToolTip(this.fieldRotate, "Angle de rotation en degrés");
			this.toolBarRot.Items.Add(this.fieldRotate);

			this.buttonRotate = new IconButton(@"file:images/operrot.icon");
			this.buttonRotate.Clicked += new MessageEventHandler(this.HandleButtonRotate);
			ToolTip.Default.SetToolTip(this.buttonRotate, "Rotation anti-horaire");
			this.toolBarRot.Items.Add(this.buttonRotate);

			this.buttonRotatei = new IconButton(@"file:images/operroti.icon");
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

			this.buttonMirrorH = new IconButton(@"file:images/opermirrorh.icon");
			this.buttonMirrorH.Clicked += new MessageEventHandler(this.HandleButtonMirrorH);
			ToolTip.Default.SetToolTip(this.buttonMirrorH, "Miroir horizontal");
			this.toolBarMirror.Items.Add(this.buttonMirrorH);

			this.buttonMirrorV = new IconButton(@"file:images/opermirrorv.icon");
			this.buttonMirrorV.Clicked += new MessageEventHandler(this.HandleButtonMirrorV);
			ToolTip.Default.SetToolTip(this.buttonMirrorV, "Miroir vertical");
			this.toolBarMirror.Items.Add(this.buttonMirrorV);

			// Zoom.
			this.boxZoom = new GroupBox(this);
			this.boxZoom.Height = 45;
			this.boxZoom.Text = "Agrandissements et réductions";
			this.boxZoom.Dock = DockStyle.Top;
			this.boxZoom.DockMargins = new Margins(0, 0, 0, 10);

			this.toolBarZoom = new HToolBar(this.boxZoom);
			this.toolBarZoom.Dock = DockStyle.Top;
			this.toolBarZoom.DockMargins = new Margins(0, 0, 0, 10);

			this.buttonZoomMul2 = new IconButton(@"file:images/operzoommul2.icon");
			this.buttonZoomMul2.Clicked += new MessageEventHandler(this.HandleButtonZoomMul2);
			ToolTip.Default.SetToolTip(this.buttonZoomMul2, "Zoom x2");
			this.toolBarZoom.Items.Add(this.buttonZoomMul2);

			this.buttonZoomDiv2 = new IconButton(@"file:images/operzoomdiv2.icon");
			this.buttonZoomDiv2.Clicked += new MessageEventHandler(this.HandleButtonZoomDiv2);
			ToolTip.Default.SetToolTip(this.buttonZoomDiv2, "Zoom /2");
			this.toolBarZoom.Items.Add(this.buttonZoomDiv2);

			this.toolBarZoom.Items.Add(new IconSeparator());

			this.fieldZoom = new TextFieldSlider();
			this.fieldZoom.Width = 50;
			this.fieldZoom.MinValue = 1.0M;
			this.fieldZoom.MaxValue = 2.0M;
			this.fieldZoom.Step = 0.1M;
			this.fieldZoom.Resolution = 0.01M;
			this.fieldZoom.Value = 1.2M;
			ToolTip.Default.SetToolTip(this.fieldZoom, "Facteur d'agrandissement/réduction");
			this.toolBarZoom.Items.Add(this.fieldZoom);

			this.buttonZoom = new IconButton(@"file:images/operzoom.icon");
			this.buttonZoom.Clicked += new MessageEventHandler(this.HandleButtonZoom);
			ToolTip.Default.SetToolTip(this.buttonZoom, "Agrandissement");
			this.toolBarZoom.Items.Add(this.buttonZoom);

			this.buttonZoomi = new IconButton(@"file:images/operzoomi.icon");
			this.buttonZoomi.Clicked += new MessageEventHandler(this.HandleButtonZoomi);
			ToolTip.Default.SetToolTip(this.buttonZoomi, "Réduction");
			this.toolBarZoom.Items.Add(this.buttonZoomi);
		}
		

		// Effectue la mise à jour du contenu.
		protected override void DoUpdateContent()
		{
			bool enabled = (this.document.Modifier.TotalSelected > 0);

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
		}


		private void HandleButtonMoveH(object sender, MessageEventArgs e)
		{
			double dx = (double) this.fieldMoveH.Value;
			this.document.Modifier.MoveSelection(new Point(dx,0));
		}

		private void HandleButtonMoveHi(object sender, MessageEventArgs e)
		{
			double dx = (double) this.fieldMoveH.Value;
			this.document.Modifier.MoveSelection(new Point(-dx,0));
		}

		private void HandleButtonMoveV(object sender, MessageEventArgs e)
		{
			double dy = (double) this.fieldMoveV.Value;
			this.document.Modifier.MoveSelection(new Point(0,dy));
		}

		private void HandleButtonMoveVi(object sender, MessageEventArgs e)
		{
			double dy = (double) this.fieldMoveV.Value;
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
			double angle = (double) this.fieldRotate.Value;
			this.document.Modifier.RotateSelection(angle);
		}

		private void HandleButtonRotatei(object sender, MessageEventArgs e)
		{
			double angle = (double) this.fieldRotate.Value;
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
			double scale = (double) this.fieldZoom.Value;
			this.document.Modifier.ZoomSelection(scale);
		}

		private void HandleButtonZoomi(object sender, MessageEventArgs e)
		{
			double scale = (double) this.fieldZoom.Value;
			this.document.Modifier.ZoomSelection(1.0/scale);
		}


		protected GroupBox				boxMove;
		protected HToolBar				toolBarMove;
		protected TextFieldSlider		fieldMoveH;
		protected IconButton			buttonMoveH;
		protected IconButton			buttonMoveHi;
		protected TextFieldSlider		fieldMoveV;
		protected IconButton			buttonMoveV;
		protected IconButton			buttonMoveVi;

		protected GroupBox				boxRotate;
		protected HToolBar				toolBarRot;
		protected IconButton			buttonRotate90;
		protected IconButton			buttonRotate180;
		protected IconButton			buttonRotate270;
		protected TextFieldSlider		fieldRotate;
		protected IconButton			buttonRotate;
		protected IconButton			buttonRotatei;
		
		protected GroupBox				boxMirror;
		protected HToolBar				toolBarMirror;
		protected IconButton			buttonMirrorH;
		protected IconButton			buttonMirrorV;
		
		protected GroupBox				boxZoom;
		protected HToolBar				toolBarZoom;
		protected IconButton			buttonZoomMul2;
		protected IconButton			buttonZoomDiv2;
		protected TextFieldSlider		fieldZoom;
		protected IconButton			buttonZoom;
		protected IconButton			buttonZoomi;
	}
}
