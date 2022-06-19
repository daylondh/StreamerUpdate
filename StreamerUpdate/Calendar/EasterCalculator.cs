using ReactiveUI;

namespace StreamerUpdate
{
  public class EasterCalculator : ReactiveObject
  {
    public int EasterMonth { get; set; } = 0;
    public int EasterDayOfMonth { get; set; } = 0;


    public EasterCalculator(int year)
    {
      int a = year % 19,
        b = year / 100,
        c = year % 100,
        d = b / 4,
        e = b % 4,
        g = (8 * b + 13) / 25,
        h = (19 * a + b - d - g + 15) % 30,
        j = c / 4,
        k = c % 4,
        m = (a + 11 * h) / 319,
        r = (2 * e + 2 * j - k - h + m + 32) % 7;
      EasterMonth = (h - m + r + 90) / 25;
      EasterDayOfMonth = (h - m + r + EasterMonth + 19) % 32;
    }
  }
}