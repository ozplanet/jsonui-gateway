using System.Text;

namespace JsonUi.Ui.Theming;

public static class ThemeGenerator
{
    public static string GenerateCss(ThemeModel theme)
    {
        var sb = new StringBuilder();
        sb.AppendLine(":root {");
        sb.AppendLine($"  --color-primary: {theme.Primary};");
        sb.AppendLine($"  --color-primary-accent: {theme.PrimaryAccent};");
        sb.AppendLine($"  --color-surface: {theme.Surface};");
        sb.AppendLine($"  --color-surface-alt: {theme.SurfaceAlt};");
        sb.AppendLine($"  --color-text: {theme.Text};");
        sb.AppendLine($"  --color-muted: {theme.Muted};");
        sb.AppendLine($"  --color-success: {theme.Success};");
        sb.AppendLine($"  --color-danger: {theme.Danger};");
        sb.AppendLine($"  --color-warning: {theme.Warning};");
        sb.AppendLine("}");
        return sb.ToString();
    }
}
