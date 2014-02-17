//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	public interface ITreeFunctions
	{
		bool IsAllCompacted
		{
			get;
		}

		bool IsAllExpanded
		{
			get;
		}

		void CompactOrExpand(int index);
		void CompactAll();
		void ExpandAll();

		int SearchBestIndex(Guid value);

		int VisibleToAll(int index);
		int AllToVisible(int index);
	}
}
