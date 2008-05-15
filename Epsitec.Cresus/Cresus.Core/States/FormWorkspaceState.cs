//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.States;
using Epsitec.Cresus.Core.Workspaces;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.States
{
	/// <summary>
	/// The <c>FormWorkspaceState</c> class manages the state associated with a
	/// form workspace, as implemented by the <see cref="FormWorkspace"/> class.
	/// </summary>
	public abstract class FormWorkspaceState : AbstractState
	{
		public FormWorkspaceState()
		{
			
		}


		public Workspaces.FormWorkspace Workspace
		{
			get
			{
				return this.workspace;
			}
			set
			{
				if (this.workspace != value)
				{
					this.workspace = value;
					
					if ((this.serialization != null) &&
						(this.workspace != null))
					{
						XElement data = this.serialization;

						this.serialization = null;
						this.RestoreWorkspace (data);
					}
				}
			}
		}

		
		public override XElement Serialize(XElement element)
		{
			return element;
		}

		public override AbstractState Deserialize(XElement element)
		{
			if (this.workspace != null)
			{
				this.RestoreWorkspace (element);
			}
			else
			{
				this.serialization = element;
			}

			return this;
		}


		protected virtual void StoreWorkspace(XElement element)
		{
		}
		
		protected virtual void RestoreWorkspace(XElement element)
		{
		}


		private Workspaces.FormWorkspace		workspace;
		private XElement						serialization;
	}
}
