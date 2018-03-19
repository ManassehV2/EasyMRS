using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasyMRS.Models;
using EasyMRS.Models.ViewModels;
using System.Web.Script.Serialization;

namespace EasyMRS.Controllers
{
    [Authorize(Roles = "Registrar")]
    public class RegistrarController : Controller
    {
        private EasyMRSDataContext db = new EasyMRSDataContext();
        
        // GET: /Registrar/
        public ActionResult Index()
        {

            return View();
        }
        [HttpGet]
        public ActionResult RegisterPaitent()
        {
            return View();
        
        }
        [HttpPost]
        public ActionResult RegisterPaitent(string FName,string LNAme,string MName,DateTime BDate,string Gender,
            string Address1, string Address2, string Phone,string photo,FormCollection fc)
    {
        Patient newP = new Patient();
        Guid newPID = Guid.NewGuid();
        try
        {


            newP.PID = newPID;
            newP.FName = FName;
            newP.LName = LNAme;
            newP.MName = MName;
            newP.BirthDate = BDate;
            newP.Gender = Gender;
            newP.Address = Address1;
            //newP.PhoneNumber = Phone;
            db.Patients.InsertOnSubmit(newP);
            db.SubmitChanges();

        }
        catch {
            return View();
        
        }

        return RedirectToAction("PatientDetail", new {PID = newPID});
    
    }
        public ActionResult PatientDetail(Guid PID)
        {
           

            ViewBag.allergies = (from a in db.Allergies
                                    join pa in db.PatientAllergies
                                    on a.AID equals pa.AID
                                    where pa.PID == PID
                                    select new AandmdReaction{ Name =  a.Name, Severity = pa.Severity, reactions = pa.reactions });

            return View(new PatientDetail(PID));
        }
        [HttpPost]
        public ActionResult AddAllergy(string PID,string severity, string[] AR,String AL)
        {
            Guid newPID = Guid.Parse(PID);
            try
            {
                
                PatientAllergy PA = new PatientAllergy();
                PA.AID = Guid.Parse(AL);
                PA.PID = newPID;
                PA.Severity = severity;
                PA.reactions = AR[0];//Have to changed to include all reactions in the list separeted by comma
                db.PatientAllergies.InsertOnSubmit(PA);
                db.SubmitChanges();
                

            }
            catch {
                return RedirectToAction("PatientDetail", new { PID = newPID });
            }
            return RedirectToAction("PatientDetail", new { PID = newPID });

        }
        public ActionResult AddVital(string PID,FormCollection fc)
        {
            Guid newPID = Guid.Parse(PID);
            return RedirectToAction("PatientDetail", new { PID = newPID });
        
        }

        public ActionResult markPatienDecesed(string PID)
        {
            Guid newPID = Guid.Parse(PID);
            return RedirectToAction("PatientDetail", new { PID = newPID });
        
        }
        public ActionResult DeletePatient(string PID)
        {
            Guid newPID = Guid.Parse(PID);
            return RedirectToAction("PatientDetail", new { PID = newPID });
        
        }
        public ActionResult StartNewVisit(string PID)
        {
            Guid newPID = Guid.Parse(PID);
            Guid newVID = Guid.NewGuid();
            try
            {
                Visit visit = new Visit();
                visit.VID = newVID;
                visit.PID = newPID;
                visit.VisitStartDate = DateTime.Now;
                visit.IsOpen = true;
                db.Visits.InsertOnSubmit(visit);
                db.SubmitChanges();
                return RedirectToAction("VisitsList", new { PID = newPID, VID = newVID });

            }
            catch 
            {
                return View();
            }
            
        }
        public ActionResult VisitsList(Guid PID, Guid VID)
        {
            Visit ActiveVisit = (from v in db.Visits
                                where v.PID == PID && v.IsOpen == true
                                select v).FirstOrDefault();

            ViewBag.ActiveVisit = ActiveVisit;

            IEnumerable<Visit> pastVisits = from v in db.Visits
                                            where v.PID == PID && v.IsOpen == false
                                            select v;
            return View(pastVisits);
        }
       

	}
    
}
public class AandmdReaction 
{
    public string Name { set; get;}
    public string Severity { set; get; }
    public string reactions { set; get; }

}