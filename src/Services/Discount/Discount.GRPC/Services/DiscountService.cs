using AutoMapper;
using Discount.GRPC.Entities;
using Discount.GRPC.Repositories.Interfaces;
using Grpc.Core;

namespace Discount.GRPC.Services;

public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
{
	private readonly IDiscountRepository _discountRepository;
	private readonly IMapper _mapper;

	public DiscountService(IDiscountRepository discountRepository, IMapper mapper)
	{
		_discountRepository = discountRepository;
		_mapper = mapper;
	}

	public override async Task<CouponResponse> GetDiscountAsync(GetDiscountRequest request, ServerCallContext context)
	{
		var coupon = await _discountRepository.GetDiscountAsync(request.ProductName);
		if (coupon is null)
		{
			throw new RpcException(new Status(
				StatusCode.NotFound,
				$"Discount with product name \"${request.ProductName}\" was not found"));
		}

		return _mapper.Map<CouponResponse>(coupon);
	}

	public override async Task<DiscountCommandResponse> CreateDiscountAsync(CreateDiscountCommand request, ServerCallContext context)
	{
		var coupon = _mapper.Map<Coupon>(request);

		return new DiscountCommandResponse() { Success = await _discountRepository.CreateDiscountAsync(coupon) };
	}

	public override async Task<DiscountCommandResponse> UpdateDiscountAsync(UpdateDiscountCommand request, ServerCallContext context)
	{
		var coupon = _mapper.Map<Coupon>(request);

		return new DiscountCommandResponse() { Success = await _discountRepository.UpdateDiscountAsync(coupon) };
	}

	public override async Task<DiscountCommandResponse> DeleteDiscountAsync(DeleteDiscountCommand request, ServerCallContext context)
	{
		return new DiscountCommandResponse() { Success = await _discountRepository.DeleteDiscountAsync(request.ProductName) };
	}
}