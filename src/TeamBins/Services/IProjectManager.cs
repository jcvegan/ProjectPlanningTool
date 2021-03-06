using System;
using TeamBins.Common;
using TeamBins.DataAccess;

namespace TeamBins.Services
{
    public interface IProjectManager
    {
        //GetProjects(int teamId);

        bool DoesProjectsExist();

        int GetDefaultProjectForCurrentTeam();
    }

    public class ProjectManager : IProjectManager
    {
        
        IUserSessionHelper userSessionHelper;
        IProjectRepository projectRepository;
        ITeamRepository teamRepository;
        public ProjectManager(IProjectRepository projectRepository,IUserSessionHelper userSessionHelper,ITeamRepository teamRepository)
        {
            this.projectRepository = projectRepository;
            this.userSessionHelper = userSessionHelper;
            this.teamRepository = teamRepository;
        }

        public bool DoesProjectsExist()
        {
           return projectRepository.DoesProjectsExist(userSessionHelper.TeamId);
        }
        public int GetDefaultProjectForCurrentTeam()
        {
            MemberVM teamMember = teamRepository.GetTeamMember(userSessionHelper.TeamId, userSessionHelper.UserId);
            if (teamMember != null)
            {
                if (teamMember.DefaultProjectId!=null)
                    return teamMember.DefaultProjectId.Value;
            }
            return 0;
        }

    }
}