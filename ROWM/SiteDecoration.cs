namespace ROWM
{
    public interface SiteDecoration
    {
        string SiteTitle();
    }

    public class Dw : SiteDecoration
    {
        public string SiteTitle() => "Denver Water";
    }

    public class ReserviorSite : SiteDecoration
    {
        public string SiteTitle() => "Sites Reservior";
    }

    public class Atc6943 : SiteDecoration
    public class B2H: SiteDecoration
    {
        public string SiteTitle() => "ATC Line 6943";
        public string SiteTitle() => "B2H";
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

    public class Wharton: SiteDecoration
    public class OppdPwp: SiteDecoration
    {
        public string SiteTitle() => "City of Wharton";
        public string SiteTitle() => "OPPD PWP";
    }

    public class B2H: SiteDecoration
    {
        public string SiteTitle() => "B2H";
    }
}
