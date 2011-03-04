//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

#region Epsitec.Common.Dialogs.ISearchable Interface
namespace Epsitec.Common.Dialogs.Entities
{
	///	<summary>
	///	The <c>ISearchable</c> entity.
	///	designer:cap/6015
	///	</summary>
	public interface ISearchable
	{
		///	<summary>
		///	The <c>SearchValue</c> field.
		///	designer:fld/6015/6016
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[6016]")]
		string SearchValue
		{
			get;
			set;
		}
	}
	public static partial class ISearchableInterfaceImplementation
	{
		public static string GetSearchValue(global::Epsitec.Common.Dialogs.Entities.ISearchable obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<string> ("[6016]");
		}
		public static void SetSearchValue(global::Epsitec.Common.Dialogs.Entities.ISearchable obj, string value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			string oldValue = obj.SearchValue;
			if (oldValue != value || !entity.IsFieldDefined("[6016]"))
			{
				ISearchableInterfaceImplementation.OnSearchValueChanging (obj, oldValue, value);
				entity.SetField<string> ("[6016]", oldValue, value);
				ISearchableInterfaceImplementation.OnSearchValueChanged (obj, oldValue, value);
			}
		}
		static partial void OnSearchValueChanged(global::Epsitec.Common.Dialogs.Entities.ISearchable obj, string oldValue, string newValue);
		static partial void OnSearchValueChanging(global::Epsitec.Common.Dialogs.Entities.ISearchable obj, string oldValue, string newValue);
	}
}
#endregion

