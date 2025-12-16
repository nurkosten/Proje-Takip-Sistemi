using AutoMapper;
using ProjeHavuzu.Data.DTOs.CategoryDto;
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
            //CreateMap<Entites.Project, DTOs.ProjectDto.ProjectListDto>().ReverseMap();
            //CreateMap<Entites.Project, DTOs.ProjectDto.ProjectAddDto>().ReverseMap();
            //CreateMap<Entites.Project, DTOs.ProjectDto.ProjectEditDto>().ReverseMap();
        }

        
    }
}
