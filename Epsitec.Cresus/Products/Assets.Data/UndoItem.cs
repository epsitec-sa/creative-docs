//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data
{
	/// <summary>
	/// Cette classe contient toute l'information permettant d'annuler une action,
	/// à savoir la fonction à appeler pour annuler et les données correspondantes.
	/// </summary>
	public class UndoItem
	{
		public UndoItem(System.Func<IUndoData, UndoItem> undoOperation, IUndoData undoData, string description)
		{
			this.undoOperation = undoOperation;
			this.undoData      = undoData;
			this.description   = description;
		}

		//	La fonction permettant d'annuler est définie ainsi:
		//	
		//		public UndoItem UndoOperation(IUndoData data)
		//		{
		//		}
		//	
		//	Elle retourne toujours l'information permettant de l'annuler
		//	elle-même (opération inverse).

		public readonly System.Func<IUndoData, UndoItem>	undoOperation;
		public readonly IUndoData							undoData;
		public readonly string								description;
	}
}
