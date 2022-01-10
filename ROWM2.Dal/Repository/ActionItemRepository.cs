using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    public interface IActionItemRepository
    {
        Task<IEnumerable<Action_Item>> GetActionItems(string parelId);
        Task<IEnumerable<Action_Item>> GetActionItemsWithHistory(string parelId);
        Task<Action_Item> GetActionItem(Guid itemId);
        Task<Action_Item> GetFullItem(Guid itemId);
        Task<IEnumerable<Action_Item>> AddActionItem(string parelId, Action_Item item, DateTimeOffset activityDate, Guid? agentId);
        Task<Action_Item> UpdateActionItem(Action_Item item, DateTimeOffset activityDate, Guid? agentId);
        Task<IEnumerable<Action_Item_Group>> GetGroups();

        #region workaround
        Task<IEnumerable<Agent>> GetAgents();
        Task<Agent> UpdateAgent(Agent a);
        Task<Agent> TryGetAgent(Guid agentId);
        #endregion
    }

    public class ActionItemNoOp : IActionItemRepository
    {
        public Task<IEnumerable<Action_Item>> AddActionItem(string parelId, Action_Item item, DateTimeOffset activityDate, Guid? agentId) => Task.FromResult(Enumerable.Empty<Action_Item>());

        public Task<Action_Item> GetActionItem(Guid itemId) => default;

        public Task<Action_Item> GetFullItem(Guid itemId) => default;

        public Task<IEnumerable<Action_Item>> GetActionItems(string parelId) => Task.FromResult(System.Linq.Enumerable.Empty<Action_Item>());

        public Task<IEnumerable<Action_Item>> GetActionItemsWithHistory(string parelId)
        {
            throw new NotImplementedException();
        }

        public Task<Action_Item> UpdateActionItem(Action_Item item, DateTimeOffset activityDate, Guid? agentId) => default;

        public Task<IEnumerable<Agent>> GetAgents()
        {
            throw new NotImplementedException();
        }

        public Task<Agent> UpdateAgent(Agent a)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Action_Item_Group>> GetGroups()
        {
            return Task.FromResult<IEnumerable<Action_Item_Group>>(Enumerable.Empty<Action_Item_Group>());
        }

        public Task<Agent> TryGetAgent(Guid agentId)
        {
            throw new NotImplementedException();
        }
    }

    public class ActionItemRepository : IActionItemRepository
    {
        readonly ROWM_Context _ctx;
        readonly OwnerRepository _ownerRepository;

        public ActionItemRepository(ROWM_Context context, OwnerRepository ownerRepository) => (_ctx, _ownerRepository) = (context, ownerRepository);

        public async Task<IEnumerable<Action_Item>> AddActionItem(string parelId, Action_Item item, DateTimeOffset activityDate, Guid? agentId)
        {
            var p = await _ownerRepository.GetParcel(parelId);
            if (p == null)
                throw new KeyNotFoundException();

            item.ParcelId = p.ParcelId;
            item.Action_Item_Activity = new List<Action_Item_Activity>()
            {
                new Action_Item_Activity
                {
                    Status = item.Status,
                    DueDate = item.DueDate,
                    Assigned = item.AssignedGroupId.Value,
                    ActivityDate = activityDate,
                    UpdateAgentId = agentId
                }
            };

            _ctx.Action_Item.Add(item);

            try
            {
                var touched = await _ctx.SaveChangesAsync();
                if (touched <= 0)
                    Trace.TraceWarning("write failed");

                return p.Action_Item;
            }
            catch ( Exception e)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Action_Item>> GetActionItems(string parelId)
        {
            var p = await _ownerRepository.GetParcel(parelId);
            if (p == null)
                throw new KeyNotFoundException();

            return p.Action_Item;
        }

        public Task<IEnumerable<Action_Item>> GetActionItemsWithHistory(string parelId)
        {
            throw new NotImplementedException();
        }

        public async Task<Action_Item> GetActionItem(Guid itemId) => await _ctx.Action_Item.FindAsync(itemId);
        public async Task<Action_Item> GetFullItem(Guid itemId)
        {
            try
            {
                return await _ctx.Action_Item
                    //.Include(ax => ax.ParentParcel)
                    .Include(ax => ax.Action_Item_Group.Action_Item_Group_Member)
                    .Include(ax => ax.Action_Item_Activity)
                    //.Include(ax => ax.Action_Item_Activity.Select(aa => aa.UpdateAgent))
                    .SingleOrDefaultAsync(ax => ax.ActionItemId == itemId);
            }
            catch ( Exception e)
            {
                throw;
            }
        }

        public async Task<Action_Item> UpdateActionItem(Action_Item item, DateTimeOffset activityDate, Guid? agentId)
        {
            _ = item ?? throw new ArgumentNullException(nameof(item));

            var axt = new Action_Item_Activity
            {
                Action = item.Action,
                Status = item.Status,
                DueDate = item.DueDate,
                Assigned = item.AssignedGroupId.Value,
                ActivityDate = activityDate,
                UpdateAgentId = agentId
            };

            var acts = item.Action_Item_Activity ?? (item.Action_Item_Activity = new List<Action_Item_Activity>());

            if (acts.Any())
            {
                var last = item.Action_Item_Activity.OrderByDescending(h => h.ActivityDate).First();
                if (last.Status != item.Status) 
                    axt.OriginalStatus = last.Status;                    

                if (last.DueDate != item.DueDate)
                    axt.OriginalDueDate = last.DueDate;

                if (last.Assigned != item.AssignedGroupId)
                    axt.OriginalAssigned = last.Assigned;

                if (last.Action != item.Action)
                    axt.OriginalAction = last.Action;
            }

            item.LastModified = DateTimeOffset.UtcNow;
            acts.Add(axt);

            try
            {
                var touched = await _ctx.SaveChangesAsync();
                if (touched <= 0)
                    Trace.TraceWarning("write failed");

                return item;
            }
            catch( Exception e)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Action_Item_Group>> GetGroups() =>
            await _ctx.Action_Item_Group.AsNoTracking().ToArrayAsync();

        #region helper
        public async Task<IEnumerable<Agent>> GetAgents() =>
            await _ctx.Agent.AsNoTracking().ToArrayAsync();

        public async Task<Agent> TryGetAgent(Guid agentId) =>
            await _ctx.Agent.FindAsync(agentId);

        public async Task<Agent> UpdateAgent(Agent a)
        {
            if (_ctx.Entry(a).State == EntityState.Detached)
            {
                _ctx.Agent.Attach(a);
                _ctx.Entry(a).State = EntityState.Modified;
            }

            await _ctx.SaveChangesAsync();
            return a;
        }
        #endregion
    }
}
