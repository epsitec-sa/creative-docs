//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Types.DateTimeType))]

namespace Epsitec.Common.Types
{
	public sealed class DateTimeType : AbstractDateTimeType
	{
		public DateTimeType()
			: this ("DateTime")
		{
		}

		public DateTimeType(string name)
			: base (name)
		{
		}

		public DateTimeType(Caption caption)
			: base (caption)
		{
		}


		public override System.Type SystemType
		{
			get
			{
				return typeof (System.DateTime);
			}
		}

		protected override bool IsInRange(object value)
		{
			System.DateTime time = (System.DateTime) value;

			throw new System.Exception ("The method or operation is not implemented.");
		}
	}
}
