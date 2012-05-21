//	Copyright © 2007-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Identity;
using Epsitec.Common.Types;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (IdentityRepository))]

namespace Epsitec.Common.Identity
{
	/// <summary>
	/// The <c>IdentityRepository</c> class manages a collection of identites,
	/// represented by <see cref="IdentityCard"/> instances.
	/// </summary>
	public class IdentityRepository : DependencyObject
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="IdentityRepository"/> class.
		/// </summary>
		public IdentityRepository()
		{
			this.identities = new Epsitec.Common.Types.Collections.HostedList<IdentityCard> (this.HandleIdentityCardInsertion, this.HandleIdentityCardRemoval);

			this.developerIdToIdentityMap = new Dictionary<int, IdentityCard> ();
			this.userNameToIdentityMap = new Dictionary<string, IdentityCard> ();
		}


		/// <summary>
		/// Gets the collection of identity cards. Items can be added or removed
		/// from the collection.
		/// </summary>
		/// <value>The collection of identity cards.</value>
		public IList<IdentityCard> IdentityCards
		{
			get
			{
				return this.identities;
			}
		}

		/// <summary>
		/// Gets the default identity repository.
		/// </summary>
		/// <value>The default identity repository.</value>
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


		/// <summary>
		/// Finds the identity card which maps to the specified developer
		/// id.
		/// </summary>
		/// <param name="developerId">The developer id.</param>
		/// <returns>The identity card or <c>null</c>.</returns>
		public IdentityCard FindIdentityCard(int developerId)
		{
			this.RefreshMaps ();
			
			IdentityCard identityCard;
			this.developerIdToIdentityMap.TryGetValue (developerId, out identityCard);

			return identityCard;
		}

		/// <summary>
		/// Finds the identity card which maps to the specified user name.
		/// </summary>
		/// <param name="userName">The user name.</param>
		/// <returns>The identity card or <c>null</c>.</returns>
		public IdentityCard FindIdentityCard(string userName)
		{
			this.RefreshMaps ();

			IdentityCard identityCard;
			this.userNameToIdentityMap.TryGetValue (userName, out identityCard);

			return identityCard;
		}

		/// <summary>
		/// Loads identities from the specified path. The identities will be
		/// merged with the ones already stored in the repository.
		/// </summary>
		/// <param name="path">The identity file path.</param>
		/// <returns><c>true</c> if the file exists, <c>false</c> otherwise.</returns>
		public bool Load(string path)
		{
			if (System.IO.File.Exists (path))
			{
				System.Diagnostics.Debug.WriteLine ("Loading identities from " + path);
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

		/// <summary>
		/// Merges the contents of this repository with the specified source
		/// repository.
		/// </summary>
		/// <param name="source">The source repository.</param>
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

			this.developerIdToIdentityMap[card.DeveloperId] = card;
			this.userNameToIdentityMap[card.UserName] = card;
		}

		private void HandleIdentityCardRemoval(IdentityCard card)
		{
			card.Detach (this);
			this.dirtyMaps = true;
		}

		private void RefreshMaps()
		{
			if (this.dirtyMaps)
			{
				lock (this.exclusion)
				{
					if (this.dirtyMaps)
					{
						Dictionary<int, IdentityCard> mapDevId = this.developerIdToIdentityMap;
						Dictionary<string, IdentityCard> mapUser  = this.userNameToIdentityMap;

						foreach (IdentityCard card in this.IdentityCards)
						{
							mapDevId[card.DeveloperId] = card;
							mapUser[card.UserName] = card;
						}

						this.developerIdToIdentityMap = mapDevId;
						this.userNameToIdentityMap    = mapUser;
						this.dirtyMaps = false;
					}
				}
			}
		}

		private static object GetIdentityCardsValue(DependencyObject obj)
		{
			IdentityRepository that = (IdentityRepository) obj;
			return that.IdentityCards;
		}


		public static readonly DependencyProperty IdentityCardsProperty = DependencyProperty<IdentityRepository>.RegisterReadOnly (x => x.IdentityCards, new DependencyPropertyMetadata (IdentityRepository.GetIdentityCardsValue).MakeReadOnlySerializable ());

		public const string DefaultIdentitiesFileName = "default.identities";

		private readonly object	exclusion = new object ();
		private Epsitec.Common.Types.Collections.HostedList<IdentityCard> identities;
		private Dictionary<int, IdentityCard> developerIdToIdentityMap;
		private Dictionary<string, IdentityCard> userNameToIdentityMap;
		private bool dirtyMaps;
		
		private static IdentityRepository repository;
	}
}
