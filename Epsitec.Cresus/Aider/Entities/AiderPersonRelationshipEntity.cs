using Epsitec.Common.Types;

using Epsitec.Cresus.Core;


namespace Epsitec.Aider.Entities
{


	public partial class AiderPersonRelationshipEntity
	{


		public override FormattedText GetSummary()
		{
			return this.GetRelationText (this.Type, this.Person1, this.Person2);
		}


		public override FormattedText GetCompactSummary()
		{
			return this.GetRelationText (this.Type, this.Person1, this.Person2);
		}


		private FormattedText GetRelationText(PersonRelationshipType relationType, AiderPersonEntity person1, AiderPersonEntity person2)
		{
			var person1Text = person1.GetCompactSummary ();
			var person2Text = person2.GetCompactSummary ();

			return TextFormatter.FormatText (person1Text, " -> ", relationType, " <- ", person2Text);
		}


	}


}
