using HRMS.API.Models.DTOs.Holiday;

namespace HRMS.API.Interfaces;

public interface IHolidayService
{
    List<HolidayResponseDto> GetAll();

    List<HolidayResponseDto> GetUpcoming();

    void AddHoliday(AddHolidayDto dto);

    void UpdateHoliday(
        Guid id,
        UpdateHolidayDto dto);

    void DeleteHoliday(Guid id);
}