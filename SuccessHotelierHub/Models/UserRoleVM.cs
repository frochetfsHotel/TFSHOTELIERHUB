﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SuccessHotelierHub.Models
{
    public class UserRoleVM
    {
        public Guid Id { get; set; }

        [DisplayName("Code")]
        [Required(ErrorMessage = "Please enter code.")]
        public string Code { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "Please enter role name.")]
        public string Name { get; set; }        

        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}