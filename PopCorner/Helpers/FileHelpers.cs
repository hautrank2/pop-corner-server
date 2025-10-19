namespace PopCorner.Helpers
{
    public class FileHelpers
    {
        public static string SanitizeFolder(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            var normalized = input.Replace('\\', '/').Trim().Trim('/');

            if (normalized.Contains("..") || Path.IsPathRooted(normalized))
                return string.Empty;

            var invalid = Path.GetInvalidFileNameChars().ToHashSet();
            var segments = normalized
                .Split('/', StringSplitOptions.RemoveEmptyEntries)
                .Select(seg => new string(seg.Where(ch => !invalid.Contains(ch)).ToArray()))
                .Where(seg => !string.IsNullOrWhiteSpace(seg));

            return string.Join(Path.DirectorySeparatorChar, segments);
        }
    }
}
