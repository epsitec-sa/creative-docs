//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.OpenType
{
	/// <summary>
	/// Summary description for Font.
	/// </summary>
	public class Font
	{
		public Font()
		{
		}
		
		
		public void Initialize(TableDirectory directory)
		{
			this.ot_directory = directory;
			this.ot_GSUB      = new Table_GSUB (directory.FindTable ("GSUB"));
		}
		
		
		public void SelectScript(string script)
		{
			this.SelectScript (script, "");
		}
		
		public void SelectScript(string script, string language)
		{
			if (this.ot_GSUB.ScriptListTable.ContainsScript (script))
			{
				uint   required_feature  = this.ot_GSUB.GetRequiredFeatureIndex (script, language);
				uint[] optional_features = this.ot_GSUB.GetFeatureIndexes (script, language);
				
				if (required_feature == 0xffff)
				{
					this.script_required_feature = null;
				}
				else
				{
					this.script_required_feature = this.ot_GSUB.FeatureListTable.GetTaggedFeatureTable (required_feature);
				}
				
				this.script_optional_features = new TaggedFeatureTable[optional_features.Length];
				
				for (int i = 0; i < optional_features.Length; i++)
				{
					this.script_optional_features[i] = this.ot_GSUB.FeatureListTable.GetTaggedFeatureTable (optional_features[i]);
				}
			}
			else
			{
				this.script_required_feature  = null;
				this.script_optional_features = null;
			}
			
			this.substitution_lookups = null;
		}
		
		public void SelectFeatures(params string[] features)
		{
			FeatureListTable             feature_list    = this.ot_GSUB.FeatureListTable;
			System.Collections.ArrayList active_features = new System.Collections.ArrayList ();
			System.Collections.Hashtable active_names    = new System.Collections.Hashtable ();
			
			for (int i = 0; i < features.Length; i++)
			{
				active_names[features[i]] = null;
			}
			
			if (this.script_required_feature != null)
			{
				active_features.Add (this.script_required_feature);
			}
			
			if (this.script_optional_features == null)
			{
				uint n = feature_list.FeatureCount;
				
				for (uint i = 0; i < n; i++)
				{
					if (active_names.Contains (feature_list.GetFeatureTag (i)))
					{
						active_features.Add (feature_list.GetFeatureTable (i));
						active_names.Remove (feature_list.GetFeatureTag (i));
					}
				}
			}
			else
			{
				int n = this.script_optional_features.Length;
				
				for (int i = 0; i < n; i++)
				{
					if (active_names.Contains (this.script_optional_features[i].Tag))
					{
						active_features.Add (this.script_optional_features[i]);
					}
				}
			}
			
			this.GenerateSubstitutionLookups (active_features);
		}
		
		
		public string[] GetSupportedScripts()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			ScriptListTable script_list = this.ot_GSUB.ScriptListTable;
			uint n = script_list.ScriptCount;
			
			for (uint i = 0; i < n; i++)
			{
				string      script_tag   = script_list.GetScriptTag (i);
				ScriptTable script_table = script_list.GetScriptTable (i);
				
				if (i > 0)
				{
					buffer.Append ("|");
				}
				
				buffer.Append (script_tag);
				
				uint m = script_table.LangSysCount;
				
				for (uint j = 0; j < m; j++)
				{
					buffer.Append ("|");
					buffer.Append (script_tag);
					buffer.Append (":");
					buffer.Append (script_table.GetLangSysTag (j));
				}
			}
			
			return buffer.ToString ().Split ('|');
		}
		
		public string[] GetSupportedFeatures()
		{
			if (this.script_optional_features == null)
			{
				FeatureListTable feature_list = this.ot_GSUB.FeatureListTable;
				System.Collections.Hashtable hash = new System.Collections.Hashtable ();
				
				uint n = feature_list.FeatureCount;
				
				for (uint i = 0; i < n; i++)
				{
					hash[feature_list.GetFeatureTag (i)] = null;
				}
				
				string[] feature_names = new string[hash.Count];
				hash.Keys.CopyTo (feature_names, 0);
				
				return feature_names;
			}
			else
			{
				int n = this.script_optional_features.Length;
				
				string[] feature_names = new string[n];
				
				for (int i = 0; i < n; i++)
				{
					feature_names[i] = this.script_optional_features[i].Tag;
				}
				
				return feature_names;
			}
		}
		
		
		protected void GenerateSubstitutionLookups(System.Collections.ICollection feature_tables)
		{
			System.Collections.ArrayList lookup_indexes = new System.Collections.ArrayList ();
			
			foreach (FeatureTable feature_table in feature_tables)
			{
				uint n = feature_table.LookupCount;
				
				for (uint i = 0; i < n; i++)
				{
					uint lookup = feature_table.GetLookupIndex (i);
					
					if (lookup_indexes.Contains (lookup) == false)
					{
						lookup_indexes.Add (lookup);
					}
				}
			}
			
			lookup_indexes.Sort ();
			
			int count = 0;
			int index = 0;
			
			foreach (uint lookup in lookup_indexes)
			{
				count += (int) this.ot_GSUB.LookupListTable.GetLookupTable (lookup).SubTableCount;
			}
			
			this.substitution_lookups = new SubstSubTable[count];
			
			foreach (uint lookup in lookup_indexes)
			{
				LookupTable lookup_table = this.ot_GSUB.LookupListTable.GetLookupTable (lookup);
				
				uint n = lookup_table.SubTableCount;
				
				for (uint i = 0; i < n; i++)
				{
					this.substitution_lookups[index++] = lookup_table.GetSubTable (i);
				}
			}
		}
		
		
		private TableDirectory					ot_directory;
		private Table_GSUB						ot_GSUB;
		
		private TaggedFeatureTable				script_required_feature;
		private TaggedFeatureTable[]			script_optional_features;
		private SubstSubTable[]					substitution_lookups;
	}
}
