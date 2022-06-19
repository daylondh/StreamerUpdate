using System;

namespace StreamerUpdate
{
  public class CalendarBuilder
  {
    private EasterCalculator _easterCalculator;
    private Calendar _calendar;
    private int _ashWednesday;
    private int _pentecost;
    private int _firstAdvent;
    private int _year;

    public CalendarBuilder(Calendar calendar)
    {
      _calendar = calendar;
    }

    public void Build(int year)
    {
      _year = year;
      _calendar.Construct(year, IsLeapYear());
      DateTime dt = new DateTime(year, 1, 1);
      // These dates never move
      _calendar.Set(1, 1, "New Year's Day");
      _calendar.Set(1, 6, "Epiphany");
      _calendar.Set(7, 4, "Independence Day");
      _calendar.Set(10, 31, "Reformation Day");
      _calendar.Set(11, 1, "All Saints Day");
      _calendar.Set(11, 11, "Veterans Day");
      _calendar.Set(12, 24, "Christmas Eve");
      _calendar.Set(12, 25, "Christmas Day");
      _calendar.Set(12, 31, "New Year's Eve");
      BuildYear();
    }

    private bool IsLeapYear()
    {
      if (_year % 4 != 0)
        return false;
      return (_year % 100 != 0) || (_year % 400 == 0);
    }

    public string LookupByDate(int month, int dayinmonth)
    {
      return _calendar.GetName(month, dayinmonth);
    }

    private void BuildYear()
    {
      _easterCalculator = new EasterCalculator(_year);
      BuildToPentecost(); // week and season...
      BuildAdvent();
      BuildPentecost();
      BuildThanksgiving();
    }

    private void BuildAdvent()
    {
      // Feast of St. Andrew is November 30. We want to find the closest Sunday, and that's Advent 1. If it falls on a
      // Wednesday means the Sunday before.
      DateTime stAndrew = new DateTime(_year, 11, 30);

      DateTime advent1 = stAndrew;

      if (stAndrew.DayOfWeek == DayOfWeek.Sunday)
        advent1 = stAndrew;
      if (stAndrew.DayOfWeek == DayOfWeek.Monday)
        advent1 = stAndrew.AddDays(-1);
      if (stAndrew.DayOfWeek == DayOfWeek.Tuesday)
        advent1 = stAndrew.AddDays(-2);
      if (stAndrew.DayOfWeek == DayOfWeek.Wednesday)
        advent1 = stAndrew.AddDays(-3);
      if (stAndrew.DayOfWeek == DayOfWeek.Thursday)
        advent1 = stAndrew.AddDays(3);
      if (stAndrew.DayOfWeek == DayOfWeek.Friday)
        advent1 = stAndrew.AddDays(2);
      if (stAndrew.DayOfWeek == DayOfWeek.Saturday)
        advent1 = stAndrew.AddDays(1);


      _calendar.Set(advent1.AddDays(-7), "Last Sunday Of The Church Year");
      _calendar.Set(advent1.AddDays(-6), "Last Monday Of The Church Year");
      _calendar.Set(advent1.AddDays(-5), "Last Tuesday Of The Church Year");
      _calendar.Set(advent1.AddDays(-4), "Last Wednesday Of The Church Year");
      _calendar.Set(advent1.AddDays(-3), "Last Thursday Of The Church Year");
      _calendar.Set(advent1.AddDays(-2), "Last Friday Of The Church Year");
      _calendar.Set(advent1.AddDays(-1), "Last Saturday Of The Church Year");
      _calendar.Set(advent1, "First Sunday in Advent");
      _firstAdvent = advent1.DayOfYear;
      var christmasIndex = new DateTime(_year, 12, 25).DayOfYear;
      for (var i = _firstAdvent; i < christmasIndex; i++)
      {
        if (_calendar.GetName(i) == "")
        {
          int weekSinceAdvent = (i - _firstAdvent) / 7 + 1;
          _calendar.Set(i, Nth(weekSinceAdvent) + WeekDay(i) + " in Advent");
        }
      }
      for (int i = christmasIndex + 1; i < (IsLeapYear() ? 366 : 365); i++)
      {
        if (_calendar.GetName(i) == "")
        {
          int weekSinceChristmas = (i - christmasIndex) / 7 + 1;
          _calendar.Set(i, Nth(weekSinceChristmas) + WeekDay(i) + " of Christmas");
        }
      }
    }

    private void BuildToPentecost()
    {
      // works from 1/1 through ephiphany to easter
      int emo = _easterCalculator.EasterMonth;
      int edomo = _easterCalculator.EasterDayOfMonth;
      DateTime easter = new DateTime(_year, emo, edomo);
      int easterDayInYear = easter.DayOfYear;
      _calendar.Set(easter, "Easter");
      _ashWednesday = easterDayInYear - 46; // 40 days of lent, plus sundays
      _calendar.Set(_ashWednesday, "Ash Wednesday");
      _calendar.Set(_ashWednesday - 3, "Transfiguration");
      // ephiphany
      for (int i = 1; i < _ashWednesday - 3; i++)
      {
        int weekInYear = i / 7;
        if (_calendar.GetName(i).Equals(""))
        {
          if (weekInYear == 0 && i < 6)
            _calendar.Set(i, "Second " + WeekDay(i) + " after Christmas");
          else if (i < 13 && i > 6 && WeekDay(i).Equals("Sunday"))
            _calendar.Set(i, "Baptism of Our Lord");
          else
          {
            // ephiphany
            int weekSinceEpiphany = (i - 5) / 7 + 1;
            _calendar.Set(i, Nth(weekSinceEpiphany) + WeekDay(i) + " after Ephiphany");
          }
        }
      }
      _calendar.Set(_ashWednesday - 2, "Monday after Transfiguration");
      _calendar.Set(_ashWednesday - 1, "Tuesday after Transfiguration");
      // lent
      for (int i = _ashWednesday - 3; i < easterDayInYear - 7; i++)
      {
        int weekInLent = (i - _ashWednesday + 6) / 7;
        if (_calendar.GetName(i).Equals(""))
        {
          _calendar.Set(i, Nth(weekInLent) + WeekDay(i) + " in Lent");
        }
      }
      _calendar.Set(easterDayInYear - 7, "Passion Sunday");
      _calendar.Set(easterDayInYear - 6, "Monday in Holy Week");
      _calendar.Set(easterDayInYear - 5, "Tuesday in Holy Week");
      _calendar.Set(easterDayInYear - 4, "Wednesday in Holy Week");
      _calendar.Set(easterDayInYear - 3, "Maundy Thursday");
      _calendar.Set(easterDayInYear - 2, "Good Friday");
      _calendar.Set(easterDayInYear - 1, "Holy Saturday");
      _calendar.Set(easterDayInYear + 1, "Easter Monday");
      _calendar.Set(easterDayInYear + 2, "Easter Tuesday");
      _calendar.Set(easterDayInYear + 3, "Easter Wednesday");
      _calendar.Set(easterDayInYear + 4, "Easter Thursday");
      _calendar.Set(easterDayInYear + 5, "Easter Friday");
      _calendar.Set(easterDayInYear + 6, "Easter Saturday");
      for (var i = easterDayInYear + 7; i < easterDayInYear + 48; i++)
      {
        var weekafterEaster = (i - easterDayInYear) / 7 + 1;
        if (_calendar.GetName(i).Equals(""))
        {
          _calendar.Set(i, Nth(weekafterEaster) + WeekDay(i) + " of Easter");
        }
      }
      var pentecost = easter.AddDays(49);
      _pentecost = pentecost.DayOfYear;
      _calendar.Set(pentecost.AddDays(-1), "Pentecost Eve");
      _calendar.Set(_pentecost, "Pentecost");
    }

    private void BuildPentecost()
    {
      int pentecostEnd = _firstAdvent - 8;
      _calendar.Set(_pentecost + 1, "Pentecost Monday");
      _calendar.Set(_pentecost + 2, "Pentecost Tuesday");
      _calendar.Set(_pentecost + 3, "Pentecost Wednesday");
      _calendar.Set(_pentecost + 4, "Pentecost Thursday");
      _calendar.Set(_pentecost + 5, "Pentecost Friday");
      _calendar.Set(_pentecost + 6, "Pentecost Saturday");
      _calendar.Set(_pentecost + 7, "Holy Trinity Sunday");
      for (var i = _pentecost + 8; i <= pentecostEnd; i++)
      {
        if (!_calendar.GetName(i).Equals("")) continue;
        int weekSincePentecost = (i - _pentecost) / 7;
        _calendar.Set(i, Nth(weekSincePentecost) + WeekDay(i) + " after Pentecost");
      }

      // find the closest Sunday to Reformation day...then update the observed days for reformation and all saints day
      DateTime refDay = new DateTime(_year, 10, 31);
      switch (refDay.DayOfWeek)
      {
        case DayOfWeek.Sunday:
          _calendar.Set(refDay.AddDays(7), "All Saints Sunday");
          break;
        case DayOfWeek.Monday:
          _calendar.Set(refDay.AddDays(-1), "Reformation Sunday");
          _calendar.Set(refDay.AddDays(6), "All Saints Sunday");
          break;
        case DayOfWeek.Tuesday:
          _calendar.Set(refDay.AddDays(-2), "Reformation Sunday");
          _calendar.Set(refDay.AddDays(5), "All Saints Sunday");
          break;
        case DayOfWeek.Wednesday:
          _calendar.Set(refDay.AddDays(-3), "Reformation Sunday");
          _calendar.Set(refDay.AddDays(6), "All Saints Sunday");
          break;
        case DayOfWeek.Thursday:
          _calendar.Set(refDay.AddDays(-4), "Reformation Sunday");
          _calendar.Set(refDay.AddDays(3), "All Saints Sunday");
          break;
        case DayOfWeek.Friday:
          _calendar.Set(refDay.AddDays(-5), "Reformation Sunday");
          _calendar.Set(refDay.AddDays(2), "All Saints Sunday");
          break;
        case DayOfWeek.Saturday:
          _calendar.Set(refDay.AddDays(-6), "Reformation Sunday");
          _calendar.Set(refDay.AddDays(1), "All Saints Sunday");
          break;
      }
    }

    private void BuildThanksgiving()
    {
      var nov1 = new DateTime(_year, 11, 1);
      var firstThursdayInNov = nov1;
      for (int i = 0; i < 7; i++)
      {
        if (firstThursdayInNov.DayOfWeek == DayOfWeek.Thursday)
          break;
        firstThursdayInNov = firstThursdayInNov.AddDays(1);
      }

      var thanksgivingDate = firstThursdayInNov.AddDays(21);


      _calendar.Set(thanksgivingDate.DayOfYear - 1, "Thanksgiving Eve");
      _calendar.Set(thanksgivingDate.DayOfYear, "Thanksgiving Day");
    }

    private string Nth(int sinceStart)
    {
      if (sinceStart == 0)
        return "";
      if (sinceStart == 1)
        return "First ";
      if (sinceStart == 2)
        return "Second ";
      if (sinceStart == 3)
        return "Third ";
      if (sinceStart == 4)
        return "Fourth ";
      if (sinceStart == 5)
        return "Fifth ";
      if (sinceStart == 6)
        return "Sixth ";
      if (sinceStart == 7)
        return "Seventh ";
      if (sinceStart == 8)
        return "Eighth ";
      if (sinceStart == 9)
        return "Ninth ";
      if (sinceStart == 10)
        return "Tenth ";
      if (sinceStart == 11)
        return "Eleventh ";
      if (sinceStart == 12)
        return "Twelfth ";
      if (sinceStart == 13)
        return "Thirteenth ";
      if (sinceStart == 14)
        return "Fourteenth ";
      if (sinceStart == 15)
        return "Fifteenth ";
      if (sinceStart == 16)
        return "Sixteenth ";
      if (sinceStart == 17)
        return "Seventeenth ";
      if (sinceStart == 18)
        return "Eighteenth ";
      if (sinceStart == 19)
        return "Nineteenth ";
      if (sinceStart == 20)
        return "Twentieth ";
      if (sinceStart == 21)
        return "Twenty-First ";
      if (sinceStart == 22)
        return "Twenty-Second ";
      if (sinceStart == 23)
        return "Twenty-Third ";
      if (sinceStart == 24)
        return "Twenty-Fourth ";
      if (sinceStart == 25)
        return "Twenty-Fifth ";
      if (sinceStart == 26)
        return "Twenty-Sixth ";
      if (sinceStart == 27)
        return "Twenty-Seventh ";
      if (sinceStart == 28)
        return "Twenty-Eighth ";
      return "Twenty-Ninth ";
    }

    private string WeekDay(int dayInYear)
    {
      DateTime dt = new DateTime(_year, 1, 1).AddDays(dayInYear - 1);
      return dt.DayOfWeek.ToString();
    }
  }
}