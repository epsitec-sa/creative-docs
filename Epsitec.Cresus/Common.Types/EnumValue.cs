//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	public class EnumValue : IEnumValue
	{
		public EnumValue(int rank, string name)
			: this (rank, name, null, null)
		{
		}

		public EnumValue(int rank, string name, string caption)
			: this (rank, name, caption, null)
		{
		}

		public EnumValue(int rank, string name, string caption, string description)
		{
			this.rank        = rank;
			this.name        = name;
			this.caption     = caption;
			this.description = description;
		}


		public void DefineCaption(string caption)
		{
			this.caption = caption;
		}

		public void DefineDescription(string description)
		{
			this.description = description;
		}

		public void DefineHidden(bool hide)
		{
			this.hidden = hide;
		}


		#region IEnumValue Members
		public int Rank
		{
			get
			{
				return this.rank;
			}
		}

		public bool IsHidden
		{
			get
			{
				return this.hidden;
			}
		}
		#endregion

		#region INameCaption Members
		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string Caption
		{
			get
			{
				return this.caption;
			}
		}

		public string Description
		{
			get
			{
				return this.description;
			}
		}
		#endregion

		private int rank;
		private bool hidden;
		private string name;
		private string caption;
		private string description;
	}
}
