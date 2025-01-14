﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SuccessHotelierHub.Models
{
    public class AdditionalChargeCategoryVM
    {
        public Guid Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Boolean? IsActive { get; set; }

        public Boolean? IsDeleted { get; set; }

        public Boolean? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public Boolean? UpdatedBy { get; set; }

        public DateTime? UPdatedOn { get; set; }

    }
}