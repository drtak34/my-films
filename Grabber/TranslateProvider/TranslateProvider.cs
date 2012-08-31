using Google.API.Translate;

namespace Grabber.TranslateProvider
{
  public class TranslateProvider
  {
    private readonly TranslateClient client;
    private readonly string referrer = string.Empty;

    public TranslateProvider(string referrer)
    {
      this.referrer = referrer;
      this.client = new TranslateClient(this.referrer);
    }

    public string Translate(string input, string translateTo, out string detectedLanguage, out bool reliable,
                            out double confidence, string lineshiftMark)
    {
      const string symbolToRepresentLineshift = "QQQQQ";
      input = input.Replace(lineshiftMark, symbolToRepresentLineshift);

      detectedLanguage = this.client.Detect(input, out reliable, out confidence);
      string translated = this.client.Translate(input, detectedLanguage, translateTo);

      translated = translated.Replace(symbolToRepresentLineshift, lineshiftMark);

      //translated = translated.Replace("[*] ", lineshiftMark);
      //translated = translated.Replace("[ *] ", lineshiftMark);
      //translated = translated.Replace("[* ] ", lineshiftMark);
      //translated = translated.Replace("[ * ] ", lineshiftMark);
      translated = translated.Replace(" " + lineshiftMark, lineshiftMark);
      return translated;
    }
  }
}