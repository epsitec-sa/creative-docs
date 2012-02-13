//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Search.Data;
using Epsitec.Cresus.Compta.Options.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Memory.Data
{
	/// <summary>
	/// Liste des paramètres des recherches, du filtre et des options, c'est-à-dire de l'ensemble des paramètres
	/// liés à une présentation.
	/// </summary>
	public class MemoryList : ISettingsData
	{
		public MemoryList()
		{
			this.list = new List<MemoryData> ();
		}


		public List<MemoryData> List
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
				return this.list.IndexOf (this.selected);
			}
			set
			{
				if (value >= 0 && value < this.list.Count)
				{
					this.selected = this.list[value];
				}
				else
				{
					this.selected = null;
				}
			}
		}

		public MemoryData Selected
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


		private readonly List<MemoryData>	list;
		private MemoryData					selected;
	}
}