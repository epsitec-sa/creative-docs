using System.Collections.Generic;

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Support.ResourceAccessors;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

namespace Epsitec.Common.FormEngine
{
	/// <summary>
	/// Proc�dures de manipulation et d'arrangement de listes et d'arbres.
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
			//	Retourne la liste fusionn�e.
			List<FieldDescription> merged = new List<FieldDescription>();

			//	G�n�re la liste fusionn�e de tous les champs. Les champs cach�s sont quand m�me dans la liste,
			//	mais avec la propri�t� PatchHidden = true.
			foreach (FieldDescription field in reference)
			{
				FieldDescription copy = new FieldDescription(field);

				int index = Arrange.IndexOfGuid(patch, field.Guid);
				if (index != -1 && patch[index].PatchHidden)
				{
					copy.PatchHidden = true;  // champ � cacher
				}

				merged.Add(copy);
			}

			foreach (FieldDescription field in patch)
			{
				field.PatchBrokenAttach = false;

				if (field.PatchMoved)  // champ � d�placer ?
				{
					int src = Arrange.IndexOfGuid(merged, field.Guid);  // cherche le champ � d�placer
					if (src != -1)
					{
						//	field.PatchAttachGuid vaut System.Guid.Empty lorsqu'il faut d�placer l'�l�ment en t�te
						//	de liste.
						int dst = -1;  // position pour mettre en-t�te de liste
						if (field.PatchAttachGuid != System.Guid.Empty)
						{
							dst = Arrange.IndexOfGuid(merged, field.PatchAttachGuid);  // cherche o� le d�placer
							if (dst == -1)  // l'�l�ment d'attache n'existe plus ?
							{
								field.PatchBrokenAttach = true;
								continue;  // on laisse le champ ici
							}
						}

						FieldDescription temp = merged[src];
						merged.RemoveAt(src);

						dst = Arrange.IndexOfGuid(merged, field.PatchAttachGuid);  // recalcule le "o�" apr�s suppression
						merged.Insert(dst+1, temp);  // remet l'�l�ment apr�s dst

						temp.PatchMoved = true;
					}
				}

				if (field.PatchInserted)  // champ � ins�rer ?
				{
					//	field.PatchAttachGuid vaut System.Guid.Empty lorsqu'il faut d�placer l'�l�ment en t�te
					//	de liste.
					int dst = -1;  // position pour mettre en-t�te de liste
					if (field.PatchAttachGuid != System.Guid.Empty)
					{
						dst = Arrange.IndexOfGuid(merged, field.PatchAttachGuid);  // cherche o� le d�placer
						if (dst == -1)  // l'�l�ment d'attache n'existe plus ?
						{
							dst = merged.Count-1;  // on ins�re le champ � la fin
							field.PatchBrokenAttach = true;
						}
					}

					FieldDescription copy = new FieldDescription(field);
					copy.PatchInserted = true;
					merged.Insert(dst+1, copy);  // ins�re l'�l�ment apr�s dst
				}

				if (field.PatchModified)  // champ � modifier ?
				{
					int index = Arrange.IndexOfGuid(merged, field.Guid);
					if (index != -1)
					{
						merged.RemoveAt(index);  // supprime le champ original

						FieldDescription copy = new FieldDescription(field);
						copy.PatchModified = true;
						merged.Insert(index, copy);  // et remplace-le par le champ modifi�
					}
				}
			}

			return merged;
		}


		public string Check(List<FieldDescription> list)
		{
			//	V�rifie une liste. Retourne null si tout est ok, ou un message d'erreur.
			//	Les sous-masques (SubForm) n'ont pas encore �t� d�velopp�s.
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
			//	Les sous-masques (SubForm) se comportent comme un d�but de groupe (BoxBegin),
			//	car ils ont d�j� �t� d�velopp�s en SubForm-Field-Field-BoxEnd.
			List<FieldDescription> list = new List<FieldDescription>();

			//	Copie la liste en rempla�ant les Glue successifs par un suel.
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

			//	Si un s�parateur est dans une 'ligne', d�place-le au d�but de la ligne.
			//	Par exemple:
			//	'Field-Glue-Field-Glue-Sep-Field' -> 'Sep-Field-Glue-Field-Glue-Field'
			//	'Field-Glue-Field-Sep-Glue-Field' -> 'Sep-Field-Glue-Field-Glue-Field'
			for (int i=0; i<list.Count; i++)
			{
				if (list[i].Type == FieldDescription.FieldType.Line ||
					list[i].Type == FieldDescription.FieldType.Title)  // s�parateur ?
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

						if (j > 0 && j < list.Count-1 && list[j+1].Type == FieldDescription.FieldType.Glue)  // glue apr�s ?
						{
							FieldDescription sep = list[j];
							list.RemoveAt(j);
							list.Insert(j-1, sep);  // remplace 'Xxx-Sep-Glue' par 'Sep-Xxx-Glue'
							j--;
							move = true;
						}
					}
					while (move);  // recommence tant qu'on a pu d�placer
				}
			}

			return list;
		}


		public List<FieldDescription> DevelopSubForm(List<FieldDescription> list)
		{
			//	Retourne une liste d�velopp�e qui ne contient plus de sous-masque.
			//	Un sous-masque (SubForm) se comporte alors comme un d�but de groupe (BoxBegin).
			//	Un BoxEnd correspond � chaque SubForm.
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
						this.DevelopSubForm(dst, subForm.Fields, field, p);  // met les champs du sous-masque dans la bo�te

						FieldDescription boxEnd = new FieldDescription(FieldDescription.FieldType.BoxEnd);
						dst.Add(boxEnd);  // met le BoxEnd pour terminer la bo�te SubForm
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
			//	Retourne une liste d�velopp�e qui ne contient plus de noeuds.
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
			//	Retourne l'index de l'�l�ment utilisant un Guid donn�.
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
