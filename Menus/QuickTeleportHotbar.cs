using CheatSheet.CustomUI;
using CheatSheet.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace CheatSheet.Menus
{
	internal class QuickTeleportHotbar : UIHotbar
	{
		internal static string CSText(string key, string category = "Teleport") => CheatSheet.CSText(category, key);
		public UIView buttonView;
		public UIImage bDungeon;
		public UIImage bSpawn;
		public UIImage bHell;
		public UIImage bTemple;
		public UIImage bRandom;

		private CheatSheet mod;

		internal bool mouseDown;
		internal bool justMouseDown;

		public QuickTeleportHotbar(CheatSheet mod)
		{
			this.mod = mod;
			//parentHotbar = mod.hotbar;

			buttonView = new UIView();
			Visible = false;
			//base.UpdateWhenOutOfBounds = true;

			Main.instance.LoadItem(ItemID.DungeonDoor);
			Main.instance.LoadItem(ItemID.WoodenDoor);
			Main.instance.LoadItem(ItemID.ObsidianDoor);
			Main.instance.LoadItem(ItemID.LihzahrdDoor);
			Main.instance.LoadItem(ItemID.SpookyDoor);
			
			bDungeon = new UIImage(TextureAssets.Item[ItemID.DungeonDoor].Value);
			bSpawn = new UIImage(TextureAssets.Item[ItemID.WoodenDoor].Value);
			bHell = new UIImage(TextureAssets.Item[ItemID.ObsidianDoor].Value);
			bTemple = new UIImage(TextureAssets.Item[ItemID.LihzahrdDoor].Value);
			bRandom = new UIImage(TextureAssets.Item[ItemID.SpookyDoor].Value);

			bDungeon.Tooltip = CSText("Dungeon");
			bSpawn.Tooltip = CSText("Spawnpoint");
			bHell.Tooltip = CSText("Hell");
			bTemple.Tooltip = CSText("Temple");
			bRandom.Tooltip = CSText("Random");

			bDungeon.onLeftClick += (s, e) => { HandleTeleport(); };
			bSpawn.onLeftClick += (s, e) => { HandleTeleport(1); };
			bHell.onLeftClick += (s, e) => { HandleTeleport(2); };
			bTemple.onLeftClick += (s, e) => { HandleTeleport(3); };
			bRandom.onLeftClick += (s, e) => { HandleTeleport(4); };

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

			//UpdateWhenOutOfBounds = true;

			buttonView.AddChild(bDungeon);
			buttonView.AddChild(bSpawn);
			buttonView.AddChild(bHell);
			buttonView.AddChild(bTemple);
			buttonView.AddChild(bRandom);

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

		public static void HandleTeleport(int teleportType = 0, bool forceHandle = false, int whoAmI = 0)
		{
			bool syncData = forceHandle || Main.netMode == 0;
			if (syncData)
			{
				TeleportPlayer(teleportType, forceHandle, whoAmI);
			}
			else
			{
				SyncTeleport(teleportType);
			}
		}

		private static void SyncTeleport(int teleportType = 0)
		{
			var netMessage = CheatSheet.instance.GetPacket();
			netMessage.Write((byte)CheatSheetMessageType.TeleportPlayer);
			netMessage.Write(teleportType);
			netMessage.Send();
		}

		private static void TeleportPlayer(int teleportType = 0, bool syncData = false, int whoAmI = 0)
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

			switch (teleportType)
			{
				case 0:
					HandleDungeonTeleport(player, syncData);
					break;

				case 1:
					HandleSpawnTeleport(player, syncData);
					break;

				case 2:
					HandleHellTeleport(player, syncData);
					break;

				case 3:
					HandleTempleTeleport(player, syncData);
					break;

				case 4:
					HandleRandomTeleport(player, syncData);
					break;
			}
		}

		private static void HandleDungeonTeleport(Player player, bool syncData = false)
		{
			RunTeleport(player, new Vector2(Main.dungeonX, Main.dungeonY), syncData, true);
		}

		private static void HandleSpawnTeleport(Player player, bool syncData = false)
		{
			RunTeleport(player, new Vector2(Main.spawnTileX, Main.spawnTileY), syncData, true);
		}

		private static void HandleHellTeleport(Player player, bool syncData = false)
		{
			bool teleportDestinationFound = false;
			bool flag2;
			int findTeleportDestinationAttempts = 0;
			int num2 = 0;
			int num3 = 0;
			int Width = player.width;

			Vector2 teleportPosition = new Vector2(num2, num3) * 16f + new Vector2((float)(-(double)Width / 2.0 + 8.0), -(float)player.height);
			while (!teleportDestinationFound && findTeleportDestinationAttempts < 1000)
			{
				++findTeleportDestinationAttempts;
				int tileX = Main.rand.Next(Main.maxTilesX - 200);
				int tileY = Main.rand.Next(Main.maxTilesY - 200, Main.maxTilesY);
				teleportPosition = new Vector2(tileX, tileY) * 16f + new Vector2((float)(-Width / 2.0 + 8.0), -player.height);
				if (!Collision.SolidCollision(teleportPosition, Width, player.height))
				{
					if (Main.tile[tileX, tileY] == null) Main.tile[tileX, tileY] = new Tile();
					if ((Main.tile[tileX, tileY].wall != 87 || tileY <= Main.worldSurface || NPC.downedPlantBoss) && (!Main.wallDungeon[Main.tile[tileX, tileY].wall] || tileY <= Main.worldSurface || NPC.downedBoss3))
					{
						int num4 = 0;
						while (num4 < 100 && WorldGen.InWorld(tileX, tileY + num4, 20))
						{
							if (Main.tile[tileX, tileY + num4] == null) Main.tile[tileX, tileY + num4] = new Tile();
							Tile tile = Main.tile[tileX, tileY + num4];
							teleportPosition = new Vector2(tileX, tileY + num4) * 16f + new Vector2((float)(-(double)Width / 2.0 + 8.0), -player.height);
							Vector4 vector4 = Collision.SlopeCollision(teleportPosition, player.velocity, Width, player.height, player.gravDir);
							flag2 = !Collision.SolidCollision(teleportPosition, Width, player.height);
							if (vector4.Z == (double)player.velocity.X)
							{
								double num5 = player.velocity.Y;
							}

							if (flag2) ++num4;
							else if (!tile.active() || tile.inActive() || !Main.tileSolid[tile.type]) ++num4;
							else break;
						}

						if (!Collision.LavaCollision(teleportPosition, Width, player.height) && Collision.HurtTiles(teleportPosition, player.velocity, Width, player.height).Y <= 0.0)
						{
							Collision.SlopeCollision(teleportPosition, player.velocity, Width, player.height, player.gravDir);
							if (Collision.SolidCollision(teleportPosition, Width, player.height) && num4 < 99)
							{
								Vector2 Velocity1 = Vector2.UnitX * 16f;
								if (!(Collision.TileCollision(teleportPosition - Velocity1, Velocity1, player.width, player.height, false, false, (int)player.gravDir) != Velocity1))
								{
									Vector2 Velocity2 = -Vector2.UnitX * 16f;
									if (!(Collision.TileCollision(teleportPosition - Velocity2, Velocity2, player.width, player.height, false, false, (int)player.gravDir) != Velocity2))
									{
										Vector2 Velocity3 = Vector2.UnitY * 16f;
										if (!(Collision.TileCollision(teleportPosition - Velocity3, Velocity3, player.width, player.height, false, false, (int)player.gravDir) != Velocity3))
										{
											Vector2 Velocity4 = -Vector2.UnitY * 16f;
											if (!(Collision.TileCollision(teleportPosition - Velocity4, Velocity4, player.width, player.height, false, false, (int)player.gravDir) != Velocity4))
											{
												teleportDestinationFound = true;
												int num5 = tileY + num4;
												break;
											}
										}
									}
								}
							}
						}
					}
				}
			}

			if (!teleportDestinationFound) return;

			RunTeleport(player, teleportPosition, syncData);
		}

		private static void HandleTempleTeleport(Player player, bool syncData = false)
		{
			Vector2 prePos = player.position;
			Vector2 pos = prePos;
			for (int x = 0; x < Main.tile.GetLength(0); ++x) // LOOP WORLD X
			{
				for (int y = 0; y < Main.tile.GetLength(1); ++y) // LOOP WORLD Y
				{
					if (Main.tile[x, y] == null) continue;
					if (Main.tile[x, y].type != 237) continue;
					//if (Main.tile[x, y].wall != 87) continue;
					pos = new Vector2((x + 2) * 16, y * 16); // get temple pos
					break;
				}
			}

			if (pos != prePos)
			{
				RunTeleport(player, new Vector2(pos.X, pos.Y), syncData);
			}
			else return; //not found
		}

		private static void HandleRandomTeleport(Player player, bool syncData = false)
		{
			player.TeleportationPotion(); // RND
			LeaveDust(player);
			if (Main.netMode != 2) return;
			if (syncData)
			{
				RemoteClient.CheckSection(player.whoAmI, player.position);
				NetMessage.SendData(65, -1, -1, null, 0, player.whoAmI, player.position.X, player.position.Y, 3);
			}
		}

		protected override bool IsMouseInside()
		{
			return base.IsMouseInside();
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

		private void DisableAllWindows()
		{
			bDungeon.ForegroundColor = buttonUnselectedColor;
			bSpawn.ForegroundColor = buttonUnselectedColor;
			bHell.ForegroundColor = buttonUnselectedColor;
			bTemple.ForegroundColor = buttonUnselectedColor;
		}

		public override void Update()
		{
			DoSlideMovement();

			CenterXAxisToParentCenter();
			base.Update();
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

		private bool preHidePaintTiles;
		private bool preHideEyeDropper;
		private bool constrainToAxis;
		private int constrainedX;
		private int constrainedY;
		private int constrainedStartX;
		private int constrainedStartY;

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

		private static void RunTeleport(Player player, Vector2 pos, bool syncData = false, bool convertFromTiles = false)
		{
			bool postImmune = player.immune;
			int postImmunteTime = player.immuneTime;

			if (convertFromTiles) pos = new Vector2(pos.X * 16 + 8 - player.width / 2, pos.Y * 16 - player.height);

			LeaveDust(player);

			//Kill hooks
			player.grappling[0] = -1;
			player.grapCount = 0;
			for (int index = 0; index < 1000; ++index)
			{
				if (Main.projectile[index].active && Main.projectile[index].owner == player.whoAmI && Main.projectile[index].aiStyle == 7) Main.projectile[index].Kill();
			}

			player.Teleport(pos, 2);
			player.velocity = Vector2.Zero;
			player.immune = postImmune;
			player.immuneTime = postImmunteTime;

			LeaveDust(player);

			if (Main.netMode != 2) return;

			if (syncData)
			{
				RemoteClient.CheckSection(player.whoAmI, player.position);
				NetMessage.SendData(65, -1, -1, null, 0, player.whoAmI, pos.X, pos.Y, 3);
			}
		}

		private static void LeaveDust(Player player)
		{
			//Leave dust
			for (int index = 0; index < 70; ++index) Main.dust[Dust.NewDust(player.position, player.width, player.height, 15, player.velocity.X * 0.2f, player.velocity.Y * 0.2f, 150, Color.Cyan, 1.2f)].velocity *= 0.5f;
			Main.TeleportEffect(player.getRect(), 1);
			Main.TeleportEffect(player.getRect(), 3);
		}
	}
}