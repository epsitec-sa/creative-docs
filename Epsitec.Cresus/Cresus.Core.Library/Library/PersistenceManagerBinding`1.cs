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
	/// The generic <c>PersistenceManagerBinding</c> class binds a widget with the
	/// <see cref="PersistenceManager"/>, in order for the persistence manager to
	/// be able to track changes and save/restore state in a uniform way.
	/// </summary>
	/// <typeparam name="T">The type of the widget.</typeparam>
	internal class PersistenceManagerBinding<T> : PersistenceManagerBinding
		where T : class
	{
		public PersistenceManagerBinding(T element, string id = null)
		{
			System.Diagnostics.Debug.Assert (element != null);

			Widget widget = element as Widget;

			if (widget != null)
			{
				System.Diagnostics.Debug.Assert (widget.Window != null, "Widget must have a valid Window");
				System.Diagnostics.Debug.Assert (!string.IsNullOrEmpty (widget.Window.Name), "Widget must have a valid Window, with a name");
				System.Diagnostics.Debug.Assert (!string.IsNullOrEmpty (widget.Name), "Widget must have a name");
			}

			this.item = new Weak<T> (element);
			this.id = id;
			this.Bind (element);
		}

		private T								Item
		{
			get
			{
				return this.item.Target;
			}
		}

		/// <summary>
		/// Sets the action used to register the change handler for the associated
		/// widget.
		/// </summary>
		/// <value>The action on the associated widget.</value>
		public System.Action<T> RegisterChangeHandler
		{
			set
			{
				T item = this.Item;
						
				if (item != null)
				{
					value (item);
				}
			}
		}

		/// <summary>
		/// Sets the action used to unregister the change handler for the associated
		/// widget.
		/// </summary>
		/// <value>The action on the associated widget.</value>
		public System.Action<T> UnregisterChangeHandler
		{
			private get;
			set;
		}

		/// <summary>
		/// Sets the action used to save the state of the associated widget to XML.
		/// </summary>
		/// <value>The action on the associated widget.</value>
		public System.Action<T, XElement> SaveXml
		{
			private get;
			set;
		}

		/// <summary>
		/// Sets the action used to restore the state of the associated widget from XML.
		/// </summary>
		/// <value>The action on the associated widget.</value>
		public System.Action<T, XElement> RestoreXml
		{
			private get;
			set;
		}

		public override string GetId()
		{
			if (string.IsNullOrEmpty (this.id))
			{
				Widget widget = this.Item as Widget;

				if (widget != null)
				{
					Window window = widget.Window;

					if ((window != null) &&
						(!string.IsNullOrEmpty (window.Name)))
					{
						return string.Concat (window.Name, ":", widget.FullPathName);
					}
					else
					{
						return widget.FullPathName;
					}
				}
			}

			return this.id;
		}

		public override void ExecuteUnregister()
		{
			T item = this.Item;

			if (item != null)
			{
				this.UnregisterChangeHandler (item);
			}
		}

		public override XElement ExecuteSave(XElement xml)
		{
			T item = this.Item;

			if (item != null)
			{
				this.SaveXml (item, xml);
			}
					
			return xml;
		}

		public override void ExecuteRestore(XElement xml)
		{
			T item = this.Item;

			if (item != null)
			{
				this.RestoreXml (item, xml);
			}
		}

		private readonly Weak<T> item;
		private readonly string id;
	}
}
