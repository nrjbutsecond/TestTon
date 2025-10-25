using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Helper
{
    public static class SlugHelper
    {
        public static string GenerateSlug(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            // Convert to lowercase
            var slug = text.ToLowerInvariant();

            // Remove accents
            slug = RemoveAccents(slug);

            // Replace spaces with hyphens
            slug = Regex.Replace(slug, @"\s+", "-");

            // Remove invalid characters
            slug = Regex.Replace(slug, @"[^a-z0-9\-]", "");

            // Remove multiple hyphens
            slug = Regex.Replace(slug, @"\-+", "-");

            // Trim hyphens from start and end
            slug = slug.Trim('-');

            return slug;
        }

        private static string RemoveAccents(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public static string EnsureUniqueSlug(string baseSlug, Func<string, bool> checkExists)
        {
            var slug = baseSlug;
            var counter = 1;

            while (checkExists(slug))
            {
                slug = $"{baseSlug}-{counter}";
                counter++;
            }

            return slug;
        }
    }
}