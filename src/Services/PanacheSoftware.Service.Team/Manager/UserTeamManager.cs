using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using PanacheSoftware.Core.Domain.API.Join;
using PanacheSoftware.Core.Domain.API.Team;
using PanacheSoftware.Core.Domain.Join;
using PanacheSoftware.Core.Domain.Team;
using PanacheSoftware.Service.Team.Core;

namespace PanacheSoftware.Service.Team.Manager
{
    public class UserTeamManager : IUserTeamManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserTeamManager(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public TeamList GetTeamsForUser(Guid userId)
        {
            TeamList teamList = new TeamList();

            var teamHeaders = _unitOfWork.UserTeams.GetTeamsForUser(userId, true);

            foreach (var teamHeader in teamHeaders)
            {
                var teamTree = _unitOfWork.TeamHeaders.GetTeamTree(teamHeader.Id);

                if (teamTree.Any())
                {
                    var queue = new Queue<TeamHeader>();
                    queue.Enqueue(teamTree[0]);

                    while (queue.Count > 0)
                    {
                        var node = queue.Dequeue();

                        if(!teamList.TeamHeaders.Where(th => th.Id == node.Id).Any())
                            teamList.TeamHeaders.Add(_mapper.Map<TeamHead>(node));

                        foreach (var childTeam in node.ChildTeams)
                        {
                            if (!teamList.TeamHeaders.Where(th => th.Id == childTeam.Id).Any())
                                teamList.TeamHeaders.Add(_mapper.Map<TeamHead>(childTeam));

                            queue.Enqueue(childTeam);
                        }
                    }
                }
            }

            return teamList;
        }

        //public UserList GetUsersForTeam(string teamShortName)
        //{
        //    return GetUsersForTeam(_unitOfWork.TeamHeaders.TeamNameToId(teamShortName));
        //}

        //public UserList GetUsersForTeam(Guid teamHeadId)
        //{
        //    UserList userList = new UserList();

        //    var userDetails = _unitOfWork.UserTeams.GetUsersForTeam(teamHeadId, true);

        //    foreach (var userDetail in userDetails)
        //    {
        //        userList.UserProfiles.Add(_mapper.Map<UserProfile>(userDetail));
        //    }

        //    return userList;
        //}

        public UserTeamJoinList GetUserTeamListForTeam(string teamShortName)
        {
            return GetUserTeamListForTeam(_unitOfWork.TeamHeaders.TeamNameToId(teamShortName));
        }

        public UserTeamJoinList GetUserTeamListForTeam(Guid teamHeadId)
        {
            UserTeamJoinList userTeamJoinList = new UserTeamJoinList();

            userTeamJoinList.UserTeamJoins = _mapper.Map<List<UserTeamJoin>>(_unitOfWork.UserTeams.GetUserTeamsForTeam(teamHeadId, true));

            return userTeamJoinList;
        }

        //public UserTeamJoinList GetUserTeamListForUser(string userName)
        //{
        //    return GetUserTeamListForUser(_unitOfWork.UserDetails.UserNameToId(userName));
        //}

        public UserTeamJoinList GetUserTeamListForUser(Guid userId)
        {
            UserTeamJoinList userTeamJoinList = new UserTeamJoinList();

            userTeamJoinList.UserTeamJoins = _mapper.Map<List<UserTeamJoin>>(_unitOfWork.UserTeams.GetUserTeamsForUser(userId, true));

            return userTeamJoinList;
        }
    }
}
