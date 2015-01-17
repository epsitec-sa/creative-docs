//	Copyright � 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Library;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	public interface IItemPicker
	{
		//	Cette interface met en commun tout ce qu'il faut pour unifier ItemPicker et ItemPickerCombo.
		//	Comme ces 2 widgets ont des h�ritages diff�rents, il est n�cessaire de proc�der ainsi, en
		//	red�finissant de fa�on redondante les m�thodes communes de provenances diverses.

		void				IPRefreshContents();
		void				IPUpdateText();
		void				IPClearSelection();
		void				IPAddSelection(int selectedIndex);
		void				IPAddSelection(IEnumerable<int> selection);
		ICollection<int>	IPGetSortedSelection();
		event EventHandler	IPSelectedItemChanged;

		int IPSelectedItemIndex
		{
			get;
			set;
		}
	}
}
