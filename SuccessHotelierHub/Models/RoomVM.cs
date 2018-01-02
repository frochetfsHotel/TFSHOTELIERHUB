﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SuccessHotelierHub.Models
{
    public class RoomVM
    {
        public Guid Id { get; set; }
        public Guid RoomTypeId { get; set; }
        public string RoomNo { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public Guid? StatusId { get; set; }
        public bool IsOccupied { get; set; }

        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }

}