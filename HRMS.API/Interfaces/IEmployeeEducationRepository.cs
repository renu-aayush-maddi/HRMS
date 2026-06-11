using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface IEmployeeEducationRepository
{
    Employee? GetEmployee(Guid employeeId);

    EmployeeEducation? GetEducation(Guid id);

    List<EmployeeEducation>
        GetEmployeeEducations(Guid employeeId);

    void AddEducation(
        EmployeeEducation education);

    void UpdateEducation(
        EmployeeEducation education);

    void DeleteEducation(
        EmployeeEducation education);

    void SaveChanges();
}