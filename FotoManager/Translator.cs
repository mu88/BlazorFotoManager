using Microsoft.Extensions.Localization;

namespace FotoManager;

public class Translator(IStringLocalizer<Translator> localizer) : ITranslator
{
    private IStringLocalizer<Translator> Localizer { get; } = localizer;

    /// <inheritdoc />
    public string Translate(string text)
    {
        return Localizer[text];
    }
}