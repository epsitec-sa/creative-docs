//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>CommandAttribute</c> class defines a <c>[Command]</c> attribute,
	/// which is used by <see cref="CommandDispatcher"/> to locate the methods
	/// implementating specific commands.
	/// </summary>
	
	[System.Serializable]
	[System.AttributeUsage (System.AttributeTargets.Method, AllowMultiple = true)]
	
	public class CommandAttribute : System.Attribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CommandAttribute"/> class.
		/// </summary>
		public CommandAttribute()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CommandAttribute"/> class.
		/// </summary>
		/// <param name="commandName">Name of the command.</param>
		public CommandAttribute(string commandName)
		{
			this.commandName = commandName;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CommandAttribute"/> class.
		/// </summary>
		/// <param name="druid">The DRUID (encoded as a raw <c>long</c> value) of the command.</param>
		public CommandAttribute(long druid)
		{
			this.druid = druid;
		}


		/// <summary>
		/// Gets or sets the name of the command.
		/// </summary>
		/// <value>The name of the command.</value>
		public string							CommandName
		{
			get
			{
				return this.commandName;
			}
			set
			{
				this.commandName = value;
			}
		}

		/// <summary>
		/// Gets or sets the DRUID of the command.
		/// </summary>
		/// <value>The DRUID of the command.</value>
		public long								Druid
		{
			get
			{
				return this.druid;
			}
			set
			{
				this.druid = value;
			}
		}
	
		
		
		private string							commandName;
		private long							druid;
	}
}
