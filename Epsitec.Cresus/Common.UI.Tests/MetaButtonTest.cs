//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using NUnit.Framework;

using System.Collections.Generic;

namespace Epsitec.Common.UI
{
	[TestFixture]
	public class MetaButtonTest
	{
		[SetUp]
		public void Initialize()
		{
			Epsitec.Common.Document.Engine.Initialize ();
			Epsitec.Common.Widgets.Widget.Initialize ();
			Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookMetal");
		}
		
		[Test]
		public void AutomatedTestEnvironment()
		{
			Epsitec.Common.Widgets.Window.RunningInAutomatedTestEnvironment = true;
		}

		[Test]
		public void Check01Variations()
		{
			Widgets.Window window = new Widgets.Window ();

			window.Text = "MetaButtonTest.Check01Variations";
			window.ClientSize = new Drawing.Size (300, 600);

			FrameBox box = new FrameBox ()
			{
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				Margins = new Drawing.Margins (4, 4, 8, 8),
				Dock = DockStyle.Fill,
				Embedder = window.Root
			};

			System.Action<MetaButton>[] modes = new System.Action<MetaButton>[]
			{
				button => { button.MarkDisposition = ButtonMarkDisposition.None;  button.BulletColor = Drawing.Color.Empty; },
				button => { button.MarkDisposition = ButtonMarkDisposition.Left;  button.BulletColor = Drawing.Color.Empty; },
				button => { button.MarkDisposition = ButtonMarkDisposition.Below; button.BulletColor = Drawing.Color.Empty; button.PreferredHeight += button.MarkLength; },
				button => { button.MarkDisposition = ButtonMarkDisposition.None;  button.BulletColor = Drawing.Color.FromName ("Lime"); }
			};

			foreach (System.Action<MetaButton> setup in modes)
			{
				MetaButton b1 = new MetaButton ()
				{
					ButtonClass = ButtonClass.DialogButton,
					Dock = DockStyle.Stacked,
					Embedder = box,
					Text = "Text, DialogButton",
					Margins = new Drawing.Margins (0, 0, 0, 2)
				};

				MetaButton b2 = new MetaButton ()
				{
					ButtonClass = ButtonClass.DialogButton,
					Dock = DockStyle.Stacked,
					Embedder = box,
					Text = "Text+Icon, DialogButton",
					IconUri = "manifest:Epsitec.Common.Widgets.Images.TableEdition.icon",
					Margins = new Drawing.Margins (0, 0, 0, 2)
				};

				MetaButton b3 = new MetaButton ()
				{
					ButtonClass = ButtonClass.RichDialogButton,
					Dock = DockStyle.Stacked,
					Embedder = box,
					Text = "Text, RichDialogButton",
					Margins = new Drawing.Margins (0, 0, 0, 2),
					PreferredHeight = 28
				};

				MetaButton b4 = new MetaButton ()
				{
					ButtonClass = ButtonClass.RichDialogButton,
					Dock = DockStyle.Stacked,
					Embedder = box,
					Text = "Text+Icon, RichDialogButton",
					IconUri = "manifest:Epsitec.Common.Widgets.Images.TableEdition.icon",
					Margins = new Drawing.Margins (0, 0, 0, 2),
					PreferredHeight = 28
				};

				MetaButton b5 = new MetaButton ()
				{
					ButtonClass = ButtonClass.FlatButton,
					Dock = DockStyle.Stacked,
					Embedder = box,
					Text = "Text, FlatButton",
					Margins = new Drawing.Margins (0, 0, 0, 2)
				};

				MetaButton b6 = new MetaButton ()
				{
					ButtonClass = ButtonClass.FlatButton,
					Dock = DockStyle.Stacked,
					Embedder = box,
					Text = "Text+Icon, FlatButton",
					IconUri = "manifest:Epsitec.Common.Widgets.Images.TableEdition.icon",
					Margins = new Drawing.Margins (0, 0, 0, 2)
				};

				setup (b1);
				setup (b2);
				setup (b3);
				setup (b4);
				setup (b5);
				setup (b6);
				 
				b1.Clicked += (s,e) => { b1.ActiveState = b1.ActiveState == ActiveState.Yes ? ActiveState.No : ActiveState.Yes; };
				b2.Clicked += (s,e) => { b2.ActiveState = b2.ActiveState == ActiveState.Yes ? ActiveState.No : ActiveState.Yes; };
				b3.Clicked += (s,e) => { b3.ActiveState = b3.ActiveState == ActiveState.Yes ? ActiveState.No : ActiveState.Yes; };
				b4.Clicked += (s,e) => { b4.ActiveState = b4.ActiveState == ActiveState.Yes ? ActiveState.No : ActiveState.Yes; };
				b5.Clicked += (s,e) => { b5.ActiveState = b3.ActiveState == ActiveState.Yes ? ActiveState.No : ActiveState.Yes; };
				b6.Clicked += (s,e) => { b6.ActiveState = b4.ActiveState == ActiveState.Yes ? ActiveState.No : ActiveState.Yes; };
			}

			window.Show ();
			
			Widgets.Window.RunInTestEnvironment (window);
		}

		[Test]
		public void Check02CreateFromCommandId()
		{
			Widgets.Window window = new Widgets.Window ();

			window.Text = "MetaButtonTest.Check02CreateFromCommandId";
			window.ClientSize = new Drawing.Size (400, 300);

			FrameBox box = new FrameBox ()
			{
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				Margins = new Drawing.Margins (4, 4, 8, 8),
				Dock = DockStyle.Fill,
				Embedder = window.Root
			};

			MetaButton b1 = new MetaButton ()
			{
				ButtonClass = ButtonClass.DialogButton,
				Dock = DockStyle.Stacked,
				Embedder = box,
				CommandId = ApplicationCommands.Cut.Caption.Id,
				Margins = new Drawing.Margins (0, 0, 0, 2),
				PreferredHeight = 32
			};

			MetaButton b2 = new MetaButton ()
			{
				ButtonClass = ButtonClass.FlatButton,
				Dock = DockStyle.Stacked,
				Embedder = box,
				CommandId = ApplicationCommands.Cut.Caption.Id,
				Margins = new Drawing.Margins (0, 0, 0, 2),
				PreferredHeight = 32
			};

			MetaButton b3 = new MetaButton ()
			{
				ButtonClass = ButtonClass.RichDialogButton,
				Dock = DockStyle.Stacked,
				Embedder = box,
				CommandId = ApplicationCommands.Cut.Caption.Id,
				Margins = new Drawing.Margins (0, 0, 0, 2),
				PreferredHeight = 32
			};

			Separator sep = new Separator ()
			{
				Dock = DockStyle.Stacked,
				Embedder = box,
				PreferredHeight = 1,
				Margins = new Drawing.Margins (0, 0, 0, 2)
			};

			MetaButton b4 = new MetaButton ()
			{
				Dock = DockStyle.Stacked,
				Embedder = box,
				CommandId = ApplicationCommands.Cut.Caption.Id,
				Margins = new Drawing.Margins (0, 0, 0, 2),
				PreferredHeight = 32
			};
			

			window.Show ();

			Widgets.Window.RunInTestEnvironment (window);
		}
	}
}
