using System;
using AutoMapper;
using StockManagementSystem.Core.Domain.Devices;
using StockManagementSystem.Core.Infrastructure.Mapper;
using StockManagementSystem.LicenseManager.Domain;
using StockManagementSystem.LicenseManager.Models;

namespace StockManagementSystem.LicenseManager.Infrastructure
{
    public class DefaultMapperConfiguration : Profile, IOrderedMapperProfile
    {
        public DefaultMapperConfiguration()
        {
            CreateMap<License, LicenseModel>()
                .ForMember(model => model.Name, x => x.MapFrom(entity => entity.LicenseToName))
                .ForMember(model => model.Email, x => x.MapFrom(entity => entity.LicenseToEmail))
                .ForMember(model => model.ExpiryDate, x => x.MapFrom(entity => entity.ExpirationDate))
                //ignore
                .ForMember(model => model.LicenseType, options => options.Ignore())
                .ForMember(model => model.AvailableLicenseType, options => options.Ignore())
                .ForMember(model => model.Generated, options => options.Ignore())
                .ForMember(model => model.CountDevices, options => options.Ignore())
                .ForMember(model => model.DeviceLicenseSearchModel, options => options.Ignore());
            CreateMap<LicenseModel, License>()
                .ForMember(entity => entity.LicenseToName, x => x.MapFrom(model => model.Name))
                .ForMember(entity => entity.LicenseToEmail, x => x.MapFrom(model => model.Email))
                .ForMember(entity => entity.ExpirationDate,
                    x => x.MapFrom(model => model.ExpiryDate ?? DateTime.Now.AddYears(2)))
                //ignore
                .ForMember(entity => entity.PassPhrase, options => options.Ignore())
                .ForMember(entity => entity.PublicKey, options => options.Ignore())
                .ForMember(entity => entity.PrivateKey, options => options.Ignore())
                .ForMember(entity => entity.LicenseType, options => options.Ignore())
                .ForMember(entity => entity.ProductFeatures, options => options.Ignore());

            CreateMap<Device, DeviceLicenseModel>()
                .ForMember(model => model.StoreName, options => options.Ignore());
        }

        public int Order => 0;
    }
}