using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface IResignationRepository
{
    Employee? GetEmployee(Guid employeeId);

    EmployeeResignation? GetResignation(Guid id);

    List<EmployeeResignation> GetAll();

    List<EmployeeResignation>
        GetEmployeeResignations(
            Guid employeeId);

    void Add(
        EmployeeResignation resignation);

    void Update(
        EmployeeResignation resignation);

    void SaveChanges();
}