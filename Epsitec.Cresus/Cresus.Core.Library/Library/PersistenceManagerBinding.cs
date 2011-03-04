//	Copyright © 2008-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

[assembly:DependencyClass (typeof (PersistenceManagerBinding))]

namespace Epsitec.Cresus.Core.Library
{
	/// <summary>
	/// The <c>PersistenceManagerBinding</c> class is used as a common base class for
	/// the generic <c>PersistenceManagerBinding&lt;T&gt;</c> class.
	/// </summary>
	internal abstract class PersistenceManagerBinding : DependencyObject
	{
		protected PersistenceManagerBinding()
		{
		}

		protected void Bind(object source)
		{
			DependencyObject obj = source as DependencyObject;

			if (obj != null)
			{
				PersistenceManagerBinding.SetValue (obj, this);
			}
		}

		public abstract string GetId();
		public abstract void ExecuteUnregister();
		public abstract XElement ExecuteSave(XElement xml);
		public abstract void ExecuteRestore(XElement xml);


		public static void SetValue(DependencyObject obj, PersistenceManagerBinding value)
		{
			obj.SetValue (PersistenceManagerBinding.PersistenceBindingProperty, value);
		}

		public static PersistenceManagerBinding GetValue(DependencyObject obj)
		{
			return obj.GetValue (PersistenceManagerBinding.PersistenceBindingProperty) as PersistenceManagerBinding;
		}

		
		public static DependencyProperty PersistenceBindingProperty = DependencyProperty<PersistenceManagerBinding>.RegisterAttached ("PersistenceBinding", typeof (PersistenceManagerBinding));
	}
}
