using System.Collections.Generic;

namespace StreamerUpdate
{
  public class FixedDates
  {
    readonly Dictionary<int, string> _dates = new Dictionary<int, string>();

    public FixedDates()
    {
      Populate();
    }

    private void Populate()
    {
      _dates[0] = "New Year's Day";
      _dates[5] = "Epiphany";
      _dates[184] = "Independence Day";
      _dates[303] = "Reformation Day";
      _dates[304] = "All Saints Day";
      _dates[314] = "Veterans Day";
      _dates[357] = "Christmas Eve";
      _dates[358] = "Christmas Day";
      _dates[364] = "New Year's Eve";
    }

    public string[] TransferDates(string[] input)
    {
      var leapYear = input.Length == 366;
      for (var i = 0; i < input.Length; i++)
      {
        var j = i;
        if (leapYear && i > 58)
          j++;
        var entry = _dates[j];
        if (entry != null)
          input[i] = entry;
        else
          entry = "";
      }
      return input;
    }
  }
}