using System;
using System.Text;
using System.Text.RegularExpressions;

namespace GraphQL.SchemaGenerator.Helpers
{
    public static class StringHelper
    {
        /// <summary>
        ///     Convert to camel case. ExampleWord -> exampleWord.
        /// </summary>
        public static string ConvertToCamelCase(string name)
        {
            if (name == null || name.Length <= 1)
            {
                return name;
            }

            return Char.ToLower(name[0]) + name.Substring(1);
        }

        /// <summary>
        ///     Remove special characters. Example@!#$23 -> Example23
        /// </summary>
        public static string SafeString(string name)
        {
            if (name == null)
            {
                return null;
            }

            return Regex.Replace(name, "[^0-9a-zA-Z]+", "");
        }

        /// <summary>
        ///     Get the graph name for this string.
        /// </summary>
        /// <returns>Camel case, safe string.</returns>
        public static string GraphName(string name)
        {
            return SafeString(ConvertToCamelCase(name));
        }

        /// <summary>
        ///     Get the real type name.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string GetRealTypeName(Type t)
        {
            if (!t.IsGenericType)
                return SafeString(t.Name);

            StringBuilder sb = new StringBuilder();
            sb.Append(SafeString(t.Name.Substring(0, t.Name.IndexOf('`'))));
            sb.Append("__");
            bool appendComma = false;
            foreach (Type arg in t.GetGenericArguments())
            {
                if (appendComma) sb.Append('_');
                sb.Append(GetRealTypeName(arg));
                appendComma = true;
            }
            return sb.ToString();
        }
    }
}
