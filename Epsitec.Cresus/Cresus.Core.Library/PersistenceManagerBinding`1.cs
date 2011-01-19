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
		where T : Widget
	{
		public PersistenceManagerBinding(T widget)
		{
			this.widget = new Weak<T> (widget);
		}

		private T								Widget
		{
			get
			{
				return this.widget.Target;
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
				T widget = this.Widget;
						
				if (widget != null)
				{
					value (widget);
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

		public override Widget GetWidget()
		{
			return this.Widget;
		}

		public override void ExecuteUnregister()
		{
			T widget = this.Widget;

			if (widget != null)
			{
				this.UnregisterChangeHandler (widget);
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
}
