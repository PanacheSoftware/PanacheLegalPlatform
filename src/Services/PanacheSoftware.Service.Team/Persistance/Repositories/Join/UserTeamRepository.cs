using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PanacheSoftware.Core.Domain.Join;
using PanacheSoftware.Service.Team.Core.Repositories;
using PanacheSoftware.Service.Team.Persistance.Context;
using PanacheSoftware.Core.Domain.Team;
using PanacheSoftware.Database.Repositories;
using PanacheSoftware.Service.Team.Manager;

namespace PanacheSoftware.Service.Team.Persistance.Repositories.Join
{
    public class UserTeamRepository : PanacheSoftwareRepository<UserTeam>, IUserTeamRepository
    {
        private readonly ITeamHeaderRepository _teamHeaderRepository;

        public UserTeamRepository(PanacheSoftwareServiceTeamContext context, ITeamHeaderRepository teamHeaderRepository) : base(context)
        {
            _teamHeaderRepository = teamHeaderRepository;
        }

        public PanacheSoftwareServiceTeamContext PanacheSoftwareServiceTeamContext
        {
            get { return Context as PanacheSoftwareServiceTeamContext; }
        }

        //public List<TeamHeader> GetTeamsForUser(string userName, bool readOnly)
        //{
        //    return GetTeamsForUser(_userDetailRepository.UserNameToId(userName), readOnly);
        //}

        public List<TeamHeader> GetTeamsForUser(Guid userId, bool readOnly)
        {
            List<TeamHeader> teamHeaders = new List<TeamHeader>();
            List<UserTeam> userTeams = new List<UserTeam>();

            if (readOnly)
            {
                userTeams = PanacheSoftwareServiceTeamContext.UserTeams
                    .Where(ut => ut.UserId == userId)
                    .Include(ut => ut.TeamHeader)
                    .AsNoTracking()
                    .ToList();
            }
            else
            {
                userTeams = PanacheSoftwareServiceTeamContext.UserTeams
                    .Where(ut => ut.UserId == userId)
                    .Include(ut => ut.TeamHeader)
                    .ToList();
            }

            foreach (var userTeam in userTeams)
            {
                teamHeaders.Add(userTeam.TeamHeader);
            }

            return teamHeaders;
        }

        //public List<UserDetail> GetUsersForTeam(string teamShortName, bool readOnly)
        //{
        //    return GetUsersForTeam(_teamHeaderRepository.TeamNameToId(teamShortName), readOnly);
        //}

        //public List<UserDetail> GetUsersForTeam(Guid teamHeaderId, bool readOnly)
        //{
        //    List<UserDetail> userDetails = new List<UserDetail>();
        //    List<UserTeam> userTeams = new List<UserTeam>();

        //    if (readOnly)
        //    {
        //        userTeams = PanacheLegalUserContext.UserTeams
        //            .Where(ut => ut.TeamHeaderId == teamHeaderId)
        //            .Include(ut => ut.UserDetail)
        //            .AsNoTracking()
        //            .ToList();
        //    }
        //    else
        //    {
        //        userTeams = PanacheLegalUserContext.UserTeams
        //            .Where(ut => ut.TeamHeaderId == teamHeaderId)
        //            .Include(ut => ut.UserDetail)
        //            .ToList();
        //    }

        //    foreach (var userTeam in userTeams)
        //    {
        //        userDetails.Add(userTeam.UserDetail);
        //    }

        //    return userDetails;
        //}

        public List<UserTeam> GetUserTeamsForTeam(string teamShortName, bool readOnly)
        {
            return GetUserTeamsForTeam(_teamHeaderRepository.TeamNameToId(teamShortName), readOnly);
        }

        public List<UserTeam> GetUserTeamsForTeam(Guid teamHeaderId, bool readOnly)
        {
            List<UserTeam> userTeams = new List<UserTeam>();

            if (readOnly)
            {
                userTeams = PanacheSoftwareServiceTeamContext.UserTeams
                    .Where(ut => ut.TeamHeaderId == teamHeaderId)
                    .Include(ut => ut.TeamHeader)
                    .AsNoTracking()
                    .ToList();
            }
            else
            {
                userTeams = PanacheSoftwareServiceTeamContext.UserTeams
                    .Where(ut => ut.TeamHeaderId == teamHeaderId)
                    .Include(ut => ut.TeamHeader)
                    .ToList();
            }

            return userTeams;
        }

        //public List<UserTeam> GetUserTeamsForUser(string userName, bool readOnly)
        //{
        //    return GetUserTeamsForUser(_userDetailRepository.UserNameToId(userName), readOnly);
        //}

        public List<UserTeam> GetUserTeamsForUser(Guid userDetailId, bool readOnly)
        {
            List<UserTeam> userTeams = new List<UserTeam>();

            if (readOnly)
            {
                userTeams = PanacheSoftwareServiceTeamContext.UserTeams
                    .Where(ut => ut.UserId == userDetailId)
                    .Include(ut => ut.TeamHeader)
                    .AsNoTracking()
                    .ToList();
            }
            else
            {
                userTeams = PanacheSoftwareServiceTeamContext.UserTeams
                    .Where(ut => ut.UserId == userDetailId)
                    .Include(ut => ut.TeamHeader)
                    .ToList();
            }

            return userTeams;
        }

        public UserTeam GetUserTeamWithRelations(Guid userTeamId, bool readOnly)
        {
            if (readOnly)
                return PanacheSoftwareServiceTeamContext.UserTeams
                    .Include(u => u.TeamHeader)
                    .AsNoTracking()
                    .SingleOrDefault(u => u.Id == userTeamId);

            return PanacheSoftwareServiceTeamContext.UserTeams
                .Include(u => u.TeamHeader)
                .SingleOrDefault(u => u.Id == userTeamId);
        }
    }
}
