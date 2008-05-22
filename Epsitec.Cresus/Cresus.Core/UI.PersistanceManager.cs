//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;
using Epsitec.Common.UI;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core
{
	public static partial class UI
	{
		/// <summary>
		/// The <c>PersistanceManager</c> class handles UI persistance.
		/// </summary>
		public class PersistanceManager
		{
			public PersistanceManager()
			{
				this.bindings = new Dictionary<string, Binding> ();
			}


			public XElement Save(string xmlNodeName)
			{
				return new XElement (xmlNodeName,
					from path in this.bindings.Keys
					orderby path ascending
					let binding = this.bindings[path]
					select binding.ExecuteSave (new XElement ("w", new XAttribute ("id", path))));
			}

			public void Restore(XElement xml)
			{
				if (xml == null)
				{
					return;
				}

				foreach (XElement node in xml.Descendants ())
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
			
			
			public void Register(RibbonBook widget)
			{
				this.AddBinding (
					new Binding<RibbonBook> (widget)
					{
						Register    = w => w.ActivePageChanged += this.HandleRibbonActivePageChanged,
						Unregister  = w => w.ActivePageChanged -= this.HandleRibbonActivePageChanged,
						Save = this.HandleRibbonSave,
						Restore = this.HandleRibbonRestore
					});
			}





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

			private void NotifyChange(object sender)
			{
				this.changeCount++;
			}



			
			private void HandleRibbonActivePageChanged(object sender)
			{
				this.NotifyChange (sender);
			}

			private void HandleRibbonSave(RibbonBook widget, XElement xml)
			{
				xml.Add (new XAttribute ("book", InvariantConverter.ToString (widget.ActivePageIndex)));
			}

			private void HandleRibbonRestore(RibbonBook widget, XElement xml)
			{
				widget.ActivePageIndex = InvariantConverter.ToInt (xml.Attribute ("book").Value);
			}


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

				public T Widget
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
						value (this.Widget);
					}
				}
				public System.Action<T> Unregister;
				public System.Action<T, XElement> Save;
				public System.Action<T, XElement> Restore;

				public override Widget GetWidget()
				{
					return this.Widget;
				}

				public override void ExecuteUnregister()
				{
					this.Unregister (this.Widget);
				}

				public override XElement ExecuteSave(XElement xml)
				{
					this.Save (this.Widget, xml);
					return xml;
				}

				public override void ExecuteRestore(XElement xml)
				{
					this.Restore (this.Widget, xml);
				}

				private readonly Weak<T> widget;
			}

			private readonly Dictionary<string, Binding> bindings;
			private int changeCount;
		}
	}
}
