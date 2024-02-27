using AuctionService.Dtos;
using AuctionService.Entities;
using AutoMapper;

namespace AuctionService.Infrastructure;

public class MapperProfiles : Profile
{
    public MapperProfiles()
    {
        CreateMap<Auction, AuctionDto>().IncludeMembers(x => x.Item);
        CreateMap<Item, AuctionDto>();
        CreateMap<CreateAuctionDto, Auction>();
        CreateMap<CreateAuctionDto, Item>();
    }
}
