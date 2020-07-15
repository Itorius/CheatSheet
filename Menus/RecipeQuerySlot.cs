using CheatSheet.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.GameContent;

namespace CheatSheet.Menus
{
	internal class RecipeQuerySlot : UIView
	{
		public static Texture2D backgroundTexture = TextureAssets.InventoryBack9.Value;
		public static Texture2D backgroundTextureFake = TextureAssets.InventoryBack8.Value;

		public Item item = new Item();
		internal bool real = true;

		public RecipeQuerySlot()
		{
			item.SetDefaults();
			onHover += Slot_OnHover;
			onLeftClick += Slot2_onLeftClick;
		}

		protected override float GetWidth()
		{
			return GenericItemSlot.backgroundTexture.Width * Scale;
		}

		protected override float GetHeight()
		{
			return GenericItemSlot.backgroundTexture.Height * Scale;
		}

		private void Slot_OnHover(object sender, EventArgs e)
		{
			Main.hoverItemName = item.Name;
			Main.HoverItem = item.Clone();
			Main.HoverItem.SetNameOverride(Main.HoverItem.Name + (Main.HoverItem.modItem != null ? " [" + Main.HoverItem.modItem.mod.Name + "]" : ""));
		}

		private void Slot2_onLeftClick(object sender, EventArgs e)
		{
			Player player = Main.LocalPlayer;
			if (real)
			{
				if (player.itemAnimation == 0 && player.itemTime == 0)
				{
					Item item = Main.mouseItem.Clone();
					Main.mouseItem = this.item.Clone();
					if (Main.mouseItem.type > 0)
					{
						Main.playerInventory = true;
					}

					this.item = item.Clone();
				}
			}
			else
			{
				if (player.itemAnimation == 0 && player.itemTime == 0)
				{
					//Item item = Main.mouseItem.Clone();
					item = Main.mouseItem.Clone();
					Main.mouseItem.SetDefaults();
					real = true;
				}
			}

			//call update.
			RecipeBrowserWindow.recipeView.ReorderSlots();
			//Main.NewText(item.type + "");
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			//if (item != null)
			{
				spriteBatch.Draw(real ? backgroundTexture : backgroundTextureFake, DrawPosition, null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
				Texture2D texture2D = TextureAssets.Item[item.type].Value;
				Rectangle rectangle2;
				if (Main.itemAnimations[item.type] != null)
				{
					rectangle2 = Main.itemAnimations[item.type].GetFrame(texture2D);
				}
				else
				{
					rectangle2 = texture2D.Frame();
				}

				float num = 1f;
				float num2 = Slot.backgroundTexture.Width * Scale * 0.6f;
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
				drawPosition.X += Slot.backgroundTexture.Width * Scale / 2f - rectangle2.Width * num / 2f;
				drawPosition.Y += Slot.backgroundTexture.Height * Scale / 2f - rectangle2.Height * num / 2f;
				item.GetColor(Color.White);
				spriteBatch.Draw(texture2D, drawPosition, rectangle2, item.GetAlpha(Color.White), 0f, Vector2.Zero, num, SpriteEffects.None, 0f);
				if (item.color != default)
				{
					spriteBatch.Draw(texture2D, drawPosition, rectangle2, item.GetColor(Color.White), 0f, Vector2.Zero, num, SpriteEffects.None, 0f);
				}

				if (item.stack > 1)
				{
					spriteBatch.DrawString(FontAssets.ItemStack.Value, item.stack.ToString(), new Vector2(DrawPosition.X + 10f * Scale, DrawPosition.Y + 26f * Scale), Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
				}
			}
			base.Draw(spriteBatch);
		}

		internal void ReplaceWithFake(int type)
		{
			if (real && RecipeBrowserWindow.lookupItemSlot.item.stack > 0)
			{
				//Main.LocalPlayer.QuickSpawnItem(RecipeBrowserWindow.lookupItemSlot.item.type, RecipeBrowserWindow.lookupItemSlot.item.stack);

				Player player = Main.LocalPlayer;
				RecipeBrowserWindow.lookupItemSlot.item.position = player.Center;
				Item item2 = player.GetItem(player.whoAmI, RecipeBrowserWindow.lookupItemSlot.item, new GetItemSettings(false,true));
				if (item2.stack > 0)
				{
					int num = Item.NewItem((int)player.position.X, (int)player.position.Y, player.width, player.height, item2.type, item2.stack, false, RecipeBrowserWindow.lookupItemSlot.item.prefix, true);
					Main.item[num].newAndShiny = false;
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, null, num, 1f);
					}
				}

				RecipeBrowserWindow.lookupItemSlot.item = new Item();
			}

			item.SetDefaults(type);
			real = false;
			RecipeBrowserWindow.recipeView.ReorderSlots();
		}
	}
}