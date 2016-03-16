using System.Collections.Generic;

namespace Common
{ 
   public  class Nameday 
    {
        public Nameday()
        {

        }
       
        public int Month { get; set; }
        public int Day { get; set; }
        public IEnumerable<string> Names { get; set; }
        public Nameday(int month, int day, IEnumerable<string> names)
        {
            Month = month;
            Day = day;
            Names = names;
        }
        public string NameAsString => string.Join(", ", Names);
    }
}
