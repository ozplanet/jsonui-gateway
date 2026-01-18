namespace JsonUi.Ui.Theming;

public sealed record ThemeModel(
    string Primary,
    string PrimaryAccent,
    string Surface,
    string SurfaceAlt,
    string Text,
    string Muted,
    string Success,
    string Danger,
    string Warning)
{
    public static ThemeModel Default() => new(
        Primary: "#ff7a18",
        PrimaryAccent: "#ffa94d",
        Surface: "#121212",
        SurfaceAlt: "#1f1f1f",
        Text: "#f5f5f5",
        Muted: "#a5a5a5",
        Success: "#37b26c",
        Danger: "#ff4f5e",
        Warning: "#ffc857");
}
