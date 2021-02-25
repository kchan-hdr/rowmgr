namespace ROWM
{
    public interface SiteDecoration
    {
        string SiteTitle();
    }

    public class ReserviorSite : SiteDecoration
    {
        public string SiteTitle() => "Sites Reservior";
    }

    public class Atc6943 : SiteDecoration
    {
        public string SiteTitle() => "ATC Line 6943";
    }

    public class Atc862 : SiteDecoration
    {
        public string SiteTitle() => "ATC Line 862";
    }

    public class AtcChc : SiteDecoration
    {
        public string SiteTitle() => "ATC Cardinal-Hickory Creek";
    }

    public class Ally : SiteDecoration
    {
        public string SiteTitle() => "Alliant - CHC Distribution";
    }
}
