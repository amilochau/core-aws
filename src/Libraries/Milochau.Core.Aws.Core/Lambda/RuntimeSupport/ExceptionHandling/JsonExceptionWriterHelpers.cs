﻿using System.Text;

namespace Milochau.Core.Aws.Core.Lambda.RuntimeSupport.ExceptionHandling
{
    internal class JsonExceptionWriterHelpers
    {
        /// <summary>
        /// This method escapes a string for use as a JSON string value.
        /// It was adapted from the PutString method in the LitJson.JsonWriter class.
        ///
        /// TODO: rewrite the *JsonExceptionWriter classes to use a JSON library instead of building strings directly.
        /// @todo
        /// </summary>
        public static string? EscapeStringForJson(string? str)
        {
            if (str == null)
                return null;

            int n = str.Length;
            var sb = new StringBuilder(n * 2);
            for (int i = 0; i < n; i++)
            {
                char c = str[i];
                switch (c)
                {
                    case '\n':
                        sb.Append(@"\n");
                        break;

                    case '\r':
                        sb.Append(@"\r");
                        break;

                    case '\t':
                        sb.Append(@"\t");
                        break;

                    case '"':
                        sb.Append(@"\""");
                        break;

                    case '\\':
                        sb.Append(@"\\");
                        break;

                    case '\f':
                        sb.Append(@"\f");
                        break;

                    case '\b':
                        sb.Append(@"\b");
                        break;

                    case '\u0085': // Next Line
                        sb.Append(@"\u0085");
                        break;

                    case '\u2028': // Line Separator
                        sb.Append(@"\u2028");
                        break;

                    case '\u2029': // Paragraph Separator
                        sb.Append(@"\u2029");
                        break;

                    default:
                        if (c < ' ')
                        {
                            // Turn into a \uXXXX sequence
                            sb.Append(@"\u");
                            sb.Append(IntToHex((int)c));
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                }
            }
            return sb.ToString().Trim();
        }

        private static char[] IntToHex(int n)
        {
            int num;
            char[] hex = new char[4];

            for (int i = 0; i < 4; i++)
            {
                num = n % 16;

                if (num < 10)
                    hex[3 - i] = (char)('0' + num);
                else
                    hex[3 - i] = (char)('A' + (num - 10));

                n >>= 4;
            }
            return hex;
        }
    }
}
