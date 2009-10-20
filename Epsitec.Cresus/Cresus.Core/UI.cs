//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.UI;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core
{
	using FormResourceAccessor=Epsitec.Common.Support.ResourceAccessors.FormResourceAccessor;

	/// <summary>
	/// The <c>UI</c> static class provides central support for user interface
	/// related tasks.
	/// </summary>
	public static partial class UI
	{
		/// <summary>
		/// Initializes everything on application startup so that the user
		/// interface related frameworks are all properly configured.
		/// </summary>
		public static void Initialize()
		{
			//	Create a default resource manager pool, used for the UI and
			//	the application.
			
			ResourceManagerPool pool = new ResourceManagerPool ("Core");

			pool.DefaultPrefix = "file";
			pool.SetupDefaultRootPaths ();
			pool.ScanForAllModules ();

			ResourceManagerPool.Default = pool;

			//	Set up the fonts, the widgets, the icon rendering engine, etc.

			Epsitec.Common.Drawing.ImageManager.InitializeDefaultCache ();
			Epsitec.Common.Widgets.Widget.Initialize ();
			Epsitec.Common.Document.Engine.Initialize ();
			Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookRoyale");
			Epsitec.Common.Drawing.ImageManager.InitializeDefaultCache ();
		}

		public static void ShutDown()
		{
			Epsitec.Common.Drawing.ImageManager.ShutDownDefaultCache ();
		}


		public static Panel LoadPanel(Druid id, PanelInteractionMode mode)
		{
			ResourceManager manager = CoreProgram.Application.ResourceManager;
			ResourceBundle  bundle  = manager.GetBundle (id);
			
			switch (bundle.Type)
			{
				case Resources.PanelTypeName:
					return UI.CreateUserInterfaceFromPanel (bundle, mode);

				case Resources.FormTypeName:
					return UI.CreateUserInterfaceFromForm (bundle, mode);

				default:
					return null;
			}
		}

		/// <summary>
		/// Saves the window positions as an XML tree.
		/// </summary>
		/// <param name="xmlNodeName">Name of the XML root node.</param>
		/// <returns>The XML tree.</returns>
		public static XElement SaveWindowPositions(string xmlNodeName)
		{
			return new XElement (xmlNodeName,
				from window in Window.GetAllLiveWindows ()
				where window.Name.StartsWith ("$") == false
				select new XElement ("window",
					new XAttribute ("name", window.Name ?? ""),
					new XAttribute ("title", window.Text ?? ""),
					new XAttribute ("placement", window.WindowPlacement.ToString ())));
		}

		/// <summary>
		/// Restores the window positions from an XML tree.
		/// </summary>
		/// <param name="xml">The XML tree.</param>
		public static void RestoreWindowPositions(XElement xml)
		{
			UI.windowPlacementHints.Clear ();

			List<Window> windows = new List<Window> (Window.GetAllLiveWindows ());

			foreach (XElement element in xml.Elements ("window"))
			{
				string name      = (string) element.Attribute ("name");
				string title     = (string) element.Attribute ("title");
				string placement = (string) element.Attribute ("placement");

				WindowPlacement wp = WindowPlacement.Parse (placement);

				UI.windowPlacementHints.Add (new WindowPlacementHint (name, title, wp));

				Window window = UI.FindBestWindowMatch (windows, name, title);

				if (window == null)
				{
					//	Should never happen
				}
				else
				{
					if (window.IsVisible)
					{
						window.Hide ();
					}

					window.WindowPlacement = wp;
					windows.Remove (window);
				}
			}
		}


		public static void SaveWindowPosition(Window window)
		{
			var hint = UI.FindBestPlacement (window);

			if (!hint.IsEmpty)
            {
				UI.windowPlacementHints.Remove (hint);
            }

			UI.windowPlacementHints.Add (new WindowPlacementHint (window.Name, window.Text, window.WindowPlacement));
		}
		
		/// <summary>
		/// Restores the window position based on previous placement information.
		/// </summary>
		/// <param name="window">The window.</param>
		/// <returns><c>true</c> if the window placement was restored; otherwise, <c>false</c>.</returns>
		public static bool RestoreWindowPosition(Window window)
		{
			var hint = UI.FindBestPlacement (window);

			window.WindowFocused += sender => UI.SaveWindowPosition (window);
			window.WindowCloseClicked += sender => UI.SaveWindowPosition (window);

			if (hint.IsEmpty)
			{
				return false;
			}
			else
			{
				window.WindowPlacement = hint.Placement;
				return true;
			}
		}


		static UI()
		{
			UI.windowPlacementHints = new List<WindowPlacementHint> ();
		}

		
		#region WindowPlacementHint Class

		struct WindowPlacementHint : System.IEquatable<WindowPlacementHint>
		{
			public WindowPlacementHint(string name, string title, WindowPlacement placement)
			{
				this.name = name ?? "";
				this.title = title ?? "";
				this.placement = placement;
			}


			public string Name
			{
				get
				{
					return this.name;
				}
			}

			public string Title
			{
				get
				{
					return this.title;
				}
			}

			public WindowPlacement Placement
			{
				get
				{
					return this.placement;
				}
			}

			public bool IsEmpty
			{
				get
				{
					return this.name == null && this.title == null;
				}
			}

			public static readonly WindowPlacementHint Empty;

			#region IEquatable<WindowPlacementHint> Members

			public bool Equals(WindowPlacementHint other)
			{
				return (this.name == other.name) && (this.title == other.title) && (this.placement.Equals (other.placement));
			}

			#endregion

			public override bool Equals(object obj)
			{
				if (obj is WindowPlacementHint)
				{
					return this.Equals ((WindowPlacementHint) obj);
				}
				else
				{
					return false;
				}
			}

			private readonly string name;
			private readonly string title;
			private readonly WindowPlacement placement;
		}

		#endregion

		private static Window FindBestWindowMatch(IEnumerable<Window> windows, string name, string title)
		{
			//	First, try to find an exact match : name + title
			foreach (Window window in windows)
			{
				string windowName = window.Name ?? "";
				string windowTitle = window.Text ?? "";

				if ((windowName == name) &&
					(windowTitle == title))
				{
					return window;
				}
			}
			
			//	Second, try to find a match based only on the anme
			foreach (Window window in windows)
			{
				string windowName = window.Name ?? "";

				if (windowName == name)
				{
					return window;
				}
			}

			return null;
		}

		private static WindowPlacementHint FindBestPlacement(Window window)
		{
			string name = window.Name ?? "";
			string title = window.Text ?? "";

			//	First, try to find an exact match : name + title
			foreach (var hint in UI.windowPlacementHints)
			{
				if ((hint.Name == name) &&
					(hint.Title == title))
				{
					return hint;
				}
			}

			//	Second, try to find a match based only on the anme
			foreach (var hint in UI.windowPlacementHints)
			{
				if (hint.Name == name)
				{
					return hint;
				}
			}

			return WindowPlacementHint.Empty;
		}
		
		private static Panel CreateUserInterfaceFromForm(ResourceBundle bundle, PanelInteractionMode mode)
		{
			string xmlSource = bundle[FormResourceAccessor.Strings.XmlSource].AsString;
			Size size = FormResourceAccessor.GetFormDefaultSize (bundle);

			Epsitec.Common.FormEngine.FormDescription formDescription = new Epsitec.Common.FormEngine.FormDescription ();
			Epsitec.Common.FormEngine.IFormResourceProvider provider = new Epsitec.Common.FormEngine.DefaultResourceProvider (bundle.ResourceManager);
			Epsitec.Common.FormEngine.Engine formEngine = new Epsitec.Common.FormEngine.Engine (provider);

			switch (mode)
			{
				case PanelInteractionMode.Search:
					formEngine.EnableSearchMode ();
					break;
			}

			formDescription.Deserialize (xmlSource);

			Panel panel = formEngine.CreateForm (formDescription);

			//formDescription.EntityId;

			return panel;
		}

		private static Panel CreateUserInterfaceFromPanel(ResourceBundle bundle, PanelInteractionMode mode)
		{
			throw new System.NotImplementedException ();
		}

		public static void ShowErrorMessage(FormattedText message, FormattedText hint, System.Exception ex)
		{
			string fullMessage = string.Format (message.ToString (), CoreProgram.Application.ShortWindowTitle, ex.Message);

			if (hint.IsNullOrEmpty)
			{
				fullMessage = string.Concat (@"<font size=""125%"">", fullMessage, "</font>");
			}
			else
			{
				fullMessage = string.Concat (
					@"<font size=""125%"">",
					fullMessage,
					@"</font>",
					@"<br/><br/>",
					string.Format (hint.ToString (), CoreProgram.Application.ShortWindowTitle, ex.Message),
					@"<br/>&#160;");
			}
			
			MessageDialog.ShowError (fullMessage, CoreProgram.Application.ShortWindowTitle, null);
		}

		private static List<WindowPlacementHint> windowPlacementHints;
	}
}
