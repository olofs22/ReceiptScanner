using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReceiptProject1.DTOs.ReceiptDTOs;
using ReceiptProject1.Services;
using System.Security.Claims;

namespace ReceiptProject1.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReceiptController : ControllerBase
{
    private readonly ScanService _scanService;

    private readonly ReceiptService _receiptService;

    public ReceiptController(ReceiptService receiptService, ScanService scanService)
    {
        _receiptService = receiptService;
        _scanService = scanService;
    }

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
    Ok(await _receiptService.GetAllAsync(GetUserId()));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var receipt = await _receiptService.GetByIdAsync(id, GetUserId());
        return receipt == null ? NotFound() : Ok(receipt);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody]CreateReceiptDTO crdto)
    {
        var receipt = await _receiptService.CreateAsync(crdto, GetUserId());
        return CreatedAtAction(nameof(GetById), new { id = receipt.Id }, receipt);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id,[FromBody] UpdateReceiptDTO urdto)
    {
        var updated = await _receiptService.UpdateAsync(id, urdto, GetUserId());
        return updated == null ? NotFound() : Ok(updated);
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _receiptService.DeleteAsync(id, GetUserId());
        return success ? NoContent() : NotFound();
    }

    [HttpPost("scan")]
    public async Task<IActionResult> Scan([FromForm] IFormFile image)
    {
        try
        {
            var dto = await _scanService.ScanReceiptAsync(image);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SCAN ERROR: {ex.Message}");
            return BadRequest(ex.Message);
        }
    }
}


