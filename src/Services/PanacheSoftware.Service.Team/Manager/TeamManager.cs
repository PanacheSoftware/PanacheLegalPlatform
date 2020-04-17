using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using PanacheSoftware.Core.Domain.API.Team;
using PanacheSoftware.Core.Domain.Team;
using PanacheSoftware.Service.Team.Core;

namespace PanacheSoftware.Service.Team.Manager
{
    public class TeamManager : ITeamManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TeamManager(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public List<Guid> GetChildTeamIds(Guid teamHeaderId)
        {
            List<Guid> childTeams = new List<Guid>();

            var teamTree = _unitOfWork.TeamHeaders.GetTeamTree(teamHeaderId);

            if(teamTree.Any())
            {
                var queue = new Queue<TeamHeader>();
                queue.Enqueue(teamTree[0]);

                while (queue.Count > 0)
                {
                    var node = queue.Dequeue();

                    foreach (var childTeam in node.ChildTeams)
                    {
                        if (!childTeams.Contains(childTeam.Id))
                            childTeams.Add(childTeam.Id);

                        queue.Enqueue(childTeam);
                    }
                }
            }

            return childTeams;
        }

        

        public TeamList GetTeamList(Guid teamHeaderId = default(Guid), bool validParents = false)
        {
            TeamList teamList = new TeamList();

            if (teamHeaderId == Guid.Empty)
            {
                foreach (var currentTeam in _unitOfWork.TeamHeaders.GetAll(true))
                {
                    teamList.TeamHeaders.Add(_mapper.Map<TeamHead>(currentTeam));
                }
            }
            else
            {
                var childTeams = new List<Guid>();

                //Check if we only want to return teams that would be applicable as a parent for the passed in TeamHeader Id (i.e. aren't already a child of the TeamHeader passed in)
                if (validParents)
                {
                    childTeams = GetChildTeamIds(teamHeaderId);
                    childTeams.Add(teamHeaderId);
                }

                foreach (var currentTeam in _unitOfWork.TeamHeaders.GetAll(true))
                {
                    if(!childTeams.Contains(currentTeam.Id))
                        teamList.TeamHeaders.Add(_mapper.Map<TeamHead>(currentTeam));
                }
            }

            return teamList;
        }

        public TeamList GetTeamsForUser(Guid userDetailId)
        {
            throw new NotImplementedException();
        }

        public TeamStruct GetTeamStructure(Guid teamHeaderId)
        {
            TeamStruct teamStruct = new TeamStruct();
            var teamTree = _unitOfWork.TeamHeaders.GetTeamTree(teamHeaderId);

            if (teamTree.Any())
                teamStruct = _mapper.Map<TeamStruct>(teamTree.First());

            return teamStruct;
        }

        //public UserList GetUsersForTeam(Guid teamHeaderId)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
