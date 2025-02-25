select 
	DISTINCT top 100  IL_AISLE, convert(int, il_Aisle)
from GroceryItem
order by convert(int, il_Aisle)




/*

"BSTCHC" -> "Best Choice"
"BST-CH" -> "Best Choice"
"BST CH" -> "Best Choice"
"AL SAVE" -> "Always Save"

130 -> "Freezer Section (130)"
200 -> "Frozen Food Section (200)"
300 -> "Dairy Cooler"
600 -> "Butcher Shop"

100	-> Condiments/Dressing/PB/Jelly/Salami/Meats
400	-> Donuts/Popcorn/Beer/Alcohol
500	-> Candy/Gum/Drinks/Hardware
501	-> Personal/Medicine
999	-> Plates/Toys/Candy/Dressings/Hardware



*/
