﻿using MelihAkıncı_webTabanliAidatTakipSistemi.DTOs;
using static MelihAkıncı_webTabanliAidatTakipSistemi.classes.PaymentStatusEnum;

namespace MelihAkıncı_webTabanliAidatTakipSistemi.Models {
   
    public class MaintenanceFee {
        public int Id { get; set; }
        public required int ResidentId { get; set; }
        public required int  ApartmentId {get; set; }
        public required decimal Amount { get; set; }
        public required DateTime DueDate { get; set; } // son ödeme tarihi
        public bool IsPaid { get; set; } = false;
        public DateTime? PaymentDate { get; set; } = null;
        public required ApartmentResident ApartmentResident { get; set; } // Navigation Property


    }
}
