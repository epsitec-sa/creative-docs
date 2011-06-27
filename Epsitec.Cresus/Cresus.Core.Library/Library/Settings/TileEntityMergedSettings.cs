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
		
		public TileFieldEditionSettings this[Druid fieldId]
		{
			get
			{
				TileFieldEditionSettings current;
				this.settings.TryGetValue (fieldId, out current);
				return current;
			}
		}


		public void Accumulate(Druid field, IEnumerable<TileUserFieldEditionSettings> settings)
		{
			foreach (var item in settings)
			{
				this.Accumulate (field, item.FieldSettings, item.FieldSettingsMode);
			}
		}

		public void Accumulate(Druid field, TileUserFieldEditionSettings settings)
		{
			this.Accumulate (field, settings.FieldSettings, settings.FieldSettingsMode);
		}

		public void Accumulate(Druid field, TileFieldEditionSettings settings, TileFieldSettingsMode mode)
		{
			TileFieldEditionSettings current;

			this.settings.TryGetValue (field, out current);
			this.settings[field] = TileUserFieldEditionSettings.Combine (current, settings, mode);
		}


		private readonly Druid entityId;
		private readonly Dictionary<Druid, TileFieldEditionSettings>	settings;
	}
}
