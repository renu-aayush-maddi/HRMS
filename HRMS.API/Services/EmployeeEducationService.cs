using ClosedXML.Excel;
using HRMS.API.Exceptions;
using HRMS.API.Interfaces;
using HRMS.API.Models.Common;
using HRMS.API.Models.DTOs.EmployeeEducation;
using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Services;

public class EmployeeEducationService : IEmployeeEducationService
{
    private readonly IEmployeeEducationRepository repository;
    private readonly IEmployeeAccessResolver accessResolver;
    private readonly IAuditLogService auditLogService;
    private readonly ILogger<EmployeeEducationService> logger;

    // private readonly IUserContextService userContextService;

    public EmployeeEducationService(
        IEmployeeEducationRepository repository,
        IEmployeeAccessResolver accessResolver,
        IAuditLogService auditLogService,
        ILogger<EmployeeEducationService> logger)
    {
        this.repository = repository;
        this.accessResolver = accessResolver;
        this.auditLogService = auditLogService;
        this.logger = logger;
        // this.userContextService = userContextService;

    }

    public async Task<EmployeeEducationResponseDto> AddEducationAsync(AddEmployeeEducationDto dto, CancellationToken cancellationToken = default)
    {
        var employeeId = await accessResolver.ResolveEmployeeIdAsync(dto.EmployeeId, cancellationToken);
        var employee = await repository.GetEmployeeAsync(employeeId, cancellationToken);

        if (employee == null) throw new NotFoundException("Employee not found.");

        var duplicateExists = await repository.EducationExistsAsync(employeeId, dto.Degree, dto.InstitutionName, dto.GraduationYear, cancellationToken);
        if (duplicateExists) throw new BusinessException("Education record already exists.");

        var education = new EmployeeEducation
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            Degree = dto.Degree,
            Specialization = dto.Specialization,
            InstitutionName = dto.InstitutionName,
            GraduationYear = dto.GraduationYear,
            Percentage = dto.Percentage,
            CreatedAt = DateTime.Now
        };

        await repository.AddEducationAsync(education, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync("Create", nameof(EmployeeEducation), education.Id, $"Education added for employee {employeeId}", cancellationToken);
        logger.LogInformation("Education {EducationId} created for employee {EmployeeId}", education.Id, employeeId);

        return MapToResponse(education);
    }

    public async Task<PagedResponse<EmployeeEducationResponseDto>> GetEducationsAsync(EmployeeEducationFilterDto filter, CancellationToken cancellationToken = default)
    {
        var employeeId = await accessResolver.ResolveEmployeeIdAsync(filter.EmployeeId, cancellationToken);
        await accessResolver.ValidateEmployeeOwnershipAsync(employeeId, cancellationToken);

        var query = repository.GetEducations().Where(x => x.EmployeeId == employeeId);

        if (!string.IsNullOrWhiteSpace(filter.Degree))
            query = query.Where(x => x.Degree != null && x.Degree.Contains(filter.Degree));

        if (!string.IsNullOrWhiteSpace(filter.InstitutionName))
            query = query.Where(x => x.InstitutionName != null && x.InstitutionName.Contains(filter.InstitutionName));

        if (filter.GraduationYear.HasValue)
            query = query.Where(x => x.GraduationYear == filter.GraduationYear);

        query = ApplySorting(query, filter);

        var totalRecords = await query.CountAsync(cancellationToken);
        var educations = await query.Skip((filter.PageNumber - 1) * filter.PageSize)
                                    .Take(filter.PageSize)
                                    .ToListAsync(cancellationToken);

        return new PagedResponse<EmployeeEducationResponseDto>
        {
            Data = educations.Select(MapToResponse).ToList(),
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)filter.PageSize)
        };
    }

    public async Task<EmployeeEducationResponseDto> UpdateEducationAsync(Guid educationId, UpdateEmployeeEducationDto dto, CancellationToken cancellationToken = default)
    {
        var education = await repository.GetEducationAsync(educationId, cancellationToken);
        if (education == null) throw new NotFoundException("Education not found.");

        await accessResolver.ValidateEmployeeOwnershipAsync(education.EmployeeId, cancellationToken);

        var duplicateExists = await repository.EducationExistsAsync(education.EmployeeId, education.Id, dto.Degree, dto.InstitutionName, dto.GraduationYear, cancellationToken);
        if (duplicateExists) throw new BusinessException("Education record already exists.");

        education.Degree = dto.Degree;
        education.Specialization = dto.Specialization;
        education.InstitutionName = dto.InstitutionName;
        education.GraduationYear = dto.GraduationYear;
        education.Percentage = dto.Percentage;

        repository.UpdateEducation(education);
        await repository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync("Update", nameof(EmployeeEducation), education.Id, $"Education updated for employee {education.EmployeeId}", cancellationToken);
        logger.LogInformation("Education {EducationId} updated", education.Id);

        return MapToResponse(education);
    }

    public async Task DeleteEducationAsync(Guid educationId, CancellationToken cancellationToken = default)
    {
        var education = await repository.GetEducationAsync(educationId, cancellationToken);
        if (education == null) throw new NotFoundException("Education not found.");

        await accessResolver.ValidateEmployeeOwnershipAsync(education.EmployeeId, cancellationToken);

        repository.DeleteEducation(education);
        await repository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync("Delete", nameof(EmployeeEducation), education.Id, $"Education deleted for employee {education.EmployeeId}", cancellationToken);
        logger.LogInformation("Education {EducationId} deleted", education.Id);
    }

    public async Task<byte[]> ExportEducationsAsync(EmployeeEducationFilterDto filter, CancellationToken cancellationToken = default)
    {
        // var employeeId = await accessResolver.ResolveEmployeeIdAsync(filter.EmployeeId, cancellationToken);
        // await accessResolver.ValidateEmployeeOwnershipAsync(employeeId, cancellationToken);

        var educations = await repository.GetEducations()
                                        .OrderByDescending(x => x.GraduationYear)
                                         .ToListAsync(cancellationToken);
                                        //  .Where(x => x.EmployeeId == employeeId)
                                        //  .OrderByDescending(x => x.GraduationYear)
                                        //  .ToListAsync(cancellationToken);

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Educations");

        worksheet.Cell(1, 1).Value = "Degree";
        worksheet.Cell(1, 2).Value = "Specialization";
        worksheet.Cell(1, 3).Value = "Institution";
        worksheet.Cell(1, 4).Value = "Graduation Year";
        worksheet.Cell(1, 5).Value = "Percentage";

        int row = 2;
        foreach (var e in educations)
        {
            worksheet.Cell(row, 1).Value = e.Degree;
            worksheet.Cell(row, 2).Value = e.Specialization;
            worksheet.Cell(row, 3).Value = e.InstitutionName;
            worksheet.Cell(row, 4).Value = e.GraduationYear;
            worksheet.Cell(row, 5).Value = e.Percentage;
            row++;
        }

        worksheet.Columns().AdjustToContents();
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        // await auditLogService.LogAsync("Export", nameof(EmployeeEducation), employeeId, "Education records exported", cancellationToken);
        return stream.ToArray();
    }

    public async Task<EmployeeEducationImportResultDto> ImportEducationsAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0) throw new BusinessException("Excel file is required.");

        var result = new EmployeeEducationImportResultDto();
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream, cancellationToken);

        using var workbook = new XLWorkbook(stream);
        var rows = workbook.Worksheet(1).RowsUsed().Skip(1).ToList();
        result.TotalRows = rows.Count;

        foreach (var row in rows)
        {
            try
            {
                var dto = new AddEmployeeEducationDto
                {
                    EmployeeId = Guid.Parse(row.Cell(1).GetString()),
                    Degree = row.Cell(2).GetString(),
                    Specialization = row.Cell(3).GetString(),
                    InstitutionName = row.Cell(4).GetString(),
                    GraduationYear = int.Parse(row.Cell(5).GetString()),
                    Percentage = decimal.Parse(row.Cell(6).GetString())
                };

                await AddEducationAsync(dto, cancellationToken);
                result.SuccessCount++;
            }
            catch (Exception ex)
            {
                result.FailedCount++;
                result.Errors.Add($"Row {row.RowNumber()}: {ex.Message}");
            }
        }

        await auditLogService.LogAsync("Import", nameof(EmployeeEducation), Guid.Empty, $"{result.SuccessCount} education records imported", cancellationToken);
        return result;
    }

    private static EmployeeEducationResponseDto MapToResponse(EmployeeEducation e) => new()
    {
        Id = e.Id,
        EmployeeId = e.EmployeeId,
        Degree = e.Degree ?? string.Empty,
        Specialization = e.Specialization ?? string.Empty,
        InstitutionName = e.InstitutionName ?? string.Empty,
        GraduationYear = e.GraduationYear ??0,
        Percentage = e.Percentage
    };

    private static IQueryable<EmployeeEducation> ApplySorting(IQueryable<EmployeeEducation> query, EmployeeEducationFilterDto filter)
    {
        return filter.SortBy?.ToLower() switch
        {
            "degree" => filter.Descending ? query.OrderByDescending(x => x.Degree) : query.OrderBy(x => x.Degree),
            "institutionname" => filter.Descending ? query.OrderByDescending(x => x.InstitutionName) : query.OrderBy(x => x.InstitutionName),
            "graduationyear" => filter.Descending ? query.OrderByDescending(x => x.GraduationYear) : query.OrderBy(x => x.GraduationYear),
            _ => query.OrderByDescending(x => x.CreatedAt)
        };
    }
}