using CheatSheet.CustomUI;
using CheatSheet.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

//TODO: projectiles, buffs/debuffs MP compatible

namespace CheatSheet.Menus
{
	internal class QuickClearHotbar : UIHotbar
	{
		internal static string CSText(string key, string category = "QuickClear") => CheatSheet.CSText(category, key);
		public UIView buttonView;
		public UIImage bItems;
		public UIImage bProjectiles;
		public UIImage bBuffs;
		public UIImage bDebuffs;

		internal bool mouseDown;
		internal bool justMouseDown;

		private CheatSheet mod;

		public QuickClearHotbar(CheatSheet mod)
		{
			this.mod = mod;
			//parentHotbar = mod.hotbar;

			buttonView = new UIView();
			Visible = false;

			Main.instance.LoadItem(ItemID.WoodenSword);
			Main.instance.LoadItem(ItemID.WoodenArrow);

			// Button images
			bItems = new UIImage(TextureAssets.Item[ItemID.WoodenSword].Value);
			bProjectiles = new UIImage(TextureAssets.Item[ItemID.WoodenArrow].Value);
			bBuffs = new UIImage(TextureAssets.Buff[BuffID.Honey].Value);
			bDebuffs = new UIImage(TextureAssets.Buff[BuffID.Poisoned].Value);

			// Button tooltips
			bItems.Tooltip = CSText("ClearDroppedItems");
			bProjectiles.Tooltip = CSText("ClearProjectiles");
			bBuffs.Tooltip = CSText("ClearBuffs");
			bDebuffs.Tooltip = CSText("ClearDebuffs");

			// Button EventHandlers
			bItems.onLeftClick += (s, e) => { HandleQuickClear(); };
			bProjectiles.onLeftClick += (s, e) => { HandleQuickClear(1); };
			bBuffs.onLeftClick += (s, e) => { HandleQuickClear(2); };
			bDebuffs.onLeftClick += (s, e) => { HandleQuickClear(3); };

			// Register mousedown
			onMouseDown += (s, e) =>
			{
				if (!Main.LocalPlayer.mouseInterface && !mod.hotbar.MouseInside && !mod.hotbar.button.MouseInside)
				{
					mouseDown = true;
					Main.LocalPlayer.mouseInterface = true;
				}
			};
			onMouseUp += (s, e) =>
			{
				justMouseDown = true;
				mouseDown = false; /*startTileX = -1; startTileY = -1;*/
			};

			// ButtonView
			buttonView.AddChild(bItems);
			buttonView.AddChild(bProjectiles);
			buttonView.AddChild(bBuffs);
			buttonView.AddChild(bDebuffs);

			Width = 200f;
			Height = 55f;
			buttonView.Height = Height;
			Anchor = AnchorPosition.Top;
			AddChild(buttonView);
			Position = new Vector2(Hotbar.xPosition, hiddenPosition);
			CenterXAxisToParentCenter();
			float num = spacing;
			for (int i = 0; i < buttonView.children.Count; i++)
			{
				buttonView.children[i].Anchor = AnchorPosition.Left;
				buttonView.children[i].Position = new Vector2(num, 0f);
				buttonView.children[i].CenterYAxisToParentCenter();
				buttonView.children[i].Visible = true;
				buttonView.children[i].ForegroundColor = buttonUnselectedColor;
				num += buttonView.children[i].Width + spacing;
			}

			Resize();
		}

		public static void HandleQuickClear(int clearType = 0, bool forceHandle = false, int whoAmI = 0)
		{
			bool syncData = forceHandle || Main.netMode == 0;
			if (syncData)
			{
				ClearObjects(clearType, forceHandle, whoAmI);
			}
			else
			{
				SyncQuickClear(clearType);
			}
		}

		private static void SyncQuickClear(int clearType = 0)
		{
			var netMessage = CheatSheet.instance.GetPacket();
			netMessage.Write((byte)CheatSheetMessageType.QuickClear);
			netMessage.Write(clearType);
			netMessage.Send();
		}

		private static void ClearObjects(int clearType = 0, bool syncData = false, int whoAmI = 0)
		{
			Player player;
			if (!syncData)
			{
				player = Main.LocalPlayer;
			}
			else
			{
				player = Main.player[whoAmI];
			}

			switch (clearType)
			{
				case 0:
					HandleClearItems(syncData);
					break;

				case 1:
					HandleClearProjectiles(syncData);
					break;

				case 2:
					HandleClearBuffs(player);
					break;

				case 3:
					HandleClearBuffs(player, true);
					break;
			}
		}

		private static void HandleClearItems(bool syncData = false)
		{
			for (int i = 0; i < Main.maxItems; i++)
			{
				if (!syncData)
				{
					Main.item[i].active = false;
				}
				else
				{
					Main.item[i].SetDefaults();
					NetMessage.SendData(21, -1, -1, null, i);
				}
			}
		}

		public static void HandleClearProjectiles(bool syncData = false)
		{
			for (int i = 0; i < Main.maxProjectiles; i++)
			{
				if (Main.projectile[i].active)
				{
					Main.projectile[i].Kill();
					//Main.projectile[i].SetDefaults(0);
					if (syncData)
					{
						NetMessage.SendData(27, -1, -1, null, i);
					}
				}
			}
		}

		public static void HandleClearBuffs(Player player, bool debuffsOnly = false)
		{
			// buffs are only syncing when added for PvP
			for (int b = 0; b < 22; b++)
			{
				if (debuffsOnly && !Main.debuff[player.buffType[b]]) continue;

				if (player.buffType[b] > 0)
				{
					player.buffTime[b] = 0;
					player.buffType[b] = 0;
					if (debuffsOnly)
					{
						for (int i = 0; i < 21; i++)
						{
							if (player.buffTime[i] == 0 || player.buffType[i] == 0)
							{
								for (int j = i + 1; j < 22; j++)
								{
									player.buffTime[j - 1] = player.buffTime[j];
									player.buffType[j - 1] = player.buffType[j];
									player.buffTime[j] = 0;
									player.buffType[j] = 0;
								}
							}
						}
					}
				}
			}
		}

		public override void Update()
		{
			DoSlideMovement();

			CenterXAxisToParentCenter();
			base.Update();
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (Visible)
			{
				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, _rasterizerState, null, Main.UIScaleMatrix);
				//	Rectangle scissorRectangle = new Rectangle((int)base.X- (int)base.Width, (int)base.Y, (int)base.Width, (int)base.Height);
				//Parent.Position.Y
				//		Main.NewText((int)Parent.Position.Y + " " + (int)shownPosition);
				//	Rectangle scissorRectangle = new Rectangle((int)(base.X - base.Width / 2), (int)(shownPosition), (int)base.Width, (int)base.Height);
				Rectangle scissorRectangle = new Rectangle((int)(X - Width / 2), (int)shownPosition, (int)Width, (int)(mod.hotbar.Position.Y - shownPosition));
				/*if (scissorRectangle.X < 0)
				{
					scissorRectangle.Width += scissorRectangle.X;
					scissorRectangle.X = 0;
				}
				if (scissorRectangle.Y < 0)
				{
					scissorRectangle.Height += scissorRectangle.Y;
					scissorRectangle.Y = 0;
				}
				if ((float)scissorRectangle.X + base.Width > (float)Main.screenWidth)
				{
					scissorRectangle.Width = Main.screenWidth - scissorRectangle.X;
				}
				if ((float)scissorRectangle.Y + base.Height > (float)Main.screenHeight)
				{
					scissorRectangle.Height = Main.screenHeight - scissorRectangle.Y;
				}*/
				scissorRectangle = CheatSheet.GetClippingRectangle(spriteBatch, scissorRectangle);
				Rectangle scissorRectangle2 = spriteBatch.GraphicsDevice.ScissorRectangle;
				spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;

				base.Draw(spriteBatch);

				spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle2;
				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Main.UIScaleMatrix);
			}

			//	base.Draw(spriteBatch);

			if (Visible && base.IsMouseInside())
			{
				Main.LocalPlayer.mouseInterface = true;
				//Main.LocalPlayer.cursorItemIconEnabled = false;
			}

			if (Visible && IsMouseInside())
			{
				Main.LocalPlayer.mouseInterface = true;
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

		protected override bool IsMouseInside()
		{
			return hidden ? false : base.IsMouseInside();
		}

		public void Resize()
		{
			float num = spacing;
			for (int i = 0; i < buttonView.children.Count; i++)
			{
				if (buttonView.children[i].Visible)
				{
					buttonView.children[i].X = num;
					num += buttonView.children[i].Width + spacing;
				}
			}

			Width = num;
			buttonView.Width = Width;
		}

		public void Hide()
		{
			hidden = true;
			arrived = false;
		}

		public void Show()
		{
			mod.hotbar.currentHotbar = this;
			arrived = false;
			hidden = false;
			Visible = true;
		}
	}
}