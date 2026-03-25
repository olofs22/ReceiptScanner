using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReceiptProject1.Data;
using ReceiptProject1.DTOs.ItemDTOs;
using ReceiptProject1.DTOs.ReceiptDTOs;
using ReceiptProject1.Models;
using ReceiptProject1.Services;
using System.Security.Claims;

namespace ReceiptProject1.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReceiptController : ControllerBase
{
    private readonly ReceiptService _receiptService;

    public ReceiptController(ReceiptService receiptService)
    {
        _receiptService = receiptService;
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
    public async Task<IActionResult> Create(CreateReceiptDTO dto)
    {
        var receipt = await _receiptService.CreateAsync(dto, GetUserId());
        return CreatedAtAction(nameof(GetById), new { id = receipt.Id }, receipt);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateReceiptDTO dto)
    {
        var success = await _receiptService.UpdateAsync(id, dto, GetUserId());
        return success ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _receiptService.DeleteAsync(id, GetUserId());
        return success ? NoContent() : NotFound();
    }
}


