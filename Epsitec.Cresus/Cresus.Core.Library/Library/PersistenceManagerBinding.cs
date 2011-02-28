//	Copyright © 2008-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Library
{
	/// <summary>
	/// The <c>PersistenceManagerBinding</c> class is used as a common base class for
	/// the generic <c>PersistenceManagerBinding&lt;T&gt;</c> class.
	/// </summary>
	internal abstract class PersistenceManagerBinding
	{
		protected PersistenceManagerBinding()
		{
		}

		public abstract string GetId();
		public abstract void ExecuteUnregister();
		public abstract XElement ExecuteSave(XElement xml);
		public abstract void ExecuteRestore(XElement xml);
	}
}
