using NUnit.Framework;
using Epsitec.Common.Widgets;
using Epsitec.Common.Pictogram;
using Epsitec.Common.Pictogram.Data;
using Epsitec.Common.Pictogram.Widgets;

namespace Epsitec.Common.Pictogram
{
	[TestFixture]
	public class PictogramTest
	{
		[Test] public void CheckApplication()
		{
			Window window = new Window();
			
			window.ClientSize = new Drawing.Size(500, 300);
			window.Text = "CheckApplication";
			window.Root.LayoutChanged += new EventHandler(this.Root_LayoutChanged);

			ToolTip tip = new ToolTip();
			tip.Behaviour = ToolTipBehaviour.Normal;

			this.root = new Widget();
			this.root.Size = window.ClientSize;
			this.root.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
			window.Root.Children.Add(this.root);

			this.toolbar = new HToolBar();
			this.toolbar.Location = new Drawing.Point(0, window.ClientSize.Height-this.toolbar.DefaultHeight);
			this.toolbar.Size = new Drawing.Size(window.ClientSize.Width, this.toolbar.DefaultHeight);
			this.toolbar.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.Top;
			this.toolbar.Parent = this.root;

			string[] texts =
			{
				"new",
				"open",
				"save",
				"",
				"cut",
				"copy",
				"paste",
				"",
				"undo",
			};

			for ( int i=0 ; i<texts.Length ; i++ )
			{
				if ( texts[i] == "" )
				{
					this.toolbar.Items.Add(new IconSeparator());
				}
				else
				{
					string name = texts[i] + "1";
					IconObjects objects = new IconObjects();
					objects.Read("..\\..\\images\\" + name + ".icon");
					SampleButton button = new SampleButton();
					button.Size = new Drawing.Size(22, 22);
					button.ButtonStyle = ButtonStyle.ToolItem;
					button.IconObjects = objects;
					button.Name = name;
					this.toolbar.Items.Add(button);
					tip.SetToolTip(button, texts[i]);
				}
			}

			StaticText title = new StaticText();
			title.SetClientZoom(2);
			title.Width = 200;
			title.Height = title.DefaultHeight*2;
			title.Location = new Drawing.Point(100, window.ClientSize.Height-70);
			title.Anchor = AnchorStyles.Left|AnchorStyles.Top;
			title.Text = "<i>Nice</i> <b>Icon</b>";
			title.Parent = this.root;

			StaticText label = new StaticText();
			label.Width = 400;
			label.Location = new Drawing.Point(100, window.ClientSize.Height-90);
			label.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.Top;
			label.Text = "Essayez de <b>modifier les dimensions</b> de la fenetre !";
			label.Parent = this.root;

			CheckButton check = new CheckButton();
			check.Width = 100;
			check.Location = new Drawing.Point(200, window.ClientSize.Height-120);
			check.Anchor = AnchorStyles.Left|AnchorStyles.Top;
			check.Text = "Enable";
			check.ActiveState = WidgetState.ActiveYes;
			check.Clicked += new MessageEventHandler(this.HandleCheck);
			check.Parent = this.root;

			RadioButton radio1 = new RadioButton();
			radio1.Width = 100;
			radio1.Location = new Drawing.Point(100, window.ClientSize.Height-120);
			radio1.Anchor = AnchorStyles.Left|AnchorStyles.Top;
			radio1.Group = "Style";
			radio1.Name = "1";
			radio1.Text = "Classique";
			radio1.ActiveState = WidgetState.ActiveYes;
			radio1.Clicked += new MessageEventHandler(this.HandleRadioStyle);
			radio1.Parent = this.root;

			RadioButton radio2 = new RadioButton();
			radio2.Width = 100;
			radio2.Location = new Drawing.Point(100, window.ClientSize.Height-120-radio1.DefaultHeight);
			radio2.Anchor = AnchorStyles.Left|AnchorStyles.Top;
			radio2.Group = "Style";
			radio2.Name = "2";
			radio2.Text = "Original";
			radio2.Clicked += new MessageEventHandler(this.HandleRadioStyle);
			radio2.Parent = this.root;

			string[] list = Epsitec.Common.Widgets.Adorner.Factory.AdornerNames;
			Drawing.Point pos = new Drawing.Point(100, window.ClientSize.Height-170);
			foreach ( string name in list )
			{
				RadioButton radio = new RadioButton();
				radio.Location = pos;
				radio.Width = 100;
				radio.Anchor = AnchorStyles.Left|AnchorStyles.Top;
				radio.Group = "Look";
				radio.Text = name;
				if ( name == Epsitec.Common.Widgets.Adorner.Factory.ActiveName )
				{
					radio.ActiveState = WidgetState.ActiveYes;
				}
				radio.Clicked += new MessageEventHandler(this.HandleRadioLook);
				radio.Parent = this.root;

				pos.Y -= radio.DefaultHeight;
			}

			window.Show();
		}

		private void HandleCheck(object sender, MessageEventArgs e)
		{
			CheckButton button = sender as CheckButton;
			this.toolbar.SetEnabled( (button.ActiveState&WidgetState.ActiveYes) == 0 );
		}

		private void HandleRadioStyle(object sender, MessageEventArgs e)
		{
			RadioButton radio = sender as RadioButton;
			foreach ( Widget widget in this.toolbar.Children )
			{
				SampleButton button = widget as SampleButton;
				if ( button == null )  continue;
				string name = button.Name;
				name = name.Remove(name.Length-1, 1);
				name += radio.Name;
				string filename = "..\\..\\images\\" + name + ".icon";
				IconObjects objects = new IconObjects();
				objects.Read(filename);
				button.IconObjects = objects;
			}
		}

		private void HandleRadioLook(object sender, MessageEventArgs e)
		{
			RadioButton button = sender as RadioButton;
			Epsitec.Common.Widgets.Adorner.Factory.SetActive(button.Text);
			Window.InvalidateAll();  // redessine toutes les fenêtres
		}

		private void Root_LayoutChanged(object sender)
		{
			if ( this.root == null )  return;
			double zoom = System.Math.Max(root.Height/300, 1);
			this.root.SetClientZoom(zoom);
		}


		[Test] public void CheckIconEditor()
		{
			Engine.Initialise();

			//Widgets.Adorner.Factory.SetActive("LookDefault");
			//Widgets.Adorner.Factory.SetActive("LookDany");
			//Widgets.Adorner.Factory.SetActive("LookCeeBot");
			//Widgets.Adorner.Factory.SetActive("LookPlastic");
			Epsitec.Common.Widgets.Adorner.Factory.SetActive("LookMetal");

			Window window = new Window();
			
			window.ClientSize = new Drawing.Size(720, 500);
			window.Text = "CheckIconEditor";

			IconEditor editor = new IconEditor();
			editor.Size = window.ClientSize;
			editor.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
			window.Root.Children.Add(editor);

			window.Show();
		}

		[Test] public void CheckButton()
		{
			Engine.Initialise();

			Window window = new Window();
			
			window.ClientSize = new Drawing.Size(400, 300);
			window.Text = "CheckButton";

			Button button = new Button();
			//button.IconName = @"<img src=""file:images/new.png""/>";
			button.Text = @"<img src=""file:images/new1.icon""/>";
			button.Location = new Drawing.Point(160, 150);
			button.Width = 24;
			button.Height = 24;
			button.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
			window.Root.Children.Add(button);

			window.Show();
		}

		[Test] public void CheckCanvasEngine()
		{
			Engine.Initialise();
			
			Window window = new Window();
			
			window.ClientSize = new Drawing.Size(200, 180);
			window.Text = "CheckCanvasEngine";
			
			StaticText icon1 = new StaticText (@"<img src=""file:images/new1.icon""/>");
			StaticText icon2 = new StaticText (@"<img src=""file:images/new1.icon""/>");
			StaticText icon3 = new StaticText (@"<img src=""file:images/new1.icon""/>");
			
			icon1.Dock = DockStyle.Top;
			icon1.Parent = window.Root;
			icon1.Size = new Drawing.Size (20, 20);
			icon1.Name = "Zoom x 1";
			icon1.DebugActive = true;
			
			icon2.Dock = DockStyle.Top;
			icon2.Parent = window.Root;
			icon2.SetClientZoom (1.5);
			icon2.Size = new Drawing.Size (20*1.5, 20*1.5);
			icon2.Name = "Zoom x 1.5";
			icon2.DebugActive = true;
			
			icon3.Dock = DockStyle.Top;
			icon3.Parent = window.Root;
			icon3.SetClientZoom (4.0);
			icon3.Size = new Drawing.Size (20*4.0, 20*4.0);
			icon3.Name = "Zoom x 4.0";
			icon3.DebugActive = true;
			icon3.SetEnabled (false);
			
			Button button = new Button ("Purge Image Cache");
			button.Dock = DockStyle.Bottom;
			button.Parent = window.Root;
			button.Clicked += new MessageEventHandler(CanvasEngineButtonClicked);
			
			window.Show();
		}

		protected Widget			root;
		protected HToolBar			toolbar;

		private void CanvasEngineButtonClicked(object sender, MessageEventArgs e)
		{
			Support.ImageProvider.Default.ClearImageCache ("file:images/new1.icon");
			System.Diagnostics.Debug.WriteLine ("Image cache cleared.");
			Widget widget = sender as Widget;
			widget.RootParent.Invalidate ();
		}
	}
}
