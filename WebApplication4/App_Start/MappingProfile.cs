using AutoMapper;
using WebApplication4.Dtos;
using WebApplication4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4.App_Start
{
    public class MappingProfile : Profile

    {
        public MappingProfile()
        {
            Mapper.CreateMap<Branch, BranchDto>();
            Mapper.CreateMap<BranchDto, Branch>();
            
        }
    
    }
}