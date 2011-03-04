//	Copyright © 2008-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Library
{
	/// <summary>
	/// The <c>PersistenceManager</c> class handles UI persistence.
	/// </summary>
	public sealed partial class PersistenceManager : CoreAppComponent, System.IDisposable
	{
		public PersistenceManager(CoreApp app = null)
			: base (app)
		{
			this.bindings = new Dictionary<string, PersistenceManagerBinding> ();
			
			this.timer = new Timer ()
			{
				Delay = 2.0,
			};
			
			this.timer.TimeElapsed += this.HandleTimerTimeElapsed;
		}


		/// <summary>
		/// Saves the UI settings in an XML tree with the specified root
		/// node name.
		/// </summary>
		/// <param name="xmlNodeName">Name of the root node.</param>
		/// <returns>The XML tree.</returns>
		public XElement Save(string xmlNodeName)
		{
			this.DiscardChanges ();

			//	<node>
			//	  <w id="path1" ... />
			//	  <w id="path2" ... />
			//	</node>

			return new XElement (xmlNodeName,
				from path in this.bindings.Keys
				orderby path ascending
				let binding = this.bindings[path]
				select binding.ExecuteSave (new XElement ("w", new XAttribute ("id", path))));
		}

		/// <summary>
		/// Restores the UI settings based on the XML tree.
		/// </summary>
		/// <param name="xml">The XML tree.</param>
		public void Restore(XElement xml)
		{
			if (xml == null)
			{
				return;
			}

			foreach (XElement node in xml.Elements ())
			{
				System.Diagnostics.Debug.Assert (node.Name == "w");

				string path = (string) node.Attribute ("id");
				PersistenceManagerBinding binding;

				if (this.bindings.TryGetValue (path, out binding))
				{
					binding.ExecuteRestore (node);
				}
			}
		}

		/// <summary>
		/// Discards all the changes which happened since the last save.
		/// </summary>
		public void DiscardChanges()
		{
			this.timer.Stop ();
			this.changeCount = 0;
		}


		/// <summary>
		/// Registers the specified widget with the persistence manager.
		/// </summary>
		/// <param name="ribbonBook">The widget.</param>
		public void Register(RibbonBook ribbonBook)
		{
			this.AddBinding (
				new PersistenceManagerBinding<RibbonBook> (ribbonBook)
				{
					RegisterChangeHandler   = w => w.ActivePageChanged += this.NotifyChange,
					UnregisterChangeHandler = w => w.ActivePageChanged -= this.NotifyChange,
					SaveXml    = (w, xml) => xml.Add (new XAttribute ("book", InvariantConverter.ToString (w.ActivePageIndex))),
					RestoreXml = (w, xml) => w.ActivePageIndex = InvariantConverter.ToInt (xml.Attribute ("book").Value),
				});
		}

		public void Register<T>(T element, string id,
			System.Action<EventHandler> registerCallback,
			System.Action<EventHandler> unregisterCallback,
			System.Action<XElement> saveXmlCallback,
			System.Action<XElement> restoreXmlCallback)
			where T : class
		{
			this.AddBinding (
				new PersistenceManagerBinding<T> (element, id)
				{
					RegisterChangeHandler   = x => registerCallback (this.NotifyChange),
					UnregisterChangeHandler = x => unregisterCallback (this.NotifyChange),
					SaveXml    = (x, xml) => saveXmlCallback (xml),
					RestoreXml = (x, xml) => restoreXmlCallback (xml),
				});
		}

		/// <summary>
		/// Registers the specified window with the persistence manager. If the window
		/// gets resized, the persistence manager will generate <see cref="SettingsChanged"/>
		/// events.
		/// </summary>
		/// <param name="window">The window.</param>
		public void Register(Window window)
		{
			window.WindowPlacementChanged += this.NotifyChange;
		}

		/// <summary>
		/// Unregisters the specified window from the persistence manager.
		/// </summary>
		/// <param name="window">The window.</param>
		public void Unregister(Window window)
		{
			window.WindowPlacementChanged -= this.NotifyChange;
		}




		#region IDisposable Members

		public void Dispose()
		{
			this.timer.Dispose ();

			foreach (PersistenceManagerBinding binding in this.bindings.Values)
			{
				binding.ExecuteUnregister ();
			}

			this.bindings.Clear ();
		}

		#endregion

			
		private void AddBinding(PersistenceManagerBinding binding)
		{
			string path = binding.GetId ();
			PersistenceManagerBinding oldBinding;

			if (this.bindings.TryGetValue (path, out oldBinding))
			{
				oldBinding.ExecuteUnregister ();
			}
				
			this.bindings[path] = binding;
		}

		private void NotifyChange(object sender)
		{
			if (this.changeCount == 0)
			{
				this.timer.Start ();
			}

			this.changeCount++;
		}



		private void HandleTimerTimeElapsed(object sender)
		{
			if (this.SettingsChanged != null)
			{
				this.SettingsChanged (this);
			}
		}

		#region Factory Class

		private sealed class Factory : Epsitec.Cresus.Core.Factories.DefaultCoreAppComponentFactory<PersistenceManager>
		{
		}

		#endregion
			

		public event EventHandler SettingsChanged;

		private readonly Dictionary<string, PersistenceManagerBinding> bindings;
		private readonly Timer timer;
		private int changeCount;
	}
}
