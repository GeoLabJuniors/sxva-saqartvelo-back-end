﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using sxva_saqartvelo_back_end.Models;
using sxva_saqartvelo_back_end.Filters;
using PagedList.Mvc;
using PagedList;
using System.Net;
using sxva_saqartvelo_back_end.Helpers;
using System.IO;
using System.Data.Entity.Validation;
using System.Diagnostics;

namespace sxva_saqartvelo_back_end.Controllers
{
    public class FreelancerController : Controller
    {

        OtherGeorgiaEntities _db = new OtherGeorgiaEntities();

        string randomSecret = "4b47a904bc5e81234a754f552355bf44"; //პაროლის დასაჰეშად

        //public string Random10()
        //{
            
        //    return Guid.NewGuid().ToString("N");
        //}

        public ActionResult Index(int? page)
        {
            ViewBag.CountFreelancers = _db.Freelancers.Count();

            //Mvc PagedList
            int pageSize = 9;
            int pageNumber = (page ?? 1);

            var freelancers = _db.Freelancers.OrderBy(x => Guid.NewGuid()).ToPagedList(pageNumber, pageSize);

            return View(freelancers);
        }



        public PartialViewResult FilterFreelancerData(string SearchInput, int[] CheckedSkills, int? RatingLow, int? RatingHight)
        {
            List<Freelancer> freelancers = new List<Freelancer>();

            var parametersExist = false;



            //For Search
            if (SearchInput != null && SearchInput != "" && SearchInput != " ")
            {
                parametersExist = true;
                var input = SearchInput.Trim();
                var searchWords = input.Split(' ');
                foreach (var word in searchWords)
                {
                    // თუ ცარიელია, ახლიდან ვავსებ. (თავზე გადაწერა)
                    if (freelancers.Count < 1)
                    {
                        freelancers.AddRange(_db.Freelancers.Where(x => x.Name.Contains(word) || x.Surname.Contains(word) || x.Freelancer_Skill.Any(e => e.Skill.Name.Contains(word)) || x.Bio.Contains(word) || x.Projects.Any(y => y.Name.Contains(word) || y.Description.Contains(word) || y.Company.Name.Contains(word))).ToList());
                    }
                    else
                    {
                        freelancers = freelancers.Where(x => x.Name.Contains(word) || x.Surname.Contains(word) || x.Freelancer_Skill.Any(e => e.Skill.Name.Contains(word)) || x.Bio.Contains(word) || x.Projects.Any(y => y.Name.Contains(word) || y.Description.Contains(word) || y.Company.Name.Contains(word))).ToList();
                    }
                }

                if (freelancers.Count < 1) return PartialView("_PartialFilterData", freelancers.Distinct());

            }
            //



            //For CheckBox Filter
            if (CheckedSkills != null && CheckedSkills.Count() > 0)
            {
                parametersExist = true;

                foreach (int skillID in CheckedSkills)
                {

                    if (freelancers.Count < 1)
                    {
                        freelancers.AddRange(_db.Freelancers.Where(x => x.Freelancer_Skill.Any(e => e.SkillID == skillID)).ToList());
                    }
                    else
                    {
                        freelancers = freelancers.Where(x => x.Freelancer_Skill.Any(e => e.SkillID == skillID)).ToList();
                    }

                }
                if (freelancers.Count < 1) return PartialView("_PartialFilterData", freelancers.Distinct());
            }
            //


            //For Range Slier Filter
            if (RatingLow >= 0 && RatingHight >= 0 && RatingLow <= RatingHight)
            {
                parametersExist = true;

                if (freelancers.Count < 1)
                {
                    freelancers.AddRange(_db.Freelancers.Where(x => x.Rating >= RatingLow && x.Rating <= RatingHight).ToList());
                }
                else
                {
                    freelancers = freelancers.Where(x => x.Rating >= RatingLow && x.Rating <= RatingHight).ToList();
                }

                if (freelancers.Count < 1) return PartialView("_PartialFilterData", freelancers.Distinct());
            }
            //

            if (parametersExist == false)
            {
                freelancers = _db.Freelancers.ToList();
            }

            return PartialView("_PartialFilterData", freelancers.Distinct());
        }


        public ActionResult Details(int id)
        {
            var freelancer = _db.Freelancers.FirstOrDefault(x => x.ID == id);
            return View(freelancer);
        }


        [LoginFilter]
        public ActionResult FreelancerProfile()
        {
            return View();
        }

        [LoginFilter]
        public ActionResult EditFreelancer(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var editFreelancerViewModel = new EditFreelancerViewModel
            {
                freelancer = _db.Freelancers.Find(id)
            };
            if (editFreelancerViewModel == null)
            {
                return HttpNotFound();
            }
            return View(editFreelancerViewModel);
        }

        [ValidateInput(false)] //რამდენათ დასაშვებია?
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditFreelancer([Bind(Include = "ID, Name, Surname, Field, Mobile, Bio, oldPassword, Password, RepeatPassword")] Freelancer editFreelancer, EditFreelancerViewModel model, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                var freelancerToEdit = _db.Freelancers.Find(editFreelancer.ID);
                
                if (freelancerToEdit == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }


                var hashedPassword = PasswordHashHelper.MD5Hash(randomSecret + model.editFreelancerModel.oldPassword);

                Freelancer freelancer = _db.Freelancers.FirstOrDefault(x => x.Password == hashedPassword);

                if(freelancer == null)
                {
                    ViewBag.error = "არსებული პაროლი არ არის სწორი";
                    return View(model);
                }
                else
                {
                    ViewBag.success = "პაროლი წარმატებით შეიცვალა";
                }

                var existingFreelancer = _db.Freelancers.FirstOrDefault(x => x.ID == editFreelancer.ID);

                

                if (model.editFreelancerModel.Password != null)
                {
                    existingFreelancer.Password = PasswordHashHelper.MD5Hash(randomSecret + model.editFreelancerModel.Password.Trim());
                    _db.SaveChanges();

                    return View(model);
                }
               

                if (model.freelancer.Name != null)
                {
                    var allowedExtensions = new[] {
                    ".Jpg", ".png", ".jpg", ".jpeg"
                    };

                    

                    try
                    {
                        if (file != null)
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            var ext = Path.GetExtension(file.FileName); //სურათის extension

                            var randomString = Guid.NewGuid().ToString("N").Substring(0, 10); //რენდომ სტრინგი სურათისთვის
                            fileName = randomString + "ID" + existingFreelancer.ID;
                            //db.ImageTbl.Remove(db.ImageTbl.Where(x => x.Id == Id).FirstOrDefault());


                            if (allowedExtensions.Contains(ext))
                            {
                                string name = Path.GetFileNameWithoutExtension(fileName);
                                var path = Path.Combine(Server.MapPath("~/img/pp"), name + ext);
                                existingFreelancer.Photo =  name + ext;
                                _db.SaveChanges();
                                file.SaveAs(path);
                                return View(model);
                            }
                        }
                    }
                    catch (DbEntityValidationException dbEx)
                    {
                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                Trace.TraceInformation("Property: {0} Error: {1}",
                                                        validationError.PropertyName,
                                                        validationError.ErrorMessage);
                            }
                        }
                    }
                    
                    

                    existingFreelancer.Name = model.freelancer.Name.Trim();
                    existingFreelancer.Surname = model.freelancer.Surname.Trim();
                    existingFreelancer.Field = model.freelancer.Field.Trim();
                    existingFreelancer.Mobile = model.freelancer.Mobile.Trim();
                    existingFreelancer.Bio = model.freelancer.Bio;
                    _db.SaveChanges();
                    return View(model);
                } 

            }
            return View(model);
        }
    }
}