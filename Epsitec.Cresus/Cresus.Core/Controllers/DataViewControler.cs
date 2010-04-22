//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Core.Controllers
{
	/// <summary>
	/// Ce contrôleur représente une bande verticale dans laquelle on empile des tuiles AbstractViewControler.
	/// </summary>
	public class DataViewControler : CoreController
	{
		public DataViewControler(string name)
			: base (name)
		{
			this.controlers = new List<CoreController> ();

		}

		public void SetEntity(List<AbstractEntity> entities)
		{
			this.entities = entities;
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			foreach (CoreController controler in this.controlers)
			{
				yield return controler;
			}
		}

		public override void CreateUI(Widget container)
		{
			System.Diagnostics.Debug.Assert (this.entities != null);

			int index = 0;
			foreach (Common.Support.EntityEngine.AbstractEntity entity in this.entities)
			{
				string name = string.Concat (this.Name, ".DataViewControler", index.ToString(System.Globalization.CultureInfo.InvariantCulture));
				AbstractViewControler viewControler = AbstractViewControler.CreateViewControler (name, entity, ViewControlerMode.Compact);

				if (viewControler != null)
				{
					FrameBox frame = new FrameBox
					{
						Parent = container,
						Margins = new Margins (0, 0, 0, (index<entities.Count-1) ? -1:0),
						Dock = DockStyle.Top,
					};

					viewControler.CreateUI (frame);
					this.controlers.Add (viewControler);

					index++;
				}
			}
		}


		private List<CoreController> controlers;
		private List<AbstractEntity> entities;
	}
}
