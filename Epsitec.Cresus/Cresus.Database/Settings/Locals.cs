//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Settings
{
	/// <summary>
	/// Summary description for Locals.
	/// </summary>
	public sealed class Locals : AbstractBase
	{
		internal Locals(DbInfrastructure infrastructure, DbTransaction transaction)
		{
			this.Setup (infrastructure, transaction, Locals.Name);
		}
		
		
		internal static string					Name
		{
			get
			{
				return "CR_SETTINGS_LOCALS";
			}
		}
		
		
		public int								ClientId
		{
			get
			{
				return this.client_id;
			}
			set
			{
				if (this.client_id != value)
				{
					this.client_id = value;
					this.NotifyPropertyChanged ("ClientId");
				}
			}
		}
		
		
		private int								client_id;
	}
}
