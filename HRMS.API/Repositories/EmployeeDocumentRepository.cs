using HRMS.API.Data;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Repositories;

public class EmployeeDocumentRepository
    : IEmployeeDocumentRepository
{
    private readonly AppDbContext context;

    public EmployeeDocumentRepository(
        AppDbContext context)
    {
        this.context = context;
    }

    public Employee? GetEmployee(
        Guid employeeId)
    {
        return context.Employees
            .FirstOrDefault(e =>
                e.Id == employeeId);
    }

    public EmployeeDocument? GetDocument(
        Guid documentId)
    {
        return context.EmployeeDocuments
            .FirstOrDefault(d =>
                d.Id == documentId);
    }

    public List<EmployeeDocument>
        GetEmployeeDocuments(Guid employeeId)
    {
        return context.EmployeeDocuments
            .Where(d =>
                d.EmployeeId == employeeId)
            .ToList();
    }

    public void AddDocument(
        EmployeeDocument document)
    {
        context.EmployeeDocuments
            .Add(document);
    }

    public void UpdateDocument(
        EmployeeDocument document)
    {
        context.EmployeeDocuments
            .Update(document);
    }

    public void DeleteDocument(
        EmployeeDocument document)
    {
        context.EmployeeDocuments
            .Remove(document);
    }

    public void SaveChanges()
    {
        context.SaveChanges();
    }
}