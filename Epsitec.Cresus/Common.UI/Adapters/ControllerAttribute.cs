//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Adapters
{
	using IComparer = System.Collections.IComparer;
	
	/// <summary>
	/// Summary description for ControllerAttribute.
	/// </summary>
	
	[System.Serializable]
	[System.AttributeUsage (System.AttributeTargets.Class, AllowMultiple = true)]
	
	public class ControllerAttribute : System.Attribute
	{
		public ControllerAttribute()
		{
		}
		
		public ControllerAttribute(int rank, System.Type type)
		{
			this.rank = rank;
			this.type = type;
		}
		
		
		public int								Rank
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
		
		public System.Type						Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}
		
		
		public static IComparer					RankComparer
		{
			get
			{
				return new RankComparerClass ();
			}
		}
		
		
		private class RankComparerClass : IComparer
		{
			#region IComparer Members
			public int Compare(object x, object y)
			{
				ControllerAttribute attr_x = x as ControllerAttribute;
				ControllerAttribute attr_y = y as ControllerAttribute;

				if (attr_x == attr_y)
				{
					return 0;
				}
				
				if (attr_x == null)
				{
					return -1;
				}
				if (attr_y == null)
				{
					return 1;
				}
				
				int rx = attr_x.rank;
				int ry = attr_y.rank;
				
				return rx - ry;
			}
			#endregion
		}
		
		
		protected System.Type					type;
		protected int							rank;
	}
}
