//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

using Epsitec.Cresus.Core.Orchestrators.Navigation;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Orchestrators
{
	/// <summary>
	/// The <c>ActiveEntityCancelEventArgs</c> class represents the event
	/// args for a cancellable event of <see cref="DataViewOrchestrator"/>.
	/// </summary>
	public class ActiveEntityCancelEventArgs : CancelEventArgs
	{
		public ActiveEntityCancelEventArgs()
		{
			this.entityKey = null;
			this.navigationPathElement = null;
		}

		public ActiveEntityCancelEventArgs(EntityKey? entityKey, NavigationPathElement navigationPathElement)
		{
			this.entityKey = entityKey;
			this.NavigationPathElement = navigationPathElement;
		}


		public EntityKey?						EntityKey
		{
			get
			{
				return this.entityKey;
			}
			set
			{
				this.entityKey = value;
			}
		}

		public NavigationPathElement			NavigationPathElement
		{
			get
			{
				return this.navigationPathElement;
			}
			set
			{
				this.navigationPathElement = value;
			}
		}

		
		private EntityKey?						entityKey;
		private NavigationPathElement			navigationPathElement;
	}
}
