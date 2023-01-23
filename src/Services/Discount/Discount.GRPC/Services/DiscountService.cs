using AutoMapper;
using Discount.GRPC.Repositories.Interfaces;

namespace Discount.GRPC.Services;

public class DiscountService
{
	private readonly IDiscountRepository _discountRepository;
	private readonly IMapper _mapper;

	public DiscountService(IDiscountRepository discountRepository, IMapper mapper)
	{
		_discountRepository = discountRepository;
		_mapper = mapper;
	}


}