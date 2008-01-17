using System.Collections.Generic;

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Support.ResourceAccessors;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

namespace Epsitec.Common.FormEngine
{
	/// <summary>
	/// Procédures de manipulation et d'arrangement de listes et d'arbres.
	/// </summary>
	internal sealed class Arrange
	{
		public Arrange(ResourceManager resourceManager, FormDescriptionFinder finder)
		{
			//	Constructeur.
			this.resourceManager = resourceManager;
			this.finder = finder;
		}


		public List<FieldDescription> Merge(List<FieldDescription> reference, List<FieldDescription> patch)
		{
			//	Retourne la liste fusionnée.
			List<FieldDescription> merged = new List<FieldDescription>();

			foreach (FieldDescription field in reference)
			{
				FieldDescription copy = new FieldDescription(field);

				int index = Arrange.IndexOfGuid(patch, field.Guid);
				if (index != -1)
				{
					if (reference[index].Type == FieldDescription.FieldType.Hide)
					{
						copy.Hidden = true;
					}
				}

				merged.Add(copy);
			}

			return merged;
		}


		public string Check(List<FieldDescription> list)
		{
			//	Vérifie une liste. Retourne null si tout est ok, ou un message d'erreur.
			//	Les sous-masques (SubForm) n'ont pas encore été développés.
			int level = 0;

			foreach (FieldDescription field in list)
			{
				if (field.Type == FieldDescription.FieldType.InsertionPoint)
				{
					return "La liste ne doit pas contenir de InsertionPoint";
				}

				if (field.Type == FieldDescription.FieldType.Hide)
				{
					return "La liste ne doit pas contenir de Hide";
				}

				if (field.Type == FieldDescription.FieldType.Node)
				{
					return "La liste ne doit pas contenir de Node";
				}

				if (field.Type == FieldDescription.FieldType.BoxBegin)
				{
					level++;
				}

				if (field.Type == FieldDescription.FieldType.BoxEnd)
				{
					level--;
					if (level < 0)
					{
						return "Il manque un BoxBegin";
					}
				}
			}

			if (level > 0)
			{
				return "Il manque un BoxEnd";
			}

			return null;
		}


		public List<FieldDescription> Organize(List<FieldDescription> fields)
		{
			//	Arrange une liste.
			//	Les sous-masques (SubForm) se comportent comme un début de groupe (BoxBegin),
			//	car ils ont déjà été développés en SubForm-Field-Field-BoxEnd.
			List<FieldDescription> list = new List<FieldDescription>();

			//	Copie la liste en remplaçant les Glue successifs par un suel.
			bool isGlue = false;
			foreach (FieldDescription field in fields)
			{
				if (field.Type == FieldDescription.FieldType.Glue)
				{
					if (isGlue)
					{
						continue;
					}

					isGlue = true;
				}
				else
				{
					isGlue = false;
				}

				list.Add(field);
			}

			//	Si un séparateur est dans une 'ligne', déplace-le au début de la ligne.
			//	Par exemple:
			//	'Field-Glue-Field-Glue-Sep-Field' -> 'Sep-Field-Glue-Field-Glue-Field'
			//	'Field-Glue-Field-Sep-Glue-Field' -> 'Sep-Field-Glue-Field-Glue-Field'
			for (int i=0; i<list.Count; i++)
			{
				if (list[i].Type == FieldDescription.FieldType.Line ||
					list[i].Type == FieldDescription.FieldType.Title)  // séparateur ?
				{
					int j = i;
					bool move;
					do
					{
						move = false;

						if (j > 0 && list[j-1].Type == FieldDescription.FieldType.Glue)  // glue avant ?
						{
							FieldDescription sep = list[j];
							list.RemoveAt(j);
							list.Insert(j-1, sep);  // remplace 'Glue-Sep' par 'Sep-Glue'
							j--;
							move = true;
						}

						if (j > 0 && j < list.Count-1 && list[j+1].Type == FieldDescription.FieldType.Glue)  // glue après ?
						{
							FieldDescription sep = list[j];
							list.RemoveAt(j);
							list.Insert(j-1, sep);  // remplace 'Xxx-Sep-Glue' par 'Sep-Xxx-Glue'
							j--;
							move = true;
						}
					}
					while (move);  // recommence tant qu'on a pu déplacer
				}
			}

			return list;
		}


		public List<FieldDescription> DevelopSubForm(List<FieldDescription> list)
		{
			//	Retourne une liste développée qui ne contient plus de sous-masque.
			//	Un sous-masque (SubForm) se comporte alors comme un début de groupe (BoxBegin).
			//	Un BoxEnd correspond à chaque SubForm.
			List<FieldDescription> dst = new List<FieldDescription>();

			this.DevelopSubForm(dst, list, null, null);

			return dst;
		}

		private void DevelopSubForm(List<FieldDescription> dst, List<FieldDescription> fields, FieldDescription source, string prefix)
		{
			foreach (FieldDescription field in fields)
			{
				if (field.Type == FieldDescription.FieldType.SubForm)
				{
					if (field.SubFormId.IsEmpty)  // aucun Form choisi ?
					{
						continue;
					}

					//	Cherche le sous-masque avec (dans Designer) ou sans (application finale) finder.
					FormDescription subForm = null;

					if (this.finder == null)  // pas de finder ?
					{
						string name = field.SubFormId.ToBundleId();
						ResourceBundle bundle = this.resourceManager.GetBundle(name, ResourceLevel.Default, null);
						if (bundle != null)
						{
							string xml = bundle[FormResourceAccessor.Strings.XmlSource].AsString;
							if (!string.IsNullOrEmpty(xml))
							{
								subForm = Serialization.DeserializeForm(xml, this.resourceManager);
							}
						}
					}
					else  // finder existe (on est dans Designer) ?
					{
						subForm = this.finder(field.SubFormId);
					}

					if (subForm != null)
					{
						dst.Add(field);  // met le SubForm, qui se comportera comme un BoxBegin

						string p = string.Concat(prefix, field.GetPath(null), ".");
						this.DevelopSubForm(dst, subForm.Fields, field, p);  // met les champs du sous-masque dans la boîte

						FieldDescription boxEnd = new FieldDescription(FieldDescription.FieldType.BoxEnd);
						dst.Add(boxEnd);  // met le BoxEnd pour terminer la boîte SubForm
					}
				}
				else
				{
					if (prefix == null || (field.Type != FieldDescription.FieldType.Field && field.Type != FieldDescription.FieldType.SubForm))
					{
						dst.Add(field);
					}
					else
					{
						FieldDescription copy = new FieldDescription(field);
						copy.SetFields(prefix+field.GetPath(null));
						copy.Source = source;
						dst.Add(copy);
					}
				}
			}
		}


		public List<FieldDescription> Develop(List<FieldDescription> fields)
		{
			//	Retourne une liste développée qui ne contient plus de noeuds.
			List<FieldDescription> dst = new List<FieldDescription>();

			this.Develop(dst, fields);
			
			return dst;
		}

		private void Develop(List<FieldDescription> dst, List<FieldDescription> fields)
		{
			foreach (FieldDescription field in fields)
			{
				if (field.Type == FieldDescription.FieldType.Node)
				{
					this.Develop(dst, field.NodeDescription);
				}
				else if (field.Type == FieldDescription.FieldType.InsertionPoint)
				{
				}
				else if (field.Type == FieldDescription.FieldType.Hide)
				{
				}
				else
				{
					dst.Add(field);
				}
			}
		}


		static private int IndexOfGuid(List<FieldDescription> list, System.Guid guid)
		{
			//	Retourne l'index de l'élément utilisant un Guid donné.
			//	Retourne -1 s'il n'en existe aucun.
			for (int i=0; i<list.Count; i++)
			{
				if (list[i].Guid == guid)
				{
					return i;
				}
			}

			return -1;
		}


		private readonly ResourceManager resourceManager;
		private FormDescriptionFinder finder;
	}
}
