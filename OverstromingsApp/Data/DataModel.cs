using System;

namespace OverstromingsApp.Models
{
    public class DataModel
    {
        public int Jaar { get; set; }
        public int Maand { get; set; }
        public int NeerslagMM { get; set; }

        public string Seizoen => Maand switch
        {
            12 or 1 or 2 => "Winter",
            3 or 4 or 5 => "Lente",
            6 or 7 or 8 => "Zomer",
            9 or 10 or 11 => "Herfst",
            _ => "Onbekend"
        };

        public override string ToString()
            => $"{Jaar} - {Maand} ({Seizoen}): {NeerslagMM}mm";
    }
}
