using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using ProjeHavuzu.Business.Services.Abstract;
using ProjeHavuzu.Data.DTOs.FacultyDto;
using ProjeHavuzu.Data.Entites;
using ProjeHavuzu.Data.Entites.Identity;
using ProjeHavuzu.Data.Repository.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjeHavuzu.Business.Services.Concrete
{
    public class FacultyService : IFacultyService
    {
        private readonly IFacultyRepository _facultyRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<FacultyCreateDto> _validator;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly UserManager<AppUser> _userManager;

        public FacultyService(
            IFacultyRepository facultyRepository,
            IMapper mapper,
            IValidator<FacultyCreateDto> validator,
            IDepartmentRepository departmentRepository,
            UserManager<AppUser> userManager)
        {
            _facultyRepository = facultyRepository;
            _mapper = mapper;
            _validator = validator;
            _departmentRepository = departmentRepository;
            _userManager = userManager;
        }

        public async Task<List<FacultyListDto>> GetAllFacultiesAsync()
        {
            var faculties = await _facultyRepository.ListAsync(f => !f.IsDeleted);
            var sortedFaculties = faculties.OrderBy(f => f.CreatedDate).ToList();
            return _mapper.Map<List<FacultyListDto>>(sortedFaculties);
        }

        public async Task<FacultyListDto> GetFacultyByIdAsync(Guid id)
        {
            var faculty = await _facultyRepository.GetAsync(f => f.Id == id && !f.IsDeleted);
            if (faculty == null)
                return null;

            return _mapper.Map<FacultyListDto>(faculty);
        }

        public async Task<bool> CreateFacultyAsync(FacultyCreateDto facultyCreateDto)
        {
            var validationResult = await _validator.ValidateAsync(facultyCreateDto);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ArgumentException(errors);
            }

            var existingFaculty = await _facultyRepository.GetAsync(
                f => f.FacultyName.Trim().ToLower() == facultyCreateDto.FacultyName.Trim().ToLower() && !f.IsDeleted);

            if (existingFaculty != null)
            {
                throw new InvalidOperationException("Bu isimde bir fakülte zaten mevcut.");
            }

            var faculty = _mapper.Map<Faculty>(facultyCreateDto);
            await _facultyRepository.AddAsync(faculty);
            return true;
        }

        public async Task<bool> UpdateFacultyAsync(Guid id, FacultyCreateDto facultyCreateDto)
        {
            var validationResult = await _validator.ValidateAsync(facultyCreateDto);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ArgumentException(errors);
            }

            var existingFaculty = await _facultyRepository.GetAsync(f => f.Id == id && !f.IsDeleted);
            if (existingFaculty == null)
            {
                throw new ArgumentException("Fakülte bulunamadı.");
            }

            var duplicateFaculty = await _facultyRepository.GetAsync(
                f => f.Id != id &&
                     f.FacultyName.Trim().ToLower() == facultyCreateDto.FacultyName.Trim().ToLower() &&
                     !f.IsDeleted);

            if (duplicateFaculty != null)
            {
                throw new InvalidOperationException("Bu isimde bir fakülte zaten mevcut.");
            }

            existingFaculty.FacultyName = facultyCreateDto.FacultyName;
            _facultyRepository.Update(existingFaculty);
            return true;
        }

        public async Task<bool> DeleteFacultyAsync(Guid id)
        {
            var faculty = await _facultyRepository.GetAsync(f => f.Id == id);
            if (faculty == null)
            {
                throw new ArgumentException("Fakülte bulunamadı.");
            }

            // 1. Fakülteye bağlı bölümleri bul
            var departments = await _departmentRepository.ListAsync(d => d.FacultyId == id);
            if (departments != null)
            {
                foreach (var dept in departments)
                {
                    // 2. Bölüme bağlı KULLANICILARI bul ve ilişkilerini kopar (DepartmentId = null)
                    // Not: Bu işlem performans açısından ideal olmayabilir ama veri bütünlüğü için gereklidir.
                    // UserManager.Users bir IQueryable döner.
                    
                    var usersInDept = _userManager.Users.Where(u => u.DepartmentId == dept.Id).ToList();
                    foreach (var user in usersInDept)
                    {
                        user.DepartmentId = null;
                        user.FacultyId = null; 
                        await _userManager.UpdateAsync(user);
                    }

                    _departmentRepository.Remove(dept);
                }
            }
            
            // 3. Fakülteye direkt bağlı kullanıcılar varsa (Bölümü olmayan ama fakültesi olanlar)
            var usersInFaculty = _userManager.Users.Where(u => u.FacultyId == id).ToList(); 
            foreach(var user in usersInFaculty)
            {
                user.FacultyId = null;
                await _userManager.UpdateAsync(user);
            }

            _facultyRepository.Remove(faculty);
            return true;
        }

        public async Task<bool> SoftDeleteFacultyAsync(Guid id)
        {
            var faculty = await _facultyRepository.GetAsync(f => f.Id == id && !f.IsDeleted);
            if (faculty == null)
            {
                throw new ArgumentException("Fakülte bulunamadı.");
            }

            faculty.IsDeleted = true;
            faculty.IsActive = false;
            _facultyRepository.Update(faculty);
            return true;
        }
    }
}
