using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface ILeaveTypeRepository
{
    List<LeaveType> GetAll();

    LeaveType? GetById(Guid id);

    bool Exists(string name);

    void Add(LeaveType leaveType);

    void Update(LeaveType leaveType);

    void Delete(LeaveType leaveType);

    bool IsUsed(Guid leaveTypeId);

    void SaveChanges();
}