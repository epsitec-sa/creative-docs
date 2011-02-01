namespace Epsitec.Cresus.Database.Logging
{


	/// <summary>
	/// The <c>LogMode</c> enum contains the modes use to configure instances of <see cref="AbstractLog"/>.
	/// </summary>
	public enum LogMode
	{


		/// <summary>
		/// The logging is turned off, no query is ever logged.
		/// </summary>
		Off,


		/// <summary>
		/// Only the basic informations about the queries are logged.
		/// </summary>
		Basic,


		/// <summary>
		/// All the informations about the queries are logged.
		/// </summary>
		Extended,


	}


}
