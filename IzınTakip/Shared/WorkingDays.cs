using Nager.Date;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace IzinTakip.UI.Shared
{
    public class WorkingDays
    {
        public List<HolidayCostum> PublicHolidayDates = new List<HolidayCostum>();//Resmi tatilleri ekleyeceğimiz listemizi oluşturuyoruz.
        List<HolidayCostum> PublicHolidays = new List<HolidayCostum>();

        int publicHoliday = 0;
        public int HolidaysAsync(DateTime sDate, DateTime eDate)
        {
            if (PublicHoliday() != null)
            {
                PublicHolidays = PublicHoliday();
                foreach (var holidays in PublicHolidays)//Resmi tatil listemizi foreach ile geziyoruz
                {
                    var startDate = Convert.ToDateTime(holidays.start.date);
                    var endDate = Convert.ToDateTime(holidays.end.date);
                    TimeSpan dif = endDate.Subtract(startDate);

                    if (dif.Days == 0)
                    {
                        PublicHolidayDates.Add(holidays);
                    }
                    else if (dif.Days == 1)
                    {
                        PublicHolidayDates.Add(holidays);
                    }

                    else
                    {
                        DateTime temp = startDate;
                        for (int i = 0; i < dif.Days; i++)
                        {
                            var currentHolidays = new HolidayCostum
                            {
                                start = new Start { date = temp.ToString("yyyy-MM-dd") },
                                end = new End { date = temp.ToString("yyyy-MM-dd") },
                                summary = holidays.summary
                            };
                            PublicHolidayDates.Add(currentHolidays);
                            temp = temp.AddDays(1);
                        }
                    }
                }

                var tempPublicHolidays = new List<HolidayCostum>();
                foreach (var item in PublicHolidayDates)
                {
                    //resmi tatiller hafta sonuna denk geliyorsa aşagıdaki metod ile hafta sonralını çıkarttığımızdan tekrar saymasına gerek yok
                    //hafta içine denk gelen resmi tatilleri sayıyoruz.
                    //TODO if time format is different then Turkish it would be issue
                    if ((Convert.ToDateTime(item.start.date).ToString("dddd") != "Pazar") && (Convert.ToDateTime(item.end.date) >= sDate && Convert.ToDateTime(item.start.date) <= eDate))
                    {
                        //if condition meets the requirenments we add the dates back to list.
                        tempPublicHolidays.Add(item);
                        publicHoliday++;
                    }
                }
                PublicHolidayDates = tempPublicHolidays;

                // Substract public holidays.
                int result = CalculateWorkingDays(sDate, eDate);

                return result;
            }
            return 0;
        }
        private int CalculateWorkingDays(DateTime sDate, DateTime eDate)
        {
            DateTime geciciTarih = sDate;
            int gunSayi = 0;
            string gun = string.Empty;
            while (geciciTarih <= eDate)
            {
                gun = geciciTarih.ToString("dddd");
                if (gun != "Pazar")
                {
                    gunSayi++;
                }
                geciciTarih = geciciTarih.AddDays(1);
            }
            return gunSayi;
        }
        public List<HolidayCostum> PublicHoliday()
        {
            string jsonResult = new WebClient().
                       DownloadString("https://www.googleapis.com/calendar/v3/calendars/turkish__tr%40holiday.calendar.google.com/events?key=AIzaSyAJcACw9-p9cgKbLkf7GlNpVhJdd7w9FCA");
            
            if (!String.IsNullOrEmpty(jsonResult))
            {
                var holiday = JsonConvert.DeserializeObject<Holiday>(jsonResult);

                // Gets the current year public holidays
                holiday.items = holiday.items.Where(x => (x.start.date.ToString().Split("-"))[0] == DateTime.Now.Year.ToString()).Aggregate(new List<Item>(), (x, y) =>
                {
                    Item item = y;
                    int ii = item.summary.ToLower().IndexOf("day");
                    if (ii > 0)
                        item.summary = item.summary.Substring(0, ii);
                    else
                        item.summary = item.summary;

                    if (item.summary.IndexOf("Bayrami") != -1)
                        item.summary = item.summary.Replace("Bayrami", "Bayramı");
                    x.Add(y);
                    return x;
                }).ToArray();

                List<HolidayCostum> holidayCostum = holiday.items.GroupBy(x => new
                {
                    x.summary,
                    startDate = DateTime.ParseExact(x.start.date, "yyyy-MM-dd", null).Year,
                },
                (key, g) => new
                {
                    key.summary,
                    items = g,
                }
                ).Aggregate(new List<HolidayCostum>(), (x, y) =>
                {
                    x.Add(new HolidayCostum
                    {
                        summary = y.summary,
                        start = new Start { date = y.items.First().start.date },
                        end = new End { date = y.items.Count() > 2 ? y.items.Last().end.date : y.items.First().start.date }
                    });
                    return x;
                });

                return holidayCostum;
            }
            return null;
        }
    }

    #region classes
    public class Holiday
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public string summary { get; set; }
        public DateTime updated { get; set; }
        public string timeZone { get; set; }
        public string accessRole { get; set; }
        public object[] defaultReminders { get; set; }
        public string nextSyncToken { get; set; }
        public Item[] items { get; set; }

    }
    public class publicHolidaylistItems
    {
        public string Name { get; set; }
        public DateTime Starts { get; set; }
        public DateTime Ends { get; set; }
    }
    public class Item
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public string id { get; set; }
        public string status { get; set; }
        public string htmlLink { get; set; }
        public DateTime created { get; set; }
        public DateTime updated { get; set; }
        public string summary { get; set; }
        public Creator creator { get; set; }
        public Organizer organizer { get; set; }
        public Start start { get; set; }
        public End end { get; set; }
        public string transparency { get; set; }
        public string visibility { get; set; }
        public string iCalUID { get; set; }
        public int sequence { get; set; }
    }
    public class Creator
    {
        public string email { get; set; }
        public string displayName { get; set; }
        public bool self { get; set; }
    }
    public class Organizer
    {
        public string email { get; set; }
        public string displayName { get; set; }
        public bool self { get; set; }
    }
    public class Start
    {
        public string date { get; set; }
    }
    public class End
    {
        public string date { get; set; }
    }
    public class HolidayCostum
    {
        public string summary { get; set; }
        public Start start { get; set; }
        public End end { get; set; }
        public bool IsChecked { get; set; } = true;
    }
    #endregion
}
