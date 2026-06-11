using HRMS.API.Data;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;

namespace HRMS.API.Repositories;

public class HolidayRepository: IHolidayRepository
{
    private readonly AppDbContext context;

    public HolidayRepository(
        AppDbContext context)
    {
        this.context = context;
    }

    public List<Holiday> GetAll()
    {
        return context.Holidays
            .OrderBy(h =>
                h.HolidayDate)
            .ToList();
    }

    public List<Holiday> GetUpcoming()
    {
        return context.Holidays
            .Where(h =>
                h.HolidayDate >=
                DateOnly.FromDateTime(
                    DateTime.Today))
            .OrderBy(h =>
                h.HolidayDate)
            .Take(5)
            .ToList();
    }

    public Holiday? GetById(Guid id)
    {
        return context.Holidays
            .FirstOrDefault(h =>
                h.Id == id);
    }

    public void Add(Holiday holiday)
    {
        context.Holidays.Add(holiday);
    }

    public void Update(Holiday holiday)
    {
        context.Holidays.Update(holiday);
    }

    public void Delete(Holiday holiday)
    {
        context.Holidays.Remove(holiday);
    }

    public void SaveChanges()
    {
        context.SaveChanges();
    }


    public bool HolidayExists(string name,DateOnly holidayDate)
    {
        return context.Holidays.Any(h =>
            h.Name.ToLower() == name.ToLower()
            &&
            h.HolidayDate == holidayDate);
    }
}