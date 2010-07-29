using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;


namespace Epsitec.Cresus.DataLayer.Expressions
{


	/// <summary>
	/// The <c>Field</c> class represents a field of an <see cref="AbstractEntity"/> in an
	/// <see cref="Expression"/>.
	/// </summary>
	public sealed class Field
	{


		/// <summary>
		/// Creates a new <c>Field</c>.
		/// </summary>
		/// <param name="FieldId">The <see cref="Druid"/> that identifies the <c>Field</c>.</param>
		public Field(Druid FieldId)
		{
			this.FieldId = FieldId;
		}


		/// <summary>
		/// Gets the <see cref="Druid"/> that identifies the <c>Field</c>.
		/// </summary>
		public Druid FieldId
		{
			get;
			private set;
		}


	}


}
