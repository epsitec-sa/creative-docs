namespace Epsitec.Cresus.DataLayer.Expressions
{


	public class Constant
	{

		
		public Constant(Type type, object value)
		{
			this.Type = type;
			this.Value = value;
		}


		public Type Type
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
