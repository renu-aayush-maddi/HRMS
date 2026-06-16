using HRMS.API.Data;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Repositories;

public class EmployeeDocumentRepository : IEmployeeDocumentRepository
{
    private readonly AppDbContext context;

    public EmployeeDocumentRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<Employee?> GetEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return await context.Employees
            .FirstOrDefaultAsync(x => x.Id == employeeId && !x.IsDeleted, cancellationToken);
    }

    public async Task<EmployeeDocument?> GetDocumentAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        return await context.EmployeeDocuments
            .FirstOrDefaultAsync(x => x.Id == documentId, cancellationToken);
    }

    public IQueryable<EmployeeDocument> GetDocuments()
    {
        return context.EmployeeDocuments.AsNoTracking();
    }

    public async Task<bool> DocumentExistsAsync(Guid employeeId, string documentType, CancellationToken cancellationToken = default)
    {
        return await context.EmployeeDocuments
            .AnyAsync(x => x.EmployeeId == employeeId && x.DocumentType == documentType, cancellationToken);
    }

    public async Task AddDocumentAsync(EmployeeDocument document, CancellationToken cancellationToken = default)
    {
        await context.EmployeeDocuments.AddAsync(document, cancellationToken);
    }

    public void UpdateDocument(EmployeeDocument document)
    {
        context.EmployeeDocuments.Update(document);
    }

    public void DeleteDocument(EmployeeDocument document)
    {
        context.EmployeeDocuments.Remove(document);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}