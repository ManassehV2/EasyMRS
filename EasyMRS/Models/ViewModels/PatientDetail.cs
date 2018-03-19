using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyMRS.Models.ViewModels
{
    public class PatientDetail
    {
        private EasyMRSDataContext db = new EasyMRSDataContext();
        public Patient p;
        public IEnumerable<AllegiesCategory> ACategory;
        public IEnumerable<Allergy> FoodAllergies;
        public IEnumerable<Allergy> DrugAllergies;
        public IEnumerable<Allergy> OtherAllergies;
        
        


        public PatientDetail(Guid PID) 
        {
            this.p = (from p in db.Patients
                      where p.PID == PID
                      select p).SingleOrDefault();
            ACategory = (from ac in db.AllegiesCategories select ac);

            this.FoodAllergies = from a in db.Allergies
                                 join ac in db.AllegiesCategories
                                 on a.ACID equals ac.ACID
                                 where ac.Name == "Food"
                                 select a;

            this.DrugAllergies = from a in db.Allergies
                                 join ac in db.AllegiesCategories
                                 on a.ACID equals ac.ACID
                                 where ac.Name == "Drugs"
                                 select a;

            this.OtherAllergies = from a in db.Allergies
                                  join ac in db.AllegiesCategories
                                  on a.ACID equals ac.ACID
                                  where ac.Name == "Others"
                                  select a;
            /*this.PAllergies = (from a in db.Allergies
                             join pa in db.PatientAllergies
                             on a.AID equals pa.AID
                             where pa.PID == PID
                             select new {a.Name,pa.Severity,pa.reactions }).AsEnumerable();*/

        }
        public string[] getAllergiesReaction()
        {
            string[] AR = new string[] {  "Unknown", "Anaemia", "Anaphylaxis",
                                          "Bronchospasm","Cough","Diarrhea","Dystonia","Fever",
                                          "Flushing","GI upset","Headache","Hepatotoxicity","Hives","Hypertension",
                                          "Itching","Mental status change",
                                           "Musculoskeletal pain","Myalgia","Rash"};
            return AR;
    }
       
    }
}