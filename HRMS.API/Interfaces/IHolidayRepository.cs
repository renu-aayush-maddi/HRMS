using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface IHolidayRepository
{
    List<Holiday> GetAll();

    List<Holiday> GetUpcoming();

    Holiday? GetById(Guid id);

    void Add(Holiday holiday);

    bool HolidayExists(string name,DateOnly holidayDate);

    void Update(Holiday holiday);

    void Delete(Holiday holiday);

    void SaveChanges();
}