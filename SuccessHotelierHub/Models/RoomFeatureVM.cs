﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SuccessHotelierHub.Models
{
    public class RoomFeatureVM
    {
        public Guid Id { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "Please enter name.")]
        public string Name { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }

        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}