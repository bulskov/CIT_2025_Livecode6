﻿using DataServiceLayer;
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

    [HttpGet(Name = nameof(GetProducts))]
    public IActionResult GetProducts(int page = 0, int pageSize = 5)
    {
        var products = _dataService
            .GetProducts(page, pageSize)
            .Select(x => CreateProductModel(x));

        var numOfItems = _dataService.GetProductCount();
        var numPages = (int)Math.Ceiling((double)numOfItems / pageSize);

        var prev = page > 0
            ? GetUrl(nameof(GetProducts), new { page = page - 1, pageSize })
            : null;

        var next = page < numPages-1
            ? GetUrl(nameof(GetProducts), new { page = page + 1, pageSize })
            : null;

        var first = GetUrl(nameof(GetProducts), new {page = 0, pageSize });
        var cur = GetUrl(nameof(GetProducts), new {page, pageSize });
        var last  = GetUrl(nameof(GetProducts), new {page = numPages-1, pageSize });

        var result = new
        {
            First = first,
            Prev = prev,
            Next = next,
            Last = last,
            Current = cur,
            NumberOfPages=numPages,
            NumberOfIems = numOfItems,
            Items = products
        };



        return Ok(result);
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
