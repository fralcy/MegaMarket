using MegaMarket.API.DTOs.Products;

namespace MegaMarket.Tests.Integration.Controllers.Products;

public class ProductsControllerTests : IClassFixture<TestServerFixture>
{
    private readonly HttpClient _client;
    private readonly TestServerFixture _factory;

    public ProductsControllerTests(TestServerFixture factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetProducts_ReturnsSuccess_WithListOfProducts()
    {
        // Act
        var response = await _client.GetAsync("/api/products");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        products.Should().NotBeNull();
        products.Should().HaveCountGreaterThan(0);
        products.Should().Contain(p => p.Name == "Organic Apples 1kg");
    }

    [Fact]
    public async Task GetProduct_ExistingId_ReturnsProduct()
    {
        // Arrange
        var productId = 1;

        // Act
        var response = await _client.GetAsync($"/api/products/{productId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var product = await response.Content.ReadFromJsonAsync<ProductDto>();
        product.Should().NotBeNull();
        product!.ProductId.Should().Be(productId);
        product.Name.Should().Be("Organic Apples 1kg");
        product.Barcode.Should().Be("1110001110001");
    }

    [Fact]
    public async Task GetProduct_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var productId = 99999;

        // Act
        var response = await _client.GetAsync($"/api/products/{productId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateProduct_ValidData_ReturnsCreated()
    {
        // Arrange
        var newProduct = new ProductCreateUpdateDto
        {
            Barcode = "9999999999999",
            Name = "Test Product",
            Category = "Test Category",
            UnitLabel = "Piece",
            UnitPrice = 50000,
            OriginalPrice = 60000,
            DiscountPercent = 10,
            QuantityInStock = 100,
            MinQuantity = 20,
            IsPerishable = false,
            ImageUrl = "https://example.com/image.jpg"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/products", newProduct);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdProduct = await response.Content.ReadFromJsonAsync<ProductDto>();
        createdProduct.Should().NotBeNull();
        createdProduct!.Name.Should().Be(newProduct.Name);
        createdProduct.Barcode.Should().Be(newProduct.Barcode);
        createdProduct.UnitPrice.Should().Be(newProduct.UnitPrice);
        createdProduct.QuantityInStock.Should().Be(newProduct.QuantityInStock);

        // Verify Location header contains product ID
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Contain(createdProduct.ProductId.ToString());
    }

    [Fact]
    public async Task CreateProduct_PerishableWithExpiryDate_ReturnsCreated()
    {
        // Arrange
        var newProduct = new ProductCreateUpdateDto
        {
            Barcode = "8888888888888",
            Name = "Fresh Vegetables",
            Category = "Produce",
            UnitLabel = "Kg",
            UnitPrice = 30000,
            QuantityInStock = 50,
            MinQuantity = 10,
            IsPerishable = true,
            ExpiryDate = DateTime.Today.AddDays(7)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/products", newProduct);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdProduct = await response.Content.ReadFromJsonAsync<ProductDto>();
        createdProduct.Should().NotBeNull();
        createdProduct!.IsPerishable.Should().BeTrue();
        createdProduct.ExpiryDate.Should().NotBeNull();
        createdProduct.ExpiryDate!.Value.Date.Should().Be(DateTime.Today.AddDays(7));
    }

    [Fact]
    public async Task UpdateProduct_ValidData_ReturnsNoContent()
    {
        // Arrange - First create a product to update
        var createDto = new ProductCreateUpdateDto
        {
            Barcode = "7777777777777",
            Name = "Product To Update",
            Category = "Test",
            UnitLabel = "Unit",
            UnitPrice = 40000,
            QuantityInStock = 80,
            MinQuantity = 15,
            IsPerishable = false
        };
        var createResponse = await _client.PostAsJsonAsync("/api/products", createDto);
        var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>();

        // Update the product
        var updateDto = new ProductCreateUpdateDto
        {
            Barcode = "7777777777777",
            Name = "Product Updated",
            Category = "Test Updated",
            UnitLabel = "Unit",
            UnitPrice = 45000,
            QuantityInStock = 90,
            MinQuantity = 15,
            IsPerishable = false
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/products/{createdProduct!.ProductId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify the update
        var getResponse = await _client.GetAsync($"/api/products/{createdProduct.ProductId}");
        var updatedProduct = await getResponse.Content.ReadFromJsonAsync<ProductDto>();
        updatedProduct!.Name.Should().Be("Product Updated");
        updatedProduct.Category.Should().Be("Test Updated");
        updatedProduct.UnitPrice.Should().Be(45000);
    }

    [Fact]
    public async Task UpdateProduct_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var productId = 99999;
        var updateDto = new ProductCreateUpdateDto
        {
            Barcode = "6666666666666",
            Name = "Non Existing Product",
            Category = "Test",
            UnitLabel = "Unit",
            UnitPrice = 10000,
            QuantityInStock = 50,
            MinQuantity = 10,
            IsPerishable = false
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/products/{productId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteProduct_ExistingId_ReturnsNoContent()
    {
        // Arrange - Create a product to delete
        var createDto = new ProductCreateUpdateDto
        {
            Barcode = "5555555555555",
            Name = "Product To Delete",
            Category = "Test",
            UnitLabel = "Unit",
            UnitPrice = 25000,
            QuantityInStock = 60,
            MinQuantity = 10,
            IsPerishable = false
        };
        var createResponse = await _client.PostAsJsonAsync("/api/products", createDto);
        var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>();

        // Act
        var response = await _client.DeleteAsync($"/api/products/{createdProduct!.ProductId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify product is deleted
        var getResponse = await _client.GetAsync($"/api/products/{createdProduct.ProductId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteProduct_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var productId = 99999;

        // Act
        var response = await _client.DeleteAsync($"/api/products/{productId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateProduct_WithDiscount_CalculatesCorrectly()
    {
        // Arrange
        var newProduct = new ProductCreateUpdateDto
        {
            Barcode = "4444444444444",
            Name = "Discounted Product",
            Category = "Sale",
            UnitLabel = "Unit",
            UnitPrice = 90000,
            OriginalPrice = 100000,
            DiscountPercent = 10,
            QuantityInStock = 40,
            MinQuantity = 10,
            IsPerishable = false
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/products", newProduct);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdProduct = await response.Content.ReadFromJsonAsync<ProductDto>();
        createdProduct.Should().NotBeNull();
        createdProduct!.OriginalPrice.Should().Be(100000);
        createdProduct.UnitPrice.Should().Be(90000);
        createdProduct.DiscountPercent.Should().Be(10);
    }

    [Fact]
    public async Task GetProducts_VerifyAllSeededProducts_Present()
    {
        // Act
        var response = await _client.GetAsync("/api/products");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        products.Should().NotBeNull();

        // Verify seeded products from TestData
        products.Should().Contain(p => p.Barcode == "1110001110001" && p.Name == "Organic Apples 1kg");
        products.Should().Contain(p => p.Barcode == "2220002220002" && p.Name == "Whole Milk 1L");
        products.Should().Contain(p => p.Barcode == "3330003330003" && p.Name == "Free Range Eggs 12ct");
        products.Should().Contain(p => p.Barcode == "4440004440004" && p.Name == "Roasted Coffee Beans 500g");
    }

    [Fact]
    public async Task UpdateProduct_UpdateStockQuantity_ReturnsNoContent()
    {
        // Arrange
        var productId = 1; // Using seeded product

        var updateDto = new ProductCreateUpdateDto
        {
            Barcode = "1110001110001",
            Name = "Organic Apples 1kg",
            Category = "Produce",
            UnitLabel = "Bag",
            UnitPrice = 45000,
            QuantityInStock = 200, // Updated quantity
            MinQuantity = 30,
            IsPerishable = true,
            ExpiryDate = DateTime.Today.AddDays(10)
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/products/{productId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify stock update
        var getResponse = await _client.GetAsync($"/api/products/{productId}");
        var product = await getResponse.Content.ReadFromJsonAsync<ProductDto>();
        product!.QuantityInStock.Should().Be(200);
    }
}
