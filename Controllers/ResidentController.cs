using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MelihAkıncı_webTabanliAidatTakipSistemi.Controllers {
    [Authorize(Roles = "ApartmentResident")]
    public class ResidentController : Controller {

        /*
         * bilgilerini guncelleyebilme / put
         * kendi dairesini gorebilme / get
         * aidat bilgilerini ve özel ücretlerini gorebilme / get
         * aidat ya da özel ücretini ödeyebilme / post
         * yapmış olduğu ödemeleri ve durumlarını(red, onay vb.) / get
         * dekont yükleme ödeme yaptıktan sonra ödemeler panelin de dekont yükle butonu olmalı / post 
        */

        
        public IActionResult Index() {
            return View();
        }
    }
}
