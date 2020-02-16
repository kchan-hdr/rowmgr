﻿namespace ROWM
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
}
