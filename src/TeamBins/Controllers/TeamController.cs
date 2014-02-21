﻿using System.Collections.Generic;
using System.Web.Mvc;
using TeamBins.DataAccess;
using TeamBins.Services;
using TechiesWeb.TeamBins.ViewModels;
using System.Linq;
using System;
using TeamBins.Helpers.Enums;
using TechiesWeb.TeamBins.Infrastructure;
namespace TechiesWeb.TeamBins.Controllers
{
    [VerifyLogin]
    public class TeamController : BaseController
    {       
       private IssueService issueService;
        public TeamController() {
            issueService=new IssueService(new Repositary(),UserID,TeamID);
        
        }

        public TeamController(IRepositary repositary)
            : base(repositary)
        {            

        }

     
        public ActionResult Index()
        {
            var teams = repo.GetTeams(UserID);
            var teamListVM = new TeamListVM();
            foreach (var team in teams)
            {
                int teamMemberCount = team.TeamMembers.Count();
                teamListVM.Teams.Add(new TeamVM { ID = team.ID, Name = team.Name ,MemberCount=teamMemberCount, IsTeamOwner=team.CreatedByID==UserID });
            }
            return View(teamListVM);
        }
       
        public ActionResult View(int id)        
        {
            
            var team = repo.GetTeam(id);
            if (team != null)
            {
                var vm = new TeamVM { ID = id, Name = team.Name };

              /*  var teamMembers = team.TeamMembers;
                foreach (var teamMember in teamMembers)
                {
                   var member=new MemberVM { Name = teamMember.User.FirstName + " " + teamMember.User.LastName, EmailAddress=teamMember.User.EmailAddress, JobTitle=teamMember.JobTitle, JoinedDate = teamMember.CreatedDate.ToShortDateString() };
                   member.EmailHash = UserService.GetImageSource(member.EmailAddress);
                   vm.Members.Add(member);
                }*/
       
                return View(vm);
            }
            return View("NotFound");
           
        }/*
        public ActionResult Create()
        {
            return View(new TeamVM());
        }*/
        public ActionResult Add()
        {
            return PartialView("Edit", new TeamVM());
        }
        public ActionResult Edit(int id)
        {
             var team = repo.GetTeam(id);
             if (team != null)
             {
                 var vm = new TeamVM { Name = team.Name, ID = team.ID }; 
                 return PartialView(vm);
             }
             return View("NotFound");
        }

        /*
        public ActionResult AddMember(int id)
        {
            var vm=new AddTeamMemberRequestVM { TeamID=id};
            return View(vm);
        }
        public ActionResult MemberInvited()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddMember(AddTeamMemberRequestVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var team = new TeamMemberRequest { EmailAddress = model.EmailAddress, CreatedByID = UserID };
                    team.TeamID = model.TeamID;
                    team.ActivationCode = model.TeamID + "-" + Guid.NewGuid().ToString("n");
                   /* var result = repo.AddTeamMemberRequest(team);
                    if (result != null)
                    {
                        return RedirectToAction("memberinvited", "Team");
                    }
                    else
                    {
                        //LOG ERROR
                    }*//*
                }
                return View(model);
            }
            catch (Exception ex)
            {
                return View(model);

            }

        }
        */
        [HttpPost]
        public ActionResult Create(TeamVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Team team = new Team { Name = model.Name, ID = model.ID };
                    bool isNew = (model.ID == 0);
                    if (!isNew)
                    {
                        team = repo.GetTeam(model.ID);
                        team.Name = model.Name;                       
                    }
                    else
                    {
                        team.CreatedByID = UserID;
                    }
                    var result = repo.SaveTeam(team);
                    if (result != null)
                    {                        
                        if (isNew)
                        {
                            var teamMember = new TeamMember { MemberID = UserID, TeamID = team.ID, CreatedByID = UserID };
                            repo.SaveTeamMember(teamMember);
                        }
                        return Json(new { Status = "Success" });
                    }
                }
                return View(model);
            }
            catch(Exception ex)
            {
                log.Error("Error updating team "+model.ID,ex);
            }
            return Json(new { Status = "Error" });
        }
        /*
        public ActionResult Created()
        {
            string strTeamId = TempData["TeamID"].ToString();
            if (!String.IsNullOrEmpty(strTeamId))
            {
                int teamId = Convert.ToInt32(strTeamId);
                var team = repo.GetTeam(teamId);
                var teamVM = new TeamVM { Name = team.Name  , ID=teamId};
                return View(teamVM);
            }
            return RedirectToAction("Index", "Dashboard");

        }

        public JsonResult NonTeamMembers(int issueId)
        {
            List<MemberVM> memberList = new List<MemberVM>();
            /*
            var members = repo.GetNonIssueMembers(TeamID, issueId);
            foreach (var member in members)
            {
                var vm = new MemberVM {MemberType = member.JobTitle, Name = member.DisplayName, MemberID=member.ID };
                vm.AvatarHash = UserService.GetImageSource(member.EmailAddress,48);
                memberList.Add(vm);
            }*//*
            return Json ( new{ Data = memberList, Status = "Success" },JsonRequestBehavior.AllowGet);
        }
        */

        // we will move this method to the web api once authentication stuff is finalized.
        public ActionResult Stream(int id,int size=25)
        {
            List<ActivityVM> list = new List<ActivityVM>();
            list = GetTeamActivityVMs(id,size);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
       
        private List<ActivityVM> GetTeamActivityVMs(int teamId, int size)
        {
            List<ActivityVM> activityVMList = new List<ActivityVM>();
            try
            {
                var activityList = repo.GetTeamActivity(teamId).OrderByDescending(s => s.CreatedDate).Take(size).ToList();

                ActivityVM activityVM = new ActivityVM();
                var issueService = new IssueService(repo, UserID, TeamID);
                issueService.SiteBaseURL = SiteBaseURL;
                var commentService = new CommentService(repo, SiteBaseURL);

                foreach (var item in activityList)
                {
                    if (item.ObjectType == ActivityObjectType.Issue.ToString())
                    {
                        activityVM = issueService.GetActivityVM(item);
                    }
                    else if (item.ObjectType == ActivityObjectType.IssueComment.ToString())
                    {
                        activityVM = commentService.GetActivityVM(item);
                    }
                    activityVMList.Add(activityVM);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return activityVMList;
        }


    }
}
