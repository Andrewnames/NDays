using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

namespace Common
{
   public class NamedaysRepository
    {
       public static List<Nameday> allNamedaysCache;
        public static async Task<List<Nameday>> GetAllNamedaysAsync()
        {
            if (allNamedaysCache!=null)
            {
                return allNamedaysCache;
            }

            using (var client= new HttpClient())
            {
                var stream = await client.GetStreamAsync("http://response.hu/namedays_hu.json");
                var serializer = new DataContractJsonSerializer(typeof(List<Nameday>));
                allNamedaysCache = (List<Nameday>)serializer.ReadObject(stream);
                return allNamedaysCache;

            }


        }

       public static async Task<string> GetTodaysNamesAsStringAsync()
       {

           var allNamedays = await GetAllNamedaysAsync();
           var todaysNamedays = allNamedays.Find(d=> d.Day == DateTime.Now.Day && d.Month == DateTime.Now.Month);

           return todaysNamedays?.NameAsString;
       }


    }
}
