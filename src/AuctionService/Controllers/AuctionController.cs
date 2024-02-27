using AuctionService.Data;
using AuctionService.Dtos;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionController : ControllerBase
{
    private readonly AuctionDbContext _dbContext;
    private readonly IMapper _mapper;

    public AuctionController(AuctionDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAuctions()
    {
        var auctions = await _dbContext.Auctions.Include(x => x.Item).OrderBy(x => x.Item.Make).ToListAsync();
        return _mapper.Map<List<AuctionDto>>(auctions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
    {
        var auction = await _dbContext.Auctions.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == id);
        if (auction == null)
        {
            return NotFound();
        }
        return _mapper.Map<AuctionDto>(auction);
    }

    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto request)
    {
        var auction = _mapper.Map<Auction>(request);
        auction.Seller = "test";
        _dbContext.Auctions.Add(auction);
        var id = await _dbContext.SaveChangesAsync() > 0;

        if (!id)
        {
            return BadRequest("Could not save the changes to the DB");
        }

        return CreatedAtAction(nameof(GetAuctionById), new { auction.Id }, _mapper.Map<AuctionDto>(auction));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<AuctionDto>> UpdateAuction(Guid id, UpdateAuctionDto request)
    {
        var auction = await _dbContext.Auctions.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == id);

        if (auction == null)
        {
            return NotFound();
        }

        auction.Item.Make = request.Make ?? request.Make;
        auction.Item.Model = request.Model ?? request.Model;
        auction.Item.Color = request.Color ?? request.Color;
        auction.Item.Mileage = request.Mileage;
        auction.Item.Year = request.Year;

        var result = await _dbContext.SaveChangesAsync();
        if (result < 0)
        {
            return BadRequest("Problem saving changes");
        }

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<AuctionDto>> DeleteAuction(Guid id)
    {
        var auction = await _dbContext.Auctions.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == id);

        if (auction == null)
        {
            return NotFound();
        }

        _dbContext.Auctions.Remove(auction);       

        var result = await _dbContext.SaveChangesAsync() > 0;
        if (!result)
        {
            return BadRequest("Problem saving changes");
        }

        return Ok();
    }
}
