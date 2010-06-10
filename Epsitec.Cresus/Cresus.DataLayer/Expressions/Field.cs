using Epsitec.Common.Support;


namespace Epsitec.Cresus.DataLayer.Expressions
{


	public class Field : Value
	{


		public Field(Druid FieldId) : base ()
		{
			this.FieldId = FieldId;
		}


		public Druid FieldId
		{
			get;
			private set;
		}


	}


}
