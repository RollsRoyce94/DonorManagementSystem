//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace testDMS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class COMPANY
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public COMPANY()
        {
            this.DONORs = new HashSet<DONOR>();
        }
    
        public int COMPANYID { get; set; }
        public string COMPANYNAME { get; set; }
        public string COMPADDRESS { get; set; }
        public string ADDRESSTWO { get; set; }
        public string CITY { get; set; }
        public string COMPANYSTATE { get; set; }
        public string ZIPCODE { get; set; }
        public string PHONE { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DONOR> DONORs { get; set; }
    }
}