namespace RecipeServer.Models;

public class RecipeRequest {
    public string Ingredients { get; set; } = string.Empty;
    public string GeneratedRecipe { get; set; } = string.Empty;
    public string ImagePath { get; set; } = string.Empty;
}
