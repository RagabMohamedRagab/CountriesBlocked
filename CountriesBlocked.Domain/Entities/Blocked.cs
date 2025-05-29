using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CountriesBlocked.Domain.Entities
{
    public class Blocked:BaseEntity
    {
        public string IP {  get; set; }

        public string CountryCode {  get; set; }
        public string CountryName {  get; set; }
        public bool?  IsBlocked {  get; set; }=default;

        public DateTime? ExpireAt { get; set; } = default;
    }
}
