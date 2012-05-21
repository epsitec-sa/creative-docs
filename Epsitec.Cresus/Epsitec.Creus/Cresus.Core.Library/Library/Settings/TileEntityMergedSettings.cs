//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Library.Settings
{
	public sealed class TileEntityMergedSettings
	{
		public TileEntityMergedSettings(Druid entityId)
		{
			this.entityId = entityId;
			this.settings = new Dictionary<Druid, TileFieldEditionSettings> ();
		}


		public Druid EntityId
		{
			get
			{
				return this.entityId;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.settings.Count == 0;
			}
		}

		public TileFieldEditionSettings this[Druid fieldId]
		{
			get
			{
				TileFieldEditionSettings current;
				this.settings.TryGetValue (fieldId, out current);
				return current;
			}
		}


		public bool Contains(Druid fieldId)
		{
			return this.settings.ContainsKey (fieldId);
		}

		public void Accumulate(Druid field, IEnumerable<UserFieldEditionSettings> settings)
		{
			foreach (var item in settings)
			{
				this.Accumulate (field, item.FieldSettings, item.MergeSettingsMode);
			}
		}

		public void Accumulate(Druid field, UserFieldEditionSettings settings)
		{
			this.Accumulate (field, settings.FieldSettings, settings.MergeSettingsMode);
		}

		public void Accumulate(Druid field, TileFieldEditionSettings settings, MergeSettingsMode mode)
		{
			TileFieldEditionSettings current;

			this.settings.TryGetValue (field, out current);
			this.settings[field] = UserFieldEditionSettings.Combine (current, settings, mode);
		}


		private readonly Druid entityId;
		private readonly Dictionary<Druid, TileFieldEditionSettings>	settings;
	}
}
