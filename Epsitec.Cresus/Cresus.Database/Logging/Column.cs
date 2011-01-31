using Epsitec.Common.Support.Extensions;


namespace Epsitec.Cresus.Database.Logging
{


	// TODO Comment this class.
	// Marc


	public sealed class Column
	{


		internal Column(string name)
		{
			name.ThrowIfNull ("name");

			this.Name = name;
		}
		
		
		public string Name
		{
			get;
			private set;
		}
		

	}


}
