using HRMS.API.Data;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;

namespace HRMS.API.Repositories;

public class LeaveTypeRepository: ILeaveTypeRepository
{
    private readonly AppDbContext context;

    public LeaveTypeRepository(AppDbContext context)
    {
        this.context = context;
    }

    public List<LeaveType> GetAll()
    {
        return context.LeaveTypes.ToList();
    }

    public LeaveType? GetById(Guid id)
    {
        return context.LeaveTypes.FirstOrDefault(x => x.Id == id);
    }

    public bool Exists(string name)
    {
        return context.LeaveTypes
            .Any(x => x.Name.ToLower() == name.ToLower());
    }

    public void Add(LeaveType leaveType)
    {
        context.LeaveTypes.Add(leaveType);
    }

    public void Update(LeaveType leaveType)
    {
        context.LeaveTypes.Update(leaveType);
    }

    public void Delete(LeaveType leaveType)
    {
        context.LeaveTypes.Remove(leaveType);
    }

    public void SaveChanges()
    {
        context.SaveChanges();
    }

    public bool IsUsed(Guid leaveTypeId)
    {
        return context.EmployeeLeaveBalances
            .Any(x => x.LeaveTypeId == leaveTypeId)
            ||
            context.LeaveRequests
            .Any(x => x.LeaveTypeId == leaveTypeId);
    }
}