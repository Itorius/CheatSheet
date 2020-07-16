using CheatSheet.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.GameContent;

namespace CheatSheet.Menus
{
	internal class GenericItemSlot : UIView
	{
		public static Texture2D backgroundTexture = TextureAssets.InventoryBack9.Value;

		public Item item = null;

		public GenericItemSlot()
		{
			onHover += Slot_OnHover;
		}

		protected override float GetWidth()
		{
			return backgroundTexture.Width * Scale;
		}

		protected override float GetHeight()
		{
			return backgroundTexture.Height * Scale;
		}

		private void Slot_OnHover(object sender, EventArgs e)
		{
			if (item != null)
			{
				//	UIView.HoverText = this.item.name;
				//	UIView.HoverItem = this.item.Clone();

				Main.hoverItemName = item.Name;
				Main.HoverItem = item.Clone();
				Main.HoverItem.SetNameOverride(Main.HoverItem.Name + (Main.HoverItem.modItem != null ? " [" + Main.HoverItem.modItem.Mod.Name + "]" : ""));
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (item != null)
			{
				spriteBatch.Draw(Slot.backgroundTexture, DrawPosition, null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
				Main.instance.LoadItem(item.type);
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
	}
}