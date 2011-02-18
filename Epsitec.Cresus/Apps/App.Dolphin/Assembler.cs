//	Copyright © 2003-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

namespace Epsitec.App.Dolphin
{
	/// <summary>
	/// Assembleur CALM selon le principe bien connu des deux passes.
	/// La 1ère passe récolte les définitions de variables et d'étiquettes dans un dictionnaire.
	/// La 2ème passe génère le code.
	/// </summary>
	public class Assembler
	{
		public Assembler(Components.AbstractProcessor processor, Components.Memory memory)
		{
			this.processor = processor;
			this.memory = memory;
		}

		public bool Action(TextFieldMulti field, Window window, bool verbose)
		{
			//	Assemble les instructions CALM du programme.
			this.field = field;
			this.window = window;

			this.memory.ClearRam();

			//	Enlève les éventuelles erreurs précédentes.
			this.RemoveErrors();

			string[] seps = {"<br/>"};
			string[] lines = this.field.Text.Split(seps, System.StringSplitOptions.None);

			int instructionCounter = 0;
			int byteCounter = 0;

			//	Ces 2 listes stockent toutes les erreurs rencontrées.
			List<int> errorLines = new List<int>();
			List<string> errorTexts = new List<string>();

			//	Crée le dictionnaire principal pour les variables et étiquettes.
			Dictionary<string, int> variables = new Dictionary<string, int>();
			this.processor.RomVariables(Components.Memory.RomBase, variables);
			this.memory.RomVariables(variables);

			//	Longueurs des instructions, construite lors de la 1ère passe et utilisées à la 2ème.
			List<int> instructionLengths = new List<int>();

			//	Effectue les deux passes.
			for (int pass=0; pass<2; pass++)
			{
				this.DoPass(lines, pass, variables, instructionLengths, errorLines, errorTexts, ref instructionCounter, ref byteCounter);

				if (errorLines.Count != 0)
				{
					break;  // si erreur pendant la 1ère passe -> pas de 2ème
				}
			}

			//	Dialogue selon le déroulement de l'assemblage.
			string icon, message;
			if (errorLines.Count == 0)
			{
				if (instructionCounter == 0)
				{
					icon = "manifest:Epsitec.Common.Dialogs.Images.Warning.icon";
					message = Res.Strings.Assembler.Message.Empty;
				}
				else
				{
					icon = "manifest:Epsitec.Common.Dialogs.Images.Information.icon";
					message = string.Format(Res.Strings.Assembler.Message.OK, instructionCounter.ToString(), byteCounter.ToString());
				}
			}
			else
			{
				this.InsertErrors(errorLines, errorTexts);

				icon = "manifest:Epsitec.Common.Dialogs.Images.Warning.icon";
				message = string.Format(Res.Strings.Assembler.Message.Error, errorLines.Count.ToString());
			}

			if (verbose || errorLines.Count != 0)
			{
				string title = TextLayout.ConvertToSimpleText(Res.Strings.Window.Title);
				Common.Dialogs.IDialog dialog = Common.Dialogs.MessageDialog.CreateOk(title, icon, message, null, null);
				dialog.OwnerWindow = this.window;
				dialog.OpenDialog();
			}

			return errorLines.Count == 0;
		}


		protected void DoPass(string[] lines, int pass, Dictionary<string, int> variables, List<int> instructionLengths, List<int> errorLines, List<string> errorTexts, ref int instructionCounter, ref int byteCounter)
		{
			//	Première ou deuxième passe de l'assemblage (pass = 0..1).
			//	La 1ère passe récolte les définitions de variables et d'étiquettes dans un dictionnaire.
			//	La 2ème passe génère le code.
			int pc = Components.Memory.RamBase;  // on place toujours le code au début de la RAM
			int lineCounter = 0;
			this.ending = false;

			foreach (string line in lines)
			{
				string instruction = this.ReplaceCharacter(line);
				instruction = this.RemoveComment(instruction);
				string err;

				if (this.ProcessPseudo(instruction, pass, variables, ref pc, out err))
				{
					if (err == null)
					{
						if (this.ending)
						{
							break;
						}
					}
					else
					{
						errorLines.Add(lineCounter);
						errorTexts.Add(err);
					}
				}
				else
				{
					instruction = this.ProcessVariables(instruction, pass, pc, variables, out err);
					if (err == null)
					{
						if (!string.IsNullOrEmpty(instruction))
						{
							int npc = pc;  // npc est faux à la 1ère passe, mais c'est sans importance !
							if (pass == 1 && instructionCounter < instructionLengths.Count)  // deuxième passe ?
							{
								npc += instructionLengths[instructionCounter];  // npc = adresse après l'instruction, pour adressage relatif
							}

							instruction = this.processor.AssemblyPreprocess(instruction);
							instruction = this.PrepareInstruction(instruction, pass, npc, variables, out err);
							if (err == null)
							{
								if (!string.IsNullOrEmpty(instruction))
								{
									List<int> codes = new List<int>();
									err = this.processor.AssemblyInstruction(instruction, codes);

									if (codes.Count == 0)
									{
										if (pass == 1)  // deuxième passe ?
										{
											if (!string.IsNullOrEmpty(err))
											{
												errorLines.Add(lineCounter);
												errorTexts.Add(Misc.RemoveTags(Assembler.FirstLine(err)));
											}
										}
									}
									else
									{
										if (pass == 0)  // première passe ?
										{
											pc += codes.Count;
											instructionLengths.Add(codes.Count);
										}
										else  // deuxième passe ?
										{
											foreach (int code in codes)
											{
												this.memory.Write(pc++, code);
												byteCounter++;
											}
											instructionCounter++;
										}
									}
								}
							}
							else
							{
								errorLines.Add(lineCounter);
								errorTexts.Add(Misc.RemoveTags(err));
							}
						}
					}
					else
					{
						errorLines.Add(lineCounter);
						errorTexts.Add(Misc.RemoveTags(err));
					}
				}
				
				lineCounter++;
			}
		}

		protected string ReplaceCharacter(string instruction)
		{
			//	Remplace les caractères par des valeurs simples.
			//	Par exemple, 'move #"A",X' est remplacé par 'move #H'41,X'.
			instruction = TextLayout.ConvertToSimpleText(instruction);

			//	Remplace les apostrophes et guillemets typographiques par
			//	leur variante simple :
			instruction = instruction.Replace ((char) 0x2019, '\'');
			instruction = instruction.Replace ((char) 0x201C, '\"');
			instruction = instruction.Replace ((char) 0x201D, '\"');

			if (instruction.IndexOf('"') == -1)
			{
				return instruction;
			}

			System.Text.StringBuilder builder = new System.Text.StringBuilder();

			int i = 0;
			while (i<instruction.Length)
			{
				if (i<instruction.Length-2 && instruction[i] == '"' && instruction[i+2] == '"')
				{
					int n = (int) instruction[i+1];
					builder.Append("H'");
					builder.Append(n.ToString("X2"));
					i += 3;
					continue;
				}

				if (i<instruction.Length-3 && instruction[i] == '"' && instruction[i+1] == '\\' && instruction[i+3] == '"')
				{
					int n = (int) instruction[i+2];
					builder.Append("H'");
					builder.Append(n.ToString("X2"));
					i += 4;
					continue;
				}

				builder.Append(instruction[i++]);
			}

			return builder.ToString();
		}

		protected string RemoveComment(string instruction)
		{
			//	Supprime l'éventuel commentaire "; blabla" en fin de ligne.
			instruction = instruction.ToUpper().Trim();

			int index = instruction.IndexOf(";");  // point-virgule: commentaire à la fin de la ligne ?
			if (index != -1)
			{
				instruction = instruction.Substring(0, index);  // enlève le commentaire
			}
			
			index = instruction.IndexOf("\\");  // boa: commentaire à la fin de la ligne ?
			if (index != -1)
			{
				instruction = instruction.Substring(0, index);  // enlève le commentaire
			}
			
			index = instruction.IndexOf("//");  // double slash (style C): commentaire à la fin de la ligne ?
			if (index != -1)
			{
				instruction = instruction.Substring(0, index);  // enlève le commentaire
			}
			
			return instruction;
		}

		protected bool ProcessPseudo(string instruction, int pass, Dictionary<string, int> variables, ref int pc, out string err)
		{
			//	Traite les pseudos-instructions .TITLE, .LOC, etc.
			//	Retourne true si une pseudo a été traitée.
			err = null;

			if (!instruction.StartsWith("."))
			{
				return false;
			}

			string[] seps = {" ", "\t"};
			string[] words = instruction.Split(seps, System.StringSplitOptions.RemoveEmptyEntries);

			switch (words[0])
			{
				case ".TITLE":
					if (words.Length == 1)
					{
						err = Res.Strings.Assembler.Pseudo.Error.Title;
					}
					break;

				case ".LOC":
					if (words.Length == 2)
					{
						int value = this.Expression(words[1], pass, true, variables, out err);
						if (err == null)
						{
							pc = value;
						}
					}
					else
					{
						err = Res.Strings.Assembler.Pseudo.Error.Loc;
					}
					break;

				case ".END":
					if (words.Length == 1)
					{
						this.ending = true;
					}
					else
					{
						err = Res.Strings.Assembler.Pseudo.Error.End;
					}
					break;

				default:
					err = Res.Strings.Assembler.Pseudo.Error.Default;
					break;
			}

			return true;
		}

		protected string ProcessVariables(string instruction, int pass, int pc, Dictionary<string, int> variables, out string err)
		{
			//	Traite les variables dans une instruction.
			//	Retourne l'instruction expurgée des variables.
			//	Gère les définitions de variables du genre "TOTO = 12*TITI", retourne NULL.
			//	Gère les étiquettes, du genre "LOOP: MOVE A,B", retourne "MOVE A,B".
			string[] defs = instruction.Split('=');
			if (defs.Length == 2)  // variable = expression ?
			{
				if (pass == 0)  // première passe ?
				{
					string variable   = defs[0].Trim();
					string expression = defs[1].Trim();

					if (this.IsRegister(variable))
					{
						err = Res.Strings.Assembler.Var.Error.Register;
					}
					else if (!this.IsVariable(variable))
					{
						err = Res.Strings.Assembler.Var.Error.Name;
					}
					else if (variables.ContainsKey(variable))
					{
						err = Res.Strings.Assembler.Var.Error.Redefine;
					}
					else
					{
						int value = this.Expression(expression, pass, false, variables, out err);
						if (err == null)
						{
							variables.Add(variable, value);
						}
					}
				}
				else  // deuxième passe ?
				{
					err = null;
				}
				return null;  // ce n'est pas une instruction
			}

			//	Gère les définitions d'étiquettes, du genre "LOOP: MOVE A,B".
			int index = instruction.IndexOf(":");
			if (index != -1)
			{
				string label = instruction.Substring(0, index).Trim();
				if (!string.IsNullOrEmpty(label) && label.IndexOf(" ") == -1 && label.IndexOf("\t") == -1)
				{
					if (pass == 0)  // première passe ?
					{
						if (this.IsRegister(label))
						{
							err = Res.Strings.Assembler.Var.Error.NoRegister;
							return null;
						}
						else if (!this.IsVariable(label))
						{
							err = Res.Strings.Assembler.Var.Error.WrongLabel;
							return null;
						}
						else if (variables.ContainsKey(label))
						{
							err = Res.Strings.Assembler.Var.Error.LabelRedefine;
							return null;
						}
						else
						{
							variables.Add(label, pc);
						}
					}

					instruction = instruction.Substring(index+1);  // enlève le "label:" au début
				}
			}

			err = null;
			return instruction;
		}

		protected string PrepareInstruction(string instruction, int pass, int npc, Dictionary<string, int> variables, out string err)
		{
			//	Prépare une instruction pour l'assemblage.
			//	Effectue les substitutions dans les arguments en fonction des variables.
			//	Par exemple, remplace "MOVE #TOTO+1,A" par "MOVE H'12 A", si TOTO=H'11.
			//	Par exemple, remplace "MOVE {SP}+TITI,A" par "MOVE {SP}+H'02 A", si TITI=H'02.
			string[] seps = {" "};
			string[] words = instruction.Split(seps, System.StringSplitOptions.RemoveEmptyEntries);

			if (words.Length == 0)
			{
				err = null;
				return null;
			}

			if (words.Length >= 2)
			{
				for (int i=1; i<words.Length; i++)  // passe en revue les arguments
				{
					string word = words[i];
					if (string.IsNullOrEmpty(word))
					{
						continue;
					}

					if (this.IsRegister(word))  // r ?
					{
						//	Aucune substitution à effectuer s'il s'agit d'un registre.
					}
					else if (word.Length >= 1 && word[0] == '#')  // #val ?
					{
						int value = this.Expression(word.Substring(1), pass, true, variables, out err);
						if (err == null)
						{
							if (Assembler.IsByteOverflow(value))
							{
								err = Res.Strings.Assembler.Instruction.Error.Overflow;
								return null;
							}
							else
							{
								word = string.Concat("#H'", value.ToString("X3"));
							}
						}
						else
						{
							return null;
						}
					}
					else  // ADDR ?
					{
						string text;
						int mode;
						this.processor.AssemblySplitAddr(word, out text, out mode);

						if (text.StartsWith("R^"))  // adressage relatif ?
						{
							text = text.Substring(2);  // enlève le R^
							int value = this.Expression(text, pass, true, variables, out err);
							if (err == null)
							{
								value -= npc;

								if (System.Math.Abs(value) > 0x7FF)
								{
									err = Res.Strings.Assembler.Instruction.Error.RelOverflow;
									return null;
								}

								text = string.Concat("{PC}+H'", value.ToString("X3"));
							}
							else
							{
								return null;
							}
						}
						else  // adressage absolu ?
						{
							int value = this.Expression(text, pass, true, variables, out err);
							if (err == null)
							{
								text = string.Concat("H'", value.ToString("X3"));
							}
							else
							{
								return null;
							}
						}

						word = this.processor.AssemblyCombineAddr(text, mode);
					}

					words[i] = word;
				}
			}

			err = null;  // ok

			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			foreach (string word in words)
			{
				if (!string.IsNullOrEmpty(word))
				{
					builder.Append(word);
					builder.Append(" ");
				}
			}
			return builder.ToString();
		}

		protected static bool IsByteOverflow(int value)
		{
			//	Vérifie si une valeur dépasse la capacité d'un byte.
			//	Erreur d'assemblage avec MOVE #256,A ou MOVE -129,A.
			if (value >= 0)
			{
				return value > 255;
			}
			else
			{
				return value < -128;
			}
		}

		public int Expression(string expression, int pass, bool acceptUndefined, Dictionary<string, int> variables, out string err)
		{
			//	Evalue une expression pouvant contenir les 4 opérations de base et des parenthèses.
			//	Donne la priorité aux multiplications et divisions: "2*3+4*5" = "(2*3)+(4*5)" = 26.
			List<string> words = Assembler.Fragment(expression);
			List<Item> items = new List<Item>();

			foreach (string word in words)
			{
				int value = Assembler.GetValue(word, pass, acceptUndefined, variables, out err);
				if (value == Misc.undefined)
				{
					if (err != null)
					{
						return Misc.undefined;
					}

					items.Add(new Item(word, value));
				}
				else
				{
					items.Add(new Item("n", value));
				}
			}

			bool action;
			do
			{
				action = false;

				action |= this.OperUnary(items, out err);
				if (err != null)
				{
					return Misc.undefined;
				}
				
				action |= this.OperBinary(items, "*/", out err);
				if (err != null)
				{
					return Misc.undefined;
				}
				
				action |= this.OperSimplify(items, out err);
				if (err != null)
				{
					return Misc.undefined;
				}
				
				action |= this.OperBinary(items, "+-", out err);
				if (err != null)
				{
					return Misc.undefined;
				}
				
				action |= this.OperSimplify(items, out err);
				if (err != null)
				{
					return Misc.undefined;
				}
			}
			while (action);

			if (items.Count != 1 || items[0].Name != "n" || items[0].Value == Misc.undefined)
			{
				err = Res.Strings.Assembler.Expression.Error.Generic;
				return Misc.undefined;
			}

			err = null;
			return items[0].Value;
		}

		protected bool OperSimplify(List<Item> items, out string err)
		{
			//	Simplifie les "(n)" en "n".
			bool action = false;
			int i = 0;
			while (i < items.Count-2)
			{
				if (items[i+0].Name == "(" && items[i+1].Name == "n" && items[i+2].Name == ")")
				{
					items.RemoveAt(i+2);
					items.RemoveAt(i+0);
					action = true;
				}
				else
				{
					i++;
				}
			}

			err = null;
			return action;
		}

		protected bool OperUnary(List<Item> items, out string err)
		{
			//	Résoud les opérations unaires "-n" en "n".
			bool action = false;
			int i = 0;
			while (i < items.Count-1)
			{
				if ((i == 0 || "(+-*/".Contains(items[i-1].Name)) && "+-".Contains(items[i+0].Name) && items[i+1].Name == "n")
				{
					items[i] = this.OperUnary(items[i+0].Name, items[i+1].Value, out err);
					if (err != null)
					{
						return false;
					}

					items.RemoveAt(i+1);
					action = true;
				}
				else
				{
					i++;
				}
			}

			err = null;
			return action;
		}

		protected Item OperUnary(string op, int v, out string err)
		{
			//	Retourne le résultat d'une opération binaire "-n".
			err = null;

			switch (op)
			{
				case "+":
					return new Item("n", v);

				case "-":
					return new Item("n", -v);
			}

			err = Res.Strings.Assembler.Expression.Error.Unary;
			return new Item(null, Misc.undefined);
		}

		protected bool OperBinary(List<Item> items, string ops, out string err)
		{
			//	Résoud les opérations binaires "n+n" en "n".
			bool action = false;
			int i = 0;
			while (i < items.Count-2)
			{
				if (items[i+0].Name == "n" && ops.Contains(items[i+1].Name) && items[i+2].Name == "n")
				{
					items[i] = OperBinary(items[i+0].Value, items[i+1].Name, items[i+2].Value, out err);
					if (err != null)
					{
						return false;
					}

					items.RemoveAt(i+2);
					items.RemoveAt(i+1);
					action = true;
				}
				else
				{
					i++;
				}
			}

			err = null;
			return action;
		}

		protected Item OperBinary(int v1, string op, int v2, out string err)
		{
			//	Retourne le résultat d'une opération binaire "n+n".
			err = null;

			switch (op)
			{
				case "+":
					return new Item("n", v1 + v2);

				case "-":
					return new Item("n", v1 - v2);

				case "*":
					return new Item("n", v1 * v2);

				case "/":
					if (v2 == 0)
					{
						err = Res.Strings.Assembler.Expression.Error.Binary.DivZero;
						return new Item(null, Misc.undefined);
					}
					return new Item("n", v1 / v2);
			}

			err = Res.Strings.Assembler.Expression.Error.Binary.Generic;
			return new Item(null, Misc.undefined);
		}

		protected struct Item
		{
			public Item(string name, int value)
			{
				this.Name = name;
				this.Value = value;
			}

			public string Name;
			public int Value;
		}


		protected enum FragmentType
		{
			Unknow,
			Number,
			Text,
			Special,
		}

		protected static List<string> Fragment(string expression)
		{
			//	Fragmente une expression en "mots" élémentaires.
			//	Par exemple, "12+H'34*(TOTO-1)" devient "12", "+", "H'34", "*", "(", "TOTO", "-", "1", ")".
			expression = Misc.RemoveSpaces(expression);  // enlève les espaces
			List<string> words = new List<string>();

			int i = 0;
			int start = 0;
			FragmentType type = FragmentType.Unknow;
			while (i < expression.Length)
			{
				char c = expression[i++];

				FragmentType newType;
				if (c >= '0' && c <= '9')
				{
					if (type == FragmentType.Text)
					{
						newType = FragmentType.Text;
					}
					else
					{
						newType = FragmentType.Number;
					}
				}
				else if (i < expression.Length && expression[i] == '\'')  // "X'" ?
				{
					newType = FragmentType.Number;
				}
				else if (c == '\'')
				{
					newType = FragmentType.Number;
				}
				else if (c >= 'A' && c <= 'Z')
				{
					if (type == FragmentType.Number)
					{
						newType = FragmentType.Number;
					}
					else
					{
						newType = FragmentType.Text;
					}
				}
				else if (c == '_')
				{
					newType = FragmentType.Text;
				}
				else
				{
					newType = FragmentType.Special;
				}

				if (newType != type || newType == FragmentType.Special)
				{
					if (type != FragmentType.Unknow)
					{
						string word = expression.Substring(start, i-start-1).Trim();
						if (!string.IsNullOrEmpty(word))
						{
							words.Add(word);
						}
					}

					type = newType;
					start = i-1;
				}
			}

			if (type != FragmentType.Unknow)
			{
				string word = expression.Substring(start, i-start).Trim();
				if (!string.IsNullOrEmpty(word))
				{
					words.Add(word);
				}
			}

			return words;
		}

		protected static int GetValue(string word, int pass, bool acceptUndefined, Dictionary<string, int> variables, out string err)
		{
			//	Cherche une valeur, qui peut être soit une constante, soit une variable.
			int value = Assembler.GetNumber(word, out err);
			
			if (err != null)
			{
				return Misc.undefined;
			}

			if (value == Misc.undefined)  // pas une constante ?
			{
				string variable = Assembler.GetString(word);
				if (variable == null)  // pas une variable ?
				{
					err = null;
					return Misc.undefined;
				}

				if (variables.ContainsKey(variable))  // variable définie ?
				{
					value = variables[variable];  // prend la valeur de la variable
				}
				else
				{
					if (pass == 0 && acceptUndefined)  // première passe ?
					{
						value = 0;  // valeur quelconque, juste pour assembler une instruction avec le bon nombre de bytes
					}
					else  // deuxième passe ?
					{
						err = string.Format(Res.Strings.Assembler.Value.Error.Undefined, variable);
						return Misc.undefined;
					}
				}
			}
			err = null;  // ok
			return value;
		}

		protected static int GetNumber(string word, out string err)
		{
			//	Cherche une constante.
			if (word.Length == 0)
			{
				err = null;
				return Misc.undefined;
			}

			if (word[0] >= '0' && word[0] <= '9')  // décimal par défaut ?
			{
				int value;
				if (!int.TryParse(word, out value))
				{
					err = Res.Strings.Assembler.Number.Error.Decimal;
					return Misc.undefined;
				}
				err = null;
				return value;
			}

			if (word.Length > 1 && word[1] == '\'')
			{
				if (word[0] == 'H')  // hexadécimal ?
				{
					int value = Misc.ParseHexa(word.Substring(2), Misc.undefined, Misc.undefined);
					if (value == Misc.undefined)
					{
						err = Res.Strings.Assembler.Number.Error.Hexa;
						return Misc.undefined;
					}
					err = null;
					return value;
				}
				else if (word[0] == 'B')  // binaire ?
				{
					int value = Misc.ParseBin(word.Substring(2), Misc.undefined, Misc.undefined);
					if (value == Misc.undefined)
					{
						err = Res.Strings.Assembler.Number.Error.Binary;
						return Misc.undefined;
					}
					err = null;
					return value;
				}
				else if (word[0] == 'D')  // décimal ?
				{
					int value;
					if (!int.TryParse(word.Substring(2), out value))
					{
						err = Res.Strings.Assembler.Number.Error.Decimal;
						return Misc.undefined;
					}
					err = null;
					return value;
				}
				else
				{
					err = Res.Strings.Assembler.Number.Error.Generic;
					return Misc.undefined;
				}
			}

			err = null;
			return Misc.undefined;
		}

		protected static string GetString(string word)
		{
			//	Cherche une chaîne (un nom de variable ou une opération).
			if (word.Length == 0)
			{
				return null;
			}

			if (word[0] >= 'A' && word[0] <= 'Z')
			{
				return word;
			}

			if (word[0] == '_')
			{
				return word;
			}

			return null;
		}

		protected bool IsRegister(string word)
		{
			//	Indique si le mot correspond à un nom de registre du processeur.
			foreach (string register in this.processor.RegisterNames)
			{
				if (word == register)
				{
					return true;
				}
			}
			return false;
		}

		protected bool IsVariable(string word)
		{
			//	Indique si le mot correspond à un nom de variable valide.
			//	Exemples valides: "TOTO", "TOTO23", "TOTO_2", "_TOTO", "A1000"
			//	Exemples invalides: "2TOTO", "TOTO 2"
			if (word.Length == 0)
			{
				return false;
			}

			if ((word[0] < 'A' || word[0] > 'Z') && word[0] != '_')
			{
				return false;
			}

			foreach (char c in word)
			{
				if (word[0] >= '0' && word[0] <= '9')
				{
					continue;
				}
				else if (word[0] >= 'A' && word[0] <= 'Z')
				{
					continue;
				}
				else if (word[0] == '_')
				{
					continue;
				}
				else
				{
					return false;
				}
			}

			return true;
		}


		protected void RemoveErrors()
		{
			//	Supprime toutes les erreurs.
			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			string program = this.field.Text;

			string[] seps = {"<br/>"};
			string[] lines = program.Split(seps, System.StringSplitOptions.None);

			int total = lines.Length;
			for (int i=lines.Length-1; i>=0; i--)
			{
				if (string.IsNullOrEmpty(lines[i]))
				{
					total--;  // supprime les lignes vides à la fin
				}
				else
				{
					break;
				}
			}

			for (int i=0; i<total; i++)
			{
				string line = lines[i];

				string simply = Misc.RemoveTags(line).Trim();
				if (!simply.StartsWith("^ "))  // erreur dans cette ligne ?
				{
					builder.Append(line);
					builder.Append("<br/>");
				}
			}

			this.field.Text = builder.ToString();
		}

		protected void InsertErrors(List<int> errorLines, List<string> errorTexts)
		{
			//	Insère les erreurs dans le texte du programme.
			string program = TextLayout.ConvertToSimpleText(this.field.Text);  // remplace <br/> par \n et <tab/> par \t

			int cursor = 0;
			for (int i=errorLines.Count-1; i>=0; i--)  // commence par la fin pour ne pas perturber les rangs des lignes
			{
				int index1 = Assembler.LineIndex(program, errorLines[i]+0);
				int index2 = Assembler.LineIndex(program, errorLines[i]+1);

				string balast = Assembler.Balast(program.Substring(index1));
				string err = string.Concat(balast, "^ ", TextLayout.ConvertToSimpleText(errorTexts[i]), "\n");
				program = program.Insert(index2, err);

				cursor = index2-1;
			}

			this.field.Text = TextLayout.ConvertToTaggedText(program);  // remplace \n par <br/> et \t par <tab/>
			this.field.Cursor = cursor;
		}


		protected static string Balast(string text)
		{
			//	Retourne tout le balast présent au début d'une ligne.
			//	TextLayout.ConvertToSimpleText doit avoir été exécuté (<br/> et <tab/> remplacés par \n et \t).
			int index = 0;
			while (true)
			{
				int i;

				i = text.IndexOf(" ", index);
				if (i == index)
				{
					index = i+1;
					continue;
				}

				i = text.IndexOf(",", index);
				if (i == index)
				{
					index = i+1;
					continue;
				}

				i = text.IndexOf(":", index);
				if (i == index)
				{
					index = i+1;
					continue;
				}

				i = text.IndexOf("\t", index);
				if (i == index)
				{
					index = i+1;
					continue;
				}

				break;
			}

			return text.Substring(0, index);
		}

		protected static int LineIndex(string text, int lineRank)
		{
			//	Cherche l'index de la nième ligne d'un texte.
			//	TextLayout.ConvertToSimpleText doit avoir été exécuté (<br/> et <tab/> remplacés par \n et \t).
			if (lineRank == 0)
			{
				return 0;
			}

			int startIndex = 0;
			while (true)
			{
				int index = text.IndexOf("\n", startIndex);
				if (index == -1)
				{
					return text.Length;
				}
				startIndex = index+1;

				if (--lineRank == 0)
				{
					return startIndex;
				}
			}
		}

		protected static string FirstLine(string err)
		{
			//	Retourne la première ligne d'un texte.
			int index = err.IndexOf("<br/>");
			if (index == -1)
			{
				return err;
			}
			else
			{
				return err.Substring(0, index);
			}
		}


		protected static readonly int undefined = int.MinValue;

		protected Components.AbstractProcessor	processor;
		protected Components.Memory				memory;
		protected Window						window;
		protected TextFieldMulti				field;
		protected string						filename;
		protected bool							ending;
	}
}