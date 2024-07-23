/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


using System.Text.RegularExpressions;

namespace Epsitec.Common.Support
{
    public enum PredefinedRegex
    {
        None,

        Alpha,
        AlphaNum,
        AlphaNumDot,

        FileName,
        PathName,

        ResourceFullName, //	"abc123#x.y3.z"
        ResourceBundleName, //	"abc"
        ResourceFieldName, //	"x.y3.z"

        PascalCaseSymbol, //	"AbcDef"

        InvariantDecimalNum,
        LocalizedDecimalNum
    }

    /// <summary>
    /// La classe RegexFactory permet de construire des objets "regex" à partir de
    /// textes simples avec des jokers "*"...
    /// </summary>
    public class RegexFactory
    {
        private RegexFactory() { }

        static RegexFactory()
        {
            RegexOptions options = RegexOptions.Compiled | RegexOptions.ExplicitCapture;

            RegexFactory.alphaName = new Regex(@"^[a-zA-Z]+$", options);
            RegexFactory.alphaNumName = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$", options);
            RegexFactory.alphaNumDotName1 = new Regex(
                @"^[a-zA-Z_]([a-zA-Z0-9_]*((?![\.]$)(?<X>[\.])(?!\k<X>))*)*$",
                options
            );
            RegexFactory.alphaNumDotName2 = new Regex(
                @"^[a-zA-Z0-9_]([a-zA-Z0-9_]*((?![\.]$)(?<X>[\.])(?!\k<X>))*)*$",
                options
            );
            RegexFactory.fileName = new Regex(
                @"^[a-zA-Z0-9_\""\'\$\+\-\=\@\&\(\)\!]([a-zA-Z0-9_\""\'\$\+\-\=\@\&\(\)\!]*((?![\. ]$)(?<X>[\. ])(?!\k<X>))*)*$",
                options
            );
            RegexFactory.pathName = new Regex(
                @"^[a-zA-Z0-9_\""\'\$\+\-\=\@\&\(\)\!]([a-zA-Z0-9_\""\'\$\+\-\=\@\&\(\)\!]*((?![\.\/\ ]$)(?<X>[\.\/\ ])(?!\k<X>))*)*$",
                options
            );
            RegexFactory.rFullName = new Regex(
                @"^([a-zA-Z_][a-zA-Z0-9_]*)(\.([a-zA-Z0-9_]+))*"
                    + @"(\#([a-zA-Z_][a-zA-Z0-9_]*)(\.([a-zA-Z0-9_]+))*)*"
                    + @"(\[[0-9]{1,4}\])?"
                    + @"$",
                options
            );
            RegexFactory.rBundleName = new Regex(
                @"^([a-zA-Z_][a-zA-Z0-9_]*)(\.([a-zA-Z0-9_]+))*$",
                options
            );
            RegexFactory.rFieldName = new Regex(
                @"^([a-zA-Z_][a-zA-Z0-9_]*)(\.([a-zA-Z0-9_]+))*$",
                options
            );

            RegexFactory.pascalCaseSymbol = new Regex(
                @"^[A-ZÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÑÒÓÔÕÖŒÙÚÛÜÝ][a-zßàáâãäåæçèéêëíîïñòóôõöœùúûüýÿA-ZÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÑÒÓÔÕÖŒÙÚÛÜÝ0-9]*$",
                options
            );

            //	TODO: recalculer l'expression régulière en fonction de la culture

            //	L'expression régulière utilisée pour déterminer si un nombre est formaté correctement
            //	devrait être recalculée chaque fois que la culture active change, mais on ne le fait
            //	pas encore :

            char decimalSeparator;

            decimalSeparator = System
                .Globalization
                .CultureInfo
                .CurrentCulture
                .NumberFormat
                .NumberDecimalSeparator[0];

            //	TODO: regénérer locDecimalNum à chaque changement de culture

            RegexFactory.locDecimalNum = new Regex(
                @"^(\-|\+)?((\d{1,12}(\"
                    + decimalSeparator
                    +
                    /* */@"\d{0,12})?0*)|(\d{0,12}\"
                    + decimalSeparator
                    +
                    /* */@"(\d{0,12})?0*))$",
                options
            );

            decimalSeparator = System
                .Globalization
                .CultureInfo
                .InvariantCulture
                .NumberFormat
                .NumberDecimalSeparator[0];

            RegexFactory.invDecimalNum = new Regex(
                @"^(\-|\+)?((\d{1,12}(\"
                    + decimalSeparator
                    +
                    /* */@"\d{0,12})?0*)|(\d{0,12}\"
                    + decimalSeparator
                    +
                    /* */@"(\d{0,12})?0*))$",
                options
            );
        }

        public static Regex FromSimpleJoker(string pattern)
        {
            return RegexFactory.FromSimpleJoker(pattern, Options.None);
        }

        public static Regex FromSimpleJoker(string pattern, Options options)
        {
            pattern = (pattern ?? "").Trim();

            if (pattern.Length == 0)
            {
                throw new System.ArgumentException("Empty pattern specified");
            }

            RegexOptions regexOptions = RegexOptions.ExplicitCapture;
            System.Text.StringBuilder regexPattern = new System.Text.StringBuilder();

            bool escape = false;
            bool capture = (options & Options.Capture) != 0;
            int captureId = 1;

            if ((options & Options.IgnoreCase) != 0)
                regexOptions |= RegexOptions.IgnoreCase;
            if ((options & Options.Compiled) != 0)
                regexOptions |= RegexOptions.Compiled;

            regexPattern.Append(@"\A(("); //	force ancrage au début

            for (int i = 0; i < pattern.Length; i++)
            {
                char c = pattern[i];

                if (escape)
                {
                    regexPattern.Append(Regex.Escape(pattern.Substring(i, 1)));
                    escape = false;
                }
                else if (c == '\\')
                {
                    escape = true;
                }
                else if (c == '*')
                {
                    if (capture)
                    {
                        regexPattern.Append(@"(?<"); //	groupe nommé..
                        regexPattern.Append(captureId++); //	..avec comme nom le range 'captureId'..
                        regexPattern.Append(@">(.){0,}?)"); //	..et acceptant zero ou plus de caractère (minimum possible)
                    }
                    else
                    {
                        regexPattern.Append(@"(.){0,}?"); //	zero ou plus de caractères (minimum possible)
                    }
                }
                else if (c == '?')
                {
                    if (capture)
                    {
                        regexPattern.Append(@"(?<"); //	groupe nommé..
                        regexPattern.Append(captureId++); //	..avec comme nom le range 'captureId'..
                        regexPattern.Append(@">(.){1})"); //	..et acceptant exactement un caractère
                    }
                    else
                    {
                        regexPattern.Append(@"(.){1}"); //	exactement un caractère
                    }
                }
                else if (c == '|')
                {
                    regexPattern.Append(")|(");
                }
                else
                {
                    regexPattern.Append(Regex.Escape(pattern.Substring(i, 1)));
                }
            }

            regexPattern.Append(@"))\z"); //	force ancrage à la fin

            System.Diagnostics.Debug.WriteLine(
                string.Format("{0} --> {1}", pattern, regexPattern.ToString())
            );

            return new Regex(regexPattern.ToString(), regexOptions);
        }

        /// <summary>
        /// Escapes the specified text so that it does not conflict with any of
        /// the regex patterns.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The escaped text.</returns>
        public static string Escape(string text)
        {
            if (text == null)
            {
                return null;
            }

            var buffer = new System.Text.StringBuilder();

            foreach (char c in text)
            {
                switch (c)
                {
                    case '^':
                    case '$':
                    case '(':
                    case ')':
                    case '[':
                    case ']': //	not really required
                    case '{':
                    case '}': //	not really required
                    case '\\':
                    case '|':
                    case '.':
                    case '*':
                    case '+':
                    case '?':
                    case '<':
                    case '>':
                        buffer.Append('\\');
                        break;

                    default:
                        break;
                }

                buffer.Append(c);
            }

            return buffer.ToString();
        }

        public static Regex FromPredefinedRegex(PredefinedRegex regex)
        {
            switch (regex)
            {
                case PredefinedRegex.Alpha:
                    return RegexFactory.AlphaName;
                case PredefinedRegex.AlphaNum:
                    return RegexFactory.AlphaNumName;
                case PredefinedRegex.AlphaNumDot:
                    return RegexFactory.AlphaNumDotName1;
                case PredefinedRegex.FileName:
                    return RegexFactory.FileName;
                case PredefinedRegex.PathName:
                    return RegexFactory.PathName;
                case PredefinedRegex.ResourceFullName:
                    return RegexFactory.ResourceFullName;
                case PredefinedRegex.ResourceBundleName:
                    return RegexFactory.ResourceBundleName;
                case PredefinedRegex.ResourceFieldName:
                    return RegexFactory.ResourceFieldName;
                case PredefinedRegex.PascalCaseSymbol:
                    return RegexFactory.PascalCaseSymbol;
                case PredefinedRegex.InvariantDecimalNum:
                    return RegexFactory.InvariantDecimalNum;
                case PredefinedRegex.LocalizedDecimalNum:
                    return RegexFactory.LocalizedDecimalNum;
            }

            return null;
        }

        public static Regex AlphaName
        {
            get { return RegexFactory.alphaName; }
        }

        public static Regex AlphaNumName
        {
            get { return RegexFactory.alphaNumName; }
        }

        public static Regex AlphaNumDotName1
        {
            get { return RegexFactory.alphaNumDotName1; }
        }

        public static Regex AlphaNumDotName2
        {
            get { return RegexFactory.alphaNumDotName2; }
        }

        public static Regex PascalCaseSymbol
        {
            get { return RegexFactory.pascalCaseSymbol; }
        }

        public static Regex FileName
        {
            get { return RegexFactory.fileName; }
        }

        public static Regex PathName
        {
            get { return RegexFactory.pathName; }
        }

        public static Regex ResourceFullName
        {
            get { return RegexFactory.rFullName; }
        }

        public static Regex ResourceBundleName
        {
            get { return RegexFactory.rBundleName; }
        }

        public static Regex ResourceFieldName
        {
            get { return RegexFactory.rFieldName; }
        }

        public static Regex LocalizedDecimalNum
        {
            get { return RegexFactory.locDecimalNum; }
        }

        public static Regex InvariantDecimalNum
        {
            get { return RegexFactory.invDecimalNum; }
        }

        [System.Flags]
        public enum Options
        {
            None = 0,
            IgnoreCase = 0x0001,
            Compiled = 0x0002,
            Capture = 0x0004,
        }

        private static Regex alphaName;
        private static Regex alphaNumName;
        private static Regex alphaNumDotName1;
        private static Regex alphaNumDotName2;
        private static Regex fileName;
        private static Regex pathName;
        private static Regex rFullName;
        private static Regex rBundleName;
        private static Regex rFieldName;

        private static Regex pascalCaseSymbol;

        private static Regex locDecimalNum;
        private static Regex invDecimalNum;
    }
}
