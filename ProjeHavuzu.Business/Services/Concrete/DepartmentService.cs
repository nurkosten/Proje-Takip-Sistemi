using AutoMapper;
using FluentValidation;
using ProjeHavuzu.Business.Services.Abstract;
using ProjeHavuzu.Data.DTOs.DepartmentDto;
using ProjeHavuzu.Data.Entites;
using ProjeHavuzu.Data.Repository.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjeHavuzu.Business.Services.Concrete
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IFacultyRepository _facultyRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<DepartmentListDto> _validator;

        public DepartmentService(
            IDepartmentRepository departmentRepository,
            IFacultyRepository facultyRepository,
            IMapper mapper,
            IValidator<DepartmentListDto> validator)
        {
            _departmentRepository = departmentRepository;
            _facultyRepository = facultyRepository;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<List<DepartmentListDto>> GetAllDepartmentsAsync()
        {
            var departments = await _departmentRepository.GetAllAsync(
                d => !d.IsDeleted,
                d => d.Faculty);
            
            return departments.Select(d => new DepartmentListDto
            {
                Id = d.Id,
                DepartmentName = d.DepartmentName,
                FacultyName = d.Faculty?.FacultyName ?? "Bilinmiyor"
            }).ToList();
        }

        public async Task<List<DepartmentFacultyDto>> GetDepartmentsByFacultyAsync()
        {
            var departments = _departmentRepository.GetDepartmentsByFacultyName();
            return await Task.FromResult(departments);
        }

        public async Task<DepartmentListDto> GetDepartmentByIdAsync(Guid id)
        {
            var departments = await _departmentRepository.GetAllAsync(
                d => d.Id == id && !d.IsDeleted,
                d => d.Faculty);
            
            var department = departments.FirstOrDefault();
            if (department == null)
                return null;

            return new DepartmentListDto
            {
                Id = department.Id,
                DepartmentName = department.DepartmentName,
                FacultyName = department.Faculty?.FacultyName ?? "Bilinmiyor"
            };
        }

        public async Task<bool> CreateDepartmentAsync(DepartmentListDto departmentDto)
        {
            // Validation
            var validationResult = await _validator.ValidateAsync(departmentDto);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ArgumentException(errors);
            }

            // Fakülte kontrolü
            var faculty = await _facultyRepository.GetAsync(
                f => f.FacultyName.Trim().ToLower() == departmentDto.FacultyName.Trim().ToLower() && !f.IsDeleted);
            
            if (faculty == null)
            {
                throw new ArgumentException("Geçersiz fakülte seçildi.");
            }

            // Aynı fakültede aynı isimde bölüm var mı kontrol et
            var existingDepartment = await _departmentRepository.GetAsync(
                d => d.FacultyId == faculty.Id && 
                     d.DepartmentName.Trim().ToLower() == departmentDto.DepartmentName.Trim().ToLower() && 
                     !d.IsDeleted);
            
            if (existingDepartment != null)
            {
                throw new InvalidOperationException("Bu fakültede bu isimde bir bölüm zaten mevcut.");
            }

            var department = new Department
            {
                DepartmentName = departmentDto.DepartmentName,
                FacultyId = faculty.Id
            };

            await _departmentRepository.AddAsync(department);
            return true;
        }

        public async Task<bool> UpdateDepartmentAsync(Guid id, DepartmentListDto departmentDto)
        {
            // Validation
            var validationResult = await _validator.ValidateAsync(departmentDto);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ArgumentException(errors);
            }

            var existingDepartment = await _departmentRepository.GetAsync(d => d.Id == id && !d.IsDeleted);
            if (existingDepartment == null)
            {
                throw new ArgumentException("Bölüm bulunamadı.");
            }

            // Fakülte kontrolü
            var faculty = await _facultyRepository.GetAsync(
                f => f.FacultyName.Trim().ToLower() == departmentDto.FacultyName.Trim().ToLower() && !f.IsDeleted);
            
            if (faculty == null)
            {
                throw new ArgumentException("Geçersiz fakülte seçildi.");
            }

            // Aynı fakültede aynı isimde başka bir bölüm var mı kontrol et
            var duplicateDepartment = await _departmentRepository.GetAsync(
                d => d.Id != id && 
                     d.FacultyId == faculty.Id && 
                     d.DepartmentName.Trim().ToLower() == departmentDto.DepartmentName.Trim().ToLower() && 
                     !d.IsDeleted);
            
            if (duplicateDepartment != null)
            {
                throw new InvalidOperationException("Bu fakültede bu isimde bir bölüm zaten mevcut.");
            }

            existingDepartment.DepartmentName = departmentDto.DepartmentName;
            existingDepartment.FacultyId = faculty.Id;
            _departmentRepository.Update(existingDepartment);
            return true;
        }

        public async Task<bool> DeleteDepartmentAsync(Guid id)
        {
            var department = await _departmentRepository.GetAsync(d => d.Id == id);
            if (department == null)
            {
                throw new ArgumentException("Bölüm bulunamadı.");
            }

            _departmentRepository.Remove(department);
            return true;
        }

        public async Task<bool> SoftDeleteDepartmentAsync(Guid id)
        {
            var department = await _departmentRepository.GetAsync(d => d.Id == id && !d.IsDeleted);
            if (department == null)
            {
                throw new ArgumentException("Bölüm bulunamadı.");
            }

            department.IsDeleted = true;
            department.IsActive = false;
            _departmentRepository.Update(department);
            return true;
        }
    }
}
