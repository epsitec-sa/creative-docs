using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Entities
{


	public partial class NaturalPersonEntity
	{


		partial void GetFavouriteBeer(ref string value)
		{
			if (this.favouriteBeer == null)
			{
				this.favouriteBeer = NaturalPersonEntity.beers.GetRandomElement ();
			}

			value = this.favouriteBeer;
		}


		partial void SetFavouriteBeer(string value)
		{
			this.favouriteBeer = value;
		}


		partial void GetFavouriteBeerMates(ref IList<NaturalPersonEntity> value)
		{
			if (this.favouriteBeerMates == null)
			{
				this.favouriteBeerMates = new List<NaturalPersonEntity> ();
			}

			value = this.favouriteBeerMates;
		}


		private string favouriteBeer;


		private List<NaturalPersonEntity> favouriteBeerMates;


		private static readonly ReadOnlyCollection<string> beers = new List<string> ()
		{
			"Eau de pierre",
			"Cooper's Pale Ale",
			"Beamish",
			"Telenn Du"
		}.AsReadOnly ();


	}



}
