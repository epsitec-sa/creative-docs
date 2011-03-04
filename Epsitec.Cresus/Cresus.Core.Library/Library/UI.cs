//	Copyright © 2008-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Dialogs;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Library
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

		public static void SetApplication(Application application)
		{
			UI.application = application;
		}

		/// <summary>
		/// Shuts down the user interface related infrastructure.
		/// </summary>
		public static void ShutDown()
		{
			Epsitec.Common.Drawing.ImageManager.ShutDownDefaultCache ();
		}

		
		public static class Settings
		{
			public static CultureSettings CultureForData
			{
				get
				{
					return UI.cultureSettingsForData;
				}
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

			if (hint.IsEmpty == false)
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
				var placement = hint.Placement;

				if (window.IsVisible == false)
				{
					placement = new WindowPlacement (placement.Bounds, placement.IsFullScreen, placement.IsMinimized, isHidden: true);
				}

				window.WindowPlacement = placement;
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
			window.TryGetLocalValue (UI.Properties.IsWindowPositionSaverActiveProperty, out defined);

			if (!defined)
			{
				window.SetLocalValue (UI.Properties.IsWindowPositionSaverActiveProperty, true);
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
			string fullMessage = string.Format (message.ToString (), UI.application.ShortWindowTitle, exMessage);
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
					string.Format (hint.ToString (), UI.application.ShortWindowTitle, exMessage),
					@"<br/>&#160;"));
			}

			MessageDialog.ShowError (formattedMessage, UI.application.ShortWindowTitle, null);
		}


		/// <summary>
		/// Sets the initial focus on the first (or last) widget which accepts
		/// tab navigation.
		/// </summary>
		/// <param name="container">The container.</param>
		/// <returns><c>true</c> if the focus was set; otherwise, <c>false</c>.</returns>
		public static bool SetInitialFocus(Visual container)
		{
			IEnumerable<Visual> children = container.Children;

			if (UI.reverseSetFocus)
			{
				//	We are called as a result to ExecuteWithReverseSetFocus, which means
				//	that we should start looking for the widgets in reverse order:

				children = children.Reverse ();
			}

			foreach (Visual widget in children)
			{
				if (widget is AbstractTextField)
				{
					var textField = widget as AbstractTextField;

					textField.SelectAll ();
					textField.Focus ();

					return true;
				}

				if (widget.HasChildren)
				{
					if (UI.SetInitialFocus (widget))
					{
						return true;
					}
				}
			}

			return false;
		}


		/// <summary>
		/// Sets up the context for <see cref="SetInitialFocus"/> and executes the action.
		/// </summary>
		/// <typeparam name="T">The return type of the action.</typeparam>
		/// <param name="action">The action.</param>
		/// <returns>The value returned by the action.</returns>
		public static T ExecuteWithDirectSetFocus<T>(System.Func<T> action)
		{
			return UI.ExecuteWithSpecifiedSetFocus (action, reverseSetFocus: false);
		}

		/// <summary>
		/// Sets up the context for <see cref="SetInitialFocus"/> and executes the action.
		/// </summary>
		/// <typeparam name="T">The return type of the action.</typeparam>
		/// <param name="action">The action.</param>
		/// <returns>The value returned by the action.</returns>
		public static T ExecuteWithReverseSetFocus<T>(System.Func<T> action)
		{
			return UI.ExecuteWithSpecifiedSetFocus (action, reverseSetFocus: true);
		}

		
		private static T ExecuteWithSpecifiedSetFocus<T>(System.Func<T> action, bool reverseSetFocus)
		{
			bool focus = UI.reverseSetFocus;
			
			UI.reverseSetFocus = reverseSetFocus;
			
			try
			{
				return action ();
			}
			finally
			{
				UI.reverseSetFocus = focus;
			}
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
			//?Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookRoyale");
			Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookBusiness");
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
			var windows = new List<Window> (Window.GetAllLiveWindows ());
			var hints   = UI.windowPlacementHints.ToArray ();

			foreach (var hint in hints)
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

				//	Changing the WindowPlacement will update the window placement hints, therefore
				//	we really need to iterate over a copy of UI.windowPlacementHints.

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

		private static void NotifyUpdateRequested(object sender)
		{
			var handler = UI.UpdateRequested;
			
			if (handler != null)
			{
				handler (sender);
			}
		}


		public static event EventHandler					UpdateRequested;
		
		private const string								StringMessageFontElement			= @"<font size=""125%"">";
		private const string								StringEndFontElement				= "</font>";
		
		private static readonly List<WindowPlacementHint>	windowPlacementHints				= new List<WindowPlacementHint> ();
		private static readonly CultureSettings				cultureSettingsForData				= new CultureSettings ();
		private static bool									reverseSetFocus;

		private static Application							application;
	}
}
