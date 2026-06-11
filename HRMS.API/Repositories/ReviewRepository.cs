using HRMS.API.Data;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly AppDbContext context;

    public ReviewRepository(AppDbContext context)
    {
        this.context = context;
    }

    public Employee? GetEmployee(Guid employeeId)
    {
        return context.Employees
            .FirstOrDefault(e => e.Id == employeeId);
    }

    public List<PerformanceReview> GetAllReviews()
    {
        return context.PerformanceReviews
            .Include(r => r.Employee)
            .Include(r => r.Reviewer)
            .ToList();
    }

    public List<PerformanceReview>
        GetEmployeeReviews(Guid employeeId)
    {
        return context.PerformanceReviews
            .Include(r => r.Employee)
            .Include(r => r.Reviewer)
            .Where(r => r.EmployeeId == employeeId)
            .ToList();
    }

    public void AddReview(PerformanceReview review)
    {
        context.PerformanceReviews.Add(review);
    }

    public void SaveChanges()
    {
        context.SaveChanges();
    }
}