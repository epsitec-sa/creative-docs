using NUnit.Framework;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	using PropertyChangedEventHandler = Epsitec.Common.Support.EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>;
	
	[TestFixture] public class WidgetTest
	{
		[SetUp] public void Initialise()
		{
			Epsitec.Common.Document.Engine.Initialise ();
			Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookMetal");
			Epsitec.Common.Widgets.Widget.Initialise ();
		}

		[Test]
		public void AutomatedTestEnvironment()
		{
			Epsitec.Common.Widgets.Window.RunningInAutomatedTestEnvironment = true;
		}
		

#if true
		[Test] public void CheckAllocation()
		{
			int runs = 10000;
			Widget dummy = new Widget ();
			Widget[] widgets = new Widget[runs];
			System.Console.WriteLine ("Testing Widget allocation.");
			
			for (int i = 0; i < 1000; i++)
			{
				long cc = Epsitec.Common.Drawing.Agg.Library.Cycles;
			}
			
			long c1 = Epsitec.Common.Drawing.Agg.Library.Cycles;
			long c2 = Epsitec.Common.Drawing.Agg.Library.Cycles;
			long c0 = Epsitec.Common.Drawing.Agg.Library.Cycles - c2;
			
			System.Console.Out.WriteLine ("Zero work: " + c0.ToString ());
			
			long s1 = System.GC.GetTotalMemory (true);
			c1 = Epsitec.Common.Drawing.Agg.Library.Cycles;
			for (int i = 0; i < runs; i++)
			{
				widgets[i] = new Widget ();
			}
			c2 = Epsitec.Common.Drawing.Agg.Library.Cycles;
			long s2 = System.GC.GetTotalMemory (true);
			
			System.Console.WriteLine ("Using {0} bytes / empty Widget instance, {1} cycles.", (s2-s1) / runs, (c2-c1) / runs);
		}
		
		[Test] public void CheckAllocationWithText()
		{
			int runs = 10000;
			Widget dummy = new Widget ();
			Widget[] widgets = new Widget[runs];
			System.Console.WriteLine ("Testing Widget allocation.");
			
			for (int i = 0; i < 1000; i++)
			{
				long cc = Epsitec.Common.Drawing.Agg.Library.Cycles;
			}
			
			long c1 = Epsitec.Common.Drawing.Agg.Library.Cycles;
			long c2 = Epsitec.Common.Drawing.Agg.Library.Cycles;
			long c0 = Epsitec.Common.Drawing.Agg.Library.Cycles - c2;
			
			System.Console.Out.WriteLine ("Zero work: " + c0.ToString ());
			
			long s1 = System.GC.GetTotalMemory (true);
			c1 = Epsitec.Common.Drawing.Agg.Library.Cycles;
			for (int i = 0; i < runs; i++)
			{
				widgets[i] = new Widget ();
				widgets[i].Text = "Test";
			}
			c2 = Epsitec.Common.Drawing.Agg.Library.Cycles;
			long s2 = System.GC.GetTotalMemory (true);
			
			System.Console.WriteLine ("Using {0} bytes / Widget instance with text, {1} cycles.", (s2-s1) / runs, (c2-c1) / runs);
		}
#endif

#if false
		[Test] public void CheckAbstractWidget()
		{
			System.Console.WriteLine ("Performance test of AbstractWidget Properties");
			System.Console.WriteLine ("--------------------------------------100'000");
			System.Console.Out.Flush ();
			this.PrivateCheckAbstractWidget (100000);
			System.Console.WriteLine ("------------------------------------1'000'000");
			System.Console.Out.Flush ();
			this.PrivateCheckAbstractWidget (1000000);
			System.Console.WriteLine ("-----------------------------------10'000'000");
			System.Console.Out.Flush ();
			this.PrivateCheckAbstractWidget (10000000);
			System.Console.WriteLine ("-----------------------------------------done");
			System.Console.Out.Flush ();
		}
		
		private void PrivateCheckAbstractWidget(int runs)
		{
			AbstractWidget widget = new AbstractWidget ();
			
			for (int i = 0; i < 1000; i++)
			{
				long cc = Epsitec.Common.Drawing.Agg.Library.Cycles;
			}
			
			long c1 = Epsitec.Common.Drawing.Agg.Library.Cycles;
			long c2 = Epsitec.Common.Drawing.Agg.Library.Cycles;
			long c0 = Epsitec.Common.Drawing.Agg.Library.Cycles - c2;
			
			System.Console.Out.WriteLine ("Zero work: " + c0.ToString ());
			
			widget.Anchor        = AnchorStyles.None;
			widget.ManagedAnchor = AnchorStyles.None;
			
			widget.ManagedAnchor = widget.Anchor;
			widget.Anchor = widget.ManagedAnchor;
			
			widget.Anchor        = AnchorStyles.All;
			widget.ManagedAnchor = AnchorStyles.All;
			
			c1 = Epsitec.Common.Drawing.Agg.Library.Cycles;
			
			for (int i = 0; i < runs; i++)
			{
				widget.Anchor = AnchorStyles.Left;
				widget.Anchor = AnchorStyles.Right;
				widget.Anchor = AnchorStyles.Bottom;
				widget.Anchor = AnchorStyles.Top;
				widget.Anchor = AnchorStyles.None;
			}
			
			c2 = Epsitec.Common.Drawing.Agg.Library.Cycles - c1;
			
			System.Console.WriteLine ("Setting 5 properties: {0} cycles.", c2 / runs);
			
			c1 = Epsitec.Common.Drawing.Agg.Library.Cycles;
			
			for (int i = 0; i < runs; i++)
			{
				AnchorStyles a1 = widget.Anchor;
				AnchorStyles a2 = widget.Anchor;
				AnchorStyles a3 = widget.Anchor;
				AnchorStyles a4 = widget.Anchor;
				AnchorStyles a5 = widget.Anchor;
			}
			
			c2 = Epsitec.Common.Drawing.Agg.Library.Cycles - c1;
			
			System.Console.WriteLine ("Getting 5 properties: {0} cycles.", c2 / runs);
			
			c1 = Epsitec.Common.Drawing.Agg.Library.Cycles;
			
			for (int i = 0; i < runs; i++)
			{
				widget.ManagedAnchor = AnchorStyles.Left;
				widget.ManagedAnchor = AnchorStyles.Right;
				widget.ManagedAnchor = AnchorStyles.Bottom;
				widget.ManagedAnchor = AnchorStyles.Top;
				widget.ManagedAnchor = AnchorStyles.None;
			}
			
			c2 = Epsitec.Common.Drawing.Agg.Library.Cycles - c1;
			
			System.Console.WriteLine ("Setting 5 managed properties: {0} cycles.", c2 / runs);
			
			c1 = Epsitec.Common.Drawing.Agg.Library.Cycles;
			
			for (int i = 0; i < runs; i++)
			{
				AnchorStyles a1 = widget.ManagedAnchor;
				AnchorStyles a2 = widget.ManagedAnchor;
				AnchorStyles a3 = widget.ManagedAnchor;
				AnchorStyles a4 = widget.ManagedAnchor;
				AnchorStyles a5 = widget.ManagedAnchor;
			}
			
			c2 = Epsitec.Common.Drawing.Agg.Library.Cycles - c1;
			
			System.Console.WriteLine ("Getting 5 managed properties: {0} cycles.", c2 / runs);
			
			
			widget.AnchorChanged += new EventHandler(this.HandleAnchorChanged);
			widget.ManagedAnchorChanged += new Types.PropertyChangedEventHandler(this.HandleManagedAnchorChanged);
			
			widget.Anchor        = AnchorStyles.None;
			widget.ManagedAnchor = AnchorStyles.None;
			
			widget.ManagedAnchor = widget.Anchor;
			widget.Anchor = widget.ManagedAnchor;
			
			widget.Anchor        = AnchorStyles.All;
			widget.ManagedAnchor = AnchorStyles.All;
			
			
			c1 = Epsitec.Common.Drawing.Agg.Library.Cycles;
			
			for (int i = 0; i < runs; i++)
			{
				widget.Anchor = AnchorStyles.Left;
				widget.Anchor = AnchorStyles.Right;
				widget.Anchor = AnchorStyles.Bottom;
				widget.Anchor = AnchorStyles.Top;
				widget.Anchor = AnchorStyles.None;
			}
			
			c2 = Epsitec.Common.Drawing.Agg.Library.Cycles - c1;
			
			System.Console.WriteLine ("Setting 5 properties, sending event: {0} cycles.", c2 / runs);
			
			c1 = Epsitec.Common.Drawing.Agg.Library.Cycles;
			
			for (int i = 0; i < runs; i++)
			{
				AnchorStyles a1 = widget.Anchor;
				AnchorStyles a2 = widget.Anchor;
				AnchorStyles a3 = widget.Anchor;
				AnchorStyles a4 = widget.Anchor;
				AnchorStyles a5 = widget.Anchor;
			}
			
			c2 = Epsitec.Common.Drawing.Agg.Library.Cycles - c1;
			
			System.Console.WriteLine ("Getting 5 properties: {0} cycles.", c2 / runs);
			
			c1 = Epsitec.Common.Drawing.Agg.Library.Cycles;
			
			for (int i = 0; i < runs; i++)
			{
				widget.ManagedAnchor = AnchorStyles.Left;
				widget.ManagedAnchor = AnchorStyles.Right;
				widget.ManagedAnchor = AnchorStyles.Bottom;
				widget.ManagedAnchor = AnchorStyles.Top;
				widget.ManagedAnchor = AnchorStyles.None;
			}
			
			c2 = Epsitec.Common.Drawing.Agg.Library.Cycles - c1;
			
			System.Console.WriteLine ("Setting 5 managed properties, sending event: {0} cycles.", c2 / runs);
			
			c1 = Epsitec.Common.Drawing.Agg.Library.Cycles;
			
			for (int i = 0; i < runs; i++)
			{
				AnchorStyles a1 = widget.ManagedAnchor;
				AnchorStyles a2 = widget.ManagedAnchor;
				AnchorStyles a3 = widget.ManagedAnchor;
				AnchorStyles a4 = widget.ManagedAnchor;
				AnchorStyles a5 = widget.ManagedAnchor;
			}
			
			c2 = Epsitec.Common.Drawing.Agg.Library.Cycles - c1;
			
			System.Console.WriteLine ("Getting 5 managed properties: {0} cycles.", c2 / runs);
		}
#endif
		
		[Test] public void CheckTextFrame()
		{
			Window window = new Window ();
			
			window.ClientSize = new Size (400, 500);
			window.Text       = "CheckTextFrame";
			
			TextFrame      frame     = new TextFrame ();
			TextNavigator2 navigator = frame.TextNavigator;
			
			Text.TextStory   story     = frame.TextStory;
			Text.TextStyle[] no_styles = new Text.TextStyle[0];
			
			foreach (string face in Common.Text.TextContext.GetAvailableFontFaces ())
			{
				System.Console.WriteLine ("Font face: {0}", face);
				
				foreach (OpenType.FontIdentity id in Common.Text.TextContext.GetAvailableFontIdentities (face))
				{
					System.Console.WriteLine ("  '{0}', FontWeight={1}, FontStyle={2}, ({3})", id.InvariantStyleName, id.FontStyle, id.FontWeight, id.FullName);
				}
			}
			
			System.Collections.ArrayList properties = new System.Collections.ArrayList ();
			
			Text.Properties.FontProperty fp = new Text.Properties.FontProperty ("Palatino Linotype", "Italic", "liga", "dlig", "kern");
			
			properties.Add (fp);
			properties.Add (new Text.Properties.FontSizeProperty (14.0, Text.Properties.SizeUnits.Points));
			properties.Add (new Text.Properties.MarginsProperty (60, 10, 10, 10, Text.Properties.SizeUnits.Points, 0.0, 0.0, 0.0, 15, 1, Text.Properties.ThreeState.True));
			properties.Add (new Text.Properties.FontColorProperty ("Black"));
			properties.Add (new Text.Properties.LanguageProperty ("fr-ch", 1.0));
			properties.Add (new Text.Properties.LeadingProperty (16.0, Text.Properties.SizeUnits.Points, 15.0, Text.Properties.SizeUnits.Points, 5.0, Text.Properties.SizeUnits.Points, Text.Properties.AlignMode.None));
			
			Text.TextStyle style = story.TextContext.StyleList.NewTextStyle (null, "Default", Text.TextStyleClass.Paragraph, properties);
			
			story.TextContext.DefaultParagraphStyle = style;
			story.TextContext.ShowControlCharacters = true;
			story.TextContext.IsDegradedLayoutEnabled = true;
			
			string words = "Bonjour, ceci est un texte d'exemple permettant de v�rifier le bon fonctionnement des divers algorithmes de d�coupe et d'affichage. Le nombre de mots moyen s'�l�ve � environ 40 mots par paragraphe, ce qui correspond � des paragraphes de taille r�duite. Quelle id�e, un fjord finlandais ! Avocat.\nAWAY.\n______\n";
			
			navigator.Insert (Text.Unicode.Code.EndOfText);
			navigator.TextNavigator.MoveTo (Text.TextNavigator.Target.TextStart, 0);
			navigator.Insert (words);
			
			frame.OpletQueue.PurgeUndo ();
			
			Widget pane = new Widget ();
			
			pane.SetParent (window.Root);
			pane.PreferredWidth = 80;
			pane.Dock = DockStyle.Right;
			
			TextFrameManager manager = new TextFrameManager (pane, frame);
			
			frame.Dock    = DockStyle.Fill;
			frame.Margins = new Margins (4, 4, 4, 4);
			frame.SetParent (window.Root);
			frame.Name    = "A";
			
			frame.Focus ();
			
			TextFrame frame2 = new TextFrame (frame);

			frame2.PreferredHeight = 150;
			frame2.Dock    = DockStyle.Bottom;
			frame2.Margins = new Margins (4, 4, 4, 4);
			frame2.SetParent (window.Root);
			frame2.Name    = "B";
			
			window.Show ();
			Window.RunInTestEnvironment (window);
		}
		
		
		#region TextFrameManager Class
		class TextFrameManager
		{
			public TextFrameManager(Widget pane, TextFrame frame)
			{
				this.frame = frame;
				
				this.CreateButton (pane, "Default",			"default");
				
				this.CreateButton (pane, "Regular",			"style:regular");
				this.CreateButton (pane, "<i>Italic</i>",	"style:italic");
				this.CreateButton (pane, "<b>Bold</b>",		"style:bold");
				this.CreateButton (pane, "Rst Style",		"style:reset");
				
				this.CreateButton (pane, "8 pts",			"size:8");
				this.CreateButton (pane, "24 pts",			"size:24");
				this.CreateButton (pane, "+20 %",			"size:+20%");
				this.CreateButton (pane, "Rst Size",		"size:reset");
				
				this.CreateButton (pane, "|&lt;---", "left-aligned");
				this.CreateButton (pane, "&lt;-|-&gt;", "centered");
				this.CreateButton (pane, "---&gt;|", "right-aligned");
				this.CreateButton (pane, "|&lt;-----&gt;|", "justified");
				this.CreateButton (pane, "Auto leading", "lead:auto");
				this.CreateButton (pane, "150% leading", "lead:150%");
				this.CreateButton (pane, "20 pts leading", "lead:20");
				
				this.CreateButton (pane, "Undo", "undo");
				this.CreateButton (pane, "Redo", "redo");
				this.CreateButton (pane, "Forget", "purge-undo-redo");
				
//-				pane.FindChild ("undo").Enable = false;
//-				pane.FindChild ("redo").Enable = false;
			}
			
			private void CreateButton(Widget pane, string title, string name)
			{
				Button button;
				
				button        = new Button (title);
				button.Dock   = DockStyle.Top;
				button.SetParent (pane);
				button.Name   = name;
				button.Clicked += new MessageEventHandler(this.CheckTextFrameButtonClicked);
				button.AutoFocus = false;
			}
			
			
			private void CheckTextFrameButtonClicked(object sender, MessageEventArgs e)
			{
				Button button = sender as Button;
				
				switch (button.Name)
				{
					case "default":
						this.frame.TextNavigator.SetParagraphStyles (this.frame.TextStory.TextContext.DefaultParagraphStyle);
						this.frame.TextNavigator.SetParagraphProperties (Common.Text.Properties.ApplyMode.Overwrite, new Text.Property[0]);
						break;
					
					case "style:regular":
						this.frame.TextNavigator.SetTextProperties (Common.Text.Properties.ApplyMode.Set, new Common.Text.Properties.FontProperty (null, "Regular"));
						break;
					case "style:bold":
						this.frame.TextNavigator.SetTextProperties (Common.Text.Properties.ApplyMode.Combine, new Common.Text.Properties.FontProperty (null, "!Bold"));
						break;
					case "style:italic":
						this.frame.TextNavigator.SetTextProperties (Common.Text.Properties.ApplyMode.Combine, new Common.Text.Properties.FontProperty (null, "!Italic"));
						break;
					case "style:reset":
						this.frame.TextNavigator.SetTextProperties (Common.Text.Properties.ApplyMode.Clear, new Common.Text.Properties.FontProperty ());
						break;
					
					case "size:8":
						this.frame.TextNavigator.SetTextProperties (Common.Text.Properties.ApplyMode.Set, new Common.Text.Properties.FontSizeProperty (8.0, Common.Text.Properties.SizeUnits.Points));
						break;
					case "size:24":
						this.frame.TextNavigator.SetTextProperties (Common.Text.Properties.ApplyMode.Set, new Common.Text.Properties.FontSizeProperty (24.0, Common.Text.Properties.SizeUnits.Points));
						break;
					case "size:+20%":
						this.frame.TextNavigator.SetTextProperties (Common.Text.Properties.ApplyMode.Combine, new Common.Text.Properties.FontSizeProperty (120.0, Common.Text.Properties.SizeUnits.Percent));
						break;
					case "size:reset":
						this.frame.TextNavigator.SetTextProperties (Common.Text.Properties.ApplyMode.Clear, new Common.Text.Properties.FontSizeProperty ());
						break;
					
					case "left-aligned":
						this.frame.TextNavigator.SetParagraphProperties (Common.Text.Properties.ApplyMode.Set, new Common.Text.Properties.MarginsProperty (double.NaN, double.NaN, double.NaN, double.NaN, Common.Text.Properties.SizeUnits.None, 0, 0, 0, double.NaN, double.NaN, Common.Text.Properties.ThreeState.Undefined));
						break;
					case "right-aligned":
						this.frame.TextNavigator.SetParagraphProperties (Common.Text.Properties.ApplyMode.Set, new Common.Text.Properties.MarginsProperty (double.NaN, double.NaN, double.NaN, double.NaN, Common.Text.Properties.SizeUnits.None, 0, 0, 1, double.NaN, double.NaN, Common.Text.Properties.ThreeState.Undefined));
						break;
					case "centered":
						this.frame.TextNavigator.SetParagraphProperties (Common.Text.Properties.ApplyMode.Set, new Common.Text.Properties.MarginsProperty (double.NaN, double.NaN, double.NaN, double.NaN, Common.Text.Properties.SizeUnits.None, 0, 0, 0.5, double.NaN, double.NaN, Common.Text.Properties.ThreeState.Undefined));
						break;
					case "justified":
						this.frame.TextNavigator.SetParagraphProperties (Common.Text.Properties.ApplyMode.Set, new Common.Text.Properties.MarginsProperty (double.NaN, double.NaN, double.NaN, double.NaN, Common.Text.Properties.SizeUnits.None, 1.0, 0, 0, double.NaN, double.NaN, Common.Text.Properties.ThreeState.Undefined));
						break;
					
					case "lead:auto":
						this.frame.TextNavigator.SetParagraphProperties (Common.Text.Properties.ApplyMode.Set, new Common.Text.Properties.LeadingProperty (0.0, Common.Text.Properties.SizeUnits.Points, Common.Text.Properties.AlignMode.Undefined));
						break;
					case "lead:150%":
						this.frame.TextNavigator.SetParagraphProperties (Common.Text.Properties.ApplyMode.Set, new Common.Text.Properties.LeadingProperty (150.0, Common.Text.Properties.SizeUnits.Percent, Common.Text.Properties.AlignMode.Undefined));
						break;
					case "lead:20":
						this.frame.TextNavigator.SetParagraphProperties (Common.Text.Properties.ApplyMode.Set, new Common.Text.Properties.LeadingProperty (20.0, Common.Text.Properties.SizeUnits.Points, Common.Text.Properties.AlignMode.Undefined));
						break;
						
					case "undo":
						this.frame.TextNavigator.Undo ();
						break;
					case "redo":
						this.frame.TextNavigator.Redo ();
						break;
					
					case "purge-undo-redo":
						this.frame.OpletQueue.PurgeUndo ();
						this.frame.OpletQueue.PurgeRedo ();
						break;
				}
				
//-				System.Diagnostics.Debug.WriteLine ("Text=" + this.frame.TextStory.GetDebugText ());
//-				System.Diagnostics.Debug.WriteLine ("Undo=" + this.frame.TextStory.GetDebugUndo ());
//-				System.Diagnostics.Debug.WriteLine ("");
				
				button.Parent.FindChild ("undo").Enable = this.frame.OpletQueue.CanUndo;
				button.Parent.FindChild ("redo").Enable = this.frame.OpletQueue.CanRedo;
				
				this.frame.TextNavigator.NotifyTextChanged ();
			}
			
			
			private TextFrame					frame;
		}
		#endregion
		
		[Test] public void CheckParentChildRelationship()
		{
			Widget root = new Widget ();
			Widget widget = new Widget ();
			
			root.Children.Add (widget);
			
			Assert.IsTrue (root.HasChildren);
			Assert.IsTrue (root.Children.Count == 1);
			Assert.IsTrue (widget.HasParent);
			Assert.AreSame (widget.Parent, root);
			Assert.AreSame (root.Children[0], widget);
			
			root.Children.Remove (widget);
			
			Assert.IsTrue (root.HasChildren == false);
			
			widget.SetParent (root);
			
			Assert.IsTrue (root.HasChildren);
			Assert.IsTrue (root.Children.Count == 1);
			Assert.IsTrue (widget.HasParent);
			Assert.AreSame (widget.Parent, root);
			Assert.AreSame (root.Children[0], widget);
		}
		
		[Test] public void CheckParentChanged()
		{
			Widget w0 = new Widget ();
			Widget w1 = new Widget ();
			Widget w2 = new Widget ();
			
			w2.ParentChanged += new PropertyChangedEventHandler (this.HandleCheckParentChangedParentChanged);
			
			this.check_parent_changed_sender = null;
			this.check_parent_changed_count  = 0;
			w2.SetParent (w1);
			Assert.AreEqual (w2, this.check_parent_changed_sender);
			Assert.AreEqual (1, this.check_parent_changed_count);
			Assert.AreEqual (null, this.check_parent_changed_old_value);
			Assert.AreEqual (w1, this.check_parent_changed_new_value);
			
			this.check_parent_changed_sender = null;
			this.check_parent_changed_count  = 0;
			w2.SetParent (null);
			Assert.AreEqual (w2, this.check_parent_changed_sender);
			Assert.AreEqual (1, this.check_parent_changed_count);
			Assert.AreEqual (w1, this.check_parent_changed_old_value);
			Assert.AreEqual (null, this.check_parent_changed_new_value);
			
			this.check_parent_changed_sender = null;
			this.check_parent_changed_count  = 0;
			w1.Children.Add (w2);
			Assert.AreEqual (w2, this.check_parent_changed_sender);
			Assert.AreEqual (1, this.check_parent_changed_count);
			Assert.AreEqual (null, this.check_parent_changed_old_value);
			Assert.AreEqual (w1, this.check_parent_changed_new_value);
			
			this.check_parent_changed_sender = null;
			this.check_parent_changed_count  = 0;
			w0.Children.Add (w2);
			Assert.AreEqual (w2, this.check_parent_changed_sender);
			Assert.AreEqual (1, this.check_parent_changed_count);			// 1 seule notification !
			Assert.AreEqual (w1, this.check_parent_changed_old_value);
			Assert.AreEqual (w0, this.check_parent_changed_new_value);
		}
		
		
		#region CheckParentChanged event handler
		private object		check_parent_changed_sender;
		private object		check_parent_changed_old_value;
		private object		check_parent_changed_new_value;
		private int			check_parent_changed_count;
		
		private void HandleCheckParentChangedParentChanged(object sender, Types.DependencyPropertyChangedEventArgs e)
		{
			this.check_parent_changed_sender = sender;
			this.check_parent_changed_old_value = e.OldValue;
			this.check_parent_changed_new_value = e.NewValue;
			this.check_parent_changed_count++;
		}
		#endregion
		
		
		[Test] public void CheckText()
		{
			Widget widget = new Widget ();
			string text = "Hel<m>l</m>o";
			widget.Text = text;
			widget.AutoMnemonic = true;
			Assert.AreEqual (text, widget.Text);
			Assert.AreEqual ('L', widget.Mnemonic);
			widget.Text = null;
			Assert.AreEqual ("", widget.Text);
		}
		
		[Test] public void CheckTextLayoutInfo()
		{
			Window window = new Window ();
			window.ClientSize = new Size (200, 120);
			window.Text       = "CheckTextLayoutInfo";
			
			StaticText text   = new StaticText ("<font size=\"300%\">Abcdefgh... Abcdefgh...</font>");
			
//-			text.SetClientZoom (3);

			text.PreferredSize = new Drawing.Size(text.PreferredSize.Width / 2, text.PreferredSize.Height * 2) * 3;
			text.Anchor = AnchorStyles.TopLeft;
			text.Margins = new Drawing.Margins (10, 0, 10, 0);
			text.SetParent (window.Root);
			text.PaintForeground += new PaintEventHandler(this.CheckTextLayoutInfoPaintForeground);
			
			window.Show ();
			Window.RunInTestEnvironment (window);
		}
		
		
		[Test] public void CheckPointMath()
		{
			Widget root = new Widget ();
			Widget widget = new Widget ();
			
			root.SetManualBounds(new Rectangle (0, 0, 300, 200));
			widget.SetManualBounds(new Rectangle (30, 20, 50, 40));

			Assert.IsTrue (widget.ActualBounds.Left == 30);
			Assert.IsTrue (widget.ActualBounds.Right == 80);
			Assert.IsTrue (widget.ActualBounds.Top == 60);
			Assert.IsTrue (widget.ActualBounds.Bottom == 20);
			Assert.IsTrue (widget.Client.Size.Width == 50);
			Assert.IsTrue (widget.Client.Size.Height == 40);
			
			root.Children.Add (widget);

			Assert.IsTrue (widget.ActualBounds.Left == 30);
			Assert.IsTrue (widget.ActualBounds.Right == 80);
			Assert.IsTrue (widget.ActualBounds.Top == 60);
			Assert.IsTrue (widget.ActualBounds.Bottom == 20);
			
			Point pt_test   = new Point (40, 35);
			Point pt_client = widget.MapParentToClient (pt_test);
			Point pt_widget = widget.MapClientToParent (pt_client);
			
			Assert.IsTrue (pt_client.X == 10);
			Assert.IsTrue (pt_client.Y == 15);
			Assert.IsTrue (pt_widget.X == pt_test.X);
			Assert.IsTrue (pt_widget.Y == pt_test.Y);
		}
		
		[Test] public void CheckTransformToClient()
		{
			Widget root = new Widget ();
			Widget widget = new Widget ();

			root.SetManualBounds(new Rectangle (0, 0, 300, 200));
			widget.SetManualBounds(new Rectangle (30, 20, 50, 40));
			
			root.Children.Add (widget);
			
			Epsitec.Common.Drawing.Transform transform = new Epsitec.Common.Drawing.Transform ();
			
//			double ox = 1.0;
//			double oy = 2.0;
//			
//			Point pt1 = new Point (widget.Left + ox, widget.Bottom + oy);
//			Point pt2;
//			Point pt3;
//			
//			widget.SetClientAngle (0);
//			transform = widget.GetTransformToClient ();
//			
//			pt2 = widget.MapParentToClient (pt1);
//			pt3 = transform.TransformDirect (pt1);
//			Assert.IsTrue (Epsitec.Common.Drawing.Transform.Equal (pt2, pt3));
//			
//			widget.SetClientZoom (3);
//			widget.SetClientAngle (90);
//			transform = widget.GetTransformToClient ();
//			
//			pt2 = widget.MapParentToClient (pt1);
//			pt3 = transform.TransformDirect (pt1);
//			Assert.IsTrue (Epsitec.Common.Drawing.Transform.Equal (pt2, pt3));
//			
//			widget.SetClientAngle (180);
//			transform = widget.GetTransformToClient ();
//			
//			pt2 = widget.MapParentToClient (pt1);
//			pt3 = transform.TransformDirect (pt1);
//			Assert.IsTrue (Epsitec.Common.Drawing.Transform.Equal (pt2, pt3));
//			
//			widget.SetClientAngle (270);
//			transform = widget.GetTransformToClient ();
//			
//			pt2 = widget.MapParentToClient (pt1);
//			pt3 = transform.TransformDirect (pt1);
//			Assert.IsTrue (Epsitec.Common.Drawing.Transform.Equal (pt2, pt3));
		}
		
		[Test] public void CheckTransformRectToClient()
		{
			Widget root = new Widget ();
			Widget widget = new Widget ();

			root.SetManualBounds(new Rectangle (0, 0, 300, 200));
			widget.SetManualBounds(new Rectangle (30, 20, 50, 40));
			
			root.Children.Add (widget);
			
//			double ox = 1.0;
//			double oy = 2.0;
//			double dx = 10.0;
//			double dy = 6.0;
//			
//			Rectangle rect1 = new Rectangle (widget.Left + ox, widget.Bottom + oy, dx, dy);
//			Rectangle rect2;
//			Point pt1 = new Point (rect1.Left,  rect1.Bottom);
//			Point pt2 = new Point (rect1.Right, rect1.Top);
//			Point pt3;
//			Point pt4;
//			
//			widget.SetClientAngle (0);
//			widget.SetClientZoom (2);
//			
//			rect2 = widget.MapParentToClient (rect1);
//			pt3   = widget.MapParentToClient (pt1);
//			pt4   = widget.MapParentToClient (pt2);
//			
//			System.Console.Out.WriteLine ("rect = " + rect2.ToString ());
//			System.Console.Out.WriteLine ("pts  = " + pt3.ToString () + " / " + pt4.ToString ());
//			
//			Assert.IsTrue (Transform.Equal (rect2.Left,   System.Math.Min (pt3.X, pt4.X)));
//			Assert.IsTrue (Transform.Equal (rect2.Bottom, System.Math.Min (pt3.Y, pt4.Y)));
//			Assert.IsTrue (Transform.Equal (rect2.Right,  System.Math.Max (pt3.X, pt4.X)));
//			Assert.IsTrue (Transform.Equal (rect2.Top,    System.Math.Max (pt3.Y, pt4.Y)));
//			
//			widget.SetClientZoom (3);
//			widget.SetClientAngle (90);
//			
//			rect2 = widget.MapParentToClient (rect1);
//			pt3   = widget.MapParentToClient (pt1);
//			pt4   = widget.MapParentToClient (pt2);
//			
//			System.Console.Out.WriteLine ("rect = " + rect2.ToString ());
//			System.Console.Out.WriteLine ("pts  = " + pt3.ToString () + " / " + pt4.ToString ());
//			
//			Assert.IsTrue (Transform.Equal (rect2.Left,   System.Math.Min (pt3.X, pt4.X)));
//			Assert.IsTrue (Transform.Equal (rect2.Bottom, System.Math.Min (pt3.Y, pt4.Y)));
//			Assert.IsTrue (Transform.Equal (rect2.Right,  System.Math.Max (pt3.X, pt4.X)));
//			Assert.IsTrue (Transform.Equal (rect2.Top,    System.Math.Max (pt3.Y, pt4.Y)));
//			
//			widget.SetClientZoom (7);
//			widget.SetClientAngle (180);
//			
//			rect2 = widget.MapParentToClient (rect1);
//			pt3   = widget.MapParentToClient (pt1);
//			pt4   = widget.MapParentToClient (pt2);
//			
//			System.Console.Out.WriteLine ("rect = " + rect2.ToString ());
//			System.Console.Out.WriteLine ("pts  = " + pt3.ToString () + " / " + pt4.ToString ());
//			
//			Assert.IsTrue (Transform.Equal (rect2.Left,   System.Math.Min (pt3.X, pt4.X)));
//			Assert.IsTrue (Transform.Equal (rect2.Bottom, System.Math.Min (pt3.Y, pt4.Y)));
//			Assert.IsTrue (Transform.Equal (rect2.Right,  System.Math.Max (pt3.X, pt4.X)));
//			Assert.IsTrue (Transform.Equal (rect2.Top,    System.Math.Max (pt3.Y, pt4.Y)));
//			
//			widget.SetClientZoom (1.5f);
//			widget.SetClientAngle (270);
//			
//			rect2 = widget.MapParentToClient (rect1);
//			pt3   = widget.MapParentToClient (pt1);
//			pt4   = widget.MapParentToClient (pt2);
//			
//			System.Console.Out.WriteLine ("rect = " + rect2.ToString ());
//			System.Console.Out.WriteLine ("pts  = " + pt3.ToString () + " / " + pt4.ToString ());
//			
//			Assert.IsTrue (Transform.Equal (rect2.Left,   System.Math.Min (pt3.X, pt4.X)));
//			Assert.IsTrue (Transform.Equal (rect2.Bottom, System.Math.Min (pt3.Y, pt4.Y)));
//			Assert.IsTrue (Transform.Equal (rect2.Right,  System.Math.Max (pt3.X, pt4.X)));
//			Assert.IsTrue (Transform.Equal (rect2.Top,    System.Math.Max (pt3.Y, pt4.Y)));
		}
		
		[Test] public void CheckTransformRectToParent()
		{
			Widget root = new Widget ();
			Widget widget = new Widget ();

			root.SetManualBounds(new Rectangle (0, 0, 300, 200));
			widget.SetManualBounds(new Rectangle (30, 20, 50, 40));
			
			root.Children.Add (widget);
			
//			double ox = 1.0;
//			double oy = 2.0;
//			double dx = 10.0;
//			double dy = 6.0;
//			
//			Rectangle rect1 = new Rectangle (widget.Left + ox, widget.Bottom + oy, dx, dy);
//			Rectangle rect2;
//			Point pt1 = new Point (rect1.Left,  rect1.Bottom);
//			Point pt2 = new Point (rect1.Right, rect1.Top);
//			Point pt3;
//			Point pt4;
//			
//			widget.SetClientAngle (0);
//			widget.SetClientZoom (2);
//			
//			rect2 = widget.MapClientToParent (rect1);
//			pt3   = widget.MapClientToParent (pt1);
//			pt4   = widget.MapClientToParent (pt2);
//			
//			System.Console.Out.WriteLine ("rect = " + rect2.ToString ());
//			System.Console.Out.WriteLine ("pts  = " + pt3.ToString () + " / " + pt4.ToString ());
//			
//			Assert.IsTrue (Transform.Equal (rect2.Left,   System.Math.Min (pt3.X, pt4.X)));
//			Assert.IsTrue (Transform.Equal (rect2.Bottom, System.Math.Min (pt3.Y, pt4.Y)));
//			Assert.IsTrue (Transform.Equal (rect2.Right,  System.Math.Max (pt3.X, pt4.X)));
//			Assert.IsTrue (Transform.Equal (rect2.Top,    System.Math.Max (pt3.Y, pt4.Y)));
//			
//			widget.SetClientZoom (3);
//			widget.SetClientAngle (90);
//			
//			rect2 = widget.MapClientToParent (rect1);
//			pt3   = widget.MapClientToParent (pt1);
//			pt4   = widget.MapClientToParent (pt2);
//			
//			System.Console.Out.WriteLine ("rect = " + rect2.ToString ());
//			System.Console.Out.WriteLine ("pts  = " + pt3.ToString () + " / " + pt4.ToString ());
//			
//			Assert.IsTrue (Transform.Equal (rect2.Left,   System.Math.Min (pt3.X, pt4.X)));
//			Assert.IsTrue (Transform.Equal (rect2.Bottom, System.Math.Min (pt3.Y, pt4.Y)));
//			Assert.IsTrue (Transform.Equal (rect2.Right,  System.Math.Max (pt3.X, pt4.X)));
//			Assert.IsTrue (Transform.Equal (rect2.Top,    System.Math.Max (pt3.Y, pt4.Y)));
//			
//			widget.SetClientZoom (7);
//			widget.SetClientAngle (180);
//			
//			rect2 = widget.MapClientToParent (rect1);
//			pt3   = widget.MapClientToParent (pt1);
//			pt4   = widget.MapClientToParent (pt2);
//			
//			System.Console.Out.WriteLine ("rect = " + rect2.ToString ());
//			System.Console.Out.WriteLine ("pts  = " + pt3.ToString () + " / " + pt4.ToString ());
//			
//			Assert.IsTrue (Transform.Equal (rect2.Left,   System.Math.Min (pt3.X, pt4.X)));
//			Assert.IsTrue (Transform.Equal (rect2.Bottom, System.Math.Min (pt3.Y, pt4.Y)));
//			Assert.IsTrue (Transform.Equal (rect2.Right,  System.Math.Max (pt3.X, pt4.X)));
//			Assert.IsTrue (Transform.Equal (rect2.Top,    System.Math.Max (pt3.Y, pt4.Y)));
//			
//			widget.SetClientZoom (1.5f);
//			widget.SetClientAngle (270);
//			
//			rect2 = widget.MapClientToParent (rect1);
//			pt3   = widget.MapClientToParent (pt1);
//			pt4   = widget.MapClientToParent (pt2);
//			
//			System.Console.Out.WriteLine ("rect = " + rect2.ToString ());
//			System.Console.Out.WriteLine ("pts  = " + pt3.ToString () + " / " + pt4.ToString ());
//			
//			Assert.IsTrue (Transform.Equal (rect2.Left,   System.Math.Min (pt3.X, pt4.X)));
//			Assert.IsTrue (Transform.Equal (rect2.Bottom, System.Math.Min (pt3.Y, pt4.Y)));
//			Assert.IsTrue (Transform.Equal (rect2.Right,  System.Math.Max (pt3.X, pt4.X)));
//			Assert.IsTrue (Transform.Equal (rect2.Top,    System.Math.Max (pt3.Y, pt4.Y)));
		}
		
		[Test] public void CheckTransformToParent()
		{
			Widget root = new Widget ();
			Widget widget = new Widget ();

			root.SetManualBounds(new Rectangle (0, 0, 300, 200));
			widget.SetManualBounds(new Rectangle (30, 20, 50, 40));
			
			root.Children.Add (widget);
			
			Epsitec.Common.Drawing.Transform transform = new Epsitec.Common.Drawing.Transform ();
			
//			double ox = 1.0;
//			double oy = 2.0;
//			
//			Point pt1 = new Point (ox, oy);
//			Point pt2;
//			Point pt3;
//			
//			widget.SetClientAngle (0);
//			transform = widget.GetTransformToParent ();
//			
//			pt2 = widget.MapClientToParent (pt1);
//			pt3 = transform.TransformDirect (pt1);
//			Assert.IsTrue (Epsitec.Common.Drawing.Transform.Equal (pt2, pt3));
//			
//			transform.Reset ();
//			widget.SetClientZoom (3);
//			widget.SetClientAngle (90);
//			transform = widget.GetTransformToParent ();
//			
//			pt2 = widget.MapClientToParent (pt1);
//			pt3 = transform.TransformDirect (pt1);
//			Assert.IsTrue (Epsitec.Common.Drawing.Transform.Equal (pt2, pt3));
//			
//			transform.Reset ();
//			widget.SetClientAngle (180);
//			transform = widget.GetTransformToParent ();
//			
//			pt2 = widget.MapClientToParent (pt1);
//			pt3 = transform.TransformDirect (pt1);
//			Assert.IsTrue (Epsitec.Common.Drawing.Transform.Equal (pt2, pt3));
//			
//			transform.Reset ();
//			widget.SetClientAngle (270);
//			transform = widget.GetTransformToParent ();
//			
//			pt2 = widget.MapClientToParent (pt1);
//			pt3 = transform.TransformDirect (pt1);
//			Assert.IsTrue (Epsitec.Common.Drawing.Transform.Equal (pt2, pt3));
		}
		
		[Test] public void CheckTransformParentClientIdentity()
		{
			Widget root = new Widget ();
			Widget widget = new Widget ();

			root.SetManualBounds(new Rectangle (0, 0, 300, 200));
			widget.SetManualBounds(new Rectangle (30, 20, 50, 40));
			
			root.Children.Add (widget);
			
			Epsitec.Common.Drawing.Transform identity  = new Epsitec.Common.Drawing.Transform ();
			Epsitec.Common.Drawing.Transform transform = new Epsitec.Common.Drawing.Transform ();
			
//			widget.SetClientZoom (3);
//			widget.SetClientAngle (90);
//			
//			transform = widget.GetTransformToClient ();
//			transform.MultiplyBy (widget.GetTransformToParent ());
//			
//			Assert.IsTrue (identity.Equals (transform));
//			
//			transform = widget.GetTransformToParent ();
//			transform.MultiplyBy (widget.GetTransformToClient ());
//			
//			Assert.IsTrue (identity.Equals (transform));
		}
		
		[Test] public void CheckTransformHierarchy()
		{
			Widget root = new Widget ();
			Widget widget = new Widget ();

			root.SetManualBounds(new Rectangle (100, 150, 300, 200));
			widget.SetManualBounds(new Rectangle (30, 20, 50, 40));
			widget.SetParent (root);
			
//			widget.SetClientZoom (3);
//			widget.SetClientAngle (90);
//			
//			Epsitec.Common.Drawing.Transform t1 = widget.GetRootToClientTransform ();
//			Epsitec.Common.Drawing.Transform t2 = widget.GetClientToRootTransform ();
//			
//			System.Console.Out.WriteLine ("root -> client : " + t1.ToString ());
//			System.Console.Out.WriteLine ("client -> root : " + t2.ToString ());
//			
//			Assert.IsTrue (Epsitec.Common.Drawing.Transform.Multiply (t1, t2).Equals (new Epsitec.Common.Drawing.Transform ()));
		}

		[Test]
		public void CheckTextFieldReal()
		{
			Window window = new Window ();

			window.ClientSize = new Size (200, 200);
			window.Text = "CheckTextFieldRead";
			
			window.Root.Padding = new Margins (8, 8, 5, 5);

			TextFieldReal real = new TextFieldReal ();

			real.PreferredWidth = 50;
			real.Value = 10;
			real.Anchor = AnchorStyles.TopLeft;
			real.Margins = new Margins (0, 0, 0, 0);
			
			window.Root.Children.Add (real);
			
			window.Show ();
			Window.RunInTestEnvironment (window);
		}

		[Test]
		public void CheckInteractiveAnchor()
		{
			Window window = new Window ();

			window.ClientSize = new Size (400, 300);
			window.Text = "CheckInteractiveAnchor";
			window.Root.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
			window.Root.Padding = new Margins (8, 8, 5, 5);

			Button button;
			button = new Button ();
			button.PreferredSize = new Size (40, 24);
			button.Text = "A";
			button.Margins = new Margins (0, 0, 0, 0);
			button.Anchor = AnchorStyles.Left | AnchorStyles.TopAndBottom;
			window.Root.Children.Add (button);

			button = new Button ();
			button.PreferredSize = new Size(40, 24);
			button.Text = "A1";
			button.Margins = new Margins (40, 0, 0, 0);
			button.Anchor = AnchorStyles.Left | AnchorStyles.TopAndBottom;
			button.VerticalAlignment = VerticalAlignment.Top;
			window.Root.Children.Add (button);

			button = new Button ();
			button.PreferredSize = new Size(40, 24);
			button.Text = "A2";
			button.Margins = new Margins (80, 0, 0, 0);
			button.Anchor = AnchorStyles.Left | AnchorStyles.TopAndBottom;
			button.VerticalAlignment = VerticalAlignment.Center;
			window.Root.Children.Add (button);

			button = new Button ();
			button.PreferredSize = new Size(40, 24);
			button.Text = "A3";
			button.Margins = new Margins (120, 0, 0, 0);
			button.Anchor = AnchorStyles.Left | AnchorStyles.TopAndBottom;
			button.VerticalAlignment = VerticalAlignment.Bottom;
			window.Root.Children.Add (button);

			button = new Button ();
			button.PreferredSize = new Size(40, 24);
			button.Text = "B";
			button.Margins = new Margins (160, 0, 0, 0);
			button.Anchor = AnchorStyles.Left | AnchorStyles.TopAndBottom;
			window.Root.Children.Add (button);

			button = new Button ();
			button.PreferredSize = new Size(40, 24);
			button.Text = "C";
			button.Margins = new Margins (0, 0, 0, 0);
			button.Anchor = AnchorStyles.Right | AnchorStyles.TopAndBottom;
			window.Root.Children.Add (button);

			button = new Button ();
			button.PreferredSize = new Size(40, 24);
			button.Text = "D";
			button.Margins = new Margins (200, 40, 0, 0);
			button.Anchor = AnchorStyles.Top | AnchorStyles.LeftAndRight;
			button.MinWidth = 30;
			window.Root.Children.Add (button);

			button = new Button ();
			button.PreferredSize = new Size(40, 24);
			button.Text = "E";
			button.Margins = new Margins (200, 40, 0, 0);
			button.Anchor = AnchorStyles.Bottom | AnchorStyles.LeftAndRight;
			window.Root.Children.Add (button);

			button = new Button ();
			button.PreferredSize = new Size(40, 24);
			button.Text = "E1";
			button.Margins = new Margins (200, 40, 0, 24);
			button.Anchor = AnchorStyles.Bottom | AnchorStyles.LeftAndRight;
			button.HorizontalAlignment = HorizontalAlignment.Left;
			window.Root.Children.Add (button);

			button = new Button ();
			button.PreferredSize = new Size(40, 24);
			button.Text = "E2";
			button.Margins = new Margins (200, 40, 0, 48);
			button.Anchor = AnchorStyles.Bottom | AnchorStyles.LeftAndRight;
			button.HorizontalAlignment = HorizontalAlignment.Center;
			window.Root.Children.Add (button);

			button = new Button ();
			button.PreferredSize = new Size(40, 24);
			button.Text = "E3";
			button.Margins = new Margins (200, 40, 0, 72);
			button.Anchor = AnchorStyles.Bottom | AnchorStyles.LeftAndRight;
			button.HorizontalAlignment = HorizontalAlignment.Right;
			window.Root.Children.Add (button);

			button = new Button ();
			button.PreferredSize = new Size(40, 24);
			button.Text = "F";
			button.Margins = new Margins (200, 40, 24, 96);
			button.Anchor = AnchorStyles.Left | AnchorStyles.TopAndBottom;
			button.MinHeight = 30;
			window.Root.Children.Add (button);

			button = new Button ();
			button.PreferredSize = new Size(40, 24);
			button.Text = "G";
			button.Margins = new Margins (240, 40, 24, 96);
			button.Anchor = AnchorStyles.Right | AnchorStyles.TopAndBottom;
			window.Root.Children.Add (button);

			button = new Button ();
			button.PreferredSize = new Size(40, 24);
			button.Text = "H";
			button.Margins = new Margins (240, 80, 24, 96);
			button.Anchor = AnchorStyles.Left | AnchorStyles.TopAndBottom;
			window.Root.Children.Add (button);

			button = new Button ();
			button.PreferredSize = new Size(40, 24);
			button.Text = "I";
			button.Margins = new Margins (240, 80, 24, 96);
			button.Anchor = AnchorStyles.Right | AnchorStyles.TopAndBottom;
			window.Root.Children.Add (button);

			window.Show ();
			Window.RunInTestEnvironment (window);
		}

		[Test]
		public void CheckInteractiveDock()
		{
			Window window = new Window ();

			window.ClientSize = new Size (400, 300);
			window.Text = "CheckInteractiveDock";
			window.Root.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
			window.Root.Padding = new Margins (8, 8, 5, 5);

			Button button;
			button = new Button ();
			button.PreferredSize = new Size(40, 24);
			button.Text = "A";
			button.Dock = DockStyle.Left;
			window.Root.Children.Add (button);

			button = new Button ();
			button.PreferredSize = new Size(40, 24);
			button.Text = "A1";
			button.Dock = DockStyle.Left;
			button.VerticalAlignment = VerticalAlignment.Top;
			window.Root.Children.Add (button);

			button = new Button ();
			button.PreferredSize = new Size(40, 24);
			button.Text = "A2";
			button.Dock = DockStyle.Left;
			button.VerticalAlignment = VerticalAlignment.Center;
			window.Root.Children.Add (button);

			button = new Button ();
			button.PreferredSize = new Size(40, 24);
			button.Text = "A3";
			button.Dock = DockStyle.Left;
			button.VerticalAlignment = VerticalAlignment.Bottom;
			window.Root.Children.Add (button);

			button = new Button ();
			button.PreferredSize = new Size(40, 24);
			button.Text = "B";
			button.Dock = DockStyle.Left;
			window.Root.Children.Add (button);

			button = new Button ();
			button.PreferredSize = new Size(40, 24);
			button.Text = "C";
			button.Dock = DockStyle.Right;
			window.Root.Children.Add (button);

			button = new Button ();
			button.PreferredSize = new Size(40, 24);
			button.Text = "D";
			button.Dock = DockStyle.Top;
			window.Root.Children.Add (button);

			button = new Button ();
			button.PreferredSize = new Size(40, 24);
			button.Text = "E";
			button.Dock = DockStyle.Bottom;
			window.Root.Children.Add (button);

			button = new Button ();
			button.PreferredSize = new Size(40, 24);
			button.Text = "E1";
			button.Dock = DockStyle.Bottom;
			button.HorizontalAlignment = HorizontalAlignment.Left;
			window.Root.Children.Add (button);

			button = new Button ();
			button.PreferredSize = new Size(40, 24);
			button.Text = "E2";
			button.Dock = DockStyle.Bottom;
			button.HorizontalAlignment = HorizontalAlignment.Center;
			window.Root.Children.Add (button);

			button = new Button ();
			button.PreferredSize = new Size(40, 24);
			button.Text = "E3";
			button.Dock = DockStyle.Bottom;
			button.HorizontalAlignment = HorizontalAlignment.Right;
			window.Root.Children.Add (button);

			button = new Button ();
			button.PreferredSize = new Size(40, 24);
			button.Text = "F";
			button.Dock = DockStyle.Left;
			window.Root.Children.Add (button);

			button = new Button ();
			button.PreferredSize = new Size(40, 24);
			button.Text = "G";
			button.Dock = DockStyle.Right;
			window.Root.Children.Add (button);

			button = new Button ();
			button.PreferredSize = new Size(40, 24);
			button.Text = "H";
			button.Dock = DockStyle.Fill;
			window.Root.Children.Add (button);

			button = new Button ();
			button.PreferredSize = new Size(40, 24);
			button.Text = "I";
			button.Dock = DockStyle.Fill;
			window.Root.Children.Add (button);

			window.Show ();
			Window.RunInTestEnvironment (window);
		}

		private class FlowPanel : Widget
		{
			public FlowPanel()
			{
			}

			public override Margins GetInternalPadding()
			{
				return new Drawing.Margins (1, 1, 1, 1);
			}
			
			protected override void MeasureMinMax(ref Size min, ref Size max)
			{
				Layouts.LayoutMeasure measureHeight = Layouts.LayoutMeasure.GetHeight (this);

				if (!measureHeight.SamePassIdAsLayoutContext (this))
				{
//					this.ResetColumnLineCount ();
				}
				
				base.MeasureMinMax (ref min, ref max);

				double width = 0;
				double height = 0;
				double dy = 0;
				int column = 0;

				foreach (Widget child in this.Children)
				{
					if (column >= this.columns)
					{
						min.Width = System.Math.Max (min.Width, width);
						column = 0;
						width = 0;
						height += dy;
						dy = 0;
					}

					width += child.PreferredWidth;
					dy = System.Math.Max (dy, child.PreferredHeight);
					column++;
				}
				
				height += dy;
				
				min.Height = System.Math.Max (min.Height, height + this.Padding.Height + this.GetInternalPadding ().Height);
			}

			protected override void ManualArrange()
			{
				base.ManualArrange ();

				Drawing.Rectangle rect = this.Client.Bounds;
				
				rect.Deflate (this.Padding);
				rect.Deflate (this.GetInternalPadding ());

				double x = 0;
				double y = 0;
				double dy = 0;
				
				int column = 0;

				foreach (Widget child in this.Children)
				{
					if (column >= this.columns)
					{
						column = 0;
						x = 0;
					}
					
					x += child.PreferredWidth;

					if ((x > rect.Width) &&
						(column > 0))
					{
						if (column < this.columns)
						{
							this.columns = column;
							this.lines = (this.Children.Count + this.columns - 1) / this.columns;
							
							Layouts.LayoutContext.AddToMeasureQueue (this);
							return;
						}
					}
					
					column++;
				}

				if (this.ActualWidth > this.lastWidth)
				{
					this.ResetColumnLineCount ();
					
					this.lastWidth = this.ActualWidth;
					
					Layouts.LayoutContext.AddToMeasureQueue (this);
					return;
				}

				x = 0;
				y = 0;
				dy = 0;

				column = 0;

				foreach (Widget child in this.Children)
				{
					if (column >= this.columns)
					{
						column = 0;
						x = 0;
						y += dy;
						dy = 0;
					}

					child.SetManualBounds(new Drawing.Rectangle (rect.Left + x, rect.Top - y - child.PreferredHeight, child.PreferredWidth, child.PreferredHeight));

					dy = System.Math.Max (dy, child.PreferredHeight);
					x += child.PreferredWidth;
					column++;
				}
				
				this.lastWidth = this.ActualWidth;
			}

			protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clip_rect)
			{
				graphics.AddRectangle (this.Client.Bounds);
				graphics.RenderSolid (Drawing.Color.FromName ("Black"));
			}
			
			protected override void OnChildrenChanged()
			{
				base.OnChildrenChanged ();
				this.ResetColumnLineCount ();
			}
			
			private void ResetColumnLineCount()
			{
				this.columns = this.Children.Count;
				this.lines = 1;
			}

			private double lastWidth;
			private int lines;
			private int columns;
		}

		[Test]
		public void CheckInteractiveFlow()
		{
			Window window = new Window ();

			window.ClientSize = new Size (400, 300);
			window.Text = "CheckInteractiveFlow";
			window.Root.Padding = new Margins (8, 8, 5, 5);

			Button button;
			FlowPanel panel;
			
			panel = new FlowPanel ();
			panel.Dock = DockStyle.Top;
			window.Root.Children.Add (panel);

			button = new Button ();
			button.PreferredSize = new Size (40, 40);
			button.Text = "A";
			panel.Children.Add (button);

			button = new Button ();
			button.PreferredSize = new Size (40, 40);
			button.Text = "B";
			panel.Children.Add (button);

			button = new Button ();
			button.PreferredSize = new Size (40, 40);
			button.Text = "C";
			panel.Children.Add (button);

			button = new Button ();
			button.PreferredSize = new Size (40, 40);
			button.Text = "D";
			panel.Children.Add (button);

			button = new Button ();
			button.PreferredSize = new Size (40, 40);
			button.Text = "E";
			panel.Children.Add (button);

			button = new Button ();
			button.PreferredHeight = 24;
			button.Text = "Below flow panel";
			button.Dock = DockStyle.Top;
			button.Margins = new Drawing.Margins (0, 0, 2, 0);
			window.Root.Children.Add (button);
			
			window.Show ();
			Window.RunInTestEnvironment (window);
		}
		[Test]
		public void CheckAliveWidgets()
		{
			System.Console.Out.WriteLine ("{0} widgets and {1} windows alive before GC.Collect", Widget.DebugAliveWidgetsCount, Window.DebugAliveWindowsCount);
			System.GC.Collect ();
			System.Console.Out.WriteLine ("{0} widgets and {1} windows alive after GC.Collect", Widget.DebugAliveWidgetsCount, Window.DebugAliveWindowsCount);
			foreach (Window window in Window.DebugAliveWindows)
			{
				System.Console.Out.WriteLine ("{0}: Name='{1}', Text='{2}'", window.GetType ().Name, window.Name, window.Text);
			}
			foreach (Widget widget in Widget.DebugAliveWidgets)
			{
				System.Console.Out.WriteLine ("{0}: Name='{1}', Text='{2}', Parent={3}", widget.GetType ().Name, widget.Name, widget.Text, (widget.Parent == null) ? "<null>" : (widget.Parent.GetType ().Name));
			}
		}
		
		[Test] public void CheckButtonNew()
		{
			Button button = new Button ();
		}
		
		[Test] public void CheckButtonNewDispose()
		{
			Button button = new Button ();
			button.Dispose ();
		}
		
		[Test] public void CheckButtonNewGC()
		{
			Button button = new Button ();
			button = null;
			System.GC.Collect ();
		}
		
		[Test] public void CheckFullPathName()
		{
			Widget w1 = new Widget (); w1.Name = "XA";
			Widget w2 = new Widget (); w2.Name = "XB"; w2.SetParent (w1);
			Widget w3 = new Widget (); w3.Name = "XC"; w3.SetParent (w2);
			
			Assert.AreEqual ("XA", w1.FullPathName);
			Assert.AreEqual ("XA.XB", w2.FullPathName);
			Assert.AreEqual ("XA.XB.XC", w3.FullPathName);
			
			Assert.AreEqual (w2, w1.FindChildByPath ("XA.XB"));
			Assert.AreEqual (w3, w1.FindChildByPath ("XA.XB.XC"));
			
			Widget[] find = Widget.FindAllFullPathWidgets (Support.RegexFactory.FromSimpleJoker ("*XB*"));
			
			Assert.AreEqual (2, find.Length);
			Assert.AreEqual (w2, find[0]);
			Assert.AreEqual (w3, find[1]);
		}
		
		[Test] public void CheckCommandName()
		{
			Widget w1 = new Widget (); w1.Name = "A"; w1.Command = "a";
			Widget w2 = new Widget (); w2.Name = "B"; w2.Command = "b ()";  w2.SetParent (w1);
			Widget w3 = new Widget (); w3.Name = "C"; w3.Command = "c (1)"; w3.SetParent (w2);
			
			Assert.AreEqual ("a",     w1.Command);
			Assert.AreEqual ("b ()",  w2.Command);
			Assert.AreEqual ("c (1)", w3.Command);
			
			Assert.AreEqual ("a", w1.CommandName);
			Assert.AreEqual ("b", w2.CommandName);
			Assert.AreEqual ("c", w3.CommandName);
			
			Assert.IsTrue (w1.IsCommand);
			Assert.IsTrue (w2.IsCommand);
			Assert.IsTrue (w3.IsCommand);
			
			Assert.AreEqual (w2, w1.FindChildByPath ("A.B"));
			Assert.AreEqual (w3, w1.FindChildByPath ("A.B.C"));
			
			Widget[] find = w1.FindCommandWidgets ("b");
			
			Assert.AreEqual (1, find.Length);
			Assert.AreEqual (w2, find[0]);
		}
		
		[Test] public void CheckCommandState()
		{
			WidgetTest.open_state.Enable = ! WidgetTest.open_state.Enable;
		}
		
		
		static CommandState open_state = CommandDispatcher.Default.GetCommandState ("open");
		
		[Test] public void CheckFindChildBasedOnName()
		{
			Widget root = new Widget ();
			Widget w1 = new Widget ();	w1.Name = "a";	w1.SetParent (root);
			Widget w2 = new Widget ();					w2.SetParent (root);
			Widget w3 = new Widget ();	w3.Name = "b";	w3.SetParent (w2);
			
			Assert.AreEqual (w1, root.FindChild ("a"));
			Assert.AreEqual (w3, root.FindChild ("b"));
		}
		
		[Test] public void CheckFindChildBasedOnCommandName()
		{
			Widget root = new Widget ();
			Widget w1 = new Widget ();	w1.Command = "a";	w1.SetParent (root);
			Widget w2 = new Widget ();						w2.SetParent (root);
			Widget w3 = new Widget ();	w3.Command = "b";	w3.SetParent (w2);
			Widget w4 = new Widget ();	w4.Command = "c";	w4.SetParent (w2);
			Widget w5 = new Widget ();	w5.Command = "d";	w5.SetParent (w4);
			Widget w6 = new Widget ();	w6.Name = "e";		w6.SetParent (w1);
			
			Assert.AreEqual (w1, root.FindCommandWidgets ("a") [0]);
			Assert.AreEqual (w3, root.FindCommandWidgets ("b") [0]);
			Assert.AreEqual (w4, root.FindCommandWidgets ("c") [0]);
			Assert.AreEqual (w5, root.FindCommandWidgets ("d") [0]);
			
			Assert.IsTrue (root.FindCommandWidgets ("e").Length == 0);
			
			Widget[] command_widgets = root.FindCommandWidgets ();
			
			Assert.AreEqual (4, command_widgets.Length);
		}
		
		[Test] public void CheckColorSelector()
		{
			Window window = new Window ();
			window.Text = "CheckColorSelector";
			window.ClientSize = new Drawing.Size (250, 250);
			
			ColorSelector selector = new ColorSelector ();
			
			selector.Dock = DockStyle.Fill;
			selector.SetParent (window.Root);
			
			Widget[] widgets = selector.Children.Widgets;
			
			foreach (Widget w in widgets)
			{
				System.Console.WriteLine ("Widget {0} :", w.ToString ());
				
				foreach (Types.LocalValueEntry entry in w.LocalValueEntries)
				{
					System.Console.WriteLine ("  {0} --> {1}", entry.Property.Name, entry.Value);
				}
				
				System.Console.WriteLine ();
			}
			
			
			window.Show ();
			Window.RunInTestEnvironment (window);
		}
		
		
		[Test] public void CheckSmartTagColor()
		{
			Window window = new Window ();
			window.Text = "CheckSmartTagColor";
			window.ClientSize = new Drawing.Size (510, 220);
			window.MakeFixedSizeWindow ();
			
			ColorSelector selector1, selector2;
			
			selector1 = new ColorSelector ();
			selector1.SetManualBounds(new Drawing.Rectangle (100, 10, 200, 200));
			selector1.SetParent (window.Root);
			selector1.Changed += new EventHandler (this.HandleSelectorChangedForeground);
			
			selector2 = new ColorSelector ();
			selector2.SetManualBounds(new Drawing.Rectangle (300, 10, 200, 200));
			selector2.SetParent (window.Root);
			selector2.Changed += new EventHandler (this.HandleSelectorChangedBackground);
			
			Tag tag;
			
			tag = new Tag ("", "tag1");
			tag.SetManualBounds(new Drawing.Rectangle (10, 10, 10, 10));
			tag.SetParent (window.Root);
			
			tag = new Tag ("", "tag2");
			tag.SetManualBounds(new Drawing.Rectangle (10, 25, 15, 15));
			tag.SetParent (window.Root);
			
			tag = new Tag ("", "tag3");
			tag.SetManualBounds(new Drawing.Rectangle (10, 45, 20, 20));
			tag.SetParent (window.Root);
			
			tag = new Tag ("", "tag4");
			tag.SetManualBounds(new Drawing.Rectangle (10, 70, 25, 25));
			tag.SetParent (window.Root);
			
			selector1.Color = new RichColor(tag.Color);
			selector2.Color = new RichColor(tag.BackColor);
			
			window.Show ();
			Window.RunInTestEnvironment (window);
		}


		private void CheckTextLayoutInfoPaintForeground(object sender, PaintEventArgs e)
		{
			Widget widget = sender as Widget;
			
			Drawing.Point pos;
			
			double ascender;
			double descender;
			double width;
			
			widget.TextLayout.GetLineGeometry (0, out pos, out ascender, out descender, out width);
			
			Drawing.Path path = new Drawing.Path ();
			
			path.MoveTo (pos.X, pos.Y);
			path.LineTo (pos.X + width, pos.Y);
			path.MoveTo (pos.X, pos.Y + ascender);
			path.LineTo (pos.X + width, pos.Y + ascender);
			path.MoveTo (pos.X, pos.Y + descender);
			path.LineTo (pos.X + width, pos.Y + descender);
			
			e.Graphics.Rasterizer.AddOutline (path, 0.2);
			e.Graphics.RenderSolid (Drawing.Color.FromRgb (1, 0, 0));
		}
		
		private void HandleSelectorChangedForeground(object sender)
		{
			ColorSelector selector = sender as ColorSelector;
			Drawing.Color color    = selector.Color.Basic;
			Widget        parent   = selector.Parent;
			
			Tag tag;
			
			tag = parent.FindChild ("tag1", Widget.ChildFindMode.Deep) as Tag;
			tag.Color = color;
			
			tag = parent.FindChild ("tag2", Widget.ChildFindMode.Deep) as Tag;
			tag.Color = color;
			
			tag = parent.FindChild ("tag3", Widget.ChildFindMode.Deep) as Tag;
			tag.Color = color;
			
			tag = parent.FindChild ("tag4", Widget.ChildFindMode.Deep) as Tag;
			tag.Color = color;
		}
		
		private void HandleSelectorChangedBackground(object sender)
		{
			ColorSelector selector = sender as ColorSelector;
			Drawing.Color color    = selector.Color.Basic;
			Widget        parent   = selector.Parent;
			
			Tag tag;
			
			tag = parent.FindChild ("tag1", Widget.ChildFindMode.Deep) as Tag;
			tag.BackColor = color;
			
			tag = parent.FindChild ("tag2", Widget.ChildFindMode.Deep) as Tag;
			tag.BackColor = color;
			
			tag = parent.FindChild ("tag3", Widget.ChildFindMode.Deep) as Tag;
			tag.BackColor = color;
			
			tag = parent.FindChild ("tag4", Widget.ChildFindMode.Deep) as Tag;
			tag.BackColor = color;
		}

		
		private void HandleAnchorChanged(object sender)
		{
			//	nothing
		}

		private void HandleManagedAnchorChanged(object sender, Types.DependencyPropertyChangedEventArgs e)
		{
			//	nothing
		}
	}
}
