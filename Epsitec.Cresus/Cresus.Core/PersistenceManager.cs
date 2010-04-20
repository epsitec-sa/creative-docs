//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core
{
	/// <summary>
	/// The <c>PersistenceManager</c> class handles UI persistence.
	/// </summary>
	public sealed class PersistenceManager : System.IDisposable
	{
		public PersistenceManager()
		{
			this.bindings = new Dictionary<string, Binding> ();
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
				Binding binding;

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
		/// Registers the specified widget.
		/// </summary>
		/// <param name="widget">The widget.</param>
		public void Register(RibbonBook widget)
		{
			this.AddBinding (
				new Binding<RibbonBook> (widget)
				{
					Register   = w => w.ActivePageChanged += this.HandleRibbonActivePageChanged,
					Unregister = w => w.ActivePageChanged -= this.HandleRibbonActivePageChanged,
					SaveXml    = (w, xml) => xml.Add (new XAttribute ("book", InvariantConverter.ToString (w.ActivePageIndex))),
					RestoreXml = (w, xml) => w.ActivePageIndex = InvariantConverter.ToInt (xml.Attribute ("book").Value),
				});
		}





		#region IDisposable Members

		public void Dispose()
		{
			this.timer.Dispose ();

			foreach (Binding binding in this.bindings.Values)
			{
				binding.ExecuteUnregister ();
			}

			this.bindings.Clear ();
		}

		#endregion

			
		private void AddBinding(Binding binding)
		{
			string path = binding.GetWidget ().FullPathName;
			Binding oldBinding;

			if (this.bindings.TryGetValue (path, out oldBinding))
			{
				oldBinding.ExecuteUnregister ();
			}
				
			this.bindings[path] = binding;
		}

		private void NotifyChange()
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
			
		private void HandleRibbonActivePageChanged(object sender)
		{
			this.NotifyChange ();
		}


		#region Binding Classes

		/// <summary>
		/// The <c>Binding</c> class is used as a common base class for
		/// the generic <c>Binding&lt;T&gt;</c> class.
		/// </summary>
		abstract class Binding
		{
			protected Binding()
			{
			}

			public abstract Widget GetWidget();
			public abstract void ExecuteUnregister();
			public abstract XElement ExecuteSave(XElement xml);
			public abstract void ExecuteRestore(XElement xml);
		}

		class Binding<T> : Binding where T : Widget
		{
			public Binding(T widget)
			{
				this.widget = new Weak<T> (widget);
			}

			private T Widget
			{
				get
				{
					return this.widget.Target;
				}
			}

			public System.Action<T> Register
			{
				set
				{
					T widget = this.Widget;
						
					if (widget != null)
					{
						value (widget);
					}
				}
			}
			public System.Action<T> Unregister;
			public System.Action<T, XElement> SaveXml;
			public System.Action<T, XElement> RestoreXml;

			public override Widget GetWidget()
			{
				return this.Widget;
			}

			public override void ExecuteUnregister()
			{
				T widget = this.Widget;

				if (widget != null)
				{
					this.Unregister (widget);
				}
			}

			public override XElement ExecuteSave(XElement xml)
			{
				T widget = this.Widget;

				if (widget != null)
				{
					this.SaveXml (widget, xml);
				}
					
				return xml;
			}

			public override void ExecuteRestore(XElement xml)
			{
				T widget = this.Widget;

				if (widget != null)
				{
					this.RestoreXml (widget, xml);
				}
			}

			private readonly Weak<T> widget;
		}

		#endregion


		public event EventHandler SettingsChanged;

		private readonly Dictionary<string, Binding> bindings;
		private readonly Timer timer;
		private int changeCount;
	}
}
