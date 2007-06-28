//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Identity;
using Epsitec.Common.Types;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (IdentityRepository))]

namespace Epsitec.Common.Identity
{
	public class IdentityRepository : DependencyObject
	{
		public IdentityRepository()
		{
			this.identities = new Epsitec.Common.Types.Collections.HostedList<IdentityCard> (this.HandleIdentityCardInsertion, this.HandleIdentityCardRemoval);
		}

		
		public IList<IdentityCard> IdentityCards
		{
			get
			{
				return this.identities;
			}
		}


		public static IdentityRepository Default
		{
			get
			{
				if (IdentityRepository.repository == null)
				{
					IdentityRepository.repository = new IdentityRepository ();
					IdentityRepository.repository.LoadDefault ();
				}

				return IdentityRepository.repository;
			}
		}


		public IdentityCard FindIdentityCard(string userName)
		{
			foreach (IdentityCard card in this.IdentityCards)
			{
				if (card.UserName == userName)
				{
					return card;
				}
			}

			return null;
		}
		
		public bool Load(string path)
		{
			if (System.IO.File.Exists (path))
			{
				string xml = System.IO.File.ReadAllText (path, System.Text.Encoding.UTF8);
				IdentityRepository load = Epsitec.Common.Types.Serialization.SimpleSerialization.DeserializeFromString (xml) as IdentityRepository;
				this.MergeWithRepository (load);
				return true;
			}
			else
			{
				return false;
			}
		}

		public void MergeWithRepository(IdentityRepository source)
		{
			if (source == null)
			{
				return;
			}

			Dictionary<string, IdentityCard> map = new Dictionary<string, IdentityCard> ();

			foreach (IdentityCard card in this.IdentityCards)
			{
				map[card.UserName] = card;
			}
			
			foreach (IdentityCard card in source.IdentityCards)
			{
				IdentityCard existingCard;

				if (map.TryGetValue (card.UserName, out existingCard))
				{
					existingCard.MergeWithCard (card);
				}
				else
				{
					map[card.UserName] = new IdentityCard (card);
				}
			}

			this.identities.Clear ();
			this.identities.AddRange (map.Values);
		}

		private void LoadDefault()
		{
			this.Load (System.IO.Path.Combine (Epsitec.Common.Support.Globals.Directories.ExecutableRoot, IdentityRepository.DefaultIdentitiesFileName));
			this.Load (System.IO.Path.Combine (Epsitec.Common.Support.Globals.Directories.CommonAppData, IdentityRepository.DefaultIdentitiesFileName));
			this.Load (System.IO.Path.Combine (Epsitec.Common.Support.Globals.Directories.UserAppData, IdentityRepository.DefaultIdentitiesFileName));
		}

		private void HandleIdentityCardInsertion(IdentityCard card)
		{
			card.Attach (this);
		}

		private void HandleIdentityCardRemoval(IdentityCard card)
		{
			card.Detach (this);
		}
		
		private static object GetIdentityCardsValue(DependencyObject obj)
		{
			IdentityRepository that = (IdentityRepository) obj;
			return that.IdentityCards;
		}

		public static readonly DependencyProperty IdentityCardsProperty = DependencyProperty.RegisterReadOnly ("IdentityCards", typeof (ICollection<IdentityCard>), typeof (IdentityRepository), new DependencyPropertyMetadata (IdentityRepository.GetIdentityCardsValue).MakeReadOnlySerializable ());

		public const string DefaultIdentitiesFileName = "default.identities";

		private Epsitec.Common.Types.Collections.HostedList<IdentityCard> identities;
		private static IdentityRepository repository;
	}
}
