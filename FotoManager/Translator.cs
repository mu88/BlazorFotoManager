using Microsoft.Extensions.Localization;

namespace FotoManager;

public class Translator : ITranslator
{
    public Translator(IStringLocalizer<Translator> localizer)
    {
        Localizer = localizer;
    }

    private IStringLocalizer<Translator> Localizer { get; }

    /// <inheritdoc />
    public string Translate(string text)
    {
        return Localizer[text];
    }
}