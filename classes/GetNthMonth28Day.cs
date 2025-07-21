namespace MelihAkıncı_webTabanliAidatTakipSistemi.classes {
    public class GetNthMonth28Day {

        public DateTime GetDueDate(int n) {
            //verilen n inci ayın 28. ci gününü tarih olarak döndürür
            int year = DateTime.UtcNow.Year;
            int month = DateTime.UtcNow.AddMonths(n).Month;
            int day = 28;
            DateTime date = new DateTime(year, month, day);
            string fullTime = date.ToString("yyyy/MM/dd");
            return DateTime.Parse(fullTime);
        }
    }
}
