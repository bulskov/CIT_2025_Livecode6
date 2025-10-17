using Microsoft.EntityFrameworkCore;

namespace DataServiceLayer;

public class DataService : IDataService
{

    /////////////////////////////////////////////////
    // Categories
    /////////////////////////////////////////////////

    public IList<Category> GetCategories()
    {
        var db = new NorthwindContext();
        return db.Categories
            .ToList();
    }

    public Category? GetCategory(int id)
    {
        var db = new NorthwindContext();
        return db.Categories.FirstOrDefault(x => x.Id == id);
    }

    public void CreateCategory(Category category)
    {
        var db = new NorthwindContext();
        var maxId = db.Categories.Max(x => x.Id);
        category.Id = maxId + 1;
        db.Categories.Add(category);
        db.SaveChanges();
    }

    public bool UpdateCategory(Category category)
    {
        var db = new NorthwindContext();
        db.Update(category);
        return db.SaveChanges() > 0;
    }

    public bool DeleteCategory(int id)
    {
        var db = new NorthwindContext();
        var category = db.Categories.Find(id);
        if ((category == null))
        {
            return false;
        }
        db.Categories.Remove(category);
        return db.SaveChanges() > 0;
    }
    public IList<Category> GetCategoriesByName(string name)
    {
        var db = new NorthwindContext();
        return db.Categories.Where(x => x.Name.ToLower().Contains(name.ToLower())).ToList();
    }

    /////////////////////////////////////////////////
    // Products
    /////////////////////////////////////////////////
    
    public IList<Product> GetProducts()
    {
        var db = new NorthwindContext();
        return db.Products.Include(x => x.Category).ToList();
    }

    public Product? GetProduct(int id)
    {
        var db = new NorthwindContext();
        return db.Products.Include(x => x.Category).FirstOrDefault(x => x.Id == id);
    }

    public IList<ProductSearchModel> GetProductByName(string search)
    {
        var db = new NorthwindContext();

        return db.Products
            .Where(x => x.Name.ToLower().Contains(search.ToLower()))
            .ToList()
            .Select(x => new ProductSearchModel { ProductName = x.Name, CategoryName = x.Category.Name })
            .ToList();
    }

}