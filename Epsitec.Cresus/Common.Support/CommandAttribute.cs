//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>CommandAttribute</c> class defines a <c>[Command]</c> attribute,
	/// which is used by <see cref="T:CommandDispatcher"/> to locate the methods
	/// implementating specific commands.
	/// </summary>
	
	[System.Serializable]
	[System.AttributeUsage (System.AttributeTargets.Method, AllowMultiple = true)]
	
	public class CommandAttribute : System.Attribute
	{
		public CommandAttribute()
		{
		}
		
		public CommandAttribute(string commandName)
		{
			this.commandName = commandName;
		}

		public CommandAttribute(long druid)
		{
			this.druid = druid;
		}


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
