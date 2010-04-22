//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Core.Controllers
{
	public class ViewLayoutControler : CoreController
	{
		public ViewLayoutControler(string name)
			: base (name)
		{
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}

		public override void CreateUI(Widget container)
		{
			this.frame = new FrameBox
			{
				Parent = container,
				Dock = DockStyle.Fill,
			};
		}

		public int ColumnsCount
		{
			get;
			set;
		}

		public int GetColumWidth(int index)
		{
			return 200;
		}


		private FrameBox frame;
	}
}
