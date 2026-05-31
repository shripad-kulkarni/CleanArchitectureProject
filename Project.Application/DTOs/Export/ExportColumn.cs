namespace Project.Application.DTOs.Export
{
    public record ExportColumn<TRow>(
        string Header,
        Func<TRow, int, string> ValueSelector,
        float RelativeWidth = 1f,
        float? ConstantWidth = null);
}
