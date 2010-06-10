namespace Epsitec.Cresus.DataLayer.Expressions
{


	public class Constant : Value
	{

		
		public Constant(object val) : base()
		{
			this.Value = val;
		}


		public object Value
		{
			get;
			private set;
		}

	}


}
