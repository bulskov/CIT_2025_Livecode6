using DataServiceLayer;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Emit;
using WebServiceLayer.Models;

namespace WebServiceLayer.Controllers;

[Route("api/products")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IDataService _dataService;
    private readonly LinkGenerator _generator;
    private readonly IMapper _mapper;

    public ProductsController(
        IDataService dataService,
        LinkGenerator generator, 
        IMapper mapper) 
    {
        _dataService = dataService;
        _generator = generator;
        _mapper = mapper;
    }

    [HttpGet]
    public IActionResult GetProducts()
    {
        var products = _dataService
            .GetProducts()
            .Select(x => CreateProductModel(x));
        return Ok(products);
    }

    

    [HttpGet("{id}", Name = nameof(GetProduct))]
    public IActionResult GetProduct(int id)
    {
        var product = _dataService.GetProduct(id);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(CreateProductModel(product));
    }

    private ProductModel CreateProductModel(Product product)
    {
        var model = _mapper.Map<ProductModel>(product);
        model.Url = GetUrl(nameof(GetProduct), new { id = product.Id });
        model.CategoryUrl = GetUrl(nameof(CategoriesController.GetCategory), new { product.Category.Id});
        return model;
    }

    private string? GetUrl(string endpointName, object values)
    {
        return _generator.GetUriByName(HttpContext, endpointName, values);
    }
}
