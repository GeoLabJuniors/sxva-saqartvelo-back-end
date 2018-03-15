﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using sxva_saqartvelo_back_end.Models;
using sxva_saqartvelo_back_end.Filters;
using PagedList.Mvc;
using PagedList;

namespace sxva_saqartvelo_back_end.Controllers
{
    public class FreelancerController : Controller
    {

        OtherGeorgiaEntities _db = new OtherGeorgiaEntities();

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

                if(freelancers.Count < 1) return PartialView("_PartialFilterData", freelancers.Distinct());

            }
            //



            //For CheckBox Filter
            if (CheckedSkills != null && CheckedSkills.Count() > 0)
            {
                parametersExist = true;

                foreach (int skillID in CheckedSkills)
                {
                    //var SkillIds = _db.Skills.FirstOrDefault(x => x.ID.Equals(skills)).ID;
                    //var result = _db.Freelancers.Where(x => x.Freelancer_Skill.Any(e => e.SkillID == SkillIds)).ToList();


                    if (freelancers.Count < 1)
                    {
                        //foreach (var f in result)
                        //{
                        //    freelancers.Add(f);
                        //}
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
    }
}






//FilterFreelancerByCheckBox
//public PartialViewResult FilterFreelancerByCheckBox(string[] skills)
//{
//    var checkboxResult = new List<Freelancer>();


//    foreach (string skillName in skills)
//    {
//        var SkillIds = _db.Skills.FirstOrDefault(x => x.Name.Equals(skillName)).ID;
//        var freelancers = _db.Freelancers.Where(x => x.Freelancer_Skill.Any(e=> e.SkillID == SkillIds)).ToList();
//        foreach(var f in freelancers)
//        {
//            checkboxResult.Add(f);   
//        }
//    }

//    return PartialView("_PartialFilterData", checkboxResult.Distinct());
//}
