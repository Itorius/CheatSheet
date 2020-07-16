using CheatSheet.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.GameContent;

namespace CheatSheet.Menus
{
	internal class RecipeSlot : UIView
	{
		public static Texture2D backgroundTexture = TextureAssets.InventoryBack9.Value;
		public static Texture2D selectedBackgroundTexture = TextureAssets.InventoryBack15.Value;

		public int recipeIndex = -1;
		public Recipe recipe;

		public RecipeSlot(int recipeIndex)
		{
			Init(recipeIndex);
		}

		private void Init(int recipeIndex)
		{
			Scale = 0.85f;
			this.recipeIndex = recipeIndex;
			recipe = Main.recipe[recipeIndex];

			onLeftClick += Slot_onLeftClick;
			onHover += Slot_onHover;
		}

		protected override float GetWidth()
		{
			return Slot.backgroundTexture.Width * Scale;
		}

		protected override float GetHeight()
		{
			return Slot.backgroundTexture.Height * Scale;
		}

		private void Slot_onHover(object sender, EventArgs e)
		{
			//UIView.HoverText = recipe.createItem.name;
			Main.hoverItemName = recipe.createItem.Name;
			Main.HoverItem = recipe.createItem.Clone();
			Main.HoverItem.SetNameOverride(Main.HoverItem.Name + (Main.HoverItem.modItem != null ? " [" + Main.HoverItem.modItem.Mod.Name + "]" : ""));
			//UIView.HoverItem = this.item.Clone();
			//	hovering = true;
		}

		private double doubleClickTimer;

		private void Slot_onLeftClick(object sender, EventArgs e)
		{
			((Parent as RecipeView).Parent as RecipeBrowserWindow).selectedRecipe = recipe;
			((Parent as RecipeView).Parent as RecipeBrowserWindow).selectedRecipeChanged = true;
			// TODO if double click, go use item as lookup.
			if (Math.Abs(Main.time - doubleClickTimer) < 20)
			{
				RecipeBrowserWindow.lookupItemSlot.ReplaceWithFake(recipe.createItem.type);
			}

			doubleClickTimer = Main.time;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (((Parent as RecipeView).Parent as RecipeBrowserWindow).selectedRecipe == recipe)
			{
				spriteBatch.Draw(selectedBackgroundTexture, DrawPosition, null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
			}
			else
			{
				spriteBatch.Draw(backgroundTexture, DrawPosition, null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
			}

			Texture2D texture2D = TextureAssets.Item[recipe.createItem.type].Value;
			Rectangle rectangle2;
			if (Main.itemAnimations[recipe.createItem.type] != null)
			{
				rectangle2 = Main.itemAnimations[recipe.createItem.type].GetFrame(texture2D);
			}
			else
			{
				rectangle2 = texture2D.Frame();
			}

			float num = 1f;
			float num2 = backgroundTexture.Width * Scale * 0.6f;
			if (rectangle2.Width > num2 || rectangle2.Height > num2)
			{
				if (rectangle2.Width > rectangle2.Height)
				{
					num = num2 / rectangle2.Width;
				}
				else
				{
					num = num2 / rectangle2.Height;
				}
			}

			Vector2 drawPosition = DrawPosition;
			drawPosition.X += backgroundTexture.Width * Scale / 2f - rectangle2.Width * num / 2f;
			drawPosition.Y += backgroundTexture.Height * Scale / 2f - rectangle2.Height * num / 2f;
			recipe.createItem.GetColor(Color.White);
			spriteBatch.Draw(texture2D, drawPosition, rectangle2, recipe.createItem.GetAlpha(Color.White), 0f, Vector2.Zero, num, SpriteEffects.None, 0f);
			if (recipe.createItem.color != default)
			{
				spriteBatch.Draw(texture2D, drawPosition, rectangle2, recipe.createItem.GetColor(Color.White), 0f, Vector2.Zero, num, SpriteEffects.None, 0f);
			}

			if (recipe.createItem.stack > 1)
			{
				spriteBatch.DrawString(FontAssets.ItemStack.Value, recipe.createItem.stack.ToString(), new Vector2(DrawPosition.X + 10f * Scale, DrawPosition.Y + 26f * Scale), Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
			}

			base.Draw(spriteBatch);
		}
	}
}