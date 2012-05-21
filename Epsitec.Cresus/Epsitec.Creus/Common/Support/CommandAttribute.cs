//	Copyright © 2004-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>CommandAttribute</c> class defines a <c>[Command]</c> attribute,
	/// which is used by <see cref="CommandDispatcher"/> to locate the methods
	/// implementing specific commands.
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
		/// <param name="commandId">The command DRUID (encoded as a raw <c>long</c> value).</param>
		public CommandAttribute(long commandId)
		{
			this.commandId = commandId;
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

		public Druid							CommandId
		{
			get
			{
				return this.commandId;
			}
		}

		private string							commandName;
		private long							commandId;
	}
}
