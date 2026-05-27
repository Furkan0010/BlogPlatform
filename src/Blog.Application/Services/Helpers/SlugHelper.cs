using System.Text;
using System.Text.RegularExpressions;

namespace Blog.Application.Helpers;

public static class SlugHelper
{
    public static string Generate(string title)
    {
        if (string.IsNullOrWhiteSpace(title)) return string.Empty;

        // Türkçe karakterleri çevir
        var slug = title.ToLowerInvariant()
            .Replace("ç", "c").Replace("ğ", "g").Replace("ı", "i")
            .Replace("ö", "o").Replace("ş", "s").Replace("ü", "u");

        // Boşlukları tire yap, geçersiz karakterleri sil
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", " ").Trim();
        slug = Regex.Replace(slug, @"\s", "-");

        return slug;
    }
}