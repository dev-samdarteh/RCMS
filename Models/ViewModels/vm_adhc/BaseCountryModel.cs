using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RhemaCMS.Models.ViewModels.vm_adhc
{
    public class BaseCountryModel
    {
        public BaseCountryModel() { }

        public string name { get; set; }
        public string capital { get; set; }
        public List<string> altSpellings { get; set; }
        public string relevance { get; set; }
        public string region { get; set; }
        public string subregion { get; set; }
        public Translations translations { get; set; }
        public int population { get; set; }
        public List<object> latlng { get; set; }
        public string demonym { get; set; }
        public double? area { get; set; }
        public double? gini { get; set; }
        public List<string> timezones { get; set; }
        public List<object> borders { get; set; }
        public string nativeName { get; set; }
        public List<string> callingCodes { get; set; }
        public List<string> topLevelDomain { get; set; }
        public string alpha2Code { get; set; }
        public string alpha3Code { get; set; }
        public List<string> currencies { get; set; }
        public List<object> languages { get; set; }

    }


    public class Translations
    {
        public string de { get; set; }
        public string es { get; set; }
        public string fr { get; set; }
        public string ja { get; set; }
        public string it { get; set; }
    }

    //public class CountryModel
    //{
    //    public string name { get; set; }
    //    public string capital { get; set; }
    //    public List<string> altSpellings { get; set; }
    //    public string relevance { get; set; }
    //    public string region { get; set; }
    //    public string subregion { get; set; }
    //    public Translations translations { get; set; }
    //    public int population { get; set; }
    //    public List<object> latlng { get; set; }
    //    public string demonym { get; set; }
    //    public double? area { get; set; }
    //    public double? gini { get; set; }
    //    public List<string> timezones { get; set; }
    //    public List<object> borders { get; set; }
    //    public string nativeName { get; set; }
    //    public List<string> callingCodes { get; set; }
    //    public List<string> topLevelDomain { get; set; }
    //    public string alpha2Code { get; set; }
    //    public string alpha3Code { get; set; }
    //    public List<string> currencies { get; set; }
    //    public List<object> languages { get; set; }
    //}


}
