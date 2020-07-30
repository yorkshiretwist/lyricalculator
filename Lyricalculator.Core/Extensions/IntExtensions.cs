namespace Lyricalculator.Core.Extensions
{
    public static class IntExtensions
    {
        public static string Pluralise(this int num)
        {
            if (num == 1)
            {
                return string.Empty;
            }

            return "s";
        }
    }
}
