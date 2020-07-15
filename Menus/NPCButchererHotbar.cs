// using CheatSheet.CustomUI;
// using CheatSheet.UI;
// using Microsoft.Xna.Framework;
// using Microsoft.Xna.Framework.Graphics;
// using System.Linq;
// using Terraria;
// using Terraria.GameContent;
// using Terraria.ID;
//
// namespace CheatSheet.Menus
// {
// 	internal class NPCButchererHotbar : UIHotbar
// 	{
// 		internal static int[] DoNotButcher = { NPCID.TargetDummy, NPCID.CultistDevote, NPCID.CultistArcherBlue, NPCID.CultistTablet, NPCID.DD2LanePortal, NPCID.DD2EterniaCrystal };
// 		internal static string CSText(string key, string category = "Butcherer") => CheatSheet.CSText(category, key);
// 		public UIView buttonView;
// 		public UIImage bButcherHostiles;
// 		public UIImage bButcherBoth;
// 		public UIImage bButcherTownNPCs;
//
// 		internal bool mouseDown;
// 		internal bool justMouseDown;
//
// 		private CheatSheet mod;
//
// 		public NPCButchererHotbar(CheatSheet mod)
// 		{
// 			this.mod = mod;
// 			//parentHotbar = mod.hotbar;
//
// 			buttonView = new UIView();
// 			Visible = false;
//
// 			// Button images
// 			bButcherHostiles = new UIImage(TextureAssets.Item[ItemID.DemonHeart].Value);
// 			bButcherBoth = new UIImage(TextureAssets.Item[ItemID.CrimsonHeart].Value);
// 			bButcherTownNPCs = new UIImage(TextureAssets.Item[ItemID.Heart].Value);
//
// 			// Button tooltips
// 			bButcherHostiles.Tooltip = CSText("ButcherHostileNPCs");
// 			bButcherBoth.Tooltip = CSText("ButcherHostileAndFriendlyNPCs");
// 			bButcherTownNPCs.Tooltip = CSText("ButcherFriendlyNPCs");
//
// 			// Button EventHandlers
// 			bButcherHostiles.onLeftClick += (s, e) => { HandleButcher(); };
// 			bButcherBoth.onLeftClick += (s, e) => { HandleButcher(1); };
// 			bButcherTownNPCs.onLeftClick += (s, e) => { HandleButcher(2); };
//
// 			// Register mousedown
// 			onMouseDown += (s, e) =>
// 			{
// 				if (!Main.LocalPlayer.mouseInterface && !mod.hotbar.MouseInside && !mod.hotbar.button.MouseInside)
// 				{
// 					mouseDown = true;
// 					Main.LocalPlayer.mouseInterface = true;
// 				}
// 			};
// 			onMouseUp += (s, e) =>
// 			{
// 				justMouseDown = true;
// 				mouseDown = false; /*startTileX = -1; startTileY = -1;*/
// 			};
//
// 			// ButtonView
// 			buttonView.AddChild(bButcherHostiles);
// 			buttonView.AddChild(bButcherBoth);
// 			buttonView.AddChild(bButcherTownNPCs);
//
// 			Width = 200f;
// 			Height = 55f;
// 			buttonView.Height = Height;
// 			Anchor = AnchorPosition.Top;
// 			AddChild(buttonView);
// 			Position = new Vector2(Hotbar.xPosition, hiddenPosition);
// 			CenterXAxisToParentCenter();
// 			float num = spacing;
// 			for (int i = 0; i < buttonView.children.Count; i++)
// 			{
// 				buttonView.children[i].Anchor = AnchorPosition.Left;
// 				buttonView.children[i].Position = new Vector2(num, 0f);
// 				buttonView.children[i].CenterYAxisToParentCenter();
// 				buttonView.children[i].Visible = true;
// 				buttonView.children[i].ForegroundColor = buttonUnselectedColor;
// 				num += buttonView.children[i].Width + spacing;
// 			}
//
// 			Resize();
// 		}
//
// 		public static void HandleButcher(int butcherType = 0, bool forceHandle = false)
// 		{
// 			bool syncData = forceHandle || Main.netMode == 0;
// 			if (syncData)
// 			{
// 				ButcherNPCs(butcherType, forceHandle);
// 			}
// 			else
// 			{
// 				SyncButcher(butcherType);
// 			}
// 		}
//
// 		private static void SyncButcher(int butcherType = 0)
// 		{
// 			var netMessage = CheatSheet.instance.GetPacket();
// 			netMessage.Write((byte)CheatSheetMessageType.ButcherNPCs);
// 			netMessage.Write(butcherType);
// 			netMessage.Send();
// 		}
//
// 		private static void ButcherNPCs(int butcherType = 0, bool syncData = false, int indexRange = -1)
// 		{
// 			//case 28 msgType == 28
// 			/*
//             writer.Write((short)number); // index
//             writer.Write((short)number2); // damage
//             writer.Write(number3); // knockback
//             writer.Write((byte)(number4 + 1f)); // hit direction
//             writer.Write((byte)number5); // crit ( 1==crit true, else false )
//
//             int num86 = (int)this.reader.ReadInt16();
//             int num87 = (int)this.reader.ReadInt16();
//             float num88 = this.reader.ReadSingle();
//             int num89 = (int)(this.reader.ReadByte() - 1);
//             byte b7 = this.reader.ReadByte();
//             */
// 			// 0 == hostiles
// 			// 1 == hostiles & town NPCs // friendlies
// 			// 2 == town NPCs // friendlies only
// 			for (int i = 0; i < Main.maxNPCs; i++) // Iteration
// 			{
// 				if (Main.npc[i].active && CheckNPC(i))
// 				{
// 					if (butcherType == 0 && (Main.npc[i].townNPC || Main.npc[i].friendly)) continue;
// 					if (butcherType == 2 && (!Main.npc[i].townNPC || !Main.npc[i].friendly)) continue;
// 					//always run for the visual effects (damage drawn and sounds) for client
// 					Main.npc[i].StrikeNPCNoInteraction(Main.npc[i].lifeMax, 0f, -Main.npc[i].direction, true);
// 					if (syncData) // syncData does not do visuals
// 					{
// 						NetMessage.SendData(28, -1, -1, null, i, Main.npc[i].lifeMax, 0f, -Main.npc[i].direction, 1);
// 						// type, -1, -1, msg, index, damage, knockback, direction, crit
// 					}
// 				}
// 			}
// 		}
//
// 		private static bool CheckNPC(int index)
// 		{
// 			return !DoNotButcher.Contains(Main.npc[index].type);
// 		}
//
// 		private static bool CheckNPC(NPC npc)
// 		{
// 			return CheckNPC(npc.whoAmI);
// 		}
//
// 		public override void Update()
// 		{
// 			DoSlideMovement();
//
// 			CenterXAxisToParentCenter();
// 			base.Update();
// 		}
//
// 		public override void Draw(SpriteBatch spriteBatch)
// 		{
// 			if (Visible)
// 			{
// 				spriteBatch.End();
// 				spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, _rasterizerState, null, Main.UIScaleMatrix);
// 				//	Rectangle scissorRectangle = new Rectangle((int)base.X- (int)base.Width, (int)base.Y, (int)base.Width, (int)base.Height);
// 				//Parent.Position.Y
// 				//		Main.NewText((int)Parent.Position.Y + " " + (int)shownPosition);
// 				//	Rectangle scissorRectangle = new Rectangle((int)(base.X - base.Width / 2), (int)(shownPosition), (int)base.Width, (int)base.Height);
// 				Rectangle scissorRectangle = new Rectangle((int)(X - Width / 2), (int)shownPosition, (int)Width, (int)(mod.hotbar.Position.Y - shownPosition));
// 				/*if (scissorRectangle.X < 0)
// 				{
// 					scissorRectangle.Width += scissorRectangle.X;
// 					scissorRectangle.X = 0;
// 				}
// 				if (scissorRectangle.Y < 0)
// 				{
// 					scissorRectangle.Height += scissorRectangle.Y;
// 					scissorRectangle.Y = 0;
// 				}
// 				if ((float)scissorRectangle.X + base.Width > (float)Main.screenWidth)
// 				{
// 					scissorRectangle.Width = Main.screenWidth - scissorRectangle.X;
// 				}
// 				if ((float)scissorRectangle.Y + base.Height > (float)Main.screenHeight)
// 				{
// 					scissorRectangle.Height = Main.screenHeight - scissorRectangle.Y;
// 				}*/
// 				scissorRectangle = CheatSheet.GetClippingRectangle(spriteBatch, scissorRectangle);
// 				Rectangle scissorRectangle2 = spriteBatch.GraphicsDevice.ScissorRectangle;
// 				spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;
//
// 				base.Draw(spriteBatch);
//
// 				spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle2;
// 				spriteBatch.End();
// 				spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Main.UIScaleMatrix);
// 			}
//
// 			//	base.Draw(spriteBatch);
//
// 			if (Visible && base.IsMouseInside())
// 			{
// 				Main.LocalPlayer.mouseInterface = true;
// 				//Main.LocalPlayer.cursorItemIconEnabled = false;
// 			}
//
// 			if (Visible && IsMouseInside())
// 			{
// 				Main.LocalPlayer.mouseInterface = true;
// 			}
//
// 			float x = FontAssets.MouseText.Value.MeasureString(HoverText).X;
// 			Vector2 vector = new Vector2(Main.mouseX, Main.mouseY) + new Vector2(16f);
// 			if (vector.Y > Main.screenHeight - 30)
// 			{
// 				vector.Y = Main.screenHeight - 30;
// 			}
//
// 			if (vector.X > Main.screenWidth - x)
// 			{
// 				vector.X = Main.screenWidth - 460;
// 			}
//
// 			Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.MouseText.Value, HoverText, vector.X, vector.Y, new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor), Color.Black, Vector2.Zero);
// 		}
//
// 		protected override bool IsMouseInside()
// 		{
// 			return hidden ? false : base.IsMouseInside();
// 		}
//
// 		public void Resize()
// 		{
// 			float num = spacing;
// 			for (int i = 0; i < buttonView.children.Count; i++)
// 			{
// 				if (buttonView.children[i].Visible)
// 				{
// 					buttonView.children[i].X = num;
// 					num += buttonView.children[i].Width + spacing;
// 				}
// 			}
//
// 			Width = num;
// 			buttonView.Width = Width;
// 		}
//
// 		public void Hide()
// 		{
// 			hidden = true;
// 			arrived = false;
// 		}
//
// 		public void Show()
// 		{
// 			mod.hotbar.currentHotbar = this;
// 			arrived = false;
// 			hidden = false;
// 			Visible = true;
// 		}
// 	}
// }