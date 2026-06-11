// public void CheckIn(CheckInDto dto)
    // {
    //     var employee = attendanceRepository.GetEmployee(dto.EmployeeId);

    //     if (employee == null)
    //     {
    //         throw new NotFoundException("Employee not found");
    //     }

    //     var alreadyCheckedIn =
    //         attendanceRepository
    //         .GetTodayAttendance(dto.EmployeeId);

    //     if (alreadyCheckedIn != null)
    //     {
    //         throw new BusinessException("Already checked in today");
    //     }

    //     AttendanceLog attendance = new AttendanceLog
    //     {
    //         Id = Guid.NewGuid(),

    //         EmployeeId = dto.EmployeeId,

    //         AttendanceDate = DateOnly.FromDateTime(DateTime.Now),

    //         CheckIn = DateTime.Now,

    //         Status = "Present"
    //     };

    //     attendanceRepository.AddAttendance(attendance);

    //     attendanceRepository.SaveChanges();
    // }

    // public void CheckOut(CheckOutDto dto)
    // {
    //     var attendance =
    //         attendanceRepository
    //         .GetTodayAttendance(dto.EmployeeId);

    //     if (attendance == null)
    //     {
    //         throw new NotFoundException("Check-in not found");
    //     }

    //     if (attendance.CheckOut != null)
    //     {
    //         throw new BusinessException("Already checked out");
    //     }

    //     attendance.CheckOut = DateTime.Now;

    //     attendanceRepository.UpdateAttendance(attendance);

    //     attendanceRepository.SaveChanges();
    // }



    //     [Authorize(Roles = "Employee,HR,Manager")]
    // [HttpPost("checkin")]
    // public IActionResult CheckIn(CheckInDto dto)
    // {
    //     attendanceService.CheckIn(dto);

    //     return Ok("Checked In Successfully");
    // }

    // [Authorize(Roles = "Employee,HR,Manager")]
    // [HttpPost("checkout")]
    // public IActionResult CheckOut(CheckOutDto dto)
    // {
    //     attendanceService.CheckOut(dto);
    //     return Ok("Checked Out Successfully");
    // }
