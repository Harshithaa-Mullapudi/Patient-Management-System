using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientManagement.Application.DTOs;
using PatientManagement.Domain.Entities;
using PatientManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;

namespace PatientManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PatientProfilesController : ControllerBase
{
    private readonly PatientDbContext _context;
    private readonly IMapper _mapper;

    public PatientProfilesController(
        PatientDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // GET: api/PatientProfiles/1
    [HttpGet("{patientId:int}")]
    public async Task<IActionResult> GetPatientProfile(int patientId)
    {
        var profile = await _context.PatientProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PatientId == patientId);

        if (profile == null)
        {
            return NotFound(
                $"Profile for patient ID {patientId} was not found.");
        }

        var response = _mapper.Map<PatientProfileResponseDto>(profile);

        return Ok(response);
    }

    // POST: api/PatientProfiles
    [HttpPost]
    public async Task<IActionResult> CreatePatientProfile(
        CreatePatientProfileDto createDto)
    {
        // Check whether the patient exists
        var patientExists = await _context.Patients
            .AnyAsync(p => p.Id == createDto.PatientId);

        if (!patientExists)
        {
            return NotFound(
                $"Patient with ID {createDto.PatientId} was not found.");
        }

        // Check whether the patient already has a profile
        var profileExists = await _context.PatientProfiles
            .AnyAsync(p => p.PatientId == createDto.PatientId);

        if (profileExists)
        {
            return Conflict(
                $"Patient ID {createDto.PatientId} already has a profile.");
        }

        var profile = _mapper.Map<PatientProfile>(createDto);

        _context.PatientProfiles.Add(profile);

        await _context.SaveChangesAsync();

        var response = _mapper.Map<PatientProfileResponseDto>(profile);

        return CreatedAtAction(
            nameof(GetPatientProfile),
            new { patientId = profile.PatientId },
            response);
    }

    // PUT: api/PatientProfiles/1
    [HttpPut("{patientId:int}")]
    public async Task<IActionResult> UpdatePatientProfile(
        int patientId,
        CreatePatientProfileDto updateDto)
    {
        var profile = await _context.PatientProfiles
            .FirstOrDefaultAsync(p => p.PatientId == patientId);

        if (profile == null)
        {
            return NotFound(
                $"Profile for patient ID {patientId} was not found.");
        }

        // Update profile properties
        profile.Address = updateDto.Address;
        profile.City = updateDto.City;
        profile.State = updateDto.State;
        profile.PostalCode = updateDto.PostalCode;
        profile.EmergencyContactName = updateDto.EmergencyContactName;
        profile.EmergencyContactNumber = updateDto.EmergencyContactNumber;

        await _context.SaveChangesAsync();

        var response = _mapper.Map<PatientProfileResponseDto>(profile);

        return Ok(response);
    }

    // DELETE: api/PatientProfiles/1
    [HttpDelete("{patientId:int}")]
    public async Task<IActionResult> DeletePatientProfile(int patientId)
    {
        var profile = await _context.PatientProfiles
            .FirstOrDefaultAsync(p => p.PatientId == patientId);

        if (profile == null)
        {
            return NotFound(
                $"Profile for patient ID {patientId} was not found.");
        }

        _context.PatientProfiles.Remove(profile);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
