using System.Text.RegularExpressions;

namespace GrapeDapper.SqlAdapter
{
    public class PagingHelper : IPagingHelper
    {
        public Regex RegexColumns = new Regex(@"\A\s*SELECT\s+((?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|.)*?)(?<!,\s+)\bFROM\b",
            RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

        public Regex RegexDistinct = new Regex(@"\ADISTINCT\s",
            RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

        public Regex RegexOrderBy =
            new Regex(
                @"\bORDER\s+BY\s+(?!.*?(?:\)|\s+)AS\s)(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\w\(\)\.])+(?:\s+(?:ASC|DESC))?(?:\s*,\s*(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\w\(\)\.])+(?:\s+(?:ASC|DESC))?)*",
                RegexOptions.RightToLeft | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

        public static IPagingHelper Instance { get; private set; }

        static PagingHelper()
        {
            Instance = new PagingHelper();
        }

        public bool SplitSQL(string sql, out SQLParts parts)
        {
            parts.Sql = sql;
            parts.SqlSelectRemoved = null;
            parts.SqlCount = null;
            parts.SqlOrderBy = null;

            var m = RegexColumns.Match(sql);
            if (!m.Success)
                return false;

            Group g = m.Groups[1];
            parts.SqlSelectRemoved = sql.Substring(g.Index);

            if (RegexDistinct.IsMatch(parts.SqlSelectRemoved))
                parts.SqlCount = sql.Substring(0, g.Index) + "COUNT(" + m.Groups[1].ToString().Trim() + ") " + sql.Substring(g.Index + g.Length);
            else
                parts.SqlCount = sql.Substring(0, g.Index) + "COUNT(*) " + sql.Substring(g.Index + g.Length);

            m = RegexOrderBy.Match(parts.SqlCount);
            if (!m.Success)
            {
                parts.SqlOrderBy = null;
            }
            else
            {
                g = m.Groups[0];
                parts.SqlOrderBy = g.ToString();
                parts.SqlCount = parts.SqlCount.Substring(0, g.Index) + parts.SqlCount.Substring(g.Index + g.Length);
            }

            return true;
        }
    }
}