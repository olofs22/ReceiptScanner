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
}


