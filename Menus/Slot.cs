using CheatSheet.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;

namespace CheatSheet.Menus
{
	internal class Slot : UIView
	{
		public Item item = new Item();

		public int index = -1;

		public static Texture2D backgroundTexture = TextureAssets.InventoryBack9.Value;

		public bool functionalSlot;
		private bool rightClicking;

		public Slot(Vector2 position, int itemNum)
		{
			Position = position;
			Init(itemNum);
		}

		public Slot(int itemNum)
		{
			Init(itemNum);
		}

		private void Init(int itemNum)
		{
			Scale = 0.85f;
			item.SetDefaults(itemNum, false);
			onLeftClick += Slot2_onLeftClick;
			//	base.onRightClick += new EventHandler(this.Slot2_onRightClick);
			onMouseDown += Slot2_onMouseDown;
			onHover += Slot2_onHover;
		}

		protected override float GetWidth()
		{
			return backgroundTexture.Width * Scale;
		}

		public override void Update()
		{
			if (!MouseRightButton)
			{
				rightClicking = false;
			}

			if (rightClicking)
			{
				Main.playerInventory = true;

				if (Main.stackSplit <= 1 /*&& Main.mouseRight */ && item.type > 0 && (Main.mouseItem.IsTheSameAs(item) || Main.mouseItem.type == 0))
				{
					int num2 = Main.superFastStack + 1;
					for (int j = 0; j < num2; j++)
					{
						if ((Main.mouseItem.stack < Main.mouseItem.maxStack || Main.mouseItem.type == 0) && item.stack > 0)
						{
							if (j == 0)
							{
								SoundEngine.PlaySound(18, -1, -1, 1);
							}

							if (Main.mouseItem.type == 0)
							{
								Main.mouseItem.netDefaults(item.netID);
								if (item.prefix != 0)
								{
									Main.mouseItem.Prefix(item.prefix);
								}

								Main.mouseItem.stack = 0;
							}

							Main.mouseItem.stack++;
							if (Main.stackSplit == 0)
							{
								Main.stackSplit = 15;
							}
							else
							{
								Main.stackSplit = Main.stackDelay;
							}
						}
					}
				}
			}
		}

		protected override float GetHeight()
		{
			return backgroundTexture.Height * Scale;
		}

		private void Slot2_onHover(object sender, EventArgs e)
		{
			//ErrorLogger.Log("On hover " + this.item.name);
			//UIView.HoverText = this.item.name;
			//UIView.HoverItem = this.item.Clone();

			//Main.craftingHide = true;
			Main.hoverItemName = item.Name; // + (item.modItem != null ? " " + item.modItem.mod.Name : "???");
			//if (item.stack > 1)
			//{
			//	object hoverItemName = Main.hoverItemName;
			//	Main.hoverItemName = string.Concat(new object[]
			//		{
			//				hoverItemName,
			//				" (",
			//				item.stack,
			//				")"
			//		});
			//}
			Main.HoverItem = item.Clone();
			Main.HoverItem.SetNameOverride(Main.HoverItem.Name + (Main.HoverItem.modItem != null ? " [" + Main.HoverItem.modItem.mod.Name + "]" : ""));
		}

		private void Slot2_onLeftClick(object sender, EventArgs e)
		{
			//ErrorLogger.Log("On Slot2_onLeftClick " + this.item.name);
			if (functionalSlot)
			{
				Item item = Main.mouseItem.Clone();
				Main.mouseItem = this.item.Clone();
				this.item = item.Clone();
				return;
			}

			if (Main.mouseItem.type == 0)
			{
				if (Main.keyState.IsKeyDown(Keys.LeftShift))
				{
					Main.LocalPlayer.QuickSpawnItem(item.type, item.maxStack);
					return;
				}

				//	ErrorLogger.Log("On Slot2_onLeftClick Here");
				//Main.mouseItem = this.item.Clone();
				Main.mouseItem.netDefaults(item.netID);
				Main.mouseItem.stack = Main.mouseItem.maxStack;
				Main.playerInventory = true;
				SoundEngine.PlaySound(18, -1, -1, 1);
			}
		}

		private void Slot2_onMouseDown(object sender, byte button)
		{
			if (button == 0)
			{
				return;
			}

			rightClicking = true;

			//ErrorLogger.Log("1");

			//if (Main.stackSplit <= 1 /*&& Main.mouseRight */&& item.type > 0 && (Main.mouseItem.IsTheSameAs(item) || Main.mouseItem.type == 0))
			//{
			//	ErrorLogger.Log("2");

			//	int num2 = Main.superFastStack + 1;
			//	for (int j = 0; j < num2; j++)
			//	{
			//		if ((Main.mouseItem.stack < Main.mouseItem.maxStack || Main.mouseItem.type == 0) && item.stack > 0)
			//		{
			//			ErrorLogger.Log("3");

			//			if (j == 0)
			//			{
			//				SoundEngine.PlaySound(18, -1, -1, 1);
			//			}
			//			if (Main.mouseItem.type == 0)
			//			{
			//				ErrorLogger.Log("4");

			//				Main.mouseItem.netDefaults(item.netID);
			//				if (item.prefix != 0)
			//				{
			//					ErrorLogger.Log("??");
			//					Main.mouseItem.Prefix((int)item.prefix);
			//				}
			//				Main.mouseItem.stack = 0;
			//			}
			//			Main.mouseItem.stack++;
			//			if (Main.stackSplit == 0)
			//			{
			//				Main.stackSplit = 15;
			//			}
			//			else
			//			{
			//				Main.stackSplit = Main.stackDelay;
			//			}
			//		}
			//	}
			//}
		}

		private void Slot2_onRightClick(object sender, EventArgs e)
		{
			//if (Main.mouseItem.type == 0)
			//{
			//	if (Main.keyState.IsKeyDown(Keys.LeftShift))
			//	{
			//		Main.LocalPlayer.QuickSpawnItem(this.item.type, this.item.maxStack);
			//		return;
			//	}
			//	//	ErrorLogger.Log("On Slot2_onLeftClick Here");
			//	Main.mouseItem = this.item.Clone();
			//	Main.mouseItem.stack = Main.mouseItem.maxStack;
			//	Main.playerInventory = true;
			//}
			//ErrorLogger.Log("1");

			if (Main.stackSplit <= 1 /*&& Main.mouseRight */ && item.type > 0 && (Main.mouseItem.IsTheSameAs(item) || Main.mouseItem.type == 0))
			{
				////ErrorLogger.Log("2");

				int num2 = Main.superFastStack + 1;
				for (int j = 0; j < num2; j++)
				{
					if ((Main.mouseItem.stack < Main.mouseItem.maxStack || Main.mouseItem.type == 0) && item.stack > 0)
					{
						//	ErrorLogger.Log("3");

						if (j == 0)
						{
							SoundEngine.PlaySound(18, -1, -1, 1);
						}

						if (Main.mouseItem.type == 0)
						{
							//	ErrorLogger.Log("4");

							Main.mouseItem.netDefaults(item.netID);
							if (item.prefix != 0)
							{
								//ErrorLogger.Log("??");
								Main.mouseItem.Prefix(item.prefix);
							}

							Main.mouseItem.stack = 0;
						}

						Main.mouseItem.stack++;
						if (Main.stackSplit == 0)
						{
							Main.stackSplit = 15;
						}
						else
						{
							Main.stackSplit = Main.stackDelay;
						}
					}
				}
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(backgroundTexture, DrawPosition, null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
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

			base.Draw(spriteBatch);
		}
	}
}