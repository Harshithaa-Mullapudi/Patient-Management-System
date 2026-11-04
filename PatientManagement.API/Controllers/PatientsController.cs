using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using AutoMapper.QueryableExtensions;
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
public class PatientsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly PatientDbContext _context;
    private readonly IConfiguration _configuration;

    public PatientsController(IMapper mapper, PatientDbContext context, IConfiguration configuration)
    {
        _mapper = mapper;
        _context = context;
        _configuration = configuration;
    }

    // GET: api/Patients
    // Filtering, sorting and pagination
    [HttpGet]
    public async Task<IActionResult> GetAllPatients(
        int? minAge,
        int? maxAge,
        Gender? gender,
        string sortBy = "id",
        string sortDirection = "asc",
        int page = 1,
        int pageSize = 10)
    {
        if (minAge.HasValue && maxAge.HasValue && minAge > maxAge)
        {
            return BadRequest("Minimum age cannot be greater than maximum age.");
        }

        if (page < 1 || pageSize < 1 || pageSize > 100)
        {
            return BadRequest("Page must be positive and pageSize must be between 1 and 100.");
        }

        IQueryable<Patient> query = _context.Patients.AsNoTracking();

        // Filtering using Where()
        if (minAge.HasValue)
        {
            query = query.Where(p => p.Age >= minAge.Value);
        }

        if (maxAge.HasValue)
        {
            query = query.Where(p => p.Age <= maxAge.Value);
        }

        if (gender.HasValue)
        {
            query = query.Where(p => p.Gender == gender.Value);
        }

        // Count matching records before pagination
        var totalCount = await query.CountAsync();

        // Sorting using OrderBy() and OrderByDescending()
        var isDescending =
            sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase);

        query = sortBy.ToLowerInvariant() switch
        {
            "name" => isDescending
                ? query.OrderByDescending(p => p.FirstName)
                       .ThenByDescending(p => p.LastName)
                : query.OrderBy(p => p.FirstName)
                       .ThenBy(p => p.LastName),

            "age" => isDescending
                ? query.OrderByDescending(p => p.Age)
                : query.OrderBy(p => p.Age),

            "createdat" => isDescending
                ? query.OrderByDescending(p => p.CreatedAt)
                : query.OrderBy(p => p.CreatedAt),

            _ => isDescending
                ? query.OrderByDescending(p => p.Id)
                : query.OrderBy(p => p.Id)
        };

        // Pagination using Skip() and Take()
        var patients = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectTo<PatientResponseDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return Ok(new
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            Data = patients
        });
    }

    // GET: api/Patients/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetPatientById(int id)
    {
        var patient = await _context.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (patient == null)
        {
            return NotFound($"Patient with ID {id} was not found.");
        }

        var response = _mapper.Map<PatientResponseDto>(patient);

        return Ok(response);
    }

    // POST: api/Patients
    [HttpPost]
    public async Task<IActionResult> CreatePatient(
        CreatePatientDto createPatientDto)
    {
        var patient = _mapper.Map<Patient>(createPatientDto);

        _context.Patients.Add(patient);

        await _context.SaveChangesAsync();

        var response = _mapper.Map<PatientResponseDto>(patient);

        return CreatedAtAction(
            nameof(GetPatientById),
            new { id = patient.Id },
            response);
    }

    // PUT: api/Patients/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdatePatient(
        int id,
        UpdatePatientDto updatePatientDto)
    {
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.Id == id);

        if (patient == null)
        {
            return NotFound($"Patient with ID {id} was not found.");
        }

        patient.FirstName = updatePatientDto.FirstName;
        patient.LastName = updatePatientDto.LastName;
        patient.Gender = updatePatientDto.Gender;
        patient.DateOfBirth = updatePatientDto.DateOfBirth!.Value;

        patient.UpdateAge(updatePatientDto.Age);

        patient.UpdateContactInformation(
            updatePatientDto.Email,
            updatePatientDto.PhoneNumber);

        await _context.SaveChangesAsync();

        var response = _mapper.Map<PatientResponseDto>(patient);

        return Ok(response);
    }

    // DELETE: api/Patients/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletePatient(int id)
    {
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.Id == id);

        if (patient == null)
        {
            return NotFound($"Patient with ID {id} was not found.");
        }

        _context.Patients.Remove(patient);

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // GET: api/Patients/statistics
    // Aggregation and GroupBy
    [HttpGet("statistics")]
    public async Task<IActionResult> GetPatientStatistics()
    {
        var query = _context.Patients.AsNoTracking();

        var totalPatients = await query.CountAsync();

        var averageAge = totalPatients > 0
            ? await query.AverageAsync(p => (double)p.Age)
            : 0;

        var minimumAge = totalPatients > 0
            ? await query.MinAsync(p => p.Age)
            : 0;

        var maximumAge = totalPatients > 0
            ? await query.MaxAsync(p => p.Age)
            : 0;

        // Group patients by age range
        var ageGroups = await query
            .GroupBy(p =>
                p.Age < 18 ? "Under 18" :
                p.Age < 60 ? "18 to 59" : "60 and above")
            .Select(group => new
            {
                AgeGroup = group.Key,
                Count = group.Count()
            })
            .ToListAsync();

        return Ok(new
        {
            TotalPatients = totalPatients,
            AverageAge = Math.Round(averageAge, 2),
            MinimumAge = minimumAge,
            MaximumAge = maximumAge,
            AgeGroups = ageGroups
        });
    }

    // GET: api/Patients/check-email?email=test@example.com
    // Any()
    [HttpGet("check-email")]
    public async Task<IActionResult> CheckEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return BadRequest("Email is required.");
        }

        var exists = await _context.Patients
            .AnyAsync(p => p.Email == email);

        return Ok(new
        {
            Email = email,
            Exists = exists
        });
    }

    [HttpGet("configuration")]
    public IActionResult GetPatientConfiguration()
    {
        var maxPatients = _configuration.GetValue<int>(
            "PatientSettings:MaxPatientsToReturn");

        var applicationName = _configuration.GetValue<string>(
            "PatientSettings:ApplicationName");

        var registrationEnabled = _configuration.GetValue<bool>(
            "PatientSettings:EnablePatientRegistration");

        return Ok(new
        {
            MaxPatientsToReturn = maxPatients,
            ApplicationName = applicationName,
            EnablePatientRegistration = registrationEnabled
        });
    }
}
