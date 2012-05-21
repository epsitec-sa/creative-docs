//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Search.Data;
using Epsitec.Cresus.Compta.Options.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.ViewSettings.Data
{
	/// <summary>
	/// Liste des paramètres des recherches, du filtre et des options, c'est-à-dire de l'ensemble des paramètres
	/// liés à une présentation (traduit en français par "réglage de présentation").
	/// </summary>
	public class ViewSettingsList : ISettingsData
	{
		public ViewSettingsList()
		{
			this.list = new List<ViewSettingsData> ();
			this.selectedIndex = -1;
		}


		public List<ViewSettingsData> List
		{
			get
			{
				return this.list;
			}
		}


		public int SelectedIndex
		{
			get
			{
				return this.selectedIndex;
			}
			set
			{
				this.selectedIndex = value;
			}
		}

		public ViewSettingsData Selected
		{
			get
			{
				if (this.selectedIndex < 0 || this.selectedIndex >= this.list.Count)
				{
					return null;
				}
				else
				{
					return this.list[this.selectedIndex];
				}
			}
			set
			{
				this.selectedIndex = this.list.IndexOf (value);
			}
		}


		private readonly List<ViewSettingsData>		list;

		private int									selectedIndex;
	}
}