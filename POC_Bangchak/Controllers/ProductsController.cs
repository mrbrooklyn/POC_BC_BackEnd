using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POC_Bangchak.Data;
using POC_Bangchak.Models;
using POC_Bangchak.Filter;
using POC_Bangchak.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel;
using POC_Bangchak.Services;

namespace POC_Bangchak.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IUriService _uriService;

        public ProductsController(ApplicationDbContext context, IUriService uriService)
        {
            _context = context;
            _uriService = uriService;
        }

        [HttpGet]
        //public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        public async Task<IActionResult> GetProducts([FromQuery] PaginationFilter filter, string? Name = null, decimal? Price = null, string? Description = null, string? Category = null, int? StockQuantity = null, DateTime? DateAdded = null)

        {
            var results = new List<Product>();
            var validFilter = new PaginationFilter(filter.pageNumber, filter.pageSize);

            if (Name != null)
            {
                if (results.Count() != 0)
                {
                    results = results.Where(x => x.Name.Contains(Name)).ToList();
                }
                else
                {
                    results = await _context.Products.Where(x => x.Name.Contains(Name)).ToListAsync();
                }
            }

            if (Price != null)
            {
                if (results.Count() != 0)
                {
                    results = results.Where(x => x.Price.Equals(Price)).ToList();
                }
                else
                {
                    results = await _context.Products.Where(x => x.Price.Equals(Price)).ToListAsync();
                }
            }

            if (Category != null)
            {

                if (results.Count() != 0)
                {
                    results = results.Where(x => x.Category.Contains(Category)).ToList();
                }
                else
                {
                    results = await _context.Products.Where(x => x.Category.Contains(Category)).ToListAsync();
                }
            }

            if (StockQuantity != null)
            {
                if (results.Count() != 0)
                {
                    results = results.Where(x => x.StockQuantity.Equals(StockQuantity)).ToList();
                }
                else
                {
                    results = await _context.Products.Where(x => x.StockQuantity.Equals(StockQuantity)).ToListAsync();
                }
            }

            if (DateAdded != null)
            {
                if (results.Count() != 0)
                {
                    results = results.Where(x => DateAdded.Equals(x.DateAdded)).ToList();
                }
                else
                {
                    results = await _context.Products.Where(x => x.DateAdded.Equals(DateAdded)).ToListAsync();
                }
            }

            int totalRecords = 0;

            if (results.Count() == 0)
            {
                totalRecords = await _context.Products.CountAsync();

                results = await _context.Products.OrderBy(e => e.Name).ThenByDescending(e => e.DateAdded)
                    .Skip((validFilter.pageNumber - 1) * validFilter.pageSize)
                    .Take(validFilter.pageSize).ToListAsync();
            }
            else
            {
                totalRecords = results.Count();
                results = results.OrderBy(e => e.Name).ThenByDescending(e => e.DateAdded)
                    .Skip((validFilter.pageNumber - 1) * validFilter.pageSize)
                    .Take(validFilter.pageSize).ToList();
            }

            results = results.OrderBy(e => e.Name).ThenByDescending(e => e.DateAdded).ThenBy(e => e.Price).ThenBy(e => e.Category).ToList();

            var pagedData = results;
            if (pagedData.Count() == 0)
            {
                var errorDetails = new Response
                {
                    Status = "Error",
                    Message = "Record Not Found"
                };

                return NotFound(errorDetails);
            }

            var route = "/api/Products?";
            var pagedReponse = PaginationHelper.CreatePagedReponse<Product>(pagedData, validFilter, totalRecords, _uriService, route);

            return Ok(pagedReponse);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return product;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }
    }
}