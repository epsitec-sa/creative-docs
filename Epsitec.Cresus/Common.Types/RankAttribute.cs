//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>RankAttribute</c> attribute can be used to associate a user-interface
	/// related rank to a specific item (for instance a value in an <c>enum</c>).
	/// </summary>

	[System.Serializable]
	[System.AttributeUsage (System.AttributeTargets.Assembly |
		/* */				System.AttributeTargets.Class |
		/* */				System.AttributeTargets.Enum |
		/* */				System.AttributeTargets.Event |
		/* */				System.AttributeTargets.Field |
		/* */				System.AttributeTargets.Property |
		/* */				System.AttributeTargets.Struct,
		/* */				AllowMultiple = false)]

	public sealed class RankAttribute : System.Attribute
	{
		public RankAttribute()
		{
			this.rank = -1;
		}

		public RankAttribute(int rank)
		{
			this.rank = rank;
		}

		public int Rank
		{
			get
			{
				return this.rank;
			}
			set
			{
				this.rank = value;
			}
		}

		private int rank;
	}
}
