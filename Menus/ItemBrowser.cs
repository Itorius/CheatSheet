using CheatSheet.CustomUI;
using CheatSheet.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CheatSheet.Menus
{
	internal enum ItemBrowserCategories
	{
		AllItems,
		Weapons,
		Tools,
		Armor,
		Accessories,
		Blocks,
		Ammo,
		Potions,
		Expert,
		Furniture,
		Pets,
		Mounts,

		//    Materials,
		ModItems,
	}

	internal class ItemBrowser : UISlideWindow
	{
		internal static string CSText(string key, string category = "ItemBrowser") => CheatSheet.CSText(category, key);

		private static string[] categNames =
		{
			CSText("AllItems"),
			CSText("Weapons"),
			CSText("Tools"),
			CSText("Armor"),
			CSText("Accessories"),
			CSText("Blocks"),
			CSText("Ammo"),
			CSText("Potions"),
			CSText("Expert"),
			CSText("Furniture"),
			CSText("Pets"),
			CSText("Mounts"),
			//      "Crafting Materials",
			CSText("CycleModSpecificItems"),
		};

		internal ItemView itemView;

		//	private static List<string> categoryNames = new List<string>();

		internal static UIImage[] bCategories;

		public static Dictionary<string, List<int>> ModToItems = new Dictionary<string, List<int>>();
		public static List<List<int>> categories = new List<List<int>>();

		private static Color buttonColor = new Color(190, 190, 190);

		private static Color buttonSelectedColor = new Color(209, 142, 13);

		private UITextbox textbox;

		private float spacing = 16f;

		public CheatSheet mod;

		public int lastModNameNumber;

		public ItemBrowser(CheatSheet mod)
		{
			Main.instance.LoadItem(ItemID.AlphabetStatueA);
			Main.instance.LoadItem(ItemID.SilverBroadsword);
			Main.instance.LoadItem(ItemID.SilverPickaxe);
			Main.instance.LoadItem(ItemID.SilverChainmail);
			Main.instance.LoadItem(ItemID.HermesBoots);
			Main.instance.LoadItem(ItemID.DirtBlock);
			Main.instance.LoadItem(ItemID.FlamingArrow);
			Main.instance.LoadItem(ItemID.GreaterHealingPotion);
			Main.instance.LoadItem(ItemID.WormScarf);
			Main.instance.LoadItem(ItemID.Dresser);
			Main.instance.LoadItem(ItemID.ZephyrFish);
			Main.instance.LoadItem(ItemID.SlimySaddle);
			Main.instance.LoadItem(ItemID.AlphabetStatueM);

			Texture2D[] categoryIcons = {
				TextureAssets.Item[ItemID.AlphabetStatueA].Value,
				TextureAssets.Item[ItemID.SilverBroadsword].Value,
				TextureAssets.Item[ItemID.SilverPickaxe].Value,
				TextureAssets.Item[ItemID.SilverChainmail].Value,
				TextureAssets.Item[ItemID.HermesBoots].Value,
				TextureAssets.Item[ItemID.DirtBlock].Value,
				TextureAssets.Item[ItemID.FlamingArrow].Value,
				TextureAssets.Item[ItemID.GreaterHealingPotion].Value,
				TextureAssets.Item[ItemID.WormScarf].Value,
				TextureAssets.Item[ItemID.Dresser].Value,
				TextureAssets.Item[ItemID.ZephyrFish].Value,
				TextureAssets.Item[ItemID.SlimySaddle].Value,
				//    TextureAssets.Item[ItemID.FallenStar].Value,
				TextureAssets.Item[ItemID.AlphabetStatueM].Value,
			};

			categories.Clear();
			bCategories = new UIImage[categoryIcons.Length];
			itemView = new ItemView();
			this.mod = mod;
			CanMove = true;
			Width = itemView.Width + spacing * 2f;
			Height = 420f;
			itemView.Position = new Vector2(spacing, Height - spacing - itemView.Height);
			AddChild(itemView);
			ParseList2();
			Texture2D texture = mod.GetTexture("UI/closeButton").Value;
			UIImage uIImage = new UIImage(texture /*UIView.GetEmbeddedTexture("Images.closeButton.png")*/);
			uIImage.Anchor = AnchorPosition.TopRight;
			uIImage.Position = new Vector2(Width - spacing, spacing);
			uIImage.onLeftClick += bClose_onLeftClick;
			AddChild(uIImage);
			textbox = new UITextbox();
			textbox.Width = 100;
			textbox.Anchor = AnchorPosition.TopRight;
			textbox.Position = new Vector2(Width - spacing * 2f - uIImage.Width, spacing + 40);
			//	this.textbox.Position = new Vector2(base.Width - this.spacing * 2f - uIImage.Width, this.spacing * 2f + uIImage.Height);
			textbox.KeyPressed += textbox_KeyPressed;
			AddChild(textbox);
			for (int j = 0; j < categoryIcons.Length; j++)
			{
				UIImage uIImage2 = new UIImage(categoryIcons[j]);
				Vector2 position = new Vector2(spacing, spacing);
				uIImage2.Scale = 32f / Math.Max(categoryIcons[j].Width, categoryIcons[j].Height);

				position.X += j % 12 * 40;
				position.Y += j / 12 * 40;

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

			itemView.selectedCategory = categories[0].ToArray();
			itemView.activeSlots = itemView.selectedCategory;
			itemView.ReorderSlots();
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);

			if (Visible && IsMouseInside())
			{
				Main.LocalPlayer.mouseInterface = true;
				Main.LocalPlayer.cursorItemIconEnabled = false;
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
		}

		public override void Update()
		{
			//if (!arrived)
			//{
			//	if (this.hidden)
			//	{
			//		this.lerpAmount -= .01f * ItemBrowser.moveSpeed;
			//		if (this.lerpAmount < 0f)
			//		{
			//			this.lerpAmount = 0f;
			//			arrived = true;
			//			this.Visible = false;
			//		}
			//		//float y = MathHelper.SmoothStep(this.hiddenPosition, this.shownPosition, this.lerpAmount);
			//		//base.Position = new Vector2(Hotbar.xPosition, y);
			//		base.Position = Vector2.SmoothStep(hiddenPosition, shownPosition, lerpAmount);
			//	}
			//	else
			//	{
			//		this.lerpAmount += .01f * ItemBrowser.moveSpeed;
			//		if (this.lerpAmount > 1f)
			//		{
			//			this.lerpAmount = 1f;
			//			arrived = true;
			//		}
			//		//float y2 = MathHelper.SmoothStep(this.hiddenPosition, this.shownPosition, this.lerpAmount);
			//		//base.Position = new Vector2(Hotbar.xPosition, y2);
			//		base.Position = Vector2.SmoothStep(hiddenPosition, shownPosition, lerpAmount);
			//	}
			//}

			//UIView.MousePrevLeftButton = UIView.MouseLeftButton;// (MasterView.previousMouseState.LeftButton == ButtonState.Pressed);
			//UIView.MouseLeftButton = Main.mouseLeft;// (MasterView.mouseState.LeftButton == ButtonState.Pressed);
			//UIView.MousePrevRightButton = UIView.MouseRightButton;//(MasterView.previousMouseState.RightButton == ButtonState.Pressed);
			//UIView.MouseRightButton = Main.mouseRight; //(MasterView.mouseState.RightButton == ButtonState.Pressed);
			//UIView.ScrollAmount = (Main.mouseState.ScrollWheelValue - Main.oldMouseState.ScrollWheelValue) / 2;
			//UIView.HoverItem = UIView.EmptyItem;
			//UIView.HoverText = "";
			//UIView.HoverOverridden = false;

			if (!Main.playerInventory)
			{
				//base.Visible = false;
			}

			base.Update();
		}

		private void bClose_onLeftClick(object sender, EventArgs e)
		{
			Hide();
			mod.hotbar.DisableAllWindows();
			//base.Visible = false;
		}

		private void buttonClick(object sender, EventArgs e, bool left)
		{
			UIImage uIImage = (UIImage)sender;
			int num = (int)uIImage.Tag;
			if (num == (int)ItemBrowserCategories.ModItems)
			{
				var mods = ModToItems.Keys.ToList();
				mods.Sort();
				if (mods.Count == 0)
				{
					Main.NewText("No Items have been added by mods.");
				}
				else
				{
					if (uIImage.ForegroundColor == buttonSelectedColor) lastModNameNumber = left ? (lastModNameNumber + 1) % mods.Count : (mods.Count + lastModNameNumber - 1) % mods.Count;
					string currentMod = mods[lastModNameNumber];
					itemView.selectedCategory = categories[0].Where(x => itemView.allItemsSlots[x].item.modItem != null && itemView.allItemsSlots[x].item.modItem.Mod.Name == currentMod).ToArray();
					itemView.activeSlots = itemView.selectedCategory;
					itemView.ReorderSlots();
					bCategories[num].Tooltip = categNames[num] + ": " + currentMod;
				}
			}
			else
			{
				itemView.selectedCategory = categories[num].ToArray();
				itemView.activeSlots = itemView.selectedCategory;
				itemView.ReorderSlots();
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
				itemView.activeSlots = itemView.selectedCategory;
				itemView.ReorderSlots();
				return;
			}

			List<int> list = new List<int>();
			int[] category = itemView.selectedCategory;
			for (int i = 0; i < category.Length; i++)
			{
				int num = category[i];
				Slot slot = itemView.allItemsSlots[num];
				if (slot.item.Name.ToLower().IndexOf(textbox.Text.ToLower(), StringComparison.Ordinal) != -1)
				{
					list.Add(num);
				}
			}

			if (list.Count > 0)
			{
				itemView.activeSlots = list.ToArray();
				itemView.ReorderSlots();
				return;
			}

			textbox.Text = textbox.Text.Substring(0, textbox.Text.Length - 1);
		}

		private void ParseList2()
		{
			//ItemBrowser.categoryNames = ItemBrowser.categNames.ToList<string>();
			for (int i = 0; i < categNames.Length; i++)
			{
				categories.Add(new List<int>());
				for (int j = 0; j < itemView.allItemsSlots.Length; j++)
				{
					Item item = itemView.allItemsSlots[j].item;
					//"Weapons",
					//"Tools",
					//"Armor",
					//"Accessories",
					//"Blocks",
					//"Ammo",
					//"Potions",
					//"Expert",
					//"Furniture"
					//"Pets"
					//"Mounts"
					//"Materials"
					if (i == 0)
					{
						categories[i].Add(j);
						if (j >= ItemID.Count)
						{
							string modName = ItemLoader.GetItem(j).Mod.Name;
							List<int> itemInMod;
							if (!ModToItems.TryGetValue(modName, out itemInMod)) ModToItems.Add(modName, itemInMod = new List<int>());
							itemInMod.Add(j);
						}
					}
					else if (i == (int)ItemBrowserCategories.Weapons && item.damage > 0)
					{
						categories[i].Add(j);
					}
					else if (i == (int)ItemBrowserCategories.Tools && (item.pick > 0 || item.axe > 0 || item.hammer > 0))
					{
						categories[i].Add(j);
					}
					else if (i == (int)ItemBrowserCategories.Armor && (item.headSlot != -1 || item.bodySlot != -1 || item.legSlot != -1))
					{
						categories[i].Add(j);
					}
					else if (i == (int)ItemBrowserCategories.Accessories && item.accessory)
					{
						categories[i].Add(j);
					}
					else if (i == (int)ItemBrowserCategories.Blocks && (item.createTile != -1 || item.createWall != -1))
					{
						categories[i].Add(j);
					}
					else if (i == (int)ItemBrowserCategories.Ammo && item.ammo != 0)
					{
						categories[i].Add(j);
					}
					else if (i == (int)ItemBrowserCategories.Potions && item.UseSound != null && item.UseSound.Style == 3)
					{
						categories[i].Add(j);
					}
					else if (i == (int)ItemBrowserCategories.Expert && item.expert)
					{
						categories[i].Add(j);
					}
					else if (i == (int)ItemBrowserCategories.Furniture && item.createTile != -1)
					{
						categories[i].Add(j);
					}
					else if (i == (int)ItemBrowserCategories.Pets && item.buffType > 0 && (Main.vanityPet[item.buffType] || Main.lightPet[item.buffType]))
					{
						categories[i].Add(j);
					}
					else if (i == (int)ItemBrowserCategories.Mounts && item.mountType != -1)
					{
						categories[i].Add(j);
					}
					//else if (i == (int)ItemBrowserCategories.Materials && (itemView.allItemsSlots[j].item.material || itemView.allItemsSlots[j].item.checkMat()))
					//{
					//    ItemBrowser.categories[i].Add(j);

					/*for (int b = 0; b < Recipe.numRecipes; b++)
                    {
                        for (int c = 0; c < Main.recipe[b].requiredItem.Length; c++)
                        {
                            if (Main.recipe[b].requiredItem[c].type == ItemID.Wire)
                            {
                                ItemBrowser.categories[i].Add(j);
                            }
                        }
                    }*/

					//&& (itemView.allItemsSlots[j].item.name.Substring(Math.Max(0, itemView.allItemsSlots[j].item.name.Length - 6)) == "Wrench")

					/*for (int b = 0; b < Recipe.numRecipes; b++)
                    {
                        bool hasItem = false;

                        for (int c = 0; c < Main.recipe[b].requiredItem.Length; c++)
                        {
                            if (Main.recipe[b].requiredItem[c].type == ItemID.Wire)
                            {
                                ItemBrowser.categories[i].Add(j);
                                hasItem = true;
                                break;
                            }
                        }
                        if (hasItem) continue;
                        //Main.recipe[b].createItem.name.Substring(Math.Max(0, Main.recipe[b].createItem.name.Length - 6)).ToLower() == "wrench")
                        if (Main.recipe[b].createItem.name.ToLower() == "wire")
                        {
                            ItemBrowser.categories[i].Add(j);
                            continue;
                        }
                    }*/
					//ItemBrowser.categories[i].Add(j);
					// }
				}
			}

			categories[(int)ItemBrowserCategories.Weapons] = categories[(int)ItemBrowserCategories.Weapons].OrderBy(x => itemView.allItemsSlots[x].item.damage).ToList();
			categories[(int)ItemBrowserCategories.Tools] = categories[(int)ItemBrowserCategories.Tools].OrderBy(x => itemView.allItemsSlots[x].item.pick).ToList();
			categories[(int)ItemBrowserCategories.Accessories] = categories[(int)ItemBrowserCategories.Accessories].OrderBy(x => itemView.allItemsSlots[x].item.rare).ToList();
			itemView.selectedCategory = categories[0].ToArray();
		}
	}
}