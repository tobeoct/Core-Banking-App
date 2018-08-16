using AutoMapper;
using WebApplication1.Dtos;
using WebApplication1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.ViewModels;

namespace WebApplication4.App_Start
{
    public class MappingProfile : Profile

    {
        public MappingProfile()
        {

            //            Map Model to Dto
            Mapper.CreateMap<Branch, BranchDto>();
            Mapper.CreateMap<GLCategory, GLCategoryDto>();
            Mapper.CreateMap<Categories, CategoriesDto>();
            Mapper.CreateMap<Customer, CustomerDto>();
            Mapper.CreateMap<GLAccount, GLAccountDto>();
            Mapper.CreateMap<UserAccount, UserAccountDto>();


            //            Map Dto to Model

            Mapper.CreateMap<BranchDto, Branch>();   
            Mapper.CreateMap<GLCategoryDto, GLCategory>();
            Mapper.CreateMap<GLAccountDto, GLAccount>();
            Mapper.CreateMap<CustomerDto, Customer>();
            Mapper.CreateMap<UserAccountDto, UserAccount>();

            //Map Model to ViewModel
            Mapper.CreateMap<GLCategory, GLCategoryViewModel>();

            //Map ViewModel to Model
            Mapper.CreateMap<GLCategoryViewModel,GLCategory>();

            
        }
    
    }
}