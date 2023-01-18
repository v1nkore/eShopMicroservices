using AutoMapper;
using Catalog.API.DTO;
using Catalog.API.Entities;
using Shared.Utils.Guid;

namespace Catalog.API.Profiles
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<Product, ProductResponse>()
				.ForMember(dest => dest.Id,
					opt =>
					{
						opt.MapFrom(src => GuidConverter.Encode(src.Id));
					});

			CreateMap<ProductCommand, Product>()
				.ForMember(dest => dest.Id,
					opt => opt.MapFrom(src => GuidConverter.Decode(src.Id)));
		}
	}
}
