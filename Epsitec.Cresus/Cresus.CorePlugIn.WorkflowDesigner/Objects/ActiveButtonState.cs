//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.CorePlugIn.WorkflowDesigner.Objects
{
	public class ActiveButtonState
	{
		public ActiveButtonState()
		{
			this.Enable = true;
			this.Detectable = true;
		}


		public bool Selected
		{
			get;
			set;
		}

		public bool Hilited
		{
			get;
			set;
		}

		public bool Enable
		{
			get;
			set;
		}

		public bool Visible
		{
			get;
			set;
		}

		public bool Detectable
		{
			get;
			set;
		}
	}
}
