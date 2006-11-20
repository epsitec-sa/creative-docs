//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Settings
{
	/// <summary>
	/// The <c>Globals</c> class stores global settings, backed by a database
	/// table.
	/// </summary>
	public sealed class Globals : AbstractBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Globals"/> class.
		/// </summary>
		/// <param name="infrastructure">The infrastructure.</param>
		/// <param name="transaction">The transaction.</param>
		internal Globals(DbInfrastructure infrastructure, DbTransaction transaction)
		{
			this.AttachAndLoad (infrastructure, transaction, Globals.Name);
		}

		/// <summary>
		/// Gets or sets the customer licence.
		/// </summary>
		/// <value>The customer licence.</value>
		public string							CustomerLicence
		{
			get
			{
				return this.customerLicence;
			}
			set
			{
				if (this.customerLicence != value)
				{
					object oldValue = this.customerLicence;
					object newValue = value;
					
					this.customerLicence = value;
					
					this.NotifyPropertyChanged ("CustomerLicence", oldValue, newValue);
				}
			}
		}

		/// <summary>
		/// Gets or sets the customer id.
		/// </summary>
		/// <value>The customer id.</value>
		public string							CustomerId
		{
			get
			{
				return this.customerId;
			}
			set
			{
				if (this.customerId != value)
				{
					object oldValue = this.customerId;
					object newValue = value;
					
					this.customerId = value;
					
					this.NotifyPropertyChanged ("CustomerId", oldValue, newValue);
				}
			}
		}
		
		internal static readonly string			Name = "CR_SETTINGS_GLOBALS";
		
		private string							customerLicence;
		private string							customerId;
	}
}
