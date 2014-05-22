//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.DataFillers;

namespace Epsitec.Cresus.Assets.App.Export
{
	public abstract class AbstractExport<T>
		where T : struct
	{
		public virtual void Export(AbstractTreeTableFiller<T> filler, ExportInstructions instructions)
		{
		}
	}
}