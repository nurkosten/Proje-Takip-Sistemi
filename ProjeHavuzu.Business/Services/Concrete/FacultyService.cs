using AutoMapper;
using FluentValidation;
using ProjeHavuzu.Business.Services.Abstract;
using ProjeHavuzu.Data.DTOs.FacultyDto;
using ProjeHavuzu.Data.Entites;
using ProjeHavuzu.Data.Repository.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjeHavuzu.Business.Services.Concrete
{
    public class FacultyService : IFacultyService
    {
        private readonly IFacultyRepository _facultyRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<FacultyCreateDto> _validator;

        public FacultyService(
            IFacultyRepository facultyRepository,
            IMapper mapper,
            IValidator<FacultyCreateDto> validator)
        {
            _facultyRepository = facultyRepository;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<List<FacultyListDto>> GetAllFacultiesAsync()
        {
            var faculties = await _facultyRepository.ListAsync(f => !f.IsDeleted);
            return _mapper.Map<List<FacultyListDto>>(faculties);
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
            // Validation
            var validationResult = await _validator.ValidateAsync(facultyCreateDto);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ArgumentException(errors);
            }

            // Aynı isimde fakülte var mı kontrol et
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
            // Validation
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

            // Aynı isimde başka bir fakülte var mı kontrol et
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

            // Fakülteye bağlı bölümler var mı kontrol et
            // Bu kontrolü DepartmentRepository üzerinden yapabilirsiniz
            // Şimdilik direkt silme işlemini yapıyoruz

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
