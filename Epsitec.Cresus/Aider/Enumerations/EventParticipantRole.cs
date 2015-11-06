//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;


namespace Epsitec.Aider.Enumerations
{
	[DesignerVisible]
	public enum EventParticipantRole
	{
		None			= 0,
		Minister		= 1,
		Father			= 2,
		Mother			= 3,
		GodFather		= 4,
		GodMother		= 5,
		Witness			= 6,
		BlessedChild	= 7,
		ChildBatise		= 8,
		Husband			= 9,	
		Spouse			= 10,	
		DeceasedPerson	= 11,
		HusbandFather	= 12,
		HusbandMother   = 13,
		SpouseFather	= 14,
		SpouseMother	= 15,
		Confirmant      = 18,
		Catechumen		= 19,
		PartnerA        = 21,
		PartnerB        = 22,
		PartnerAFather	= 23,
		PartnerAMother  = 24,
		PartnerBFather	= 25,
		PartnerBMother	= 26,
		[Hidden]
		SecondWitness   = 16,
		[Hidden]
		FirstWitness	= 17,
		[Hidden]
		Partner         = 20,
	}
}