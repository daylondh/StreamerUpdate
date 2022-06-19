using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace StreamerUpdate
{

  public class Calendar
  {
    List<CalendarEntry> Dates = new List<CalendarEntry>();

    public void Construct(int year, bool isLeapYear)
    {
      for (var i = 0; i < 365; i++)
      {
        var currentDate = new DateTime(year, 1, 1).AddDays(i);
        var entry = new CalendarEntry() { Date = currentDate, DateName = "" };
        Dates.Add(entry);
      }

      if (!isLeapYear) return;

      var date = new DateTime(year, 2, 29);
      var centry = new CalendarEntry() { Date = date, DateName = "" };
      Dates.Insert(59,centry);

    }

    public void Set(DateTime date, string name)
    {
      Dates.First(d => d.Date.DayOfYear == date.DayOfYear).DateName = name;
    }

    public void Set(int month, int dayInMonth, string name)
    {
      Dates.First(d => d.Date.Month == month && d.Date.Day == dayInMonth).DateName = name;
    }

    public void Set(int dayInYear, string name)
    {
      Dates.First(d => d.Date.DayOfYear == dayInYear).DateName = name;
    }

    public string GetName(int dayInYear)
    {
      var d = Dates.FirstOrDefault(d => d.Date.DayOfYear == dayInYear);
      if (d != null)
        return d.DateName;
      return "";
    }


    public string GetName(int month, int dayInMonth)
    {
      var d = Dates.FirstOrDefault(d => d.Date.Month == month && d.Date.Day == dayInMonth);
      if (d != null)
        return d.DateName;
      return "";
    }

    public void Print()
    {
      foreach (var calendarEntry in Dates)
      {
        Debug.WriteLine(calendarEntry.Date.ToLongDateString() + "\t\t" + calendarEntry.DateName);
      }
    }
  }
}