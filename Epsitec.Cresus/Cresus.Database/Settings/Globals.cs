//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Settings
{
	/// <summary>
	/// Summary description for Globals.
	/// </summary>
	public sealed class Globals : AbstractBase
	{
		internal Globals(DbInfrastructure infrastructure, DbTransaction transaction)
		{
			this.Setup (infrastructure, transaction, Globals.Name);
		}
		
		
		internal static string					Name
		{
			get
			{
				return "CR_SETTINGS_GLOBALS";
			}
		}
		
		
		public string							CustomerLicence
		{
			get
			{
				return this.customer_licence;
			}
			set
			{
				if (this.customer_licence != value)
				{
					this.customer_licence = value;
					this.NotifyPropertyChanged ("CustomerLicence");
				}
			}
		}
		
		public string							CustomerId
		{
			get
			{
				return this.customer_id;
			}
			set
			{
				if (this.customer_id != value)
				{
					this.customer_id = value;
					this.NotifyPropertyChanged ("CustomerId");
				}
			}
		}
		
		
		private string							customer_licence;
		private string							customer_id;
	}
}
