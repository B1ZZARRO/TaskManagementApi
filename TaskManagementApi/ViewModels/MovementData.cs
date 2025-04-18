﻿using System;

namespace TaskManagementApi.ViewModels
{
    public class MovementData
    {
        public int MovementId { get; set; }
        public int ItemId { get; set; }
        public string MovementType { get; set; }
        public int Quantity { get; set; }
        public DateTime MovementDate { get; set; }
        public int? RelatedTaskId { get; set; }
    }
}