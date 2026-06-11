using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.EmployeeAddress;
using HRMS.API.Models.Entities;

namespace HRMS.API.Services;

public class EmployeeAddressService
    : IEmployeeAddressService
{
    private readonly
        IEmployeeAddressRepository repository;

    public EmployeeAddressService(
        IEmployeeAddressRepository repository)
    {
        this.repository = repository;
    }

    public void AddAddress(
        AddEmployeeAddressDto dto)
    {
        var employee =
            repository.GetEmployee(
                dto.EmployeeId);

        if(employee == null)
        {
            throw new Exception(
                "Employee not found");
        }

        EmployeeAddress address = new()
        {
            Id = Guid.NewGuid(),

            EmployeeId =
                dto.EmployeeId,

            AddressLine1 =
                dto.AddressLine1,

            AddressLine2 =
                dto.AddressLine2,

            City =
                dto.City,

            State =
                dto.State,

            Country =
                dto.Country,

            PostalCode =
                dto.PostalCode,

            AddressType =
                dto.AddressType
        };

        repository.AddAddress(address);

        repository.SaveChanges();
    }

    public List<EmployeeAddressResponseDto>
        GetEmployeeAddresses(
            Guid employeeId)
    {
        return repository
            .GetEmployeeAddresses(employeeId)
            .Select(a =>
                new EmployeeAddressResponseDto
                {
                    Id = a.Id,

                    AddressLine1 =
                        a.AddressLine1 ?? "",

                    AddressLine2 =
                        a.AddressLine2,

                    City =
                        a.City ?? "",

                    State =
                        a.State ?? "",

                    Country =
                        a.Country ?? "",

                    PostalCode =
                        a.PostalCode ?? "",

                    AddressType =
                        a.AddressType ?? ""
                })
            .ToList();
    }

    public void UpdateAddress(
        Guid id,
        UpdateEmployeeAddressDto dto)
    {
        var address =
            repository.GetAddress(id);

        if(address == null)
        {
            throw new Exception(
                "Address not found");
        }

        address.AddressLine1 =
            dto.AddressLine1;

        address.AddressLine2 =
            dto.AddressLine2;

        address.City =
            dto.City;

        address.State =
            dto.State;

        address.Country =
            dto.Country;

        address.PostalCode =
            dto.PostalCode;

        address.AddressType =
            dto.AddressType;

        repository.UpdateAddress(address);

        repository.SaveChanges();
    }

    public void DeleteAddress(Guid id)
    {
        var address =
            repository.GetAddress(id);

        if(address == null)
        {
            throw new Exception(
                "Address not found");
        }

        repository.DeleteAddress(address);

        repository.SaveChanges();
    }
}