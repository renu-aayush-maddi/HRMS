using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface IReviewRepository
{
    Employee? GetEmployee(Guid employeeId);

    List<PerformanceReview> GetAllReviews();

    List<PerformanceReview> GetEmployeeReviews(Guid employeeId);

    void AddReview(PerformanceReview review);

    void SaveChanges();

    Employee? GetEmployeeByUserId(Guid userId);

    PerformanceCycle? GetCycle(Guid cycleId);

    
}