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
	public class Arrange
	{
		public Arrange(IFormResourceProvider resourceProvider)
		{
			//	Constructeur.
			this.resourceProvider = resourceProvider;
		}


		public List<FieldDescription> Merge(List<FieldDescription> reference, List<FieldDescription> patch)
		{
			//	Retourne la liste fusionnée.
			List<FieldDescription> merged = new List<FieldDescription>();

			//	Génère la liste fusionnée de tous les champs. Les champs cachés sont quand même dans la liste,
			//	mais avec la propriété PatchHidden = true.
			foreach (FieldDescription field in reference)
			{
				FieldDescription copy = new FieldDescription(field);

				int index = Arrange.IndexOfGuid(patch, field.Guid);
				if (index != -1 && patch[index].PatchHidden)
				{
					copy.PatchHidden = true;  // champ à cacher
				}

				merged.Add(copy);
			}

			foreach (FieldDescription field in patch)
			{
				field.PatchBrokenAttach = false;

				if (field.PatchMoved)  // champ à déplacer ?
				{
					int src = Arrange.IndexOfGuid(merged, field.Guid);  // cherche le champ à déplacer
					if (src != -1)
					{
						//	field.PatchAttachGuid vaut System.Guid.Empty lorsqu'il faut déplacer l'élément en tête
						//	de liste.
						int dst = -1;  // position pour mettre en-tête de liste
						if (field.PatchAttachGuid != System.Guid.Empty)
						{
							dst = Arrange.IndexOfGuid(merged, field.PatchAttachGuid);  // cherche où le déplacer
							if (dst == -1)  // l'élément d'attache n'existe plus ?
							{
								field.PatchBrokenAttach = true;
								continue;  // on laisse le champ ici
							}
						}

						FieldDescription temp = merged[src];
						merged.RemoveAt(src);

						dst = Arrange.IndexOfGuid(merged, field.PatchAttachGuid);  // recalcule le "où" après suppression
						merged.Insert(dst+1, temp);  // remet l'élément après dst

						temp.PatchMoved = true;
					}
				}

				if (field.PatchInserted)  // champ à insérer ?
				{
					//	field.PatchAttachGuid vaut System.Guid.Empty lorsqu'il faut déplacer l'élément en tête
					//	de liste.
					int dst = -1;  // position pour mettre en-tête de liste
					if (field.PatchAttachGuid != System.Guid.Empty)
					{
						dst = Arrange.IndexOfGuid(merged, field.PatchAttachGuid);  // cherche où le déplacer
						if (dst == -1)  // l'élément d'attache n'existe plus ?
						{
							dst = merged.Count-1;  // on insère le champ à la fin
							field.PatchBrokenAttach = true;
						}
					}

					FieldDescription copy = new FieldDescription(field);
					copy.PatchInserted = true;
					merged.Insert(dst+1, copy);  // insère l'élément après dst
				}

				if (field.PatchModified)  // champ à modifier ?
				{
					int index = Arrange.IndexOfGuid(merged, field.Guid);
					if (index != -1)
					{
						merged.RemoveAt(index);  // supprime le champ original

						FieldDescription copy = new FieldDescription(field);
						copy.PatchModified = true;
						merged.Insert(index, copy);  // et remplace-le par le champ modifié
					}
				}
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

					//	Cherche le sous-masque
					FormDescription subForm = null;
					string xml = this.resourceProvider.GetFormXmlSource(field.SubFormId);
					if (!string.IsNullOrEmpty(xml))
					{
						subForm = Serialization.DeserializeForm(xml);
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
				else
				{
					dst.Add(field);
				}
			}
		}


		static public int IndexOfGuid(List<FieldDescription> list, System.Guid guid)
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


		private readonly IFormResourceProvider resourceProvider;
	}
}
