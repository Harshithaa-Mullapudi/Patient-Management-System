using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientManagement.Application.DTOs;
using PatientManagement.Domain.Entities;
using PatientManagement.Domain.Enums;
using PatientManagement.Infrastructure.Data;

namespace PatientManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly PatientDbContext _context;
    private readonly IMapper _mapper;

    public AppointmentsController(
        PatientDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // GET: api/Appointments
    // Filtering, sorting and pagination
    [HttpGet]
    public async Task<IActionResult> GetAllAppointments(
        int? patientId,
        AppointmentStatus? status,
        string sortBy = "appointmentDate",
        string sortDirection = "asc",
        int page = 1,
        int pageSize = 10)
    {
        if (page < 1 || pageSize < 1 || pageSize > 100)
        {
            return BadRequest("Page must be positive and pageSize must be between 1 and 100.");
        }

        IQueryable<Appointment> query =
            _context.Appointments.AsNoTracking();

        // Filtering using Where()
        if (patientId.HasValue)
        {
            query = query.Where(a => a.PatientId == patientId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(a => a.Status == status.Value);
        }

        var totalCount = await query.CountAsync();

        // Sorting
        var isDescending =
            sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase);

        query = sortBy.ToLowerInvariant() switch
        {
            "doctorname" => isDescending
                ? query.OrderByDescending(a => a.DoctorName)
                : query.OrderBy(a => a.DoctorName),

            "createdat" => isDescending
                ? query.OrderByDescending(a => a.CreatedAt)
                : query.OrderBy(a => a.CreatedAt),

            _ => isDescending
                ? query.OrderByDescending(a => a.AppointmentDate)
                : query.OrderBy(a => a.AppointmentDate)
        };

        // Pagination
        var appointments = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var response =
            _mapper.Map<List<AppointmentResponseDto>>(appointments);

        return Ok(new
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            Data = response
        });
    }

    // GET: api/Appointments/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetAppointmentById(int id)
    {
        var appointment = await _context.Appointments
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appointment == null)
        {
            return NotFound($"Appointment with ID {id} was not found.");
        }

        var response =
            _mapper.Map<AppointmentResponseDto>(appointment);

        return Ok(response);
    }

    // POST: api/Appointments
    [HttpPost]
    public async Task<IActionResult> CreateAppointment(
        CreateAppointmentDto createDto)
    {
        var patientExists = await _context.Patients
            .AnyAsync(p => p.Id == createDto.PatientId);

        if (!patientExists)
        {
            return NotFound(
                $"Patient with ID {createDto.PatientId} was not found.");
        }

        var appointment = _mapper.Map<Appointment>(createDto);

        appointment.CreatedAt = DateTime.UtcNow;

        _context.Appointments.Add(appointment);

        await _context.SaveChangesAsync();

        var response =
            _mapper.Map<AppointmentResponseDto>(appointment);

        return CreatedAtAction(
            nameof(GetAppointmentById),
            new { id = appointment.Id },
            response);
    }

    // PUT: api/Appointments/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateAppointment(
        int id,
        CreateAppointmentDto updateDto)
    {
        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appointment == null)
        {
            return NotFound($"Appointment with ID {id} was not found.");
        }

        var patientExists = await _context.Patients
            .AnyAsync(p => p.Id == updateDto.PatientId);

        if (!patientExists)
        {
            return NotFound(
                $"Patient with ID {updateDto.PatientId} was not found.");
        }

        appointment.PatientId = updateDto.PatientId;
        appointment.AppointmentDate = updateDto.AppointmentDate!.Value;
        appointment.DoctorName = updateDto.DoctorName;
        appointment.Reason = updateDto.Reason;

        await _context.SaveChangesAsync();

        var response =
            _mapper.Map<AppointmentResponseDto>(appointment);

        return Ok(response);
    }

    // PATCH: api/Appointments/1/complete
    [HttpPatch("{id:int}/complete")]
    public async Task<IActionResult> CompleteAppointment(int id)
    {
        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appointment == null)
        {
            return NotFound($"Appointment with ID {id} was not found.");
        }

        try
        {
            appointment.CompleteAppointment();

            await _context.SaveChangesAsync();

            var response =
                _mapper.Map<AppointmentResponseDto>(appointment);

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // PATCH: api/Appointments/1/cancel
    [HttpPatch("{id:int}/cancel")]
    public async Task<IActionResult> CancelAppointment(int id)
    {
        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appointment == null)
        {
            return NotFound($"Appointment with ID {id} was not found.");
        }

        try
        {
            appointment.CancelAppointment();

            await _context.SaveChangesAsync();

            var response =
                _mapper.Map<AppointmentResponseDto>(appointment);

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // DELETE: api/Appointments/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAppointment(int id)
    {
        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appointment == null)
        {
            return NotFound($"Appointment with ID {id} was not found.");
        }

        _context.Appointments.Remove(appointment);

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // GET: api/Appointments/statistics
    // Count() and GroupBy()
    [HttpGet("statistics")]
    public async Task<IActionResult> GetAppointmentStatistics()
    {
        var query = _context.Appointments.AsNoTracking();

        var totalAppointments = await query.CountAsync();

        var statusGroups = await query
            .GroupBy(a => a.Status)
            .Select(group => new
            {
                Status = group.Key,
                Count = group.Count()
            })
            .ToListAsync();

        return Ok(new
        {
            TotalAppointments = totalAppointments,
            StatusGroups = statusGroups
        });
    }

    // GET: api/Appointments/check-patient/1
    // Any()
    [HttpGet("check-patient/{patientId:int}")]
    public async Task<IActionResult> CheckPatientAppointments(int patientId)
    {
        var hasAppointments = await _context.Appointments
            .AnyAsync(a => a.PatientId == patientId);

        return Ok(new
        {
            PatientId = patientId,
            HasAppointments = hasAppointments
        });
    }
}
