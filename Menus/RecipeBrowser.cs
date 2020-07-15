using CheatSheet.CustomUI;
using CheatSheet.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;

namespace CheatSheet.Menus
{
	internal enum RecipeBrowserCategories
	{
		AllRecipes,
		ModRecipes
	}

	internal class RecipeBrowserWindow : UISlideWindow
	{
		internal static string CSText(string key, string category = "RecipeBrowser") => CheatSheet.CSText(category, key);

		private static string[] categNames =
		{
			CSText("AllRecipes"),
			CSText("CycleModSpecificRecipes")
		};

		internal static RecipeView recipeView;
		public CheatSheet mod;

		//private static List<string> categoryNames = new List<string>();
		internal static UIImage[] bCategories;

		//private static GenericItemSlot[] lookupItem = new GenericItemSlot[1];
		internal static RecipeQuerySlot lookupItemSlot;

		internal static GenericItemSlot[] ingredients;
		//internal static GenericItemSlot[] tiles = new GenericItemSlot[Recipe.maxRequirements];

		public static List<List<int>> categories = new List<List<int>>();
		private static Color buttonColor = new Color(190, 190, 190);

		private static Color buttonSelectedColor = new Color(209, 142, 13);

		private UITextbox textbox;

		private float spacing = 16f;
		private float halfspacing = 8f;

		public int lastModNameNumber;

		public Recipe selectedRecipe = null;
		internal bool selectedRecipeChanged;

		// 270 : 16 40 ?? 16

		public RecipeBrowserWindow(CheatSheet mod)
		{
			Main.instance.LoadItem(ItemID.AlphabetStatueA);
			Main.instance.LoadItem(ItemID.AlphabetStatueM);
			
			Texture2D[] categoryIcons =
			{
				TextureAssets.Item[ItemID.AlphabetStatueA].Value,
				TextureAssets.Item[ItemID.AlphabetStatueM].Value,
			};
			
			categories.Clear();
			bCategories = new UIImage[categoryIcons.Length];
			recipeView = new RecipeView();
			this.mod = mod;
			CanMove = true;
			Width = recipeView.Width + spacing * 2f;
			Height = 420f;
			recipeView.Position = new Vector2(spacing, spacing + 40);
			AddChild(recipeView);
			InitializeRecipeCategories();
			Texture2D texture = mod.GetTexture("UI/closeButton").Value;
			UIImage uIImage = new UIImage(texture);
			uIImage.Anchor = AnchorPosition.TopRight;
			uIImage.Position = new Vector2(Width - spacing, spacing);
			uIImage.onLeftClick += bClose_onLeftClick;
			AddChild(uIImage);
			textbox = new UITextbox();
			textbox.Anchor = AnchorPosition.TopRight;
			textbox.Position = new Vector2(Width - spacing * 2f - uIImage.Width, spacing /** 2f + uIImage.Height*/);
			textbox.KeyPressed += textbox_KeyPressed;
			AddChild(textbox);

			//lookupItemSlot = new Slot(0);
			lookupItemSlot = new RecipeQuerySlot();
			lookupItemSlot.Position = new Vector2(spacing, halfspacing);
			lookupItemSlot.Scale = .85f;
			//lookupItemSlot.functionalSlot = true;
			AddChild(lookupItemSlot);

			for (int j = 0; j < categoryIcons.Length; j++)
			{
				UIImage uIImage2 = new UIImage(categoryIcons[j]);
				Vector2 position = new Vector2(spacing + 48, spacing);
				uIImage2.Scale = 32f / Math.Max(categoryIcons[j].Width, categoryIcons[j].Height);

				position.X += j % 6 * 40;
				position.Y += j / 6 * 40;

				if (categoryIcons[j].Height > categoryIcons[j].Width)
				{
					position.X += (32 - categoryIcons[j].Width) / 2;
				}
				else if (categoryIcons[j].Height < categoryIcons[j].Width)
				{
					position.Y += (32 - categoryIcons[j].Height) / 2;
				}

				uIImage2.Position = position;
				uIImage2.Tag = j;
				uIImage2.onLeftClick += (s, e) => buttonClick(s, e, true);
				uIImage2.onRightClick += (s, e) => buttonClick(s, e, false);
				uIImage2.ForegroundColor = buttonColor;
				if (j == 0)
				{
					uIImage2.ForegroundColor = buttonSelectedColor;
				}

				uIImage2.Tooltip = categNames[j];
				bCategories[j] = uIImage2;
				AddChild(uIImage2);
			}

			ingredients = new GenericItemSlot[Recipe.maxRequirements];
			for (int j = 0; j < Recipe.maxRequirements; j++)
			{
				GenericItemSlot genericItemSlot = new GenericItemSlot();
				Vector2 position = new Vector2(spacing, spacing);

				//position.X += j * 60 + 120;
				//position.Y += 250;

				position.X += 166 + j % cols * 51;
				position.Y += 244 + j / cols * 51;

				genericItemSlot.Position = position;
				genericItemSlot.Tag = j;
				ingredients[j] = genericItemSlot;
				AddChild(genericItemSlot, false);
			}

			recipeView.selectedCategory = categories[0].ToArray();
			recipeView.activeSlots = recipeView.selectedCategory;
			recipeView.ReorderSlots();
		}

		private const int cols = 5;

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);

			if (Visible && IsMouseInside())
			{
				Main.LocalPlayer.mouseInterface = true;
				Main.LocalPlayer.cursorItemIconEnabled = false;
			}

			if (Visible && Recipe.numRecipes > recipeView.allRecipeSlot.Length)
			{
				//			ErrorLogger.Log("New " + Recipe.numRecipes + " " + recipeView.allRecipeSlot.Length);

				recipeView.allRecipeSlot = new RecipeSlot[Recipe.numRecipes];
				for (int i = 0; i < recipeView.allRecipeSlot.Length; i++)
				{
					recipeView.allRecipeSlot[i] = new RecipeSlot(i);
				}

				InitializeRecipeCategories();

				recipeView.selectedCategory = categories[0].ToArray();
				recipeView.activeSlots = recipeView.selectedCategory;
				recipeView.ReorderSlots();
			}

			float x = FontAssets.MouseText.Value.MeasureString(HoverText).X;
			Vector2 vector = new Vector2(Main.mouseX, Main.mouseY) + new Vector2(16f);
			if (vector.Y > Main.screenHeight - 30)
			{
				vector.Y = Main.screenHeight - 30;
			}

			if (vector.X > Main.screenWidth - x)
			{
				vector.X = Main.screenWidth - 460;
			}

			Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.MouseText.Value, HoverText, vector.X, vector.Y, new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor), Color.Black, Vector2.Zero);

			float positionX = X + spacing;
			float positionY = Y + 270; //320;
			string text4;
			if (selectedRecipe != null && Visible)
			{
				Color color3 = new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor);

				text4 = Lang.inter[21] + " " + Main.guideItem.Name;
				spriteBatch.DrawString(FontAssets.MouseText.Value, Lang.inter[22].Value, new Vector2(positionX, positionY), color3, 0f, default, 1f, SpriteEffects.None, 0f);
				//	int num60 = Main.focusRecipe;
				int num61 = 0;
				int num62 = 0;
				while (num62 < Recipe.maxRequirements)
				{
					int num63 = (num62 + 1) * 26;
					if (selectedRecipe.requiredTile[num62] == -1)
					{
						if (num62 == 0 && !selectedRecipe.needWater && !selectedRecipe.needHoney && !selectedRecipe.needLava)
						{
							spriteBatch.DrawString(FontAssets.MouseText.Value, Lang.inter[23].Value, new Vector2(positionX, positionY + num63), color3, 0f, default, 1f, SpriteEffects.None, 0f);
						}

						break;
					}

					num61++;
					spriteBatch.DrawString(FontAssets.MouseText.Value, Lang.GetMapObjectName(MapHelper.TileToLookup(selectedRecipe.requiredTile[num62], 0)), new Vector2(positionX, positionY + num63), color3, 0f, default, 1f, SpriteEffects.None, 0f);
					num62++;
				}

				if (selectedRecipe.needWater)
				{
					int num64 = (num61 + 1) * 26;
					spriteBatch.DrawString(FontAssets.MouseText.Value, Lang.inter[53].Value, new Vector2(positionX, positionY + num64), color3, 0f, default, 1f, SpriteEffects.None, 0f);
				}

				if (selectedRecipe.needHoney)
				{
					int num65 = (num61 + 1) * 26;
					spriteBatch.DrawString(FontAssets.MouseText.Value, Lang.inter[58].Value, new Vector2(positionX, positionY + num65), color3, 0f, default, 1f, SpriteEffects.None, 0f);
				}

				if (selectedRecipe.needLava)
				{
					int num66 = (num61 + 1) * 26;
					spriteBatch.DrawString(FontAssets.MouseText.Value, Lang.inter[56].Value, new Vector2(positionX, positionY + num66), color3, 0f, default, 1f, SpriteEffects.None, 0f);
				}
			}

			//else
			//{
			//	text4 = Lang.inter[24];
			//}
			//spriteBatch.DrawString(FontAssets.MouseText.Value, text4, new Vector2((float)(positionX + 50), (float)(positionY + 12)), new Microsoft.Xna.Framework.Color((int)Main.mouseTextColor, (int)Main.mouseTextColor, (int)Main.mouseTextColor, (int)Main.mouseTextColor), 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
		}

		public override void Update()
		{
			//	UIView.MousePrevLeftButton = UIView.MouseLeftButton;
			//	UIView.MouseLeftButton = Main.mouseLeft;
			//	UIView.MousePrevRightButton = UIView.MouseRightButton;
			//	UIView.MouseRightButton = Main.mouseRight;
			//	UIView.ScrollAmount = (Main.mouseState.ScrollWheelValue - Main.oldMouseState.ScrollWheelValue) / 2;
			//	UIView.HoverItem = UIView.EmptyItem;
			//	UIView.HoverText = "";
			//	UIView.HoverOverridden = false;

			if (selectedRecipeChanged)
			{
				//ErrorLogger.Log("1");
				//foreach(var a in CheatSheet.ButtonClicked)
				//{
				//	Main.NewText(">");
				//	ErrorLogger.Log("button pressing");

				//	a(selectedRecipe.requiredItem[0].type);
				//	Main.NewText("<");
				//}

				selectedRecipeChanged = false;
				string oldname = Main.HoverItem.Name;
				for (int i = 0; i < Recipe.maxRequirements; i++)
				{
					if (selectedRecipe.requiredItem[i].type > 0)
					{
						ingredients[i].item = selectedRecipe.requiredItem[i];

						string name;
						if (selectedRecipe.ProcessGroupsForText(selectedRecipe.requiredItem[i].type, out name))
						{
							Main.HoverItem.SetNameOverride(name);
						}

						if (selectedRecipe.anyIronBar && selectedRecipe.requiredItem[i].type == 22)
						{
							Main.HoverItem.SetNameOverride(Lang.misc[37] + " " + Lang.GetItemNameValue(22));
						}
						else if (selectedRecipe.anyWood && selectedRecipe.requiredItem[i].type == 9)
						{
							Main.HoverItem.SetNameOverride(Lang.misc[37] + " " + Lang.GetItemNameValue(9));
						}
						else if (selectedRecipe.anySand && selectedRecipe.requiredItem[i].type == 169)
						{
							Main.HoverItem.SetNameOverride(Lang.misc[37] + " " + Lang.GetItemNameValue(169));
						}
						else if (selectedRecipe.anyFragment && selectedRecipe.requiredItem[i].type == 3458)
						{
							Main.HoverItem.SetNameOverride(Lang.misc[37] + " " + Lang.misc[51]);
						}
						else if (selectedRecipe.anyPressurePlate && selectedRecipe.requiredItem[i].type == 542)
						{
							Main.HoverItem.SetNameOverride(Lang.misc[37] + " " + Lang.misc[38]);
						}
						//else
						//{
						//	ModRecipe recipe = selectedRecipe as ModRecipe;
						//	if (recipe != null)
						//	{
						//		recipe.CraftGroupDisplayName(i);
						//	}
						//}

						if (Main.HoverItem.Name != oldname)
						{
							Main.HoverItem.SetNameOverride(oldname);
							ingredients[i].item.SetNameOverride(Main.HoverItem.Name);
						}
					}
					else
					{
						ingredients[i].item = null;
					}

					//				if (selectedRecipe.requiredTile[i] > -1)
					//				{
					//					tiles[i].item = selectedRecipe.requiredItem[i]
					//;
					//				}
					//				else
					//				{
					//					ingredients[i].item = null;
					//				}
					//				this.requiredItem[i] = new Item();
					//				this.requiredTile[i] = -1;
				}
			}

			base.Update();
		}

		private void bClose_onLeftClick(object sender, EventArgs e)
		{
			if (lookupItemSlot.real && lookupItemSlot.item.stack > 0)
			{
				//Main.LocalPlayer.QuickSpawnItem(lookupItemSlot.item.type, lookupItemSlot.item.stack);
				//lookupItemSlot.item.SetDefaults(0);

				Player player = Main.LocalPlayer;
				lookupItemSlot.item.position = player.Center;
				Item item = player.GetItem(player.whoAmI, lookupItemSlot.item, new GetItemSettings(false, true));
				if (item.stack > 0)
				{
					int num = Item.NewItem((int)player.position.X, (int)player.position.Y, player.width, player.height, item.type, item.stack, false, lookupItemSlot.item.prefix, true);
					Main.item[num].newAndShiny = false;
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, null, num, 1f);
					}
				}

				lookupItemSlot.item = new Item();

				recipeView.ReorderSlots();
			}

			Hide();
			mod.hotbar.DisableAllWindows();
			//base.Visible = false;
		}

		private void buttonClick(object sender, EventArgs e, bool left)
		{
			UIImage uIImage = (UIImage)sender;
			int num = (int)uIImage.Tag;
			if (num == (int)RecipeBrowserCategories.ModRecipes)
			{
				var mods = ModLoader.Mods.Select(x => x.Name).ToList();
				mods = mods.Intersect(categories[0].Select(x => (recipeView.allRecipeSlot[x].recipe as ModRecipe)?.mod.Name ?? null)).ToList();
				mods.Sort();
				if (mods.Count == 0)
				{
					Main.NewText("No Recipes have been added by mods.");
					return;
				}

				if (uIImage.ForegroundColor == buttonSelectedColor) lastModNameNumber = left ? (lastModNameNumber + 1) % mods.Count : (mods.Count + lastModNameNumber - 1) % mods.Count;
				string currentMod = mods[lastModNameNumber];
				recipeView.selectedCategory = categories[0].Where(x => recipeView.allRecipeSlot[x].recipe as ModRecipe != null && (recipeView.allRecipeSlot[x].recipe as ModRecipe).mod.Name == currentMod).ToArray();
				recipeView.activeSlots = recipeView.selectedCategory;
				recipeView.ReorderSlots();
				bCategories[num].Tooltip = categNames[num] + ": " + currentMod;
			}
			else
			{
				recipeView.selectedCategory = categories[num].ToArray();
				recipeView.activeSlots = recipeView.selectedCategory;
				recipeView.ReorderSlots();
			}

			textbox.Text = "";
			UIImage[] array = bCategories;
			for (int j = 0; j < array.Length; j++)
			{
				UIImage uIImage2 = array[j];
				uIImage2.ForegroundColor = buttonColor;
			}

			uIImage.ForegroundColor = buttonSelectedColor;
		}

		private void textbox_KeyPressed(object sender, char key)
		{
			if (textbox.Text.Length <= 0)
			{
				recipeView.activeSlots = recipeView.selectedCategory;
				recipeView.ReorderSlots();
				return;
			}

			List<int> list = new List<int>();
			int[] category = recipeView.selectedCategory;
			for (int i = 0; i < category.Length; i++)
			{
				int num = category[i];
				RecipeSlot slot = recipeView.allRecipeSlot[num];
				if (slot.recipe.createItem.Name.ToLower().IndexOf(textbox.Text.ToLower(), StringComparison.Ordinal) != -1)
				{
					list.Add(num);
				}

				//else
				//{
				//	for (int j = 0; j < slot.recipe.requiredItem.Length; i++)
				//	{
				//		if (slot.recipe.requiredItem[j].type > 0 && slot.recipe.requiredItem[j].name.ToLower().IndexOf(this.textbox.Text.ToLower(), StringComparison.Ordinal) != -1)
				//		{
				//			list.Add(num);
				//			break;
				//		}
				//	}
				//}
			}

			if (list.Count > 0)
			{
				recipeView.activeSlots = list.ToArray();
				recipeView.ReorderSlots();
				return;
			}

			textbox.Text = textbox.Text.Substring(0, textbox.Text.Length - 1);
		}

		private void InitializeRecipeCategories()
		{
			//	RecipeBrowser.categoryNames = RecipeBrowser.categNames.ToList<string>();
			for (int i = 0; i < categNames.Length; i++)
			{
				categories.Add(new List<int>());
				for (int j = 0; j < recipeView.allRecipeSlot.Length; j++)
				{
					if (i == 0)
					{
						categories[i].Add(j);
					}

					//else if (i == 1 && recipeView.allNPCSlot[j].npc.boss)
					//{
					//	RecipeBrowser.categories[i].Add(j);
					//}
					//else if (i == 2 && recipeView.allNPCSlot[j].npc.townNPC)
					//{
					//	RecipeBrowser.categories[i].Add(j);
					//}
				}
			}

			recipeView.selectedCategory = categories[0].ToArray();
		}
	}
}