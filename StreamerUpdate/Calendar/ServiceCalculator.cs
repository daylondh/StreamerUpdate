using System;
using System.Globalization;
using ReactiveUI;

namespace StreamerUpdate
{

    public class ServiceCalculator
    {
      Calendar calendar;


      public string CalculateService()
      {
        DateTime now = DateTime.Now;
        int currentHour = now.Hour;
        int currentMin = now.Minute;
        if (currentHour < 9)
          return "Early Service";
        if (currentHour > 10) return "Evening Service";
        // either a late service or a single service
        if (currentHour < 10 && currentMin < 5)
          return "Single Service";
        return "Late Service";
      }
    }
  }