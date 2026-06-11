using HRMS.API.Models.DTOs.LeaveType;

namespace HRMS.API.Interfaces;

public interface ILeaveTypeService
{
    List<LeaveTypeResponseDto> GetAll();

    void Add(AddLeaveTypeDto dto);

    void Update(Guid id,UpdateLeaveTypeDto dto);

    void Delete(Guid id);
}