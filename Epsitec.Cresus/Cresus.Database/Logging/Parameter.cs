using Epsitec.Common.Support.Extensions;


namespace Epsitec.Cresus.Database.Logging
{


	// TODO Comment this class.
	// Marc


	public sealed class Parameter
	{


		internal Parameter(string name, object value)
		{
			name.ThrowIfNull ("name");

			this.Name = name;
			this.Value = value;
		}


		public string Name
		{
			get;
			private set;
		}


		public object Value
		{
			get;
			private set;
		}


	}


}
