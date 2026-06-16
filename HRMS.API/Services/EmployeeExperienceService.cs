using ClosedXML.Excel;
using HRMS.API.Exceptions;
using HRMS.API.Interfaces;
using HRMS.API.Models.Common;
using HRMS.API.Models.DTOs.EmployeeExperience;
using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Services;

public class EmployeeExperienceService : IEmployeeExperienceService
{
    private readonly IEmployeeExperienceRepository repository;
    private readonly IEmployeeAccessResolver accessResolver;
    private readonly IAuditLogService auditLogService;
    private readonly ILogger<EmployeeExperienceService> logger;

    public EmployeeExperienceService(
        IEmployeeExperienceRepository repository,
        IEmployeeAccessResolver accessResolver,
        IAuditLogService auditLogService,
        ILogger<EmployeeExperienceService> logger)
    {
        this.repository = repository;
        this.accessResolver = accessResolver;
        this.auditLogService = auditLogService;
        this.logger = logger;
    }

    public async Task<EmployeeExperienceResponseDto> AddExperienceAsync(AddEmployeeExperienceDto dto, CancellationToken cancellationToken = default)
    {
        var employeeId = await accessResolver.ResolveEmployeeIdAsync(dto.EmployeeId, cancellationToken);
        var employee = await repository.GetEmployeeAsync(employeeId, cancellationToken);

        if (employee == null) throw new NotFoundException("Employee not found.");

        var exists = await repository.ExperienceExistsAsync(employeeId, dto.CompanyName, dto.Designation, dto.StartDate, cancellationToken);
        if (exists) throw new BusinessException("Experience record already exists.");

        var experience = new EmployeeExperience
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            CompanyName = dto.CompanyName,
            Designation = dto.Designation,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Responsibilities = dto.Responsibilities,
            CreatedAt = DateTime.Now
        };

        await repository.AddExperienceAsync(experience, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync("Create", nameof(EmployeeExperience), experience.Id, $"Experience added for employee {employeeId}", cancellationToken);
        logger.LogInformation("Experience {ExperienceId} created for employee {EmployeeId}", experience.Id, employeeId);

        return MapToResponse(experience);
    }

    public async Task<PagedResponse<EmployeeExperienceResponseDto>> GetExperiencesAsync(EmployeeExperienceFilterDto filter, CancellationToken cancellationToken = default)
    {
        var employeeId = await accessResolver.ResolveEmployeeIdAsync(filter.EmployeeId, cancellationToken);
        await accessResolver.ValidateEmployeeOwnershipAsync(employeeId, cancellationToken);

        var query = repository.GetExperiences().Where(x => x.EmployeeId == employeeId);

        if (!string.IsNullOrWhiteSpace(filter.CompanyName))
            query = query.Where(x => x.CompanyName != null && x.CompanyName.Contains(filter.CompanyName));

        if (!string.IsNullOrWhiteSpace(filter.Designation))
            query = query.Where(x => x.Designation != null && x.Designation.Contains(filter.Designation));

        query = ApplySorting(query, filter);

        var totalRecords = await query.CountAsync(cancellationToken);
        var experiences = await query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync(cancellationToken);

        return new PagedResponse<EmployeeExperienceResponseDto>
        {
            Data = experiences.Select(MapToResponse).ToList(),
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)filter.PageSize)
        };
    }

    public async Task<EmployeeExperienceResponseDto> UpdateExperienceAsync(Guid experienceId, UpdateEmployeeExperienceDto dto, CancellationToken cancellationToken = default)
    {
        var experience = await repository.GetExperienceAsync(experienceId, cancellationToken);
        if (experience == null) throw new NotFoundException("Experience not found.");

        await accessResolver.ValidateEmployeeOwnershipAsync(experience.EmployeeId, cancellationToken);

        var exists = await repository.ExperienceExistsAsync(experience.EmployeeId, experience.Id, dto.CompanyName, dto.Designation, dto.StartDate, cancellationToken);
        if (exists) throw new BusinessException("Experience record already exists.");

        experience.CompanyName = dto.CompanyName;
        experience.Designation = dto.Designation;
        experience.StartDate = dto.StartDate;
        experience.EndDate = dto.EndDate;
        experience.Responsibilities = dto.Responsibilities;

        repository.UpdateExperience(experience);
        await repository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync("Update", nameof(EmployeeExperience), experience.Id, $"Experience updated for employee {experience.EmployeeId}", cancellationToken);

        return MapToResponse(experience);
    }

    public async Task DeleteExperienceAsync(Guid experienceId, CancellationToken cancellationToken = default)
    {
        var experience = await repository.GetExperienceAsync(experienceId, cancellationToken);
        if (experience == null) throw new NotFoundException("Experience not found.");

        await accessResolver.ValidateEmployeeOwnershipAsync(experience.EmployeeId, cancellationToken);

        repository.DeleteExperience(experience);
        await repository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync("Delete", nameof(EmployeeExperience), experience.Id, $"Experience deleted for employee {experience.EmployeeId}", cancellationToken);
    }

    public async Task<byte[]> ExportExperiencesAsync(EmployeeExperienceFilterDto filter, CancellationToken cancellationToken = default)
    {
        var employeeId = await accessResolver.ResolveEmployeeIdAsync(filter.EmployeeId, cancellationToken);
        await accessResolver.ValidateEmployeeOwnershipAsync(employeeId, cancellationToken);

        var experiences = await repository.GetExperiences()
            .Where(x => x.EmployeeId == employeeId)
            .OrderByDescending(x => x.StartDate)
            .ToListAsync(cancellationToken);

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Experiences");

        worksheet.Cell(1, 1).Value = "Company";
        worksheet.Cell(1, 2).Value = "Designation";
        worksheet.Cell(1, 3).Value = "Start Date";
        worksheet.Cell(1, 4).Value = "End Date";
        worksheet.Cell(1, 5).Value = "Responsibilities";

        int row = 2;
        foreach (var experience in experiences)
        {
            worksheet.Cell(row, 1).Value = experience.CompanyName;
            worksheet.Cell(row, 2).Value = experience.Designation;
            worksheet.Cell(row, 3).Value = experience.StartDate?.ToString();
            worksheet.Cell(row, 4).Value = experience.EndDate?.ToString();
            worksheet.Cell(row, 5).Value = experience.Responsibilities;
            row++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        await auditLogService.LogAsync("Export", nameof(EmployeeExperience), employeeId, "Experience records exported", cancellationToken);
        return stream.ToArray();
    }

    public async Task<EmployeeExperienceImportResultDto> ImportExperiencesAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0) throw new BusinessException("Excel file is required.");

        var result = new EmployeeExperienceImportResultDto();
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
                var dto = new AddEmployeeExperienceDto
                {
                    EmployeeId = Guid.Parse(row.Cell(1).GetString()),
                    CompanyName = row.Cell(2).GetString(),
                    Designation = row.Cell(3).GetString(),
                    StartDate = DateOnly.Parse(row.Cell(4).GetString()),
                    EndDate = string.IsNullOrWhiteSpace(row.Cell(5).GetString()) ? null : DateOnly.Parse(row.Cell(5).GetString()),
                    Responsibilities = row.Cell(6).GetString()
                };

                await AddExperienceAsync(dto, cancellationToken);
                result.SuccessCount++;
            }
            catch (Exception ex)
            {
                result.FailedCount++;
                result.Errors.Add($"Row {row.RowNumber()}: {ex.Message}");
            }
        }

        await auditLogService.LogAsync("Import", nameof(EmployeeExperience), Guid.Empty, $"{result.SuccessCount} experience records imported", cancellationToken);
        return result;
    }

    private static EmployeeExperienceResponseDto MapToResponse(EmployeeExperience e) => new()
    {
        Id = e.Id,
        EmployeeId = e.EmployeeId,
        CompanyName = e.CompanyName ?? string.Empty,
        Designation = e.Designation ?? string.Empty,
        StartDate = e.StartDate,
        EndDate = e.EndDate,
        Responsibilities = e.Responsibilities
    };

    private static IQueryable<EmployeeExperience> ApplySorting(IQueryable<EmployeeExperience> query, EmployeeExperienceFilterDto filter)
    {
        return filter.SortBy?.ToLower() switch
        {
            "companyname" => filter.Descending ? query.OrderByDescending(x => x.CompanyName) : query.OrderBy(x => x.CompanyName),
            "designation" => filter.Descending ? query.OrderByDescending(x => x.Designation) : query.OrderBy(x => x.Designation),
            "startdate" => filter.Descending ? query.OrderByDescending(x => x.StartDate) : query.OrderBy(x => x.StartDate),
            _ => query.OrderByDescending(x => x.CreatedAt)
        };
    }
}