using ClosedXML.Excel;
using HRMS.API.Exceptions;
using HRMS.API.Interfaces;
using HRMS.API.Models.Common;
using HRMS.API.Models.DTOs.EmployeeEmergencyContact;
using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Services;

public class EmployeeEmergencyContactService : IEmployeeEmergencyContactService
{
    private readonly IEmployeeEmergencyContactRepository repository;
    private readonly IEmployeeAccessResolver accessResolver;
    private readonly IAuditLogService auditLogService;
    private readonly ILogger<EmployeeEmergencyContactService> logger;

    public EmployeeEmergencyContactService(
        IEmployeeEmergencyContactRepository repository,
        IEmployeeAccessResolver accessResolver,
        IAuditLogService auditLogService,
        ILogger<EmployeeEmergencyContactService> logger)
    {
        this.repository = repository;
        this.accessResolver = accessResolver;
        this.auditLogService = auditLogService;
        this.logger = logger;
    }

    public async Task<EmployeeEmergencyContactResponseDto> AddContactAsync(AddEmployeeEmergencyContactDto dto, CancellationToken cancellationToken = default)
    {
        var employeeId = await accessResolver.ResolveEmployeeIdAsync(dto.EmployeeId, cancellationToken);
        var employee = await repository.GetEmployeeAsync(employeeId, cancellationToken);

        if (employee == null) throw new NotFoundException("Employee not found.");

        var count = await repository.GetContactCountAsync(employeeId, cancellationToken);
        if (count >= 3) throw new BusinessException("Maximum 3 emergency contacts are allowed.");

        var phoneExists = await repository.PhoneExistsAsync(employeeId, dto.Phone, cancellationToken);
        if (phoneExists) throw new BusinessException("Emergency contact phone number already exists.");

        var contact = new EmployeeEmergencyContact
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            ContactName = dto.ContactName,
            Relationship = dto.Relationship,
            Phone = dto.Phone,
            Email = dto.Email,
            CreatedAt = DateTime.Now
        };

        await repository.AddContactAsync(contact, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync("Create", nameof(EmployeeEmergencyContact), contact.Id, $"Emergency contact added for employee {employeeId}", cancellationToken);
        logger.LogInformation("Emergency contact {ContactId} created for employee {EmployeeId}", contact.Id, employeeId);

        return MapToResponse(contact);
    }

    public async Task<PagedResponse<EmployeeEmergencyContactResponseDto>> GetContactsAsync(EmployeeEmergencyContactFilterDto filter, CancellationToken cancellationToken = default)
    {
        var employeeId = await accessResolver.ResolveEmployeeIdAsync(filter.EmployeeId, cancellationToken);
        await accessResolver.ValidateEmployeeOwnershipAsync(employeeId, cancellationToken);

        var query = repository.GetContacts().Where(x => x.EmployeeId == employeeId);

        if (!string.IsNullOrWhiteSpace(filter.ContactName))
            query = query.Where(x => x.ContactName.Contains(filter.ContactName));

        if (!string.IsNullOrWhiteSpace(filter.Relationship))
            query = query.Where(x => x.Relationship != null && x.Relationship.Contains(filter.Relationship));

        query = ApplySorting(query, filter);

        var totalRecords = await query.CountAsync(cancellationToken);
        var contacts = await query.Skip((filter.PageNumber - 1) * filter.PageSize)
                                  .Take(filter.PageSize)
                                  .ToListAsync(cancellationToken);

        return new PagedResponse<EmployeeEmergencyContactResponseDto>
        {
            Data = contacts.Select(MapToResponse).ToList(),
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)filter.PageSize)
        };
    }

    public async Task<EmployeeEmergencyContactResponseDto> UpdateContactAsync(Guid contactId, UpdateEmployeeEmergencyContactDto dto, CancellationToken cancellationToken = default)
    {
        var contact = await repository.GetContactAsync(contactId, cancellationToken);
        if (contact == null) throw new NotFoundException("Contact not found.");

        await accessResolver.ValidateEmployeeOwnershipAsync(contact.EmployeeId, cancellationToken);

        var phoneExists = await repository.PhoneExistsAsync(contact.EmployeeId, contact.Id, dto.Phone, cancellationToken);
        if (phoneExists) throw new BusinessException("Emergency contact phone number already exists.");

        contact.ContactName = dto.ContactName;
        contact.Relationship = dto.Relationship;
        contact.Phone = dto.Phone;
        contact.Email = dto.Email;

        repository.UpdateContact(contact);
        await repository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync("Update", nameof(EmployeeEmergencyContact), contact.Id, $"Emergency contact updated for employee {contact.EmployeeId}", cancellationToken);
        return MapToResponse(contact);
    }

    public async Task DeleteContactAsync(Guid contactId, CancellationToken cancellationToken = default)
    {
        var contact = await repository.GetContactAsync(contactId, cancellationToken);
        if (contact == null) throw new NotFoundException("Contact not found.");

        await accessResolver.ValidateEmployeeOwnershipAsync(contact.EmployeeId, cancellationToken);

        repository.DeleteContact(contact);
        await repository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync("Delete", nameof(EmployeeEmergencyContact), contact.Id, $"Emergency contact deleted for employee {contact.EmployeeId}", cancellationToken);
    }

    private static EmployeeEmergencyContactResponseDto MapToResponse(EmployeeEmergencyContact contact) => new()
    {
        Id = contact.Id,
        EmployeeId = contact.EmployeeId,
        ContactName = contact.ContactName,
        Relationship = contact.Relationship ?? string.Empty,
        Phone = contact.Phone ?? string.Empty,
        Email = contact.Email ?? string.Empty
    };

    private static IQueryable<EmployeeEmergencyContact> ApplySorting(IQueryable<EmployeeEmergencyContact> query, EmployeeEmergencyContactFilterDto filter)
    {
        return filter.SortBy?.ToLower() switch
        {
            "contactname" => filter.Descending ? query.OrderByDescending(x => x.ContactName) : query.OrderBy(x => x.ContactName),
            "relationship" => filter.Descending ? query.OrderByDescending(x => x.Relationship) : query.OrderBy(x => x.Relationship),
            _ => query.OrderByDescending(x => x.CreatedAt)
        };
    }

    public async Task<byte[]> ExportContactsAsync(EmployeeEmergencyContactFilterDto filter, CancellationToken cancellationToken = default)
    {
        var employeeId = await accessResolver.ResolveEmployeeIdAsync(filter.EmployeeId, cancellationToken);
        await accessResolver.ValidateEmployeeOwnershipAsync(employeeId, cancellationToken);

        var contacts = await repository.GetContacts()
            .Where(x => x.EmployeeId == employeeId)
            .OrderBy(x => x.ContactName)
            .ToListAsync(cancellationToken);

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("EmergencyContacts");

        worksheet.Cell(1, 1).Value = "Contact Name";
        worksheet.Cell(1, 2).Value = "Relationship";
        worksheet.Cell(1, 3).Value = "Phone";
        worksheet.Cell(1, 4).Value = "Email";

        var row = 2;
        foreach (var contact in contacts)
        {
            worksheet.Cell(row, 1).Value = contact.ContactName;
            worksheet.Cell(row, 2).Value = contact.Relationship;
            worksheet.Cell(row, 3).Value = contact.Phone;
            worksheet.Cell(row, 4).Value = contact.Email;
            row++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        await auditLogService.LogAsync(
            "Export",
            nameof(EmployeeEmergencyContact),
            employeeId,
            "Emergency contacts exported",
            cancellationToken);

        return stream.ToArray();
    }

    public async Task<EmployeeEmergencyContactImportResultDto> ImportContactsAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
        {
            throw new BusinessException("Excel file is required.");
        }

        var result = new EmployeeEmergencyContactImportResultDto();
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream, cancellationToken);

        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheet(1);
        var rows = worksheet.RowsUsed().Skip(1).ToList();

        result.TotalRows = rows.Count;

        foreach (var row in rows)
        {
            try
            {
                var dto = new AddEmployeeEmergencyContactDto
                {
                    EmployeeId = Guid.Parse(row.Cell(1).GetString()),
                    ContactName = row.Cell(2).GetString(),
                    Relationship = row.Cell(3).GetString(),
                    Phone = row.Cell(4).GetString(),
                    Email = row.Cell(5).GetString()
                };

                await AddContactAsync(dto, cancellationToken);
                result.SuccessCount++;
            }
            catch (Exception ex)
            {
                result.FailedCount++;
                result.Errors.Add($"Row {row.RowNumber()}: {ex.Message}");
            }
        }

        await auditLogService.LogAsync(
            "Import",
            nameof(EmployeeEmergencyContact),
            Guid.Empty,
            $"{result.SuccessCount} emergency contacts imported",
            cancellationToken);

        return result;
    }


}