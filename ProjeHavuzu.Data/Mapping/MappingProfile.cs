using AutoMapper;
using ProjeHavuzu.Data.DTOs.CategoryDto;
using ProjeHavuzu.Data.DTOs.DepartmentDto;
using ProjeHavuzu.Data.DTOs.FacultyDto;
using ProjeHavuzu.Data.DTOs.ProjectDto;
using ProjeHavuzu.Data.DTOs.SystemLogDto;
using ProjeHavuzu.Data.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjeHavuzu.Data.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Category Mappings
            CreateMap<Category, CategoryListDto>().ReverseMap();
            CreateMap<Category, CategoryAddDto>().ReverseMap();
            CreateMap<Category, CategoryEditDto>().ReverseMap();
            // Project Mappings
            CreateMap<Project, ProjectListDto>().ReverseMap();
            CreateMap<Project, ProjectCreateDto>().ReverseMap();
            //faculty mappings
            CreateMap<Faculty, FacultyCreateDto>().ReverseMap();
            CreateMap<Faculty, FacultyListDto>().ReverseMap();  
          
            //department mappings
            CreateMap<DepartmentListDto, Department>().ReverseMap();

            // Log Mappings
            CreateMap<SystemLog, SystemLogListDto>()
                .ForMember(dest => dest.CreatedByUserName, opt => opt.MapFrom(src => src.CreatedBy.ToString())) // Kullanıcı adı için repository join lazım ama şimdilik ID
                .ReverseMap();



        }


    }
}
