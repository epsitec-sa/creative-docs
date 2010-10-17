//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.WorkflowDesigner.Objects
{
	public class ActiveButtonState
	{
		public ActiveButtonState()
		{
			this.enable = true;
		}


		public bool Selected
		{
			get
			{
				return this.selected;
			}
			set
			{
				this.selected = value;
			}
		}

		public bool Hilited
		{
			get
			{
				return this.hilited;
			}
			set
			{
				this.hilited = value;
			}
		}

		public bool Enable
		{
			get
			{
				return this.enable;
			}
			set
			{
				this.enable = value;
			}
		}

		public bool Visible
		{
			get
			{
				return this.visible;
			}
			set
			{
				this.visible = value;
			}
		}


		private bool			selected;
		private bool			hilited;
		private bool			enable;
		private bool			visible;
	}
}
