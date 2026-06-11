using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Holiday;
using HRMS.API.Models.Entities;
using HRMS.API.Exceptions;

namespace HRMS.API.Services;

public class HolidayService : IHolidayService
{
    private readonly IHolidayRepository repository;

    public HolidayService(IHolidayRepository repository)
    {
        this.repository = repository;
    }

    public List<HolidayResponseDto> GetAll()
    {
        return repository
            .GetAll()
            .Select(h =>
                new HolidayResponseDto
                {
                    Id = h.Id,

                    Name = h.Name,

                    HolidayDate = h.HolidayDate,

                    Description = h.Description,

                    IsOptional = h.IsOptional
                })
            .ToList();
    }

    public List<HolidayResponseDto> GetUpcoming()
    {
        return repository
            .GetUpcoming()
            .Select(h =>
                new HolidayResponseDto
                {
                    Id = h.Id,

                    Name = h.Name,

                    HolidayDate = h.HolidayDate,

                    Description = h.Description,

                    IsOptional = h.IsOptional
                })
            .ToList();
    }

    public void AddHoliday(AddHolidayDto dto)
    {

    if(dto.HolidayDate < DateOnly.FromDateTime(DateTime.Today.AddYears(-1)))
        {
            throw new BusinessException("Holiday date is invalid");
        }

    if(repository.HolidayExists(dto.Name,dto.HolidayDate))
        {
            throw new BusinessException("Holiday already exists");
        }
        
        Holiday holiday = new()
        {
            Id = Guid.NewGuid(),

            Name = dto.Name,

            HolidayDate = dto.HolidayDate,

            Description = dto.Description,

            IsOptional = dto.IsOptional
        };

        repository.Add(holiday);

        repository.SaveChanges();
    }

    public void UpdateHoliday(Guid id,UpdateHolidayDto dto)
    {
        var holiday = repository.GetById(id);

        if(holiday == null)
        {
            throw new NotFoundException("Holiday not found");
        }

        holiday.Name = dto.Name;

        holiday.HolidayDate = dto.HolidayDate;

        holiday.Description = dto.Description;

        holiday.IsOptional = dto.IsOptional;

        repository.Update(holiday);

        repository.SaveChanges();
    }

    public void DeleteHoliday(Guid id)
    {
        var holiday = repository.GetById(id);

        if(holiday == null)
        {
            throw new NotFoundException("Holiday not found");
        }

        repository.Delete(holiday);

        repository.SaveChanges();
    }
}