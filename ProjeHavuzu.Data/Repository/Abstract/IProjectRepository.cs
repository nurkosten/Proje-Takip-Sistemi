using ProjeHavuzu.Data.DTOs.ProjectDto;
using ProjeHavuzu.Data.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjeHavuzu.Data.Repository.Abstract
{
    public interface IProjectRepository:IRepository<Project>
    {


        Task<List<ProjectListDto>> GetAllProjectsByCategoryAsync();
      //  Task<List<ProjectListDto>> ProjectsWithCount();


    }
}
