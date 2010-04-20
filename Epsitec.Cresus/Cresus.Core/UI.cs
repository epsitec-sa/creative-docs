//	Copyright © 2008-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
		/// interface related infrastructure is properly configured.
		/// </summary>
		public static void Initialize()
		{
			UI.SetupResourceManagerPool ();
			UI.SetupWidgetInfrastructure ();
		}

		/// <summary>
		/// Shuts down the user interface related infrastructure.
		/// </summary>
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

			foreach (XElement element in xml.Elements ("window"))
			{
				string name      = (string) element.Attribute ("name");
				string title     = (string) element.Attribute ("title");
				string placement = (string) element.Attribute ("placement");

				WindowPlacement wp = WindowPlacement.Parse (placement);

				UI.windowPlacementHints.Add (new WindowPlacementHint (name, title, wp));
			}

			UI.RestoreWindowPositionsOfExistingWindows ();
		}

		/// <summary>
		/// Saves the window position for future calls to <see cref="RestoreWindowPosition"/>.
		/// </summary>
		/// <param name="window">The window.</param>
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

		/// <summary>
		/// Sets up the window position saver, which will invoke <see cref="SaveWindowPosition"/>
		/// every time the window gets the focus or before it is being closed.
		/// </summary>
		/// <param name="window">The window.</param>
		public static void RegisterWindowPositionSaver(Window window)
		{
			//	Do this only once for a given window; the attached property is used to
			//	find out if we already registered the position saver, or not.
			
			bool defined;
			window.TryGetLocalValue (UI.IsWindowPositionSaverActiveProperty, out defined);

			if (!defined)
			{
				window.SetLocalValue (UI.IsWindowPositionSaverActiveProperty, true);
				window.WindowFocused      += sender => UI.SaveWindowPosition (window);
				window.WindowCloseClicked += sender => UI.SaveWindowPosition (window);
			}
		}

		/// <summary>
		/// Shows the error message.
		/// The message and the hint can contain <code>{0}</code> and <code>{1}</code>
		/// placeholders which will be filled in with the short application name and the
		/// exception message (if any).
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="hint">The hint.</param>
		/// <param name="ex">The exception.</param>
		public static void ShowErrorMessage(FormattedText message, FormattedText hint, System.Exception ex)
		{
			string exMessage   = ex == null ? "" : ex.Message;
			string fullMessage = string.Format (message.ToString (), CoreProgram.Application.ShortWindowTitle, exMessage);
			FormattedText formattedMessage;

			if (hint.IsNullOrEmpty)
			{
				formattedMessage = new FormattedText (string.Concat (UI.StringMessageFontElement, fullMessage, UI.StringEndFontElement));
			}
			else
			{
				formattedMessage = new FormattedText (string.Concat (
					UI.StringMessageFontElement,
					fullMessage,
					UI.StringEndFontElement,
					@"<br/><br/>",
					string.Format (hint.ToString (), CoreProgram.Application.ShortWindowTitle, exMessage),
					@"<br/>&#160;"));
			}

			MessageDialog.ShowError (formattedMessage, CoreProgram.Application.ShortWindowTitle, null);
		}


		private static void SetupResourceManagerPool()
		{
			//	Create a default resource manager pool, used for the UI and
			//	the application.

			var pool = new ResourceManagerPool ("Core")
			{
				DefaultPrefix = "file"
			};

			pool.SetupDefaultRootPaths ();
			pool.ScanForAllModules ();

			ResourceManagerPool.Default = pool;
		}
		
		private static void SetupWidgetInfrastructure()
		{
			//	Set up the fonts, the widgets, the icon rendering engine, etc.

			Epsitec.Common.Drawing.ImageManager.InitializeDefaultCache ();
			Epsitec.Common.Widgets.Widget.Initialize ();
			Epsitec.Common.Document.Engine.Initialize ();
			Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookRoyale");
			Epsitec.Common.Drawing.ImageManager.InitializeDefaultCache ();
		}
        
		
		
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

		private static void RestoreWindowPositionsOfExistingWindows()
		{
			List<Window> windows = new List<Window> (Window.GetAllLiveWindows ());

			foreach (var hint in UI.windowPlacementHints)
			{
				Window window = UI.FindBestWindowMatch (windows, hint.Name, hint.Title);

				if (window == null)
				{
					continue;
				}

				UI.RegisterWindowPositionSaver (window);

				if (window.IsVisible)
				{
					window.Hide ();
				}

				window.WindowPlacement = hint.Placement;
				windows.Remove (window);
			}
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

			//	Second, try to find a match based only on the name
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

		
		private const string								StringMessageFontElement			= @"<font size=""125%"">";
		private const string								StringEndFontElement				= "</font>";
		
		private static readonly DependencyProperty			IsWindowPositionSaverActiveProperty	= DependencyProperty.RegisterAttached ("isWindowPositionSaverActive", typeof (bool), typeof (UI));
		
		private static readonly List<WindowPlacementHint>	windowPlacementHints				= new List<WindowPlacementHint> ();
	}
}
