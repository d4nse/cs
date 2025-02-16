using RecipeServer.Models;
using System.Text;

namespace RecipeServer;

public class DumbRecipeGenerator {
    private List<string> ingredients;

    public DumbRecipeGenerator(RecipeRequest request) {
        this.ingredients =
            request.Ingredients.Split(',').Select(e => e.ToLower()).ToList();
    }

    private bool ContainsAny(params string[] items) {
        foreach (var item in items) {
            if (ingredients.Contains(item))
                return true;
        }
        return false;
    }

    public RecipeRequest Generate() {
        var request = new RecipeRequest();
        if (ContainsAny("egg", "eggs")) {
            request.GeneratedRecipe = @"
            You can make fried eggs.
            1. Breaks eggs into a pan.
            2. Fry eggs.
            3. Done.";
            request.ImagePath = "/images/fried-eggs.jpg";
        } else if (ContainsAny("potato", "potatoes")) {
            request.GeneratedRecipe = @"
            You can make fried potatoes.
            1. Wash potatoes.
            2. Peel potatoes.
            3. Cut potatoes.
            4. Fry potatoes in a pan.
            5. Done.";
            request.ImagePath = "/images/fried-potatoes.jpg";
        } else if (ContainsAny("cucumber", "cucumbers", "tomato", "tomatoes")) {
            request.GeneratedRecipe = @"
            You can make a summer salad.
            1. Wash ingredients.
            2. Cut ingredients into small cubes.
            3. Add together, season, mix well.
            4. Done.";
            request.ImagePath = "/images/summer-salad.png";
        } else {
            request.GeneratedRecipe = @"
            You can't make anything. Here's a picture you can drool at:";
            request.ImagePath = "/images/sample-dish.jpg";
        }
        return request;
    }
}
