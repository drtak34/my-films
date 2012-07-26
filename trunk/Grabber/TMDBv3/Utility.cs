namespace Grabber.TMDBv3
{
  using System;
  using System.Text.RegularExpressions;
  using System.IO;

  public static class Utility
    {
        // remove invalid characters for use in URL
        public static string EscapeString(this string s)
        {
#if WINDOWS_PHONE
            return Regex.Replace(s, "[" + Regex.Escape(new String(Path.GetInvalidPathChars())) + "]", "-");
#else
            return Regex.Replace(s, "[" + Regex.Escape(new String(Path.GetInvalidFileNameChars())) + "]", "-");
#endif
        }
    }
}
