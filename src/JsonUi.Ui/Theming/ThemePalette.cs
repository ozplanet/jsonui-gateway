namespace JsonUi.Ui.Theming;

public static class ThemePalette
{
    public static IReadOnlyList<ThemeModel> Presets { get; } = new List<ThemeModel>
    {
        ThemeModel.Default(),
        new("#7a5cff", "#af8cff", "#101223", "#1a2033", "#f2f2ff", "#9fa2c7", "#48E1A5", "#FF5B8F", "#FFD480"),
        new("#1ccad8", "#46e9ff", "#0e141b", "#16222d", "#e8f9ff", "#9bb4c5", "#7CF49A", "#FF6B6B", "#FFD93D"),
    };
}
