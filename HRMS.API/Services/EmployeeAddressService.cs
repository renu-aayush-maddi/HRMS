using HRMS.API.Exceptions;
using HRMS.API.Interfaces;
using HRMS.API.Models.Common;
using HRMS.API.Models.DTOs.EmployeeAddress;
using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using HRMS.API.Common.Constants;
using ClosedXML.Excel;
using FluentValidation;

namespace HRMS.API.Services;

public class EmployeeAddressService : IEmployeeAddressService
{
    private readonly IEmployeeAddressRepository repository;
    private readonly IEmployeeAccessResolver accessResolver;
    private readonly IAuditLogService auditLogService;
    private readonly ILogger<EmployeeAddressService> logger;

    public EmployeeAddressService(
        IEmployeeAddressRepository repository,
        IEmployeeAccessResolver accessResolver,
        IAuditLogService auditLogService,
        ILogger<EmployeeAddressService> logger)
    {
        this.repository = repository;
        this.accessResolver = accessResolver;
        this.auditLogService = auditLogService;
        this.logger = logger;
    }

    public async Task<EmployeeAddressResponseDto> AddAddressAsync(AddEmployeeAddressDto dto, CancellationToken cancellationToken = default)
    {
        var employeeId = await accessResolver.ResolveEmployeeIdAsync(dto.EmployeeId, cancellationToken);
        var employee = await repository.GetEmployeeAsync(employeeId, cancellationToken);

        if (employee == null)
        {
            throw new NotFoundException("Employee not found.");
        }

        if (dto.AddressType == AddressTypes.Permanent)
        {
            var permanentExists =
                await repository.AddressTypeExistsAsync(
                    employeeId,
                    AddressTypes.Permanent,
                    cancellationToken);

            if (permanentExists)
            {
                throw new BusinessException("Permanent address already exists.");
            }
        }

        if (dto.AddressType == AddressTypes.Current)
        {
            var currentExists =
                await repository.AddressTypeExistsAsync(
                    employeeId,
                    AddressTypes.Current,
                    cancellationToken);

            if (currentExists)
            {
                throw new BusinessException("Current address already exists.");
            }
        }


        var address = new EmployeeAddress
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            AddressLine1 = dto.AddressLine1,
            AddressLine2 = dto.AddressLine2,
            City = dto.City,
            State = dto.State,
            Country = dto.Country,
            PostalCode = dto.PostalCode,
            AddressType = dto.AddressType,
            CreatedAt = DateTime.Now
        };

        await repository.AddAddressAsync(address, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync("Create", nameof(EmployeeAddress), address.Id, $"Address added for employee {employeeId}", cancellationToken);
        logger.LogInformation("Address {AddressId} created for employee {EmployeeId}", address.Id, employeeId);

        return MapToResponse(address);
    }

    public async Task<PagedResponse<EmployeeAddressResponseDto>> GetAddressesAsync(EmployeeAddressFilterDto filter, CancellationToken cancellationToken = default)
    {
        var employeeId = await accessResolver.ResolveEmployeeIdAsync(filter.EmployeeId, cancellationToken);
        await accessResolver.ValidateEmployeeOwnershipAsync(employeeId, cancellationToken);

        var query = repository.GetAddresses().Where(a => a.EmployeeId == employeeId);

        if (!string.IsNullOrWhiteSpace(filter.City))
            query = query.Where(a => a.City != null && a.City.Contains(filter.City));

        if (!string.IsNullOrWhiteSpace(filter.State))
            query = query.Where(a => a.State != null && a.State.Contains(filter.State));

        if (!string.IsNullOrWhiteSpace(filter.Country))
            query = query.Where(a => a.Country != null && a.Country.Contains(filter.Country));

        if (!string.IsNullOrWhiteSpace(filter.AddressType))
            query = query.Where(a => a.AddressType != null && a.AddressType == filter.AddressType);

        query = ApplySorting(query, filter);

        var totalRecords = await query.CountAsync(cancellationToken);
        var addresses = await query.Skip((filter.PageNumber - 1) * filter.PageSize)
                                .Take(filter.PageSize)
                                .ToListAsync(cancellationToken);

        return new PagedResponse<EmployeeAddressResponseDto>
        {
            Data = addresses.Select(MapToResponse).ToList(),
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)filter.PageSize)
        };
    }

    public async Task<EmployeeAddressResponseDto> UpdateAddressAsync(Guid addressId, UpdateEmployeeAddressDto dto, CancellationToken cancellationToken = default)
    {
        var address = await repository.GetAddressAsync(addressId, cancellationToken);
        if (address == null) throw new NotFoundException("Address not found.");

        await accessResolver.ValidateEmployeeOwnershipAsync(address.EmployeeId, cancellationToken);

        if (dto.AddressType == AddressTypes.Permanent)
        {
            var permanentExists =
                await repository.AddressTypeExistsAsync(
                    address.EmployeeId,
                    address.Id,
                    AddressTypes.Permanent,
                    cancellationToken);

            if (permanentExists)
            {
                throw new BusinessException("Permanent address already exists.");
            }
        }

        if (dto.AddressType == AddressTypes.Current)
        {
            var currentExists =
                await repository.AddressTypeExistsAsync(
                    address.EmployeeId,
                    address.Id,
                    AddressTypes.Current,
                    cancellationToken);

            if (currentExists)
            {
                throw new BusinessException("Current address already exists.");
            }
        }

        address.AddressLine1 = dto.AddressLine1;
        address.AddressLine2 = dto.AddressLine2;
        address.City = dto.City;
        address.State = dto.State;
        address.Country = dto.Country;
        address.PostalCode = dto.PostalCode;
        address.AddressType = dto.AddressType;

        repository.UpdateAddress(address);
        await repository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync("Update", nameof(EmployeeAddress), address.Id, $"Address updated for employee {address.EmployeeId}", cancellationToken);
        logger.LogInformation("Address {AddressId} updated", address.Id);

        return MapToResponse(address);
    }

    public async Task DeleteAddressAsync(Guid addressId, CancellationToken cancellationToken = default)
    {
        var address = await repository.GetAddressAsync(addressId, cancellationToken);
        if (address == null) throw new NotFoundException("Address not found.");

        await accessResolver.ValidateEmployeeOwnershipAsync(address.EmployeeId, cancellationToken);

        repository.DeleteAddress(address);
        await repository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync("Delete", nameof(EmployeeAddress), address.Id, $"Address deleted for employee {address.EmployeeId}", cancellationToken);
        logger.LogInformation("Address {AddressId} deleted", address.Id);
    }

    private static EmployeeAddressResponseDto MapToResponse(EmployeeAddress address) => new()
    {
        Id = address.Id,
        EmployeeId = address.EmployeeId,
        AddressLine1 = address.AddressLine1 ?? string.Empty,
        AddressLine2 = address.AddressLine2,
        City = address.City ?? string.Empty,
        State = address.State ?? string.Empty,
        Country = address.Country ?? string.Empty,
        PostalCode = address.PostalCode ?? string.Empty,
        AddressType = address.AddressType ?? string.Empty
    };

    private static IQueryable<EmployeeAddress> ApplySorting(IQueryable<EmployeeAddress> query, EmployeeAddressFilterDto filter)
    {
        return filter.SortBy?.ToLower() switch
        {
            "city" => filter.Descending ? query.OrderByDescending(x => x.City) : query.OrderBy(x => x.City),
            "state" => filter.Descending ? query.OrderByDescending(x => x.State) : query.OrderBy(x => x.State),
            "country" => filter.Descending ? query.OrderByDescending(x => x.Country) : query.OrderBy(x => x.Country),
            _ => query.OrderBy(x => x.CreatedAt)
        };
    }

    public async Task<byte[]> ExportAddressesAsync(EmployeeAddressFilterDto filter, CancellationToken cancellationToken = default)
    {
        var query = repository.GetAddresses();

        if (!string.IsNullOrWhiteSpace(filter.City))
            query = query.Where(x => x.City != null && x.City.Contains(filter.City));

        if (!string.IsNullOrWhiteSpace(filter.State))
            query = query.Where(x => x.State != null && x.State.Contains(filter.State));

        if (!string.IsNullOrWhiteSpace(filter.Country))
            query = query.Where(x => x.Country != null && x.Country.Contains(filter.Country));

        if (!string.IsNullOrWhiteSpace(filter.AddressType))
            query = query.Where(x => x.AddressType == filter.AddressType);

        var addresses = await query.OrderBy(x => x.CreatedAt).ToListAsync(cancellationToken);

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Addresses");

        worksheet.Cell(1, 1).Value = "Employee Id";
        worksheet.Cell(1, 2).Value = "Address Line 1";
        worksheet.Cell(1, 3).Value = "Address Line 2";
        worksheet.Cell(1, 4).Value = "City";
        worksheet.Cell(1, 5).Value = "State";
        worksheet.Cell(1, 6).Value = "Country";
        worksheet.Cell(1, 7).Value = "Postal Code";
        worksheet.Cell(1, 8).Value = "Address Type";

        var row = 2;
        foreach (var address in addresses)
        {
            worksheet.Cell(row, 1).Value = address.EmployeeId.ToString();
            worksheet.Cell(row, 2).Value = address.AddressLine1;
            worksheet.Cell(row, 3).Value = address.AddressLine2;
            worksheet.Cell(row, 4).Value = address.City;
            worksheet.Cell(row, 5).Value = address.State;
            worksheet.Cell(row, 6).Value = address.Country;
            worksheet.Cell(row, 7).Value = address.PostalCode;
            worksheet.Cell(row, 8).Value = address.AddressType;
            row++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }


    public async Task<EmployeeAddressImportResultDto> ImportAddressesAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
        {
            throw new BusinessException("Excel file is required.");
        }

        var result = new EmployeeAddressImportResultDto();

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
                var employeeId = Guid.Parse(row.Cell(1).GetString());

                var dto = new AddEmployeeAddressDto
                {
                    EmployeeId = employeeId,
                    AddressLine1 = row.Cell(2).GetString(),
                    AddressLine2 = row.Cell(3).GetString(),
                    City = row.Cell(4).GetString(),
                    State = row.Cell(5).GetString(),
                    Country = row.Cell(6).GetString(),
                    PostalCode = row.Cell(7).GetString(),
                    AddressType = row.Cell(8).GetString()
                };

                await AddAddressAsync(dto, cancellationToken);
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
            nameof(EmployeeAddress),
            Guid.Empty,
            $"{result.SuccessCount} addresses imported",
            cancellationToken);

        return result;
    }
}