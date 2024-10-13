﻿using AutoMapper;
using WebBimba.Data.Entities;
using WebBimba.Models.Category;
using WebBimba.Models.Product;

namespace WebBimba.Mapper
{
    public class AppMapperProfile : Profile
    {
        public AppMapperProfile()
        {

            CreateMap<CategoryEntity, CatagoryItemViewModel>();
            CreateMap<CategoryCreateViewModel, CategoryEntity>();

            CreateMap<ProductEntity, ProductItemViewModel>()
                 .ForMember(x => x.Images, opt => opt.MapFrom(p => p.ProductImages.Select(x => x.Image).ToList()))
                 .ForMember(x => x.CategoryName, opt => opt.MapFrom(c => c.Category.Name));

        }
    }
}
