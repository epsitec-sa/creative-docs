//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Export
{
	public static class ExportHelpers<T>
		where T : struct
	{
		public static void StartExportProcess(Widget target, DataAccessor accessor, AbstractTreeTableFiller<T> dataFiller, ColumnsState columnsState)
		{
			//	Débute le processus d'exportation qui ouvrira plusieurs popups successifs.
			using (var engine = new ExportEngine<T> (target, accessor, dataFiller, columnsState))
			{
				engine.StartExportProcess ();
			}
		}
	}
}