﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SuccessHotelierHub.Models
{
    public class IndividualProfileVM
    {
        public Guid Id { get; set; }
        public Guid ProfileTypeId { get; set; }
        
        [Display(Name = "First Name *")]
        public string FirstName { get; set; }
        
        [Display(Name = "Last Name *")]
        public string LastName { get; set; }

        public Guid? TitleId { get; set; }

        [Display(Name = "Telephone No")]
        public string TelephoneNo { get; set; }

        [Display(Name = "Business Telephone No")]
        public string BusinessTelephoneNo { get; set; }

        [EmailAddress(ErrorMessage = "Please enter valid email address")]
        public string Email { get; set; }

        public string Address { get; set; }

        [Display(Name = "Home Address")]
        public string HomeAddress { get; set; }

        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public int? CityId { get; set; }

        [Display(Name = "Zip Code")]
        public string ZipCode { get; set; }

        public Guid? VipId { get; set; }

        public Guid? NationalityId { get; set; }

        [Display(Name = "Car Registration No")]
        public string CarRegistrationNo { get; set; }

        [Display(Name = "Passport No")]
        public string PassportNo { get; set; }
        
        public DateTime? DOB { get; set; }
        public bool IsMailingList { get; set; }
        public string Remarks { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public IEnumerable<SelectListItem> TitleList { get; set; }
        public IEnumerable<SelectListItem> VipList { get; set; }
        public IEnumerable<SelectListItem> CountryList { get; set; }
        public IEnumerable<SelectListItem> StateList { get; set; }
        public IEnumerable<SelectListItem> CityList { get; set; }

        public string PreferenceItems { get; set; }
    }
}